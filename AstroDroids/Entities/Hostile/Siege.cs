using AstroDroids.Coroutines;
using AstroDroids.Entities.Friendly;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AstroDroids.Entities.Hostile
{
    public enum SiegeFlightMode
    {
        Freeroam,
        Locked
    }

    public class Siege : Enemy
    {
        Texture2D texture;

        float angle;

        SiegeFlightMode Flight = SiegeFlightMode.Freeroam;

        CoroutineInstance attackLoop;

        Vector2 targetPos;

        public Vector2 Left1Cannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f) + 15, -Height + 24), Vector2.Zero, angle); } }
        public Vector2 Left2Cannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f), -Height + 5), Vector2.Zero, angle); } }

        public Vector2 Right1Cannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f) + 15, Height - 24), Vector2.Zero, angle); } }
        public Vector2 Right2Cannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f), Height - 5), Vector2.Zero, angle); } }

        public bool moving { get; set; } = true;
        public bool firing { get; set; } = false;

        public Siege() : base(Vector2.Zero, 1000)
        {
            AddCircleCollider(Vector2.Zero, 40);
            texture = TextureManager.Get("Ships/Siege/ship_012");
        }

        public override void Destroyed()
        {
            base.Destroyed();

            if (attackLoop != null)
            {
                Scene.World.StopCoroutine(attackLoop);
                attackLoop = null;
            }
        }

        public override void Spawned()
        {
            angle = MathHelper.ToRadians(90);

            targetPos = new Vector2(Random.NextSingle(60, Scene.World.Bounds.Width - 60), 92);

            attackLoop = Scene.World.StartCoroutine(Behavior());
        }

        private bool IsAnyOtherSiegeFiring()
        {
            var enemies = Scene.World.Enemies;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] is Siege siege && siege != this && siege.firing)
                {
                    return true;
                }
            }
            return false;
        }

        IEnumerator Behavior()
        {
            while (true)
            {
                yield return new WaitUntil(() => !IsAnyOtherSiegeFiring());
                firing = true;
                yield return new WaitForSeconds(seconds: 1f);
                int choice = Random.Next(2);
                switch (choice)
                {
                    case 0:
                    default:
                        yield return Fire();
                        break;
                    case 1:
                        yield return FireShotgun();
                        break;
                }

                firing = false;

                yield return new WaitForSeconds(seconds: 2f);
            }
        }

        void Shoot(Vector2 cannonPos, float angle, float speed = 5f, float phaseSpeed = 0f, float phaseMax = 0f)
        {
            var projectile = new CircleProjectile(GameHelper.OrbitPos(cannonPos, angle, 20), angle, speed, 12f);
            projectile.SetPhase(phaseSpeed, phaseMax);
            Scene.World.AddProjectile(projectile, true);
        }

        public override void Update(GameTime gameTime)
        {
            float targetAngle = 0f;
            float dt = gameTime.GetElapsedSeconds();

            if (!moving)
            {
                Player player = Scene.World.GetRandomPlayer();
                if (player != null)
                {
                    targetAngle = GameHelper.AngleBetween(Transform.Position, player.Transform.Position);

                    angle = MathHelper.Lerp(angle, targetAngle, 8f * dt);
                }

                return;
            }

            Vector2 direction = targetPos - Transform.Position;
            float distance = direction.Length();

            const float maxSpeed = 100f;
            const float slowRadius = 60f;

            if (distance < 1f)
            {
                Transform.Position = targetPos;

                if (Flight == SiegeFlightMode.Freeroam)
                    targetPos = new Vector2(Random.NextSingle(60, Scene.World.Bounds.Width - 60), 92);
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

                targetAngle = neutralAngle + MathF.Sign(direction.X) * maxBank * t + MathHelper.ToRadians(180);

                angle = MathHelper.Lerp(angle, targetAngle, 8f * dt);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, Transform.Position, null, Color.White, angle, new Vector2(texture.Width / 2f, texture.Height / 2f), 0.7f, SpriteEffects.None, 0f);

            if (AstroDroidsGame.Debug)
            {
                Screen.spriteBatch.DrawCircle(Left1Cannon + Transform.Position, 12, 12, Color.Green);
                Screen.spriteBatch.DrawCircle(Left2Cannon + Transform.Position, 12, 12, Color.Green);

                Screen.spriteBatch.DrawCircle(Right1Cannon + Transform.Position, 12, 12, Color.Green);
                Screen.spriteBatch.DrawCircle(Right2Cannon + Transform.Position, 12, 12, Color.Green);
            }
        }

        IEnumerator Fire()
        {
            moving = false;

            for (int i = 0; i < 10; i++)
            {
                Shoot(Left2Cannon + Transform.Position, angle - 0.5f, 5, 10, 10);
                Shoot(Right2Cannon + Transform.Position, angle + 0.5f, 5, 10, 10);


                Shoot(Left1Cannon + Transform.Position, angle, 5);
                Shoot(Right1Cannon + Transform.Position, angle, 5);

                yield return new WaitForSeconds(0.2f);
            }
            moving = true;
        }

        IEnumerator FireShotgun()
        {
            moving = false;
            for (int i = 0; i < 10; i++)
            {
                List<float> angles = GameHelper.SpreadAngle(angle, 5, 35);
                List<float> angles2 = GameHelper.SpreadAngle(angle, 5, 45);
                for (int j = 0; j < angles.Count; j++)
                {
                    float item = i % 2 == 0 ? angles[j] : angles2[j];
                    Shoot(Left1Cannon + Transform.Position, item, 5, 0, 0);
                    Shoot(Right1Cannon + Transform.Position, item, 5, 0, 0);
                }

                yield return new WaitForSeconds(0.4f);
            }
            moving = true;
        }
    }
}
