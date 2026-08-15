using AstroDroids.Entities.Warnings;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using AstroDroids.Paths;
using AstroDroids.Scenes;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AstroDroids.Entities.Hostile
{
    public class ShielderSpawnData : IEnemySpawnData
    {
        public int EnemiesToShield { get; set; } = 1;

        public void DrawEditor()
        {
            int enemies = EnemiesToShield;
            if (ImGui.InputInt("Enemies to shield", ref enemies))
            {
                EnemiesToShield = enemies;
            }
        }

        public void Load(BinaryReader reader, int version)
        {
            if (version >= 10)
            {
                EnemiesToShield = reader.ReadInt32();
            }
            else
            {
                EnemiesToShield = 1;
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(EnemiesToShield);
        }
    }

    public class ShieldedEntry
    {
        public Enemy Enemy { get; set; }
        public ShielderConnection Connection { get; set; }
    }

    public class Shielder : Enemy
    {
        public float t = 0f;

        Texture2D texture;

        float angle = MathHelper.ToRadians(180);

        Vector2 targetPos;

        int enemiesToShield = 1;

        List<ShieldedEntry> shielded = new List<ShieldedEntry>();
        //Enemy targetToShield = null;
        //ShielderConnection connection = null;

        public Shielder() : base(Vector2.Zero, 100)
        {
            texture = TextureManager.Get("Ships/Shielder/ship_022");
            Score = 100;

            AddCircleCollider(Vector2.Zero, 50f);
        }

        public override void Spawned()
        {
            targetPos = new Vector2(Random.NextSingle(60, Scene.World.Bounds.Width - 60), Random.NextSingle(92, 120));

            angle = MathHelper.ToRadians(90);
        }

        public override void ApplySpawnData(IEnemySpawnData spawnData)
        {
            ShielderSpawnData data = (ShielderSpawnData)spawnData;
            enemiesToShield = data.EnemiesToShield;
        }

        public override void Destroyed()
        {
            base.Destroyed();

            foreach (var item in shielded)
            {
                item.Enemy.ShieldedAmount = 0;
                Scene.World.RemoveWarning(item.Connection);
            }
            shielded.Clear();
        }

        public override void Update(GameTime gameTime)
        {
            if (PathManager != null && PathManager.Active)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
            }
            else
            {
                Vector2 direction = targetPos - Transform.Position;
                float distance = direction.Length();

                const float maxSpeed = 100f;
                const float slowRadius = 60f;

                float dt = gameTime.GetElapsedSeconds();

                if (distance < 1f)
                {
                    Transform.Position = targetPos;

                    targetPos = new Vector2(Random.NextSingle(60, Scene.World.Bounds.Width - 60), Random.NextSingle(92, 120));
                }
                else
                {
                    direction /= distance;

                    float speed = maxSpeed;

                    if (distance < slowRadius)
                        speed *= distance / slowRadius;

                    Transform.Position += direction * speed * dt;

                    float t = MathF.Abs(direction.X) * (speed / maxSpeed);

                    float neutralAngle = -MathF.PI / 2f;
                    float maxBank = 0.18f;

                    float targetAngle = neutralAngle + MathF.Sign(direction.X) * maxBank * t + MathHelper.ToRadians(180);

                    angle = MathHelper.Lerp(angle, targetAngle, 8f * dt);
                }
            }

            List<ShieldedEntry> toRemove = new List<ShieldedEntry>();

            foreach (var item in shielded)
            {
                if(item.Enemy.destroyed)
                {
                    toRemove.Add(item);
                }
            }

            foreach (var item in toRemove)
            {
                Scene.World.RemoveWarning(item.Connection);
                shielded.Remove(item);
            }
            toRemove.Clear();

            while(shielded.Count < enemiesToShield)
            {
                Enemy enemy = Scene.World.Enemies.OfType<Enemy>().FirstOrDefault(x => x != this && x.CanBeDamaged && !x.destroyed && x.CanBeShielded && x.ShieldedAmount == 0);
                if (enemy != null)
                {
                    var connection = new ShielderConnection() { Transform = new Transform(Transform.Position), target = enemy.Transform.Position };
                    shielded.Add(new ShieldedEntry() { Enemy = enemy, Connection = connection });
                    enemy.ShieldedAmount = 1;
                    Scene.World.AddWarning(connection, true);
                }
                else
                {
                    //there are no suitable enemies, abort
                    break;
                }
            }

            foreach (var item in shielded)
            {
                item.Connection.Transform.Position = Transform.Position;
                item.Connection.target = item.Enemy.Transform.Position;
            }

            //if (targetToShield == null)
            //{
            //    Enemy enemy = Scene.World.Enemies.OfType<Enemy>().FirstOrDefault(x => x != this && x.CanBeDamaged && !x.destroyed && x.CanBeShielded && x.ShieldedAmount == 0);
            //    if (enemy != null)
            //    {
            //        targetToShield = enemy;
            //        targetToShield.ShieldedAmount = 1;

            //        connection = new ShielderConnection() { Transform = new Transform(Transform.Position), target = targetToShield.Transform.Position };
            //        Scene.World.AddWarning(connection, true);
            //    }
            //}
            //else if (targetToShield.destroyed)
            //{
            //    targetToShield = null;

            //    if (connection != null)
            //    {
            //        Scene.World.RemoveWarning(connection);
            //        connection = null;
            //    }
            //}
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, new Vector2(Transform.Position.X, Transform.Position.Y), null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), 0.8f, SpriteEffects.None, 0f);
        }
    }
}
