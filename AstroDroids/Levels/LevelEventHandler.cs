using AstroDroids.Coroutines;
using AstroDroids.Gameplay;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AstroDroids.Levels
{
    public class LevelEventHandler
    {
        protected GameScene Scene { get { return SceneManager.GetScene() as GameScene; } }

        Dictionary<int, LevelEvent> Events = new Dictionary<int, LevelEvent>();

        public virtual void RegisterEvents()
        {
            AddEvent(0, "Test Event!", () => 
            {
                GameStateManager.Firepower = GameStateManager.MaxFirepower;
            });

            AddEvent(1, "Show Boss Warning", () =>
            {
                Scene.World.PauseWaves = true;
                Scene.World.StartCoroutine(BossWarning());
            });

            AddEvent(2, "Finish Level", () =>
            {
                //Scene.World.StartCoroutine(BossWarning());

                GameScene gameScene = Scene as GameScene;
                gameScene.FinishLevel();
            });
        }

        IEnumerator BossWarning()
        {
            GameScene gameScene = Scene as GameScene;

            SoundManager.FadeOutMusic();

            gameScene.DisplayBossWarning();

            yield return new WaitForSeconds(5);

            gameScene.HideBossWarning();

            SoundManager.PlayMusic(GameDatabase.GetMusic(LevelManager.CurrentLevel.BossMusicId));

            yield return new WaitForSeconds(1);

            gameScene.DisableBossWarning();

            Scene.World.PauseWaves = false;
        }

        protected void AddEvent(int id, string name, Action callback)
        {
            Events[id] = new LevelEvent { ID = id, Name = name, Callback = callback };
        }

        public void RunEvent(int id)
        {
            if (Events.TryGetValue(id, out LevelEvent Event))
            {
                Event.Callback();
            }
        }

        public Dictionary<int, LevelEvent> GetEvents()
        {
            return Events;
        }
    }
}
