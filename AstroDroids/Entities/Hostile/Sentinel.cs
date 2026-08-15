using AstroDroids.Coroutines;
using AstroDroids.Entities.Friendly;
using AstroDroids.Entities.Hostile.Bosses;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Paths;
using AstroDroids.Projectiles.Hostile;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections;

namespace AstroDroids.Entities.Hostile
{
    public class Sentinel : Enemy
    {
        public float t = 0f;

        Texture2D texture;

        float angle = MathHelper.ToRadians(180);

        public Vector2 Left1Cannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f) + 20, -Height + 20), Vector2.Zero, angle); } }
        public Vector2 Right1Cannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f) + 20, Height - 20), Vector2.Zero, angle); } }
        public Vector2 MiddleCannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f) + 20, Height - 50), Vector2.Zero, angle); } }

        CoroutineInstance behavior;
        CoroutineInstance fireLoop;

        bool charging = false;

        Vector2 targetPos;

        SentinelCircleProjectile projectile;

        public Sentinel() : base(Vector2.Zero, 150)
        {
            CanBeShielded = true;
            texture = TextureManager.Get("Ships/Sentinel/ship003_black1");
            Score = 100;

            AddCircleCollider(Vector2.Zero, 50f);

            HorizontalShieldRadius = 50f;
            VerticalShieldRadius = 50f;
        }

        public override void Spawned()
        {
            targetPos = new Vector2(Random.NextSingle(60, Scene.World.Bounds.Width - 60), Random.NextSingle(92, 120));

            angle = MathHelper.ToRadians(90);

            behavior = Scene.World.StartCoroutine(Behavior());
        }

        public override void Destroyed()
        {
            base.Destroyed();

            if(fireLoop != null)
            {
                Scene.World.StopCoroutine(fireLoop);
                fireLoop = null;
            }

            if (behavior != null)
            {
                Scene.World.StopCoroutine(behavior);
                behavior = null;
            }

            if(projectile != null)
            {
                float angle = MathHelper.ToRadians(90);
                Player player = Scene.World.GetRandomPlayer();
                if (player != null)
                    angle = GameHelper.AngleBetween(Transform.Position, player.Transform.Position);

                projectile.Angle = angle;

                projectile.Speed = 3f;
                projectile = null;
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

            if (projectile != null && charging)
            {
                projectile.Transform.Position = MiddleCannon + Transform.Position + new Vector2(0, 10);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, new Vector2(Transform.Position.X, Transform.Position.Y), null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), 0.8f, SpriteEffects.None, 0f);

            if (charging)
            {
                Screen.shapeBatch.FillLineBlurred(Left1Cannon + Transform.Position, MiddleCannon + Transform.Position + new Vector2(0, 10), 2, Color.DarkRed, 5f);
                Screen.shapeBatch.FillLineBlurred(Right1Cannon + Transform.Position, MiddleCannon + Transform.Position + new Vector2(0, 10), 2, Color.DarkRed, 5f);
                Screen.shapeBatch.FillLineBlurred(MiddleCannon + Transform.Position, MiddleCannon + Transform.Position + new Vector2(0, 10), 2, Color.DarkRed, 5f);
            }

            if (AstroDroidsGame.Debug)
            {
                Screen.spriteBatch.DrawCircle(Left1Cannon + Transform.Position, 12, 12, Color.Green);
                Screen.spriteBatch.DrawCircle(Right1Cannon + Transform.Position, 12, 12, Color.Green);
                Screen.spriteBatch.DrawCircle(MiddleCannon + Transform.Position, 12, 12, Color.Green);
            }
        }

        IEnumerator Behavior()
        {
            while(true)
            {
                int choice = Random.Next(2);

                switch (choice)
                {
                    case 0:
                    default:
                        for (int i = 0; i < 3; i++)
                        {
                            yield return Fire();
                        }
                        break;
                    case 1:
                        for (int i = 0; i < 3; i++)
                        {
                            yield return FirePop();
                        }
                        break;
                }

                yield return new WaitForSeconds(2f);
            }
        }

        IEnumerator Fire()
        {
            charging = true;
            projectile = new SentinelCircleProjectile(MiddleCannon + Transform.Position + new Vector2(0, 10), 0, 0, 0, 32f);
            Scene.World.AddProjectile(projectile, true);

            yield return new WaitUntil(() => projectile.Size >= 32f);

            float angle = MathHelper.ToRadians(90);

            Player player = Scene.World.GetRandomPlayer();
            if (player != null)
                angle = GameHelper.AngleBetween(Transform.Position, player.Transform.Position);

            projectile.Angle = angle;

            projectile.CanDetonate = false;
            projectile.Speed = 3f;
            projectile.AddOrbit();
            projectile = null;
            charging = false;
        }

        IEnumerator FirePop()
        {
            charging = true;
            projectile = new SentinelCircleProjectile(MiddleCannon + Transform.Position + new Vector2(0, 10), 0, 0, 0, 32f);
            Scene.World.AddProjectile(projectile, true);

            yield return new WaitUntil(() => projectile.Size >= 32f);

            float angle = MathHelper.ToRadians(90);

            Player player = Scene.World.GetRandomPlayer();
            if (player != null)
                angle = GameHelper.AngleBetween(Transform.Position, player.Transform.Position);

            projectile.Angle = angle;

            projectile.CanDetonate = true;
            projectile.Speed = 3f;
            //projectile = null;
            charging = false;

            yield return new WaitForSeconds(0.5f);

            var laserProj = new SentinelLaser(MiddleCannon + Transform.Position + new Vector2(0, 10), GameHelper.GetShootingAngle(MiddleCannon + Transform.Position + new Vector2(0, 10), projectile.Transform.Position, GameHelper.DirFromAngle(projectile.Angle) * projectile.Speed, 5f, 5), 5f, 12f);
            Scene.World.AddProjectile(laserProj, true);

            projectile = null;
        }
    }
}
