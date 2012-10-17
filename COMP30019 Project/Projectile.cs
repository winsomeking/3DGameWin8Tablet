using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace SharpDX_Windows_8_Abstraction
{
    // Projectile classed, used by both player and enemy.
    class Projectile : VisibleGameObject
    {
        private Vector3 vel;
        private GameObjectType targetType;
        private float hitRadius = 0.5f;
        private float squareHitRadius;

        // Constructor.
        public Projectile(Game game, Model model, Vector3 pos, Vector3 vel, GameObjectType targetType)
            : base(game)
        {
            this.model = model;
            this.pos = pos;
            this.vel = vel;
            this.targetType = targetType;
            squareHitRadius = hitRadius * hitRadius;
        }

        // Frame update method.
        public override void Update(float timeDelta)
        {
            // Apply velocity to position.
            pos += vel * timeDelta;

            // Remove self if off screen.
            if (
                pos.X < game.boundaryLeft ||
                pos.X > game.boundaryRight ||
                pos.Y < game.boundaryBottom ||
                pos.Y > game.boundaryTop
                ) {
                game.Remove(this);
            }

            // Set local transformation to be spinning according to time for fun.
            transformation = Matrix.RotationY(game.time) * Matrix.RotationZ(game.time * game.time);

            // Check if collided with the target type of object.
            checkForCollisions();
        }

        // Check if collided with the target type of object.
        private void checkForCollisions()
        {
            foreach (var obj in game.gameObjects)
            {
                // Check of object is the target type and if it's within the projectile hit range.
                if (obj.type == targetType && ((((VisibleGameObject)obj).pos-pos).LengthSquared() <= squareHitRadius)) {
                    // Cast to object class and call Hit method.
                    switch (obj.type) {
                        case GameObjectType.Player:
                            ((Player)obj).Hit();
                            break;
                        case GameObjectType.Enemy:
                            ((Enemy)obj).Hit();
                            break;
                    }

                    // Destroy self.
                    game.Remove(this);
                }
            }
        }
    }
}
