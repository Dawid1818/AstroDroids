
using AstroDroids.Entities.Friendly;
using AstroDroids.Input;
using AstroDroids.Weapons;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace AstroDroids.Gameplay
{
    public class GameState
    {
        static int Lives = 3;
        static int Score = 0;
        public static int Firepower { get; set; } = 1;
        public const int MaxFirepower = 5;

        public static int CurrentWeapon { get; set; } = 0;

        static List<Weapon> Weapons = new List<Weapon>();

        public static void NewState()
        {
            Lives = 3;
            Score = 0;
            Firepower = 1;
            CurrentWeapon = 0; 
            Weapons = new List<Weapon>();
            Weapons.Add(new PulseCannon());
            Weapons.Add(new LaserCannon());
            Weapons.Add(new PlasmaMortar());
        }

        public static void AddScore(int amount)
        {
            Score += amount;
        }

        public static void UpdateCurrentWeapon(Player player, GameTime gameTime)
        {
            if(InputSystem.IsActionDown(GameAction.NextWeapon))
            {
                SelectNextWeapon();
            }

            Weapons[CurrentWeapon].Update(player, gameTime);
        }

        public static void DrawCurrentWeapon(Player player, GameTime gameTime)
        {
            Weapons[CurrentWeapon].DrawEffects(player, gameTime);
        }

        public static void SelectNextWeapon()
        {
            CurrentWeapon++;
            if (CurrentWeapon >= Weapons.Count)
            {
                CurrentWeapon = 0;
            }
            Weapons[CurrentWeapon].ResetState();
        }

        public static void RemoveLife()
        {
            Lives--;

            if(Lives <= 0)
            {
                //game over
            }
        }
    }
}
