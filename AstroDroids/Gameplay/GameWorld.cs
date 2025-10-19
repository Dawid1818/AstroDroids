using AstroDroids.Entities;
using AstroDroids.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroDroids.Gameplay
{
    public class GameWorld
    {
        public readonly Rectangle Bounds = new Rectangle(0, 0, 800, 600);

        public List<AliveEntity> Enemies { get; } = new List<AliveEntity>();
        public List<AliveEntity> EnemiesToRemove { get; } = new List<AliveEntity>();

        public List<Projectile> Projectiles { get; } = new List<Projectile>();
        public List<Projectile> ProjectilesToRemove { get; } = new List<Projectile>();

        public Player Player { get; set; }

        public void Update(GameTime gameTime)
        {
            Player.Update(gameTime);

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
            Player.Draw(gameTime);

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
            ProjectilesToRemove.Remove(projectile);
        }
    }
}
