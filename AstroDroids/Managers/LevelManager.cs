using AstroDroids.Helpers;
using AstroDroids.Levels;
using AstroDroids.Scenes;
using System;
using System.Collections;
using System.IO;

namespace AstroDroids.Managers
{
    public static class LevelManager
    {
        public static Level CurrentLevel { get; set; }
        public static bool Playtesting { get; set; }

        static Level backedLevel { get; set; }
        static Scene backedScene { get; set; }

        public static void Initialize()
        {
            if(!Directory.Exists("Content/Levels"))
                Directory.CreateDirectory("Content/Levels");
        }

        public static void LoadLevel(int levelIndex)
        {
            CurrentLevel = new TestLevel();
        }

        public static void StartLevel()
        {
            CurrentLevel.StartLevel();
        }

        public static IEnumerator GetLevelScript()
        {
            return CurrentLevel.LevelScript();
        }

        internal static void Playtest(float yStart)
        {
            backedLevel = CurrentLevel;
            CurrentLevel = new Level();
            FileSaver.CloneObject(backedLevel, CurrentLevel);

            backedScene = SceneManager.GetScene();

            Playtesting = true;

            GameScene scene = new GameScene();

            SceneManager.SetScene(scene);

            scene.World.SetProgress(yStart);
        }

        internal static void QuitPlaytest()
        {
            if (backedLevel != null)
            {
                CurrentLevel = backedLevel;
                backedLevel = null;

            }

            if (backedScene != null)
            {
                SceneManager.SetScene(backedScene);
                backedScene = null;
            }

            Playtesting = false;
        }
    }
}
