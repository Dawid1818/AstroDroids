using AstroDroids.Coroutines;
using AstroDroids.Entities.Hostile;
using System.Collections;
using System.Numerics;

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
        }
    }
}
