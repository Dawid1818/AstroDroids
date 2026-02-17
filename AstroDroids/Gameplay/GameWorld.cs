using AstroDroids.Coroutines;
using AstroDroids.Drawables;
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
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using System;
using System.Collections;
using System.Collections.Generic;
using static HexaGen.Runtime.MemoryPool;

namespace AstroDroids.Gameplay
{
    public class GameWorld
    {
        public readonly Rectangle Bounds = new Rectangle(0, 0, 800, 600);
        public Starfield Starfield { get; set; }

        public List<AliveEntity> Enemies { get; } = new List<AliveEntity>();
        public List<AliveEntity> EnemiesToRemove { get; } = new List<AliveEntity>();

        public List<Projectile> Projectiles { get; } = new List<Projectile>();
        public List<Projectile> ProjectilesToRemove { get; } = new List<Projectile>();

        public List<EntityGroup> EntityGroups { get; } = new List<EntityGroup>();
        public List<EntityGroup> EntityGroupsToRemove { get; } = new List<EntityGroup>();

        public List<Entity> Warnings { get; } = new List<Entity>();
        public List<Entity> WarningsToRemove { get; } = new List<Entity>();

        public List<ParticleEffect> Effects { get; } = new List<ParticleEffect>();
        public List<ParticleEffect> EffectsToRemove { get; } = new List<ParticleEffect>();

        List<EnemySpawner> Spawners = new List<EnemySpawner>();
        List<EnemySpawner> SpawnersToRemove = new List<EnemySpawner>();
        List<EventNode> Events = new List<EventNode>();
        List<EventNode> EventsToRemove = new List<EventNode>();
        List<LaserBarrierGroupNode> LaserBarrierSpawners = new List<LaserBarrierGroupNode>();
        List<LaserBarrierGroupNode> LaserBarrierSpawnersToRemove = new List<LaserBarrierGroupNode>();

        //public Player Player { get; set; }
        List<Player> Players = new List<Player>();
        List<Player> PlayersToRemove = new List<Player>();

        public CameraEntity camEntity { get; private set; } = new CameraEntity();

        CoroutineManager coroutineManager = new CoroutineManager();

        public void Initialize()
        {
            Spawners.Clear();
            Spawners.AddRange(LevelManager.CurrentLevel.Spawners);

            Events.Clear();
            Events.AddRange(LevelManager.CurrentLevel.Events);

            LaserBarrierSpawners.Clear();
            LaserBarrierSpawners.AddRange(LevelManager.CurrentLevel.LaserBarriers);
        }

        IEnumerator SpawnEnemies(EnemySpawner spawner)
        {
            for (int i = 0; i < spawner.EnemyIDs.Count; i++)
            {
                int id = spawner.EnemyIDs[i];

                Type type = EntityDatabase.GetEnemyType(id);
                Enemy enemy = (Enemy)Activator.CreateInstance(type);

                if (spawner.HasPath)
                {
                    enemy.PathManager = new PathManager(spawner.Path);
                    enemy.PathManager.Speed = spawner.PathSpeed;
                    enemy.PathManager.Loop = spawner.PathLoop;
                    enemy.PathManager.MinPath = spawner.MinPath;
                }

                AddEnemy(enemy, spawner.FollowsCamera, false);

                enemy.Transform.LocalPosition = spawner.HasPath ? spawner.Path.StartPoint != null ? spawner.Path.StartPoint : spawner.Transform.Position : spawner.SpawnPosition;
                enemy.Spawned();

                yield return new WaitForSeconds(spawner.DelayBetweenEnemies);
            }
        }

        void SpawnBarriers(LaserBarrierGroupNode spawner)
        {
            Dictionary<int, LaserBarrier> barriers = new Dictionary<int, LaserBarrier>();

            foreach (var node in spawner.Nodes.Values)
            {
                var barrier = new LaserBarrier(node.Position, node.Id, node.Health);
                barriers[node.Id] = barrier;
            }

            foreach (var node in spawner.Nodes.Values)
            {
                var barrier = barriers[node.Id];

                var connections = new List<LaserBarrier>();

                foreach (var connectedId in node.Connections)
                {
                    if (barriers.TryGetValue(connectedId, out var targetBarrier))
                    {
                        connections.Add(targetBarrier);
                    }
                }

                barrier.SetConnections(connections);
                AddEnemy(barrier, false, true);
            }
        }

        public void StartCoroutine(IEnumerator coro)
        {
            coroutineManager.StartCoroutine(coro);
        }

