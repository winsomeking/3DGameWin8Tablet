using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace SharpDX_Windows_8_Abstraction
{
    // Enemy Controller class.
    class EnemyController : GameObject
    {
        // Spacing and counts.
        private Random rand = new Random();

        private int number_of_enemies = 16;

        // Constructor.
        public EnemyController(Game game)
            : base(game)
        {
            nextWave();
        }

        // Set up the next wave.
        private void nextWave()
        {
            number_of_enemies += 8;
            createEnemies();
        }

        // Create a grid of enemies for the current wave.
        private void createEnemies()
        {
            float y = game.boundaryTop;
            for (int i = 0; i < number_of_enemies; i++)
            {
                int x = rand.Next(0, game.getTerrain().getGridlen());
                int z = rand.Next(0, game.getTerrain().getGridlen());
                game.Add(new Enemy(game, new Vector3(x, game.getTerrain().getWorldHeight(x,z)+1.0f, z)));
            }

        }

        // Frame update method.
        public override void Update(float timeDelta)
        {
            // Invoke next wave once current one has ended.
            if (allEnemiesAreDead())
            {
                nextWave();
            }
        }

        // Return whether all enemies are dead or not.
        private bool allEnemiesAreDead()
        {
            return game.Count(GameObjectType.Enemy) == 0;
        }
        
        // Method for when the game ends.
        private void gameOver()
        {
            game.Exit();
        }
    }
}
