
using AstroDroids.Entities.Friendly;
using AstroDroids.Helpers;
using AstroDroids.Input;
using AstroDroids.Managers;
using AstroDroids.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace AstroDroids.Gameplay
{
    public class GameStateManager
    {
        public static int Lives { get { return CurrentMissionProgress.Lives; } set { CurrentMissionProgress.Lives = value; } }
        //static int Score = 0;
        public static int Firepower { get { return CurrentMissionProgress.Firepower; } set { CurrentMissionProgress.Firepower = value; } }
        public const int MaxFirepower = 5;

        public const int ExtraLivesThreshold = 20000;
        public static int NextExtraLivesThreshold { get { return ExtraLivesThreshold + (CurrentMissionProgress.ExtraLivesObtained * ExtraLivesThreshold); } }
        public static int PreviousExtraLivesThreshold { get { return ExtraLivesThreshold + ((CurrentMissionProgress.ExtraLivesObtained - 1) * ExtraLivesThreshold); } }

        public static int CurrentWeapon { get { return CurrentMissionProgress.CurrentWeapon; } set { CurrentMissionProgress.CurrentWeapon = value; } }

        //static List<Weapon> Weapons = new List<Weapon>();

        static MissionProgress CurrentMissionProgress = new MissionProgress();

        static List<Weapon> Weapons = new List<Weapon>();

        static GameMission CurrentMission;

        public static void NewState(GameMission mission, int levelIndex = 0)
        {
            CurrentMissionProgress = new MissionProgress() { Type = mission.Type, LevelIndex = levelIndex };
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
            CurrentMissionProgress.Score += amount;

            if(CurrentMissionProgress.Score >= NextExtraLivesThreshold)
            {
                CurrentMissionProgress.ExtraLivesObtained++;
                CurrentMissionProgress.Lives++;
                if(CurrentMissionProgress.Lives > 99)
                {
                    CurrentMissionProgress.Lives = 99;
                }
            }
        }

        public static void UpdateCurrentWeapon(Player player, GameTime gameTime)
        {
            if (InputSystem.IsActionDown(GameAction.NextWeapon) || InputSystem.GetRMBDown())
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

        public static void IncreaseFirepower()
        {
            Firepower++;
            if (Firepower > MaxFirepower)
                Firepower = MaxFirepower;
        }

        public static void DecreaseFirepower()
        {
            Firepower--;
            if (Firepower < 1)
                Firepower = 1;
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

        public static void IncreaseLevelIndex()
        {
            CurrentMissionProgress.LevelIndex++;
        }

        public static bool MissionInitialized()
        {
            return CurrentMission != null;
        }

        internal static void SaveState()
        {
            SaveManager.curSave.MissionProgress = (MissionProgress)FileSaver.CloneObject(CurrentMissionProgress, new MissionProgress());
            SaveManager.SaveGame();
        }

        internal static void LoadState(GameMission mission)
        {
            if (SaveManager.curSave.MissionProgress != null)
            {
                CurrentMissionProgress = (MissionProgress)FileSaver.CloneObject(SaveManager.curSave.MissionProgress, new MissionProgress());
                CurrentMission = mission;

                Weapons = new List<Weapon>();
                Weapons.Add(new PulseCannon());
                Weapons.Add(new LaserCannon());
                Weapons.Add(new PlasmaMortar());
            }
        }

        internal static void ClearState()
        {
            SaveManager.curSave.MissionProgress = null;

            SaveManager.SaveGame();
        }

        internal static MissionProgress GetMissionProgress()
        {
            return CurrentMissionProgress;
        }

        internal static void SetVictory(bool victory)
        {
            CurrentMissionProgress.Victory = victory;
        }

        internal static float GetPowerupChance()
        {
            return CurrentMissionProgress.PowerupChance;
        }

        internal static void ResetPowerupChance()
        {
            CurrentMissionProgress.PowerupChance = 0f;
        }

        internal static void IncreasePowerupChance(float value = 0.05f)
        {
            CurrentMissionProgress.PowerupChance += value;
            if (CurrentMissionProgress.PowerupChance > 100f)
            {
                CurrentMissionProgress.PowerupChance = 100f;
            }
        }
    }
}
