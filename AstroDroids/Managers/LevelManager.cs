using AstroDroids.Levels;
using System.Collections;

namespace AstroDroids.Managers
{
    public static class LevelManager
    {
        static Level CurrentLevel;
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
    }
}
