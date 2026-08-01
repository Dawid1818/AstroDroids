
using AstroDroids.Entities.Friendly;
using AstroDroids.Input;
using AstroDroids.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AstroDroids.Gameplay
{
    public class GameStateManager
    {
        //static int Lives = 3;
        //static int Score = 0;
        public static int Firepower { get { return CurrentMissionProgress.Firepower; } set { CurrentMissionProgress.Firepower = value; } }
        public const int MaxFirepower = 5;

        public static int CurrentWeapon { get { return CurrentMissionProgress.CurrentWeapon; } set { CurrentMissionProgress.CurrentWeapon = value; } }

        //static List<Weapon> Weapons = new List<Weapon>();

        static MissionProgress CurrentMissionProgress = new MissionProgress();

        static List<Weapon> Weapons = new List<Weapon>();

        static GameMission CurrentMission;

        public static void NewState(GameMission mission)
        {
            CurrentMissionProgress = new MissionProgress() { Type = mission.Type };
            CurrentMission = mission;

            //Lives = 3;
            //Score = 0;
            //Firepower = 1;
            //CurrentWeapon = 0;
            Weapons = new List<Weapon>();
            Weapons.Add(new PulseCannon());
            Weapons.Add(new LaserCannon());
            Weapons.Add(new PlasmaMortar());
        }

        public static void AddScore(int amount)
        {
            //Score += amount;
            CurrentMissionProgress.Score += amount;
        }

        public static void UpdateCurrentWeapon(Player player, GameTime gameTime)
        {
            if (InputSystem.IsActionDown(GameAction.NextWeapon))
            {
                SelectNextWeapon();
            }

            if (CurrentMissionProgress.CurrentWeapon < 0 || CurrentMissionProgress.CurrentWeapon > Weapons.Count - 1)
                return;
            Weapons[CurrentMissionProgress.CurrentWeapon].Update(player, gameTime);
        }

        public static void DrawCurrentWeapon(Player player, GameTime gameTime)
        {
            if (CurrentMissionProgress.CurrentWeapon < 0 || CurrentMissionProgress.CurrentWeapon > Weapons.Count - 1)
                return;
            Weapons[CurrentMissionProgress.CurrentWeapon].DrawEffects(player, gameTime);
        }

        public static void SelectNextWeapon()
        {
            CurrentMissionProgress.CurrentWeapon++;
            if (CurrentMissionProgress.CurrentWeapon >= Weapons.Count)
            {
                CurrentMissionProgress.CurrentWeapon = 0;
            }

            if (CurrentMissionProgress.CurrentWeapon < 0 || CurrentMissionProgress.CurrentWeapon > Weapons.Count - 1)
                return;
            Weapons[CurrentMissionProgress.CurrentWeapon].ResetState();
        }

        public static void RemoveLife()
        {
            CurrentMissionProgress.Lives--;

            if (CurrentMissionProgress.Lives <= 0)
            {
                //game over
            }
        }

        public static int GetLives()
        {
            return CurrentMissionProgress.Lives;
        }

        public static int GetFirepower()
        {
            return CurrentMissionProgress.Firepower;
        }

        public static Texture2D GetWeaponIcon()
        {
            return Weapons[CurrentMissionProgress.CurrentWeapon].WeaponIcon;
        }

        public static int GetScore()
        {
            return CurrentMissionProgress.Score;
        }

        public static MissionType GetMissionType()
        {
            return CurrentMission.Type;
        }

        public static List<string> GetLevels()
        {
            return CurrentMission.LevelNames;
        }

        public static int GetLevelIndex()
        {
            return CurrentMissionProgress.LevelIndex;
        }
    }
}
