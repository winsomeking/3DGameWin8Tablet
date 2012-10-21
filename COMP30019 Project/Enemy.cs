using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using SharpDX;

namespace SharpDX_Windows_8_Abstraction
{
    // Enemy class
    // Basically just shoots randomly, see EnemyController for enemy movement.
    class Enemy : VisibleGameObject
    {
        private float projectileSpeed = 10;

        float fireTimer;
        float fireWaitMin = 2;
        float fireWaitMax = 20;

        public Enemy(Game game, Vector3 pos) : base(game) {
            type = GameObjectType.Enemy;
            model = game.assets.GetModel("ship", CreateEnemyModel);
            this.pos = pos;
            game.resetTimeLimit();

            //setFireTimer();
        }

        /*
        private void setFireTimer()
        {
            fireTimer = fireWaitMin + (float)game.random.NextDouble() * (fireWaitMax - fireWaitMin);
        }
        */

        public Model CreateEnemyModel()
        {
            return game.assets.CreateTexturedCube("Target.gif", 1.0f);
        }

        /*
        private Model CreateEnemyProjectileModel()
        {
            return game.assets.CreateTexturedBox("enemy projectile.png", new Vector3(0.2f, 0.2f, 0.4f));
        }

        public override void Update(float timeDelta)
        {
            fireTimer -= timeDelta;
            if (fireTimer < 0) {
                //fire();
                setFireTimer();
            }
        }

        private void fire()
        {
            game.Add(new Projectile(game,
                game.assets.GetModel("enemy projectile", CreateEnemyProjectileModel),
                pos,
                new Vector3(0, -projectileSpeed, 0),
                GameObjectType.Player
            ));
        }
        */

        public void Hit()
        {
            game.Remove(this);
        }
    }
}
