using AstroDroids.Coroutines;
using AstroDroids.Entities.Hostile;
using AstroDroids.Gameplay;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroDroids.Levels
{
    public class Level3EventHandler : LevelEventHandler
    {
        public override void RegisterEvents()
        {
            base.RegisterEvents();

            AddEvent(10, "Orange Barrier tutorial", () =>
            {
                Scene.World.StartCoroutine(TutorialCoro());
            });
        }

        IEnumerator TutorialCoro()
        {
            GameScene gameScene = Scene as GameScene;

            List<LaserBarrier> allBarriers = Scene.World.Enemies.OfType<LaserBarrier>().ToList();
            List<LaserBarrier> targetBarriers = allBarriers.Where(x => x.CanBeDamaged).ToList();

            if (targetBarriers.Count > 1)
            {
                foreach (var item in allBarriers)
                {
                    item.SetMoveDir(Vector2.Zero);
                }

                Scene.World.camEntity.PausePath = true;

                gameScene.ShowLevel3Help();

                yield return new WaitUntil(() => targetBarriers.Count(x => x.GetHealth() != 1) >= 1);

                Scene.World.camEntity.PausePath = false;

                gameScene.HideLevel3Help();

                foreach (var item in allBarriers)
                {
                    item.SetMoveDir(new Vector2(0, 2f));
                }
            }
        }
    }
}
