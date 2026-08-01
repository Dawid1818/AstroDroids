using AstroDroids.Collections;
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
using AstroDroids.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AstroDroids.Gameplay
{
    public class GameWorld
    {
        public readonly RectangleF BaseBounds = new RectangleF(0, 0, 800, 600);
        public RectangleF Bounds = new Rectangle(0, 0, 800, 600);
        public RectangleF ExpandedBounds = new Rectangle(-100, -100, 900, 700);

        float targetZoom = 1f;
        RectangleF targetBounds = new RectangleF(0, 0, 800, 600);

        public AliveEntity BossEntity { get; set; }

        public Starfield Starfield { get; set; }

        public EntityList<CollidableEntity> AllCollidables { get; } = new EntityList<CollidableEntity>();

        public EntityList<AliveEntity> Enemies { get; } = new EntityList<AliveEntity>();
        public EntityList<AliveEntity> Neutrals { get; } = new EntityList<AliveEntity>();

        public EntityList<Entity> BackgroundObjects { get; } = new EntityList<Entity>();

        public EntityList<Projectile> Projectiles { get; } = new EntityList<Projectile>();

        public EntityList<EntityGroup> EntityGroups { get; } = new EntityList<EntityGroup>();

        public EntityList<Entity> Warnings { get; } = new EntityList<Entity>();

        public EntityList<Entity> Effects { get; } = new EntityList<Entity>();

        List<AttackWave> AttackWaves = new List<AttackWave>();
        List<AttackWave> AttackWavesToRemove = new List<AttackWave>();

        int startPoint = 0;

        List<Player> Players = new List<Player>();
        List<Player> PlayersToRemove = new List<Player>();

        public CameraEntity camEntity { get; private set; } = new CameraEntity();

        CoroutineManager coroutineManager = new CoroutineManager();

        public double speed { get; set; } = 2;

        public bool PauseWaves { get; set; } = false;

        int ongoingWaves = 0;

        double timePassed = 0;

        int currentWave = 0;

        public void Initialize()
        {
            AttackWaves.Clear();
            AttackWaves.AddRange(LevelManager.CurrentLevel.AttackWaves.Slice(startPoint, LevelManager.CurrentLevel.AttackWaves.Count - startPoint));

            if (AttackWaves.Count > 0)
                StartCoroutine(ProcessWaves());
        }

        IEnumerator ProcessWaves()
        {
            if (AttackWaves[0].Delay > 0)
                yield return new WaitForSeconds(AttackWaves[0].Delay);

            for (int i = 0; i < AttackWaves.Count; i++)
            {
                currentWave = i;

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

                    switch (nextWave.WaitStyle)
                    {
                        case WaveWaitStyle.None:
                            break;
                        case WaveWaitStyle.WaitForPreviousWave:
                            if (ongoingWaves > 0)
                                yield return new WaitUntil(() => ongoingWaves == 0);
                            break;
                        case WaveWaitStyle.WaitForAllEnemiesDefeated:
                            if (ongoingWaves > 0 || Enemies.Count > 0)
                                yield return new WaitUntil(() => Enemies.Count == 0 && ongoingWaves == 0);
                            break;
                        default:
                            break;
                    }

                    yield return new WaitUntil(() => ongoingWaves == 0);

                    if (PauseWaves)
                        yield return new WaitUntil(() => !PauseWaves);

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

            if (bgObjectN.HasPath)
            {
                bgObj.PathManager = new PathManager(bgObjectN.Path, bgObjectN.PathSpeed);
                bgObj.PathManager.Loop = bgObjectN.PathLoop;
                bgObj.PathManager.MinPath = bgObjectN.MinPath;
            }

            bgObj.FollowsCamera = bgObjectN.FollowsCamera;

            AddBackgroundObject(bgObj);

            ongoingWaves--;
        }

        IEnumerator SpawnEnemies(EnemySpawner spawner)
        {
            if (spawner.InitialDelay > 0)
                yield return new WaitForSeconds(spawner.InitialDelay);

            for (int i = 0; i < spawner.EnemyIDs.Count; i++)
            {
                EnemySpawnEntry entry = spawner.EnemyIDs[i];

                Type type = GameDatabase.GetEnemyType(entry.EnemyID);
                Enemy enemy = (Enemy)Activator.CreateInstance(type);

                if (spawner.HasPath)
                {
                    //var flattened = GameHelper.FlattenComposite(spawner.Path.Decompose().Cast<CatmullRomPath>().ToList());

                    enemy.PathManager = new PathManager(spawner.Path, spawner.PathSpeed);
                    //enemy.PathManager = new PathManager(flattened, spawner.PathSpeed);
                    enemy.PathManager.Loop = spawner.PathLoop;
                    enemy.PathManager.MinPath = spawner.MinPath;
                }

                enemy.FollowsCamera = spawner.FollowsCamera;

                AddEnemy(enemy, spawner.FollowsCamera, false, entry.SpawnData);

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
                var barrier = new LaserBarrier(node.Position, node.Id, node.Health, new Vector2(0, 2), node.Type);
                barriers[node.Id] = barrier;
            }

            foreach (var connection in spawner.Connections)
            {
                var b1 = barriers[connection.FirstBarrierID];
                var b2 = barriers[connection.SecondBarrierID];

                b1.AddConnection(b2);
                b2.AddConnection(b1);

                LaserBarrierBeam beam = new LaserBarrierBeam(b1.Type == LaserBarrierType.Relay || b2.Type == LaserBarrierType.Relay ? LaserBarrierType.Relay : LaserBarrierType.Normal, b1, b2, b1.Transform.LocalPosition, !b1.CanBeDamaged, connection.BlocksPlayerProjectiles);
                //beams.Add(item, beam);
                AddProjectile(beam, false);
            }


            foreach (var node in spawner.Nodes.Values)
            {
                var barrier = barriers[node.Id];

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
            LevelManager.CurrentLevel.RunEvent(eventN.EventId);

            ongoingWaves--;
        }

        public CoroutineInstance StartCoroutine(IEnumerator coro)
        {
            return coroutineManager.StartCoroutine(coro);
        }

        public void StopAllCoroutines()
        {
            coroutineManager.StopAllCoroutines();
        }

        public void StopCoroutine(CoroutineInstance instance)
        {
            coroutineManager.StopCoroutine(instance);
        }

        bool NearlyEqual(RectangleF a, RectangleF b, float limit = 0.01f)
        {
            return MathF.Abs(a.X - b.X) < limit &&
                   MathF.Abs(a.Y - b.Y) < limit &&
                   MathF.Abs(a.Width - b.Width) < limit &&
                   MathF.Abs(a.Height - b.Height) < limit;
        }

        public void Update(GameTime gameTime)
        {
            timePassed += gameTime.ElapsedGameTime.TotalSeconds;

            float t = 1f - MathF.Exp(-5f * gameTime.GetElapsedSeconds());

            if (!NearlyEqual(Bounds, targetBounds))
            {
                Bounds.Width = MathHelper.Lerp(Bounds.Width, targetBounds.Width, t);
                Bounds.Height = MathHelper.Lerp(Bounds.Height, targetBounds.Height, t);

                Bounds.X = MathHelper.Lerp(Bounds.X, targetBounds.X, t);
                Bounds.Y = MathHelper.Lerp(Bounds.Y, targetBounds.Y, t);
            }
            else
            {
                Bounds.Width = targetBounds.Width;
                Bounds.Height = targetBounds.Height;
                Bounds.X = targetBounds.X;
                Bounds.Y = targetBounds.Y;
            }

            const float ZoomLimit = 0.001f;

            float curZoom = Screen.GetCameraZoom();

            if (MathF.Abs(curZoom - targetZoom) > ZoomLimit)
            {
                curZoom = MathHelper.Lerp(curZoom, targetZoom, t);

                if (MathF.Abs(curZoom - targetZoom) < ZoomLimit)
                    curZoom = targetZoom;

                Screen.SetCameraZoom(curZoom);
            }

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

            Warnings.Update(gameTime);

            EntityGroups.Update(gameTime);

            Enemies.Update(gameTime);

            Neutrals.Update(gameTime);

            Projectiles.Update(gameTime);

            Effects.Update(gameTime);

            BackgroundObjects.Update(gameTime);
        }

        float debugY = 40;

        public void DrawDebugText(string text)
        {
            Screen.DrawText(text, new Vector2(10, debugY), Color.White, 12f);
            debugY += 20;
        }

        public void Draw(GameTime gameTime)
        {
            if (Starfield != null)
                Starfield.Draw();

            Screen.spriteBatch.Begin(transformMatrix: Screen.GetCameraMatrix(), blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointWrap);

            //Screen.spriteBatch.DrawRectangle(new RectangleF(0, 0, Bounds.Width, Bounds.Height), Color.Gray, 2f);

            BackgroundObjects.Draw(gameTime);

            Warnings.Draw(gameTime);

            Enemies.Draw(gameTime);

            Neutrals.Draw(gameTime);

            Projectiles.Draw(gameTime);

            foreach (var item in Players)
            {
                item.Draw(gameTime);

                RenderColliders(item);
            }

            Effects.Draw(gameTime);

            Screen.spriteBatch.End();
        }

        public void DrawDebug()
        {
            if (AstroDroidsGame.Debug)
            {
                debugY = 40f;

                Screen.spriteBatch.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.LinearClamp);
                DrawDebugText($"Time: {TimeSpan.FromSeconds(timePassed).ToString(@"hh\:mm\:ss")}");
                DrawDebugText($"Enemies: {Enemies.Count}");
                DrawDebugText($"Neutrals: {Neutrals.Count}");
                DrawDebugText($"Projectiles: {Projectiles.Count}");
                DrawDebugText($"Warnings: {Warnings.Count}");
                DrawDebugText($"Background Objects: {BackgroundObjects.Count}");
                DrawDebugText($"Effects: {Effects.Count}");
                DrawDebugText($"Waves: {currentWave + 1}/{AttackWaves.Count}");
                DrawDebugText($"Current Weapon: {GameStateManager.CurrentWeapon}");
                DrawDebugText($"Firepower: {GameStateManager.Firepower}/5");
                DrawDebugText($"Coroutines: {coroutineManager.Coroutines.Count}");
                Screen.spriteBatch.End();
            }
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

        public void AddEffect(Entity effect)
        {
            Effects.Add(effect);
        }

        public void RemoveEffect(Entity effect)
        {
            Effects.Remove(effect);
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
            }
            Warnings.Add(entity);
        }

        public void RemoveWarning(Entity entity)
        {
            Warnings.Remove(entity);
        }

        public void AddEnemy(Enemy enemy, bool followsCamera, bool invokeSpawned = true, IEnemySpawnData spawnData = null)
        {
            if (followsCamera)
            {
                enemy.Transform.SetParent(camEntity.Transform);
                enemy.Transform.LocalPosition -= camEntity.Transform.Position;
            }

            Enemies.Add(enemy);
            AllCollidables.Add(enemy);

            if (spawnData == null)
                spawnData = GameDatabase.CreateEnemySpawnData(enemy.GetType());

            //If the enemy entity hasn't been registered, the spawn data will not be found (for example in case of the laser barriers), so we just skip doing that in this case
            if (spawnData != null)
                enemy.ApplySpawnData(spawnData);

            if (invokeSpawned)
                enemy.Spawned();
        }

        public void AddNeutral(Enemy neutral, bool followsCamera, bool invokeSpawned = true, IEnemySpawnData spawnData = null)
        {
            if (followsCamera)
            {
                neutral.Transform.SetParent(camEntity.Transform);
                neutral.Transform.LocalPosition -= camEntity.Transform.Position;
            }

            Neutrals.Add(neutral);
            AllCollidables.Add(neutral);

            if (spawnData == null)
                spawnData = GameDatabase.CreateEnemySpawnData(neutral.GetType());

            //If the enemy entity hasn't been registered, the spawn data will not be found (for example in case of the laser barriers), so we just skip doing that in this case
            if (spawnData != null)
                neutral.ApplySpawnData(spawnData);

            if (invokeSpawned)
                neutral.Spawned();
        }

        public void RemoveNeutral(AliveEntity neutral)
        {
            Neutrals.Remove(neutral);
            AllCollidables.Remove(neutral);
        }

        public void RemoveEnemy(AliveEntity enemy)
        {
            Enemies.Remove(enemy);
            AllCollidables.Remove(enemy);
        }

        public void AddProjectile(Projectile projectile, bool followsCamera)
        {
            if (followsCamera)
            {
                projectile.Transform.SetParent(camEntity.Transform);
                projectile.Transform.LocalPosition -= camEntity.Transform.Position;
            }

            Projectiles.Add(projectile);
            AllCollidables.Add(projectile);
        }

        public void RemoveProjectile(Projectile projectile)
        {
            Projectiles.Remove(projectile);
            AllCollidables.Remove(projectile);
        }

        public void AddEntityGroup(EntityGroup group)
        {
            EntityGroups.Add(group);
        }

        public void RemoveEntityGroup(EntityGroup group)
        {
            EntityGroups.Remove(group);
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
            player.ApplyCustomization(SaveManager.curSave.Ship);
            player.Transform.SetParent(camEntity.Transform);
            Players.Add(player);
            AllCollidables.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            PlayersToRemove.Add(player);
            AllCollidables.Remove(player);
        }

        public void RequestPlayerRespawn(int index)
        {
            if(GameStateManager.GetLives() <= 0)
            {
                //Game over
                return;
            }
            coroutineManager.StartCoroutine(RespawnPlayer(index));
        }

        IEnumerator RespawnPlayer(int index)
        {
            yield return new WaitForSeconds(2);
            AddPlayer(new Player(index, new Vector2(Bounds.Width / 2 - 16, Bounds.Bottom - 64)));
        }

        internal void SetProgress(int startPoint)
        {
            this.startPoint = startPoint;

            Vector2 newCamera = new Vector2(Screen.ScreenWidth / 2f, Screen.ScreenHeight / 2f);
            Screen.SetCameraPosition(newCamera);
        }

        public void SetZoom(float scale)
        {
            targetBounds.Width = BaseBounds.Width / scale;
            targetBounds.Height = BaseBounds.Height / scale;

            targetBounds.X = BaseBounds.X + (BaseBounds.Width - targetBounds.Width) / 2;
            targetBounds.Y = BaseBounds.Y + (BaseBounds.Height - targetBounds.Height) / 2;

            targetZoom = scale;
        }
    }
}
