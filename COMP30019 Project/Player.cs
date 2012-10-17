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
<<<<<<< HEAD
        private float speedXZ = 1.0f;
=======
        private float speedXZ = 0;
>>>>>>> 66d995487f7c61833f1ed343e2b98795994aea2d
        private float accelerationY = 0;
        private float accelerationXZ = 0;
        private float gravity = -0.5f;
        
        private bool accel_flag;
        private bool leftDown;
        private bool rightDown;
        private bool force_down;
<<<<<<< HEAD

        private float  timer = 0;
=======
>>>>>>> 66d995487f7c61833f1ed343e2b98795994aea2d
        //private bool upDown;
        
        private Terrain terrain;
        
        //private float projectileSpeed = 20;

        private float angleY = 0;
        private float angleXZ = 0;
        
        public Player(Game game) : base(game) {
            type = GameObjectType.Player;
            model = game.assets.GetModel("player", CreatePlayerModel);
<<<<<<< HEAD
            pos = new SharpDX.Vector3(game.getTerrain().getGridlen() / 2, game.getTerrain().getWorldHeight(game.getTerrain().getGridlen() / 2, game.getTerrain().getGridlen() / 2), game.getTerrain().getGridlen() / 2);
=======
            pos = new SharpDX.Vector3(game.getTerrain().getGridlen() / 2, game.getTerrain().getWorldHeight(game.getTerrain().getGridlen() / 2, game.getTerrain().getGridlen() / 2) + 50, game.getTerrain().getGridlen() / 2);
>>>>>>> 66d995487f7c61833f1ed343e2b98795994aea2d
            //pos = new SharpDX.Vector3(0, game.getTerrain().getWorldHeight(0, 0), 0);
            vel = Vector3.Zero;
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

            if (args.Delta.Translation.Y > 0)
            {
                accel_flag = true;
            }
            else
            {
                accel_flag = false;
            }
        }

        // Simulates a key release when the horizontal drag has completed
        public override void OnManipulationCompleted(GestureRecognizer sender, ManipulationCompletedEventArgs args)
        {
            leftDown = false;
            rightDown = false;
            accel_flag = false;
        }

        // Keyboard controls.
        public override void KeyDown(KeyEventArgs arg)
        {
            switch (arg.VirtualKey)
            {
                case VirtualKey.Left: rotateAngle("left"); break;
                case VirtualKey.Right: rotateAngle("right"); break;
                case VirtualKey.Up: speedXZ++; break;
                case VirtualKey.Down: speedXZ--; break;
                case VirtualKey.Space: accel_flag = true; force_down = true; break;
            }
        }
        public override void KeyUp(KeyEventArgs arg)
        {
            switch (arg.VirtualKey)
            {
                //case VirtualKey.Left: leftDown = false; break;
                //case VirtualKey.Right: rightDown = false; break;
                case VirtualKey.Space: accel_flag = false; ; force_down = false; break;
            }
        }

        // Shoot a projectile.
        private void fire()
        {
           /*game.Add(new Projectile(game,
                game.assets.GetModel("player projectile", CreatePlayerProjectileModel),
                pos,
                new Vector3(0, projectileSpeed, 0),
                GameObjectType.Enemy
            ));*/

            //accelerationY = -5.0f;


        }

	// Shoot a projectile.
        private void rotateAngle(String direction)
        {
<<<<<<< HEAD
			/*
            if (direction == "left"){
=======
			if (direction == "left"){
>>>>>>> 66d995487f7c61833f1ed343e2b98795994aea2d
                angleXZ = angleXZ + 0.1f;
			} else if (direction == "right") {
                angleXZ = angleXZ - 0.1f;
            
<<<<<<< HEAD
            }*/
=======
            }
>>>>>>> 66d995487f7c61833f1ed343e2b98795994aea2d
		}

        // Frame update.
        public override void Update(float timeDelta)
        {
            // Determine velocity based on keys being pressed.
<<<<<<< HEAD
            timer += timeDelta;

            angleXZ = angleXZ + game.getAccelX() * 0.1f;

=======
>>>>>>> 66d995487f7c61833f1ed343e2b98795994aea2d
            

            vel.X = speedXZ * (float)Math.Cos(angleXZ); //*cos(angleXZ);
            vel.Z = speedXZ * (float)Math.Sin(angleXZ); //*sin(angleXZ);

            vel.Y += accelerationY;

            // If accelerometer is tilted, then 
			// TO BE FILLED
			
			// Speed control
            /*
             * if (accel_flag){
                accelerationY = gravity - 1.0f; 
            } 
            else
            {
                if (vel.Y <= 0.5f && vel.Y >= -0.5f)
                {
                    accelerationY = gravity;
                    vel.Y = 0;
                }
                else
                {
                    accelerationY = gravity + 2f;
                }
            }*/

            /* CHECK COLLISION WITH TERRAIN */
            /*if (pos.Y - game.getTerrain().getWorldHeight((int)pos.X, (int)pos.Z) < 1.0f && pos.Y - game.getTerrain().getWorldHeight((int)pos.X, (int)pos.Z) > -1.0f && force_down == false)
            {
                vel.Y = 1.0f;
                // COLLISION in Y axis!
                // Check for angle of terrain and player XYZ angle
                // If the dot product is close to one, then the player accelerates in XYZ (&& if accel_flag == 1)
            }
            else
            {
                accelerationY = -0.1f;
            }*/

            vel.Y = -3f;

			/* CHECK COLLISION WITH OBSTACLE */
			
            //if (leftDown) {vel.X -= speed; }
            //if (rightDown) { vel.X += speed; }
            //if (upDown) { vel.Y = speed; } else { vel.Y = 0; }

            // Apply velocity to position.
<<<<<<< HEAD
            // pos += vel * timeDelta;
=======
            pos += vel * timeDelta;
>>>>>>> 66d995487f7c61833f1ed343e2b98795994aea2d

            // Keep within the boundaries.
            if (pos.X < game.boundaryLeft) { pos.X = game.boundaryLeft; }
            if (pos.X > game.boundaryRight) { pos.X = game.boundaryRight; }
        }

        // React to getting hit by an enemy bullet.
        public void Hit()
        {
            game.Exit();
        }
    }
}
