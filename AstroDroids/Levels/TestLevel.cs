using AstroDroids.Coroutines;
using AstroDroids.Entities.Hostile;
using Microsoft.Xna.Framework;
using System.Collections;

namespace AstroDroids.Levels
{
    public class TestLevel : Level
    {
        public override void StartLevel()
        {

        }

        public override IEnumerator LevelScript()
        {
            yield return new WaitForSeconds(2);

            Scene.World.AddEnemy(new BasicEnemy(Vector2.Zero));

            yield return new WaitForSeconds(0.3f);

            Scene.World.AddEnemy(new BasicEnemy(Vector2.Zero));
        }
    }
}
