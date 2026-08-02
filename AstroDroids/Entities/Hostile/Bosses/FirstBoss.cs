using AstroDroids.Coroutines;
using AstroDroids.Entities.Friendly;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AstroDroids.Entities.Hostile.Bosses
{
    public enum FirstBossFlightMode
    {
        Freeroam,
        Locked
    }

    public class FirstBoss : Enemy
    {
        Texture2D texture;

        float angle;

        FirstBossFlightMode Flight = FirstBossFlightMode.Freeroam;

        CoroutineInstance attackLoop;
        CoroutineInstance missilesLoop;

        Vector2 targetPos;

        float asteroidTimer = 0f;

        List<Vector2> cannons = new List<Vector2>();

        public Vector2 Left1Cannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f) + 15, -Height + 24), Vector2.Zero, angle); } }
        public Vector2 Left2Cannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f) + 23, -Height + 35), Vector2.Zero, angle); } }
        public Vector2 Left3Cannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f) + 30, -Height + 46), Vector2.Zero, angle); } }

        public Vector2 Right1Cannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f) + 15, Height - 24), Vector2.Zero, angle); } }
        public Vector2 Right2Cannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f) + 23, Height - 35), Vector2.Zero, angle); } }
        public Vector2 Right3Cannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f) + 30, Height - 46), Vector2.Zero, angle); } }

        public Vector2 LeftMissile { get { return GameHelper.RotateAroundPoint(new Vector2((-Width / 2f), -Height + 46), Vector2.Zero, angle); } }
        public Vector2 RightMissile { get { return GameHelper.RotateAroundPoint(new Vector2((-Width / 2f), Height - 46), Vector2.Zero, angle); } }


        public FirstBoss() : base(Vector2.Zero, 1000)
        {
            AddCircleCollider(Vector2.Zero, 70);
            texture = TextureManager.Get("Ships/FirstBoss/FirstBoss");
        }

        public override void Destroyed()
        {
            base.Destroyed();

            if (attackLoop != null)
            {
                Scene.World.StopCoroutine(attackLoop);
                attackLoop = null;
            }

            if (missilesLoop != null)
            {
                Scene.World.StopCoroutine(missilesLoop);
                missilesLoop = null;
            }

            if (Scene.World.BossEntity == this)
                Scene.World.BossEntity = null;
        }

        public override void Spawned()
        {
            angle = MathHelper.ToRadians(90);

            targetPos = new Vector2(Random.NextSingle(60, Scene.World.Bounds.Width - 60), 92);

            cannons.Add(Left1Cannon);
            cannons.Add(Left2Cannon);
            cannons.Add(Left3Cannon);
            cannons.Add(Right3Cannon);
            cannons.Add(Right2Cannon);
            cannons.Add(Right1Cannon);

            attackLoop = Scene.World.StartCoroutine(BossBehavior());

            Scene.World.BossEntity = this;
        }

        Asteroid spawnAsteroid(float xPos, Vector2 pushDir)
        {
            Asteroid asteroid = new Asteroid();
            asteroid.Transform.Position = new Vector2(xPos, Random.NextSingle(200, Scene.World.Bounds.Height - 200));
            asteroid.Push(pushDir);
            //asteroid.FollowsCamera = true;
            return asteroid;
        }

        IEnumerator ConstantMissiles()
        {
            while (true)
            {
                Entity target = Scene.World.GetRandomPlayer();

                if (target == null)
                {
                    yield return new WaitForSeconds(5f);
                    continue;
                }

                ChallengerHomingMissile missile = new ChallengerHomingMissile(RightMissile + Transform.Position, target, MathHelper.ToRadians(-90));
                missile.SetHomingMaxTime(7f);
                Scene.World.AddProjectile(missile, true);

                missile = new ChallengerHomingMissile(LeftMissile + Transform.Position, target, MathHelper.ToRadians(-90));
                missile.SetHomingMaxTime(7f);
                Scene.World.AddProjectile(missile, true);

                yield return new WaitForSeconds(5f);
            }
        }

        IEnumerator BossBehavior()
        {
            var attackActions = new List<Func<IEnumerator>>()
            {
                CannonBurst1Attack,
                CannonBurst2Attack,
                MissileAttack,
                CannonBurst3Attack,
                CannonBurst4Attack,
                CannonBurst5Attack,
                CannonBurst6Attack
            };

            missilesLoop = Scene.World.StartCoroutine(ConstantMissiles());

            while (true)
            {
                attackActions.Shuffle(Random);

                foreach (var attack in attackActions)
                {
                    yield return attack();
                    yield return new WaitForSeconds(1.5f);
                }
            }
        }

        void FireShotgun(int cannonId, float angle, int shotsToFire, float spacing, float speed, float phaseSpeed = 0f, float phaseMax = 0f)
        {
            List<float> angles = null;

            angles = GameHelper.SpreadAngle(angle, shotsToFire, spacing);
            foreach (var item in angles)
            {
                ShootCannon(cannonId, item, speed, phaseSpeed, phaseMax);
            }
        }

        void FireArc(int cannonId, float startAngle, float endAngle, int bulletCount, float bulletSpeed)
        {
            if (bulletCount < 2) return;

            float angleStep = (endAngle - startAngle) / (bulletCount - 1);

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = MathHelper.ToRadians(startAngle + i * angleStep);

                ShootCannon(cannonId, angle, bulletSpeed);
            }
        }

        void ShootCannon(int id, float angle, float speed = 5f, float phaseSpeed = 0f, float phaseMax = 0f)
        {
            if (id >= cannons.Count || id < 0)
                return;

            Shoot(cannons[id] + Transform.Position, angle, speed, phaseSpeed, phaseMax);
        }

        void Shoot(Vector2 cannonPos, float angle, float speed = 5f, float phaseSpeed = 0f, float phaseMax = 0f)
        {
            var projectile = new CircleProjectile(GameHelper.OrbitPos(cannonPos, angle, 20), angle, speed, 12f);
            projectile.SetPhase(phaseSpeed, phaseMax);
            Scene.World.AddProjectile(projectile, true);
        }

        public override void Update(GameTime gameTime)
        {
            if (asteroidTimer <= 0f)
            {
                Scene.World.AddNeutral(spawnAsteroid(-Random.Next(30, 30), new Vector2(1, 0)), true, true);
                Scene.World.AddNeutral(spawnAsteroid(Scene.World.Bounds.Width + 30, new Vector2(-1, 0)), true, true);

                asteroidTimer = 3f;
            }

            asteroidTimer -= gameTime.GetElapsedSeconds();

            Vector2 direction = targetPos - Transform.Position;
            float distance = direction.Length();

            const float maxSpeed = 100f;
            const float slowRadius = 60f;

            float dt = gameTime.GetElapsedSeconds();

            if (distance < 1f)
            {
                Transform.Position = targetPos;

                if (Flight == FirstBossFlightMode.Freeroam)
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

                float targetAngle = neutralAngle + MathF.Sign(direction.X) * maxBank * t + MathHelper.ToRadians(180);

                angle = MathHelper.Lerp(angle, targetAngle, 8f * dt);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, Transform.Position, null, CanBeDamaged ? Color.White : Color.Red, angle, new Vector2(texture.Width / 2f, texture.Height / 2f), 0.7f, SpriteEffects.None, 0f);

            //Screen.DrawText($"Boss Health: {GetHealth()}/1000", new Vector2(20, 10), Color.White, 12f);

            if (AstroDroidsGame.Debug)
            {
                Screen.spriteBatch.DrawCircle(Left1Cannon + Transform.Position, 12, 12, Color.Green);
                Screen.spriteBatch.DrawCircle(Left2Cannon + Transform.Position, 12, 12, Color.Green);
                Screen.spriteBatch.DrawCircle(Left3Cannon + Transform.Position, 12, 12, Color.Green);

                Screen.spriteBatch.DrawCircle(Right1Cannon + Transform.Position, 12, 12, Color.Green);
                Screen.spriteBatch.DrawCircle(Right2Cannon + Transform.Position, 12, 12, Color.Green);
                Screen.spriteBatch.DrawCircle(Right3Cannon + Transform.Position, 12, 12, Color.Green);

                Screen.spriteBatch.DrawCircle(LeftMissile + Transform.Position, 12, 12, Color.Green);
                Screen.spriteBatch.DrawCircle(RightMissile + Transform.Position, 12, 12, Color.Green);
            }
        }

        #region Attacks

        IEnumerator CannonBurst1Attack()
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    for (int k = 0; k < cannons.Count; k++)
                    {
                        FireShotgun(k, MathHelper.ToRadians(90), 1, 2f, 6f);
                    }

                    yield return new WaitForSeconds(0.3f);
                }

                yield return new WaitForSeconds(0.5f);
            }

            yield return null;
        }

        IEnumerator CannonBurst2Attack()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    for (int k = 0; k < cannons.Count; k++)
                    {
                        if (k == 1 || k == 4)
                            FireShotgun(k, MathHelper.ToRadians(90), 3, 45f, 6f);
                    }

                    yield return new WaitForSeconds(0.3f);
                }

                yield return new WaitForSeconds(0.5f);
            }

            yield return null;
        }

        IEnumerator CannonBurst3Attack()
        {
            bool phase = false;

            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < cannons.Count; j++)
                {
                    if (j == 0 || j == 5)
                        FireShotgun(j, MathHelper.ToRadians(j == 0 ? 50 : 130), 1, 35f, 6f);

                    if (j == 2 || j == 3)
                    {
                        if (i % 5 == 0)
                        {
                            FireShotgun(j, MathHelper.ToRadians(phase ? 90 : j == 2 ? 70 : 110), 1, 35f, 6f);
                        }
                    }
                }

                phase = !phase;

                yield return new WaitForSeconds(0.1f);
            }


            yield return null;
        }

        IEnumerator CannonBurst4Attack()
        {
            float offset = 0f;

            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < cannons.Count; j++)
                {
                    if (j == 1 || j == 4)
                        FireShotgun(j, MathHelper.ToRadians(j == 1 ? 90 - offset : 90 + offset), 1, 35f, 6f, 0, 0);
                }

                offset += 35;

                yield return new WaitForSeconds(0.1f);
            }


            yield return null;
        }

        IEnumerator CannonBurst5Attack()
        {
            bool phase = false;
            float offset = 0f;

            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < cannons.Count; j++)
                {
                    if (j == 1 || j == 4)
                        FireShotgun(j, MathHelper.ToRadians(j == 1 ? 90 - offset : 90 + offset), 1, 35f, 6f, 5, 10);
                }

                if (!phase)
                {
                    offset += 35;

                    if (offset > 100)
                        phase = !phase;
                }
                else
                {
                    offset -= 35;

                    if (offset < -100)
                        phase = !phase;
                }

                yield return new WaitForSeconds(0.2f);
            }


            yield return null;
        }

        IEnumerator CannonBurst6Attack()
        {
            bool phase = false;
            float offset = 0f;

            for (int i = 0; i < 10; i++)
            {
                Player player = Scene.World.GetRandomPlayer();

                if (player != null)
                {
                    for (int j = 0; j < cannons.Count; j++)
                    {
                        if (j == 1 || j == 4)
                            FireShotgun(j, GameHelper.AngleBetween(Transform.Position, player.Transform.Position), 1, 35f, 6f, 10, 10);
                    }

                    if (!phase)
                    {
                        offset += 20;

                        if (offset > 100)
                            phase = !phase;
                    }
                    else
                    {
                        offset -= 20;

                        if (offset < -100)
                            phase = !phase;
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }


            yield return null;
        }

        IEnumerator MissileAttack()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (j % 2 == 0)
                    {
                        for (int k = 0; k < cannons.Count; k++)
                        {
                            if (k == 1 || k == 4)
                                FireShotgun(k, MathHelper.ToRadians(90), 2, 35f, 6f);
                        }
                    }

                    ChallengerHomingMissile missile = new ChallengerHomingMissile(LeftMissile + Transform.Position, Scene.World.GetRandomPlayer(), MathHelper.ToRadians(-90));
                    missile.SetHomingMaxTime(7f);
                    Scene.World.AddProjectile(missile, true);

                    missile = new ChallengerHomingMissile(RightMissile + Transform.Position, Scene.World.GetRandomPlayer(), MathHelper.ToRadians(-90));
                    missile.SetHomingMaxTime(7f);
                    Scene.World.AddProjectile(missile, true);
                    yield return new WaitForSeconds(1f);
                }

                yield return new WaitForSeconds(1.5f);
            }
        }

        #endregion
    }
}
