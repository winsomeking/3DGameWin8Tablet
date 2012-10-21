using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using SharpDX;
using Windows.UI.Input;
using Windows.UI.Core;
using Windows.System;
using Windows.Devices.Sensors;
using Windows.Storage;
using Windows.Storage.Streams;


// Check collision
// Check

namespace SharpDX_Windows_8_Abstraction
{
    // Player class.
    class Player : VisibleGameObject
    {
        Player_Vertices player_vertices = new Player_Vertices();
        int num_of_lines;
        private float[] floatArrayVertices;
        private float[] floatArrayNormal;
        private int[] intArrayFace;
        private float[] floatArray;

        private Vector3 vel;
        private Vector3 next_pos;
        private Vector3 next_pos_vector;
        private Vector3 normalized_vel;
        private float resultant_collision;
        private int score = 0;

        private float speedXZ = 8.0f;
        private float accelerationY = 0;
        private float accelerationXZ = 0;
        private float gravity = -0.08f;

        /*
        private bool leftDown;
        private bool rightDown;
        */

        private float projectileSpeed = 20;

        private float angleXZ = 0;
        public Player(Game game) : base(game) {
            
            num_of_lines = player_vertices.separated.Length / 4;
            next_pos = Vector3.Zero;
            next_pos_vector = Vector3.Zero;
            normalized_vel = Vector3.Zero;
            type = GameObjectType.Player;
            model = game.assets.GetModel("player", CreatePlayerModel);
            pos = new SharpDX.Vector3(game.getTerrain().getGridlen() / 2, game.getTerrain().getWorldHeight(game.getTerrain().getGridlen() / 2, game.getTerrain().getGridlen() / 2) + 32, game.getTerrain().getGridlen() / 2);
            vel = Vector3.Zero;
            
        }

        public Model CreatePlayerModel()
        {
            readVerticesNormal();
            return game.assets.CreateCustomModel(floatArray);
            //return game.assets.CreateTexturedBox("player.png", new Vector3(1.0f, 1.0f, 1.0f));
        }

        // Method to create projectile texture to give to newly created projectiles.
        private Model CreatePlayerProjectileModel()
        {
            return game.assets.CreateTexturedBox("player projectile.png", new Vector3(0.3f, 0.2f, 0.25f));
        }

        public override void Tapped(GestureRecognizer sender, TappedEventArgs args)
        {
            fire();
        }

        // Simulates a key press when a horizontal drag occurs
        public override void OnManipulationUpdated(GestureRecognizer sender, ManipulationUpdatedEventArgs args)
        {
            /*
            if (args.Delta.Translation.X > 0)
            {
                rightDown = true;
                leftDown = false;
            }
            else
            {
                rightDown = false;
                leftDown = true;
            }
            */
        }

        // Simulates a key release when the horizontal drag has completed
        public override void OnManipulationCompleted(GestureRecognizer sender, ManipulationCompletedEventArgs args)
        {
            /*
            leftDown = false;
            rightDown = false;
            */
        }

        // Keyboard controls.
        public override void KeyDown(KeyEventArgs arg)
        {
            switch (arg.VirtualKey)
            {
                case VirtualKey.Left: angleXZ += 0.1f; break;
                case VirtualKey.Right: angleXZ -= 0.1f; break;
                case VirtualKey.Space: break;
            }
        }
        public override void KeyUp(KeyEventArgs arg)
        {
            switch (arg.VirtualKey)
            {
                case VirtualKey.Left: /*leftDown = false;*/ break;
                case VirtualKey.Right: /*rightDown = false;*/ break;
                case VirtualKey.Space: break;
            }
        }

        // Shoot a projectile.
        private void fire()
        {
            game.Add(new Projectile(game,
                 game.assets.GetModel("player projectile", CreatePlayerProjectileModel),
                 pos,
                 new Vector3(0, projectileSpeed, 0),
                 GameObjectType.Enemy
             ));

        }

