
using AstroDroids.Weapons;

namespace AstroDroids.Gameplay
{
    public class GameState
    {
        static int Lives = 3;
        static int Score = 0;
        public static int Firepower { get; set; } = 1;

        public static Weapon CurrentWeapon { get; set; }

        public static void NewState()
        {
            Lives = 3;
            Score = 0;
            Firepower = 1;
            CurrentWeapon = new BasicWeapon();
        }
    }
}
