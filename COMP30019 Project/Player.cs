using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpDX;
using Windows.UI.Input;
using Windows.UI.Core;
using Windows.System;
using Windows.Devices.Sensors;

// Check collision
// Check

namespace SharpDX_Windows_8_Abstraction
{
    // Player class.
    class Player : VisibleGameObject
    {
        private Vector3 vel;
        private Vector3 next_pos;
        private Vector3 next_pos_vector;
        private Vector3 normalized_vel;
        private float resultant_collision;

        private float speedXZ = 8.0f;
        private float accelerationY = 0;
        private float accelerationXZ = 0;
        private float gravity = -0.08f;
        
        private bool leftDown;
        private bool rightDown;
        
        private float projectileSpeed = 20;

        private float angleXZ = 0;
        
        public Player(Game game) : base(game) {
            type = GameObjectType.Player;
            model = game.assets.GetModel("player", CreatePlayerModel);
            pos = new SharpDX.Vector3(game.getTerrain().getGridlen() / 2, game.getTerrain().getWorldHeight(game.getTerrain().getGridlen() / 2, game.getTerrain().getGridlen() / 2) + 32, game.getTerrain().getGridlen() / 2);
            vel = Vector3.Zero;
            next_pos = Vector3.Zero;
            next_pos_vector = Vector3.Zero;
            normalized_vel = Vector3.Zero;
        }

        public Model CreatePlayerModel()
        {
            return game.assets.CreateTexturedCube("player.png", 1.0f);
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
        }

        // Simulates a key release when the horizontal drag has completed
        public override void OnManipulationCompleted(GestureRecognizer sender, ManipulationCompletedEventArgs args)
        {
            leftDown = false;
            rightDown = false;
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
                case VirtualKey.Left: leftDown = false; break;
                case VirtualKey.Right: rightDown = false; break;
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

                vel.Y = 0.0f;
                pos.Y = game.getTerrain().getWorldHeight((int)pos.X, (int)pos.Z);

            }
            else
            {
                accelerationY = gravity;
                accelerationXZ = accelerationXZ * 0.9f;
            }

			/* CHECK COLLISION WITH OBSTACLE */


            // Apply velocity to position.
            pos += vel * timeDelta;

            // Keep within the boundaries.
            //if (pos.X < game.boundaryLeft) { pos.X = game.boundaryLeft; }
            //if (pos.X > game.boundaryRight) { pos.X = game.boundaryRight; }
        }

        // React to getting hit by an enemy bullet.
        public void Hit()
        {
            game.Exit();
        }

        // React to getting hit by an enemy bullet.
        public float getAngleXZ()
        {
            return angleXZ;
        }
    }
}
