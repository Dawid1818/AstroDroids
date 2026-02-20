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

namespace AstroDroids.Gameplay
{
    public class GameWorld
    {
        public readonly Rectangle Bounds = new Rectangle(0, 0, 800, 600);
        public Starfield Starfield { get; set; }

        public List<AliveEntity> Enemies { get; } = new List<AliveEntity>();
        public List<AliveEntity> EnemiesToRemove { get; } = new List<AliveEntity>();

        public List<Entity> BackgroundObjects { get; } = new List<Entity>();
        public List<Entity> BackgroundObjectsToRemove { get; } = new List<Entity>();

        public List<Projectile> Projectiles { get; } = new List<Projectile>();
        public List<Projectile> ProjectilesToRemove { get; } = new List<Projectile>();

        public List<EntityGroup> EntityGroups { get; } = new List<EntityGroup>();
        public List<EntityGroup> EntityGroupsToRemove { get; } = new List<EntityGroup>();

        public List<Entity> Warnings { get; } = new List<Entity>();
        public List<Entity> WarningsToRemove { get; } = new List<Entity>();

        public List<ParticleEffect> Effects { get; } = new List<ParticleEffect>();
        public List<ParticleEffect> EffectsToRemove { get; } = new List<ParticleEffect>();

        List<AttackWave> AttackWaves = new List<AttackWave>();
        List<AttackWave> AttackWavesToRemove = new List<AttackWave>();

        int startPoint = 0;

        List<Player> Players = new List<Player>();
        List<Player> PlayersToRemove = new List<Player>();

        public CameraEntity camEntity { get; private set; } = new CameraEntity();

        CoroutineManager coroutineManager = new CoroutineManager();

        public double speed { get; set; } = 2;

        int ongoingWaves = 0;

        public void Initialize()
        {
            AttackWaves.Clear();
            AttackWaves.AddRange(LevelManager.CurrentLevel.AttackWaves.Slice(startPoint, LevelManager.CurrentLevel.AttackWaves.Count - startPoint));

            if(AttackWaves.Count > 0)
                StartCoroutine(ProcessWaves());
        }

        IEnumerator ProcessWaves()
        {
            if (AttackWaves[0].Delay > 0)
                yield return new WaitForSeconds(AttackWaves[0].Delay);

            for (int i = 0; i < AttackWaves.Count; i++)
            {
                AttackWave item = AttackWaves[i];

                foreach (var spawner in item.Spawners)
                {
                    ongoingWaves++;
                    StartCoroutine(SpawnEnemies(spawner));
                }

                foreach (var barrier in item.LaserBarriers)
                {
                    ongoingWaves++;
                    StartCoroutine(SpawnBarriers(barrier));
                }

                foreach (var eventN in item.Events)
                {
                    ongoingWaves++;
                    StartCoroutine(ProcessEvents(eventN));
                }

                foreach (var eventN in item.BackgroundObjects)
                {
                    ongoingWaves++;
                    StartCoroutine(SpawnBackgroundObjects(eventN));
                }

                if (i != AttackWaves.Count - 1)
                {
                    AttackWave nextWave = AttackWaves[i + 1];
                    if (nextWave.WaitForPreviousWave && ongoingWaves > 0)
                        yield return new WaitUntil(() => ongoingWaves == 0);

                    yield return new WaitForSeconds(nextWave.Delay);
                }
            }

            yield return null;
        }

        IEnumerator SpawnBackgroundObjects(BackgroundObjectNode bgObjectN)
        {
            if (bgObjectN.InitialDelay > 0)
                yield return new WaitForSeconds(bgObjectN.InitialDelay);

            BackgroundObject bgObj = new BackgroundObject(bgObjectN.TextureName, bgObjectN.Angle, bgObjectN.FlipH, bgObjectN.FlipV) { Transform = new Transform(bgObjectN.Transform.Position.X, bgObjectN.Transform.Position.Y) };
            AddBackgroundObject(bgObj);

            ongoingWaves--;
        }

        IEnumerator SpawnEnemies(EnemySpawner spawner)
        {
            if (spawner.InitialDelay > 0)
                yield return new WaitForSeconds(spawner.InitialDelay);

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

            ongoingWaves--;
        }

        IEnumerator SpawnBarriers(LaserBarrierGroupNode spawner)
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

                if (spawner.InitialDelay == 0)
                {
                    AddEnemy(barrier, false, true);
                }
            }

            if (spawner.InitialDelay > 0)
            {
                yield return new WaitForSeconds(spawner.InitialDelay);

                foreach (var node in spawner.Nodes.Values)
                {
                    var barrier = barriers[node.Id];

                    AddEnemy(barrier, false, true);
                }
            }

            ongoingWaves--;
        }

        IEnumerator ProcessEvents(EventNode eventN)
        {
            if (eventN.InitialDelay > 0)
                yield return new WaitForSeconds(eventN.InitialDelay);

            //run event from Level class
            ongoingWaves--;
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

            foreach (var item in BackgroundObjects)
            {
                item.Update(gameTime);
            }

            foreach (var item in BackgroundObjectsToRemove)
            {
                BackgroundObjects.Remove(item);
            }
            BackgroundObjectsToRemove.Clear();
        }

        public void Draw(GameTime gameTime)
        {
            if (Starfield != null)
                Starfield.Draw();

            Screen.spriteBatch.Begin(transformMatrix: Screen.GetCameraMatrix(), blendState: BlendState.NonPremultiplied, samplerState: SamplerState.LinearClamp);

            Screen.spriteBatch.DrawRectangle(new RectangleF(0, camEntity.Transform.Position.Y, Bounds.Width, Bounds.Height), Color.Gray, 2f);

            foreach (var item in BackgroundObjects)
            {
                item.Draw(gameTime);
            }

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

        public void AddBackgroundObject(BackgroundObject bgObj)
        {
            BackgroundObjects.Add(bgObj);
        }

        public void RemoveBackgroundObject(BackgroundObject bgObj)
        {
            BackgroundObjects.Remove(bgObj);
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

        internal void SetProgress(int startPoint)
        {
            this.startPoint = startPoint;

            Vector2 newCamera = new Vector2(Screen.ScreenWidth / 2f, Screen.ScreenHeight / 2f);
            Screen.SetCameraPosition(newCamera);
        }
    }
}
