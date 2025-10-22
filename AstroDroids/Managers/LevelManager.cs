using AstroDroids.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
