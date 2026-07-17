using AstroDroids.Entities;
using AstroDroids.Entities.Hostile;
using AstroDroids.Entities.Hostile.Bosses;
using AstroDroids.Graphics;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AstroDroids.Managers
{
    public class EntityDatabase
    {
        static Dictionary<int, EntityRegistration> entityTypes = new Dictionary<int, EntityRegistration>();
        static Dictionary<int, ImTextureRef> entityPreviews = new Dictionary<int, ImTextureRef>();
        static bool previewsInitialized = false;

        public static void Initialize()
        {
            RegisterEnemy(0, typeof(BasicEnemy), typeof(DefaultSpawnData));
            RegisterEnemy(1, typeof(SpinLaser), typeof(DefaultSpawnData));
            RegisterEnemy(2, typeof(DroneController), typeof(DefaultSpawnData));
            RegisterEnemy(3, typeof(ProximityMine), typeof(DefaultSpawnData));
            RegisterEnemy(4, typeof(TriGunTurret), typeof(DefaultSpawnData));
            RegisterEnemy(5, typeof(Gunner), typeof(GunnerSpawnData));
            RegisterEnemy(6, typeof(SnakeBoss), typeof(DefaultSpawnData));
            RegisterEnemy(7, typeof(DroneBoss), typeof(DefaultSpawnData));
            RegisterEnemy(8, typeof(ChallengerBoss), typeof(DefaultSpawnData));
            RegisterEnemy(9, typeof(LBBoss), typeof(DefaultSpawnData));
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

                if(bounds.Width == 0 || bounds.Height == 0)
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

            previewsInitialized = true;
        }

        static void RegisterEnemy(int id, Type entity, Type spawnData)
        {
            entityTypes[id] = new EntityRegistration { EnemyType = entity, SpawnDataType = spawnData };
        }

        public static Type GetEnemyType(int id)
        {
            if (entityTypes.TryGetValue(id, out EntityRegistration registration))
            {
                return registration.EnemyType;
            }
            else
            {
                throw new Exception($"Enemy with ID {id} not found in EntityDatabase.");
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
                throw new Exception($"Enemy with ID {id} not found in EntityDatabase.");
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
                throw new Exception($"Entity Preview with ID {id} not found in EntityDatabase.");
            }
        }

        public static List<Type> GetAllEnemyTypes()
        {
            return entityTypes.Values.Select(r => r.EnemyType).ToList();
        }
    }
}
