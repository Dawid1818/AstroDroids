using AstroDroids.Entities;
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
        static Dictionary<int, Type> entityTypes = new Dictionary<int, Type>();
        static Dictionary<int, ImTextureRef> entityPreviews = new Dictionary<int, ImTextureRef>();
        static bool previewsInitialized = false;

        public static void Initialize()
        {
            RegisterEnemy(0, typeof(Entities.Hostile.BasicEnemy));
            RegisterEnemy(1, typeof(Entities.Hostile.SpinLaser));
            RegisterEnemy(2, typeof(Entities.Hostile.DroneController));
            RegisterEnemy(3, typeof(Entities.Hostile.ProximityMine));
            RegisterEnemy(4, typeof(Entities.Hostile.TriGunTurret));
        }

        public static void InitializePreviews()
        {
            if (previewsInitialized)
                return;

            GraphicsDeviceManager manager = Screen.GetGraphicsManager();

            foreach (var entity in entityTypes)
            {
                Enemy enemy = (Enemy)Activator.CreateInstance(entity.Value);
                Rectangle bounds = enemy.ToRectangle();

                RenderTarget2D target = new RenderTarget2D(manager.GraphicsDevice, bounds.Width + (bounds.Width), bounds.Height + (bounds.Height));

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

        static void RegisterEnemy(int id, Type entity)
        {
            entityTypes[id] = entity;
        }

        public static Type GetEnemyType(int id)
        {
            if (entityTypes.TryGetValue(id, out Type type))
            {
                return type;
            }
            else
            {
                throw new Exception($"Enemy with ID {id} not found in EntityDatabase.");
            }
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
            return entityTypes.Values.ToList();
        }
    }
}