        public void Update(GameTime gameTime)
        {
            coroutineManager.Update();

            if (Starfield != null)
                Starfield.Update();

            camEntity.Update(gameTime);

            foreach (var spawner in Spawners)
            {
                if (spawner.Transform.Position.Y > camEntity.Transform.Position.Y)
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

            foreach (var spawner in LaserBarrierSpawners)
            {
                if (spawner.Transform.Position.Y > camEntity.Transform.Position.Y)
                {
                    LaserBarrierSpawnersToRemove.Add(spawner);

                    SpawnBarriers(spawner);
                }
            }

            foreach (var item in LaserBarrierSpawnersToRemove)
            {
                LaserBarrierSpawners.Remove(item);
            }
            LaserBarrierSpawnersToRemove.Clear();

            foreach (var item in Players)
            {
                item.Update(gameTime);
            }

            foreach (var item in PlayersToRemove)
            {
                Players.Remove(item);
            }
            PlayersToRemove.Clear();

            foreach (var item in Warnings)
            {
                item.Update(gameTime);
            }

            foreach (var item in WarningsToRemove)
            {
                Warnings.Remove(item);
            }
            WarningsToRemove.Clear();

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

            for (int i = 0; i < Projectiles.Count; i++)
            {
                Projectile item = Projectiles[i];
                item.Update(gameTime);
            }

            foreach (var item in ProjectilesToRemove)
            {
                Projectiles.Remove(item);
            }
            ProjectilesToRemove.Clear();

            foreach (var item in Effects)
            {
                item.Update(gameTime);
            }

            foreach (var item in EffectsToRemove)
            {
                Effects.Remove(item);
            }
            EffectsToRemove.Clear();
        }

        public void Draw(GameTime gameTime)
        {
            if (Starfield != null)
                Starfield.Draw();

            Screen.spriteBatch.Begin(transformMatrix: Screen.GetCameraMatrix(), blendState: BlendState.NonPremultiplied);

            Screen.spriteBatch.DrawRectangle(new RectangleF(0, camEntity.Transform.Position.Y, Bounds.Width, Bounds.Height), Color.Gray, 2f);

            foreach (var item in Warnings)
            {
                item.Draw(gameTime);
            }

            foreach (var item in Players)
            {
                item.Draw(gameTime);

                RenderColliders(item);
            }

            foreach (var item in Enemies)
            {
                item.Draw(gameTime);

                RenderColliders(item);
            }

            for (int i = 0; i < Projectiles.Count; i++)
            {
                Projectile item = Projectiles[i];
                item.Draw(gameTime);

                RenderColliders(item);
            }

            Screen.spriteBatch.End();

            Screen.spriteBatch.Begin(blendState: BlendState.NonPremultiplied);

            foreach (var item in Effects)
            {
                Screen.spriteBatch.Draw(item);
            }

            Screen.spriteBatch.End();
        }

        void RenderColliders(CollidableEntity entity)
        {
            if (AstroDroidsGame.Debug)
            {
                foreach (var col in entity.Colliders)
                {
                    col.DrawDebug(entity.Transform);
                }
            }
        }

        public void AddEffect(ParticleEffect effect)
        {
            Effects.Add(effect);
        }

        public void RemoveEffect(ParticleEffect effect)
        {
            EffectsToRemove.Add(effect);
        }

        public void AddWarning(Entity entity, bool followsCamera)
        {
            if (followsCamera)
            {
                entity.Transform.SetParent(camEntity.Transform);
                //entity.Transform.LocalPosition -= camEntity.Transform.Position;
            }
            Warnings.Add(entity);
        }

        public void RemoveWarning(Entity entity)
        {
            WarningsToRemove.Add(entity);
        }

        public void AddEnemy(Enemy enemy, bool followsCamera, bool invokeSpawned = true)
        {
            if (followsCamera)
            {
                enemy.Transform.SetParent(camEntity.Transform);
                enemy.Transform.LocalPosition -= camEntity.Transform.Position;
            }

            Enemies.Add(enemy);

            if (invokeSpawned)
                enemy.Spawned();
        }

        public void RemoveEnemy(AliveEntity enemy)
        {
            EnemiesToRemove.Add(enemy);
        }

        public void AddProjectile(Projectile projectile, bool followsCamera)
        {
            if (followsCamera)
            {
                projectile.Transform.SetParent(camEntity.Transform);
                projectile.Transform.LocalPosition -= camEntity.Transform.Position;
            }

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

            return Players[AstroDroidsGame.rnd.Next(Players.Count)];
        }

        public List<Player> GetPlayers()
        {
            return Players;
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
            Vector2 newCamera = new Vector2(Screen.ScreenWidth / 2f, yStart + Screen.ScreenHeight / 2f);
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
