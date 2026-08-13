using AstroDroids.Coroutines;
using AstroDroids.Entities.Warnings;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using AstroDroids.Paths;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Linq;

namespace AstroDroids.Entities.Hostile
{
    internal class Shielder : Enemy
    {
        public float t = 0f;

        Texture2D texture;

        float angle = MathHelper.ToRadians(180);

        Vector2 targetPos;

        Enemy targetToShield = null;
        ShielderConnection connection = null;

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

        public override void Destroyed()
        {
            base.Destroyed();

            if (targetToShield != null)
            {
                targetToShield.ShieldedAmount = 0;
                targetToShield = null;
            }

            if(connection != null)
            {
                Scene.World.RemoveWarning(connection);
                connection = null;
            }
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

            if(connection != null && targetToShield != null)
            {
                connection.Transform.Position = Transform.Position;
                connection.target = targetToShield.Transform.Position;
            }

            if (targetToShield == null)
            {
                Enemy enemy = Scene.World.Enemies.OfType<Enemy>().FirstOrDefault(x => x != this && x.CanBeDamaged && !x.destroyed && x.CanBeShielded && x.ShieldedAmount == 0);
                if (enemy != null)
                {
                    targetToShield = enemy;
                    targetToShield.ShieldedAmount = 1;

                    connection = new ShielderConnection() { Transform = new Transform(Transform.Position), target = targetToShield.Transform.Position };
                    Scene.World.AddWarning(connection, true);
                }
            }
            else if (targetToShield.destroyed)
            {
                targetToShield = null;

                if (connection != null)
                {
                    Scene.World.RemoveWarning(connection);
                    connection = null;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, new Vector2(Transform.Position.X, Transform.Position.Y), null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), 0.8f, SpriteEffects.None, 0f);
        }
    }
}