        // Frame update.
        public override void Update(float timeDelta)
        {
            // Determine velocity based on keys being pressed.
            vel.X = (speedXZ + accelerationXZ) * (float)Math.Cos(angleXZ); //*cos(angleXZ);
            vel.Z = (speedXZ + accelerationXZ) * (float)Math.Sin(angleXZ); //*sin(angleXZ);
            vel.Y += accelerationY;

            // If accelerometer is tilted, then 
            angleXZ = angleXZ + game.getAccelX() * 0.1f;


            /* CHECK COLLISION WITH TERRAIN */
            if (pos.Y - game.getTerrain().getWorldHeight((int)pos.X, (int)pos.Z) < 1.0f && pos.Y - game.getTerrain().getWorldHeight((int)pos.X, (int)pos.Z) > -10.0f)
            {
                // COLLISION in Y axis!
                // Check for angle of terrain and player XYZ angle
                // If the dot product is close to one, then the player accelerates in XYZ (&& if accel_flag == 1)

                // Next 4 line is just for calculation purpose upon collision
                normalized_vel = SharpDX.Vector3.Normalize(vel);
                if (normalized_vel.X > 0) { normalized_vel.X = 1; } else { normalized_vel.X = -1; }
                if (normalized_vel.Z > 0) { normalized_vel.Z = 1; } else { normalized_vel.Z = -1; }
                next_pos = new Vector3((int)(pos.X + normalized_vel.X), game.getTerrain().getWorldHeight((int)(pos.X + normalized_vel.X), (int)(pos.Z + normalized_vel.Z)), (int)(pos.Z + normalized_vel.Z));
                next_pos_vector = next_pos - pos;
                resultant_collision = SharpDX.Vector3.Dot(normalized_vel, next_pos_vector);

                accelerationXZ = resultant_collision;
                vel.Y = -1/2 * vel.Y;
                vel.Y = -1 / 4 * vel.Y;
                pos.Y = game.getTerrain().getWorldHeight((int)pos.X, (int)pos.Z) + 1.0f;

            }
            else
            {
                accelerationY = gravity;
                accelerationXZ = accelerationXZ * 0.9f;
            }

            foreach (var obj in game.gameObjects)
            {
                // Check of object is the target type and if it's within the projectile hit range.
                if (obj.type == GameObjectType.Enemy && ((((VisibleGameObject)obj).pos - pos).LengthSquared() <= 2.0f))
                {
                    // Cast to object class and call Hit method.
                    switch (obj.type)
                    {
                        case GameObjectType.Player:
                            ((Player)obj).Hit();
                            break;
                        case GameObjectType.Enemy:
                            ((Enemy)obj).Hit();
                            break;
                    }

                    score += (int)(1000.0 * vel.LengthSquared());
                }
            }
            /* CHECK COLLISION WITH OBSTACLE */
            // Apply velocity to position.
            pos += vel * timeDelta;

            // Keep within the boundaries.
            if (pos.X < 0) { pos.X = game.getTerrain().getGridlen(); pos.Y = game.getTerrain().getWorldHeight((int)pos.X, (int)pos.Z); }
            if (pos.X > game.getTerrain().getGridlen()) { pos.X = 0; pos.Y = game.getTerrain().getWorldHeight((int)pos.X, (int)pos.Z); }

            if (pos.Z < 0) { pos.Z = game.getTerrain().getGridlen(); pos.Y = game.getTerrain().getWorldHeight((int)pos.X, (int)pos.Z); }
            if (pos.Z > game.getTerrain().getGridlen()) { pos.Z = 0; pos.Y = game.getTerrain().getWorldHeight((int)pos.X, (int)pos.Z); }
        }

