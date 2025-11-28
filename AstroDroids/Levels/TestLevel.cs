using AstroDroids.Coroutines;
using AstroDroids.Entities.Hostile;
using AstroDroids.Entities.Neutral;
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
            return null;

            //int rows = 3;
            //int cols = 6;

            //EntityGroup group = CreateGroup(new Vector2(Scene.World.Bounds.Width / 2f - ((32f * cols) + (24 * cols - 1)) / 2f, 20f), rows, cols, 32f, 32f, 24f);

            //for (int i = 0; i < rows; i++)
            //{
            //    for (int j = 0; j < cols; j++)
            //    {
            //        Scene.World.AddEnemy(new BasicEnemy(Vector2.Zero, group.GetCell(i, j)));

            //        yield return new WaitForSeconds(0.3f);
            //    }
            //}

            //yield return new WaitUntil(() => { return Scene.World.Enemies.Count == 0; } );

            //Scene.World.RemoveEntityGroup(group);

            //rows = 5;
            //cols = 10;

            //group = CreateGroup(new Vector2(Scene.World.Bounds.Width / 2f - ((32f * cols) + (24 * cols - 1)) / 2f, 20f), rows, cols, 32f, 32f, 24f);

            //for (int i = 0; i < rows; i++)
            //{
            //    for (int j = 0; j < cols; j++)
            //    {
            //        Scene.World.AddEnemy(new BasicEnemy(Vector2.Zero, group.GetCell(i, j)));

            //        yield return new WaitForSeconds(0.3f);
            //    }
            //}
        }
    }
}
