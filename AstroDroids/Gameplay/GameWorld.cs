using AstroDroids.Entities;
using AstroDroids.Entities.Friendly;
using AstroDroids.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace AstroDroids.Gameplay
{
    public class GameWorld
    {
        public readonly Rectangle Bounds = new Rectangle(0, 0, 800, 600);

        public List<AliveEntity> Enemies { get; } = new List<AliveEntity>();
        public List<AliveEntity> EnemiesToRemove { get; } = new List<AliveEntity>();

        public List<Projectile> Projectiles { get; } = new List<Projectile>();
        public List<Projectile> ProjectilesToRemove { get; } = new List<Projectile>();

        //public Player Player { get; set; }
        List<Player> Players = new List<Player>();

        Random rnd = new Random();

        public void Update(GameTime gameTime)
        {
            foreach (var item in Players)
            {
                item.Update(gameTime);
            }

            foreach (var item in Enemies)
            {
                item.Update(gameTime);
            }

            foreach (var item in EnemiesToRemove)
            {
                Enemies.Remove(item);
            }
            EnemiesToRemove.Clear();

            foreach (var item in Projectiles)
            {
                item.Update(gameTime);
            }

            foreach (var item in ProjectilesToRemove)
            {
                Projectiles.Remove(item);
            }
            ProjectilesToRemove.Clear();
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var item in Players)
            {
                item.Draw(gameTime);
            }

            foreach (var item in Enemies)
            {
                item.Draw(gameTime);
            }

            foreach (var item in Projectiles)
            {
                item.Draw(gameTime);
            }
        }

        public void AddEnemy(AliveEntity enemy)
        {
            Enemies.Add(enemy);
        }

        public void RemoveEnemy(AliveEntity enemy)
        {
            EnemiesToRemove.Add(enemy);
        }

        public void AddProjectile(Projectile projectile)
        {
            Projectiles.Add(projectile);
        }

        public void RemoveProjectile(Projectile projectile)
        {
            ProjectilesToRemove.Add(projectile);
        }

        public Player GetRandomPlayer()
        {
            if (Players.Count == 0)
                return null;

            return Players[rnd.Next(Players.Count)];
        }

        public void AddPlayer(Player player)
        {
            Players.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            Players.Remove(player);
        }
    }
}
