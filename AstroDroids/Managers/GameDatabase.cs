using AstroDroids.Entities;
using AstroDroids.Entities.Hostile;
using AstroDroids.Entities.Hostile.Bosses;
using AstroDroids.Gameplay;
using AstroDroids.Graphics;
using AstroDroids.Levels;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AstroDroids.Managers
{
    public class GameDatabase
    {
        static Dictionary<int, EntityRegistration> entityTypes = new Dictionary<int, EntityRegistration>();
        static Dictionary<int, ImTextureRef> entityPreviews = new Dictionary<int, ImTextureRef>();
        static Dictionary<int, ImTextureRef> starfieldPreviews = new Dictionary<int, ImTextureRef>();
        static bool previewsInitialized = false;

        static Dictionary<int, Type> eventHandlers = new Dictionary<int, Type>();

        static Dictionary<MissionType, GameMission> Missions = new Dictionary<MissionType, GameMission>();

        static Dictionary<int, string> Music = new Dictionary<int, string>();

        public static void Initialize()
        {
            RegisterEnemy(0, typeof(BasicEnemy), typeof(DefaultSpawnData));
            RegisterEnemy(1, typeof(SpinLaser), typeof(DefaultSpawnData));
            RegisterEnemy(2, typeof(DroneController), typeof(DroneControllerSpawnData));
            RegisterEnemy(3, typeof(ProximityMine), typeof(ProximityMineSpawnData));
            RegisterEnemy(4, typeof(TriGunTurret), typeof(DefaultSpawnData));
            RegisterEnemy(5, typeof(Gunner), typeof(GunnerSpawnData));
            RegisterEnemy(6, typeof(SnakeBoss), typeof(DefaultSpawnData));
            RegisterEnemy(7, typeof(DroneBoss), typeof(DefaultSpawnData));
            RegisterEnemy(8, typeof(ChallengerBoss), typeof(DefaultSpawnData));
            RegisterEnemy(9, typeof(LBBoss), typeof(DefaultSpawnData));
            RegisterEnemy(10, typeof(FirstBoss), typeof(DefaultSpawnData));
            RegisterEnemy(11, typeof(Asteroid), typeof(AsteroidSpawnData));
            RegisterEnemy(12, typeof(Sentinel), typeof(DefaultSpawnData));
            RegisterEnemy(13, typeof(Chaser), typeof(DefaultSpawnData));
            RegisterEnemy(14, typeof(Siege), typeof(DefaultSpawnData));
            RegisterEnemy(15, typeof(Overseer), typeof(DefaultSpawnData));
            RegisterEnemy(16, typeof(SolarKnight), typeof(DefaultSpawnData));
            RegisterEnemy(17, typeof(Shielder), typeof(DefaultSpawnData));

            RegisterEventHandler(0, typeof(LevelEventHandler));
            RegisterEventHandler(1, typeof(TestLevelEventHandler));

            RegisterMission(MissionType.Tutorial, new GameMission() { Name = "Tutorial", Type = MissionType.Tutorial, LevelNames = { "Tutorial" } });
            RegisterMission(MissionType.Story, new GameMission() { Name = "Story", Type = MissionType.Story, LevelNames = { "Level1", "Level2", "Level3", "Level4", "Level5" } });
            RegisterMission(MissionType.BossRush, new GameMission() { Name = "BossRush", Type = MissionType.BossRush, LevelNames = { "BossRush" } });

            RegisterMusic(0, "ZeroRanger - For Your Security");
            RegisterMusic(1, "Industria");
            RegisterMusic(2, "space_boss_battle_bpm175");
        }

        public static void InitializePreviews()
        {
            if (previewsInitialized)
                return;

            GraphicsDeviceManager manager = Screen.GetGraphicsManager();

            foreach (var entity in entityTypes)
            {
                Enemy enemy = (Enemy)Activator.CreateInstance(entity.Value.EnemyType);
                Rectangle bounds = enemy.ToRectangle();
                RenderTarget2D target;

                if (bounds.Width == 0 || bounds.Height == 0)
                {
                    target = new RenderTarget2D(manager.GraphicsDevice, 32, 32);
                    entityPreviews.Add(entity.Key, Screen.GetImGuiRenderer().BindTexture(target));
                    continue;
                }

                target = new RenderTarget2D(manager.GraphicsDevice, bounds.Width + (bounds.Width), bounds.Height + (bounds.Height));

                manager.GraphicsDevice.SetRenderTarget(target);
                manager.GraphicsDevice.Clear(Color.Transparent);

                Screen.spriteBatch.Begin();
                enemy.Transform.Position = new Vector2((bounds.Width / 2) + (bounds.Width) / 2, (bounds.Height / 2) + (bounds.Height) / 2);
                enemy.Draw(new GameTime());
                Screen.spriteBatch.End();

                manager.GraphicsDevice.SetRenderTarget(null);

                var textureRef = Screen.GetImGuiRenderer().BindTexture(target);

                entityPreviews.Add(entity.Key, textureRef);
            }

            List<Texture2D> starfields = TextureManager.GetStarfields();
            for (int i = 0; i < starfields.Count; i++)
            {
                RenderTarget2D target;

                target = new RenderTarget2D(manager.GraphicsDevice, 128, 128);

                manager.GraphicsDevice.SetRenderTarget(target);
                manager.GraphicsDevice.Clear(Color.Transparent);

                Screen.spriteBatch.Begin();
                Screen.spriteBatch.Draw(starfields[i], new Rectangle(0, 0, 128, 128), Color.White);
                Screen.spriteBatch.End();

                manager.GraphicsDevice.SetRenderTarget(null);

                var textureRef = Screen.GetImGuiRenderer().BindTexture(target);

                starfieldPreviews.Add(i + 1, textureRef);
            }

            previewsInitialized = true;
        }

        static void RegisterEnemy(int id, Type entity, Type spawnData)
        {
            entityTypes[id] = new EntityRegistration { EnemyType = entity, SpawnDataType = spawnData };
        }

        static void RegisterEventHandler(int id, Type eventHandlerType)
        {
            eventHandlers[id] = eventHandlerType;
        }

        static void RegisterMission(MissionType type, GameMission mission)
        {
            Missions[type] = mission;
        }

        static void RegisterMusic(int id, string musicPath)
        {
            Music[id] = musicPath;
        }

        public static Type GetEnemyType(int id)
        {
            if (entityTypes.TryGetValue(id, out EntityRegistration registration))
            {
                return registration.EnemyType;
            }
            else
            {
                throw new Exception($"Enemy with ID {id} not found in GameDatabase.");
            }
        }

        public static Type GetEventHandlerType(int id)
        {
            if (eventHandlers.TryGetValue(id, out Type eventHType))
            {
                return eventHType;
            }
            else
            {
                throw new Exception($"Level Event Handler with ID {id} not found in GameDatabase.");
            }
        }

        public static Type GetEnemySpawnDataType(int id)
        {
            if (entityTypes.TryGetValue(id, out EntityRegistration registration))
            {
                return registration.SpawnDataType;
            }
            else
            {
                throw new Exception($"Enemy with ID {id} not found in GameDatabase.");
            }
        }

        public static GameMission GetMission(MissionType type)
        {
            if (Missions.TryGetValue(type, out GameMission mission))
            {
                return mission;
            }
            else
            {
                throw new Exception($"Mission with type {type} not found in GameDatabase.");
            }
        }

        public static string GetMusic(int id)
        {
            if (Music.TryGetValue(id, out string musicPath))
            {
                return musicPath;
            }
            else
            {
                return string.Empty;
            }
        }

        public static LevelEventHandler CreateEventHandler(int id)
        {
            Type eventHandlerType = GetEventHandlerType(id);
            if (eventHandlerType != null)
            {
                return (LevelEventHandler)Activator.CreateInstance(eventHandlerType);
            }
            else
            {
                return null;
            }
        }

        public static IEnemySpawnData CreateEnemySpawnData(int id)
        {
            Type spawnDataType = GetEnemySpawnDataType(id);
            if (spawnDataType != null)
            {
                return (IEnemySpawnData)Activator.CreateInstance(spawnDataType);
            }
            else
            {
                return null;
            }
        }

        public static IEnemySpawnData CreateEnemySpawnData(Type enemyType)
        {
            var registration = entityTypes.Values.FirstOrDefault(r => r.EnemyType == enemyType);
            if (registration != null)
            {
                return (IEnemySpawnData)Activator.CreateInstance(registration.SpawnDataType);
            }
            return null;
        }

        public static ImTextureRef GetEntityPreview(int id)
        {
            if (entityPreviews.TryGetValue(id, out ImTextureRef textureRef))
            {
                return textureRef;
            }
            else
            {
                throw new Exception($"Entity Preview with ID {id} not found in GameDatabase.");
            }
        }

        public static ImTextureRef GetStarfieldPreview(int id)
        {
            if (starfieldPreviews.TryGetValue(id, out ImTextureRef textureRef))
            {
                return textureRef;
            }
            else
            {
                throw new Exception($"Starfield Preview with ID {id} not found in GameDatabase.");
            }
        }

        public static List<Type> GetAllEnemyTypes()
        {
            return entityTypes.Values.Select(r => r.EnemyType).ToList();
        }

        public static List<string> GetAllMusic()
        {
            return Music.Values.ToList();
        }

        public static Dictionary<int, Type> GetAllEventHandlers()
        {
            return eventHandlers;
        }
    }
}
