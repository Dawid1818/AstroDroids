using AstroDroids.Entities.Hostile;
using AstroDroids.Managers;
using System.Numerics;

namespace AstroDroids.Levels
{
    public class TestLevel : Level
    {
        public override void StartLevel()
        {
            SceneManager.GetScene().World.AddEnemy(new BasicEnemy(Vector2.Zero));
        }
    }
}
