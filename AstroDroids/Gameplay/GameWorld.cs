using AstroDroids.Coroutines;
using AstroDroids.Entities;
using AstroDroids.Entities.Friendly;
using AstroDroids.Entities.Hostile;
using AstroDroids.Entities.Neutral;
using AstroDroids.Graphics;
using AstroDroids.Levels;
using AstroDroids.Managers;
using AstroDroids.Paths;
using AstroDroids.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
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

        public List<EntityGroup> EntityGroups { get; } = new List<EntityGroup>();
        public List<EntityGroup> EntityGroupsToRemove { get; } = new List<EntityGroup>();

        List<EnemySpawner> Spawners = new List<EnemySpawner>();
        List<EnemySpawner> SpawnersToRemove = new List<EnemySpawner>();
        List<EventNode> Events = new List<EventNode>();

        //public Player Player { get; set; }
        List<Player> Players = new List<Player>();
        List<Player> PlayersToRemove = new List<Player>();

        CameraEntity camEntity = new CameraEntity();

        Random rnd = new Random();

        CoroutineManager coroutineManager = new CoroutineManager();

        public void Initialize()
        {
            Spawners.Clear();
            Spawners.AddRange(LevelManager.CurrentLevel.Spawners);

            Events.Clear();
            Events.AddRange(LevelManager.CurrentLevel.Events);
        }

        IEnumerator SpawnEnemies(EnemySpawner spawner)
        {
            for (int i = 0; i < spawner.EnemyCount; i++)
            {
                BasicEnemy enemy = new BasicEnemy(spawner.HasPath ? spawner.Path.StartPoint != null ? spawner.Path.StartPoint : spawner.Transform.Position : spawner.SpawnPosition, null);
                enemy.PathManager = new PathManager(spawner.Path);
                enemy.PathManager.Speed = spawner.PathSpeed;
                enemy.PathManager.Loop = spawner.PathLoop;
                enemy.PathManager.MinPath = spawner.MinPath;
                AddEnemy(enemy, spawner.FollowsCamera);

                yield return new WaitForSeconds(spawner.DelayBetweenEnemies);
            }
        }

        public void Update(GameTime gameTime)
        {
            coroutineManager.Update();

            camEntity.Update(gameTime);

            foreach (var spawner in Spawners)
            {
                if(spawner.Transform.Position.Y > camEntity.Transform.Position.Y)
                {
                    SpawnersToRemove.Add(spawner);

                    coroutineManager.StartCoroutine(SpawnEnemies(spawner));
                }
            }

            foreach (var item in SpawnersToRemove)
            {
                Spawners.Remove(item);
            }
            SpawnersToRemove.Clear();

            foreach (var item in Players)
            {
                item.Update(gameTime);
            }

            foreach (var item in PlayersToRemove)
            {
                Players.Remove(item);
            }
            PlayersToRemove.Clear();

            foreach (var item in EntityGroups)
            {
                item.Update(gameTime);
            }

            foreach (var item in EntityGroupsToRemove)
            {
                EntityGroups.Remove(item);
            }
            EntityGroupsToRemove.Clear();

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

        public void AddEnemy(AliveEntity enemy, bool followsCamera)
        {
            if (followsCamera)
            {
                enemy.Transform.SetParent(camEntity.Transform);
                enemy.Transform.LocalPosition -= camEntity.Transform.Position;
            }
            Enemies.Add(enemy);
        }

        public void RemoveEnemy(AliveEntity enemy)
        {
            EnemiesToRemove.Add(enemy);
        }

        public void AddProjectile(Projectile projectile)
        {
            projectile.Transform.SetParent(camEntity.Transform);
            Projectiles.Add(projectile);
        }

        public void RemoveProjectile(Projectile projectile)
        {
            ProjectilesToRemove.Add(projectile);
        }

        public void AddEntityGroup(EntityGroup group)
        {
            EntityGroups.Add(group);
        }

        public void RemoveEntityGroup(EntityGroup group)
        {
            EntityGroupsToRemove.Add(group);
        }

        public Player GetRandomPlayer()
        {
            if (Players.Count == 0)
                return null;

            return Players[rnd.Next(Players.Count)];
        }

        public void AddPlayer(Player player)
        {
            player.Transform.SetParent(camEntity.Transform);
            Players.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            PlayersToRemove.Add(player);
        }

        internal void SetProgress(float yStart)
        {
            Vector2 newCamera = new Vector2(Screen.GetCameraPosition().X, yStart + Screen.ScreenHeight / 2f);
            Screen.SetCameraPosition(newCamera);

            foreach (var spawner in Spawners)
            {
                if (spawner.Transform.Position.Y > yStart)
                {
                    SpawnersToRemove.Add(spawner);
                }
            }
            foreach (var item in SpawnersToRemove)
            {
                Spawners.Remove(item);
            }
            SpawnersToRemove.Clear();
        }
    }
}