        public void readVerticesNormal()
        {

            floatArrayVertices = new float[checkLength("v") * 4];
            floatArrayNormal = new float[checkLength("n") * 4];
            intArrayFace = new int[checkLength("f") * 6];
            floatArray = new float[checkLength("f") * 3 * 12];

            int indexVertices = 0;
            int indexNormal = 0;
            int indexFace = 0;

            String[] faceTemp1 = new String[3];
            String[] faceTemp2 = new String[3];
            String[] faceTemp3 = new String[3];

            for (int i = 0; i < num_of_lines; i++)
            {
                if (player_vertices.separated[i * 4].StartsWith("v"))
                {
                    floatArrayVertices[indexVertices * 3] = System.Convert.ToSingle(player_vertices.separated[i * 4 + 1]);
                    floatArrayVertices[indexVertices * 3 + 1] = System.Convert.ToSingle(player_vertices.separated[i * 4 + 2]);
                    floatArrayVertices[indexVertices * 3 + 2] = System.Convert.ToSingle(player_vertices.separated[i * 4 + 3]);
                    indexVertices++;
                }
                else if (player_vertices.separated[i * 4].StartsWith("n"))
                {
                    floatArrayNormal[indexNormal * 3] = System.Convert.ToSingle(player_vertices.separated[i * 4 + 1]);
                    floatArrayNormal[indexNormal * 3 + 1] = System.Convert.ToSingle(player_vertices.separated[i * 4 + 2]);
                    floatArrayNormal[indexNormal * 3 + 2] = System.Convert.ToSingle(player_vertices.separated[i * 4 + 3]);
                    indexNormal++;
                }
                else if (player_vertices.separated[i * 4].StartsWith("f"))
                {
                    faceTemp1 = player_vertices.separated[i * 4 + 1].Split('/');
                    faceTemp2 = player_vertices.separated[i * 4 + 2].Split('/');
                    faceTemp3 = player_vertices.separated[i * 4 + 3].Split('/');
                    intArrayFace[indexFace * 6] = System.Convert.ToInt32(faceTemp1[0]) - 1;
                    intArrayFace[indexFace * 6 + 1] = System.Convert.ToInt32(faceTemp1[2]) - 1;
                    intArrayFace[indexFace * 6 + 2] = System.Convert.ToInt32(faceTemp2[0]) - 1;
                    intArrayFace[indexFace * 6 + 3] = System.Convert.ToInt32(faceTemp2[2]) - 1;
                    intArrayFace[indexFace * 6 + 4] = System.Convert.ToInt32(faceTemp3[0]) - 1;
                    intArrayFace[indexFace * 6 + 5] = System.Convert.ToInt32(faceTemp3[2]) - 1;
                    indexFace++;
                }
            }

            for (int i = 0; i < checkLength("f"); i++)
            {
                floatArray[i * 36 + 12] = floatArrayVertices[intArrayFace[i * 6] * 3];
                floatArray[i * 36 + 13] = floatArrayVertices[intArrayFace[i * 6] * 3 + 1];
                floatArray[i * 36 + 14] = floatArrayVertices[intArrayFace[i * 6] * 3 + 2];
                floatArray[i * 36 + 15] = 1.0f;
                floatArray[i * 36 + 16] = 0.4f;
                floatArray[i * 36 + 17] = 0.4f;
                floatArray[i * 36 + 18] = 0.4f;
                floatArray[i * 36 + 19] = 1.0f;
                floatArray[i * 36 + 20] = floatArrayNormal[intArrayFace[i * 6 + 1] * 3];
                floatArray[i * 36 + 21] = floatArrayNormal[intArrayFace[i * 6 + 1] * 3 + 1];
                floatArray[i * 36 + 22] = floatArrayNormal[intArrayFace[i * 6 + 1] * 3 + 2];
                floatArray[i * 36 + 23] = 1.0f;

                floatArray[i * 36 + 0] = floatArrayVertices[intArrayFace[i * 6 + 2] * 3];
                floatArray[i * 36 + 1] = floatArrayVertices[intArrayFace[i * 6 + 2] * 3 + 1];
                floatArray[i * 36 + 2] = floatArrayVertices[intArrayFace[i * 6 + 2] * 3 + 2];
                floatArray[i * 36 + 3] = 1.0f;
                floatArray[i * 36 + 4] = 0.4f;
                floatArray[i * 36 + 5] = 0.4f;
                floatArray[i * 36 + 6] = 0.4f;
                floatArray[i * 36 + 7] = 1.0f;
                floatArray[i * 36 + 8] = floatArrayNormal[intArrayFace[i * 6 + 3] * 3];
                floatArray[i * 36 + 9] = floatArrayNormal[intArrayFace[i * 6 + 3] * 3 + 1];
                floatArray[i * 36 + 10] = floatArrayNormal[intArrayFace[i * 6 + 3] * 3 + 2];
                floatArray[i * 36 + 11] = 1.0f;

                floatArray[i * 36 + 24] = floatArrayVertices[intArrayFace[i * 6 + 4] * 3];
                floatArray[i * 36 + 25] = floatArrayVertices[intArrayFace[i * 6 + 4] * 3 + 1];
                floatArray[i * 36 + 26] = floatArrayVertices[intArrayFace[i * 6 + 4] * 3 + 2];
                floatArray[i * 36 + 27] = 1.0f;
                floatArray[i * 36 + 28] = 0.4f;
                floatArray[i * 36 + 29] = 0.4f;
                floatArray[i * 36 + 30] = 0.4f;
                floatArray[i * 36 + 31] = 1.0f;
                floatArray[i * 36 + 32] = floatArrayNormal[intArrayFace[i * 6 + 5] * 3];
                floatArray[i * 36 + 33] = floatArrayNormal[intArrayFace[i * 6 + 5] * 3 + 1];
                floatArray[i * 36 + 34] = floatArrayNormal[intArrayFace[i * 6 + 5] * 3 + 2];
                floatArray[i * 36 + 35] = 1.0f;
            }
        }

        public int checkLength(String targetString)
        {
            int count = 0;
            for (int i = 0; i < num_of_lines; i++)
            {
                if (player_vertices.separated[i * 4].StartsWith(targetString))
                {
                    count++;
                }
            }
            return count;
        }

        // React to getting hit by an enemy bullet.
        public void Hit()
        {
            game.Exit();
        }

        // Return value for angle in XZ plane
        public float getAngleXZ()
        {
            return angleXZ;
        }

        // Return value for angle in YX plane
        public float getAngleYX()
        {
            if (vel.Y == 0 && vel.X == 0) { return 0; }
            else { return (float)Math.Atan(vel.Y / vel.X); }
        }

        // Return value for angle in YZ plane
        public float getAngleYZ()
        {
            if (vel.Y == 0 && vel.Z == 0) { return 0; }
            else { return (float)Math.Atan(vel.Y / vel.Z); }
        }
    }
}
