using AstroDroids.Levels;

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
    }
}
