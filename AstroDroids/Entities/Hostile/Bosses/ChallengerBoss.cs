using AstroDroids.Coroutines;
using AstroDroids.Entities.Friendly;
using AstroDroids.Entities.Warnings;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Paths;
using AstroDroids.Projectiles.Hostile;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AstroDroids.Entities.Hostile.Bosses
{
    public enum ChallengerBossFlightMode
    {
        Freeroam,
        Locked
    }

    public class ChallengerBoss : Enemy
    {
        Texture2D texture;

        float angle;

        RandomMoveManager RMM;

        ChallengerBossFlightMode Flight = ChallengerBossFlightMode.Freeroam;

        CoroutineInstance attackLoop;

        public ChallengerBoss() : base(Vector2.Zero, 1000)
        {
            AddCircleCollider(Vector2.Zero, 80);
            texture = TextureManager.Get("Ships/ChallengerBoss/ChallengerBoss");
        }

        public override void Spawned()
        {
            RMM = new RandomMoveManager(Transform.LocalPosition);
            RMM.maxMoveDistance = 1000;
            RMM.SetNewPath2(angle);

            attackLoop = Scene.World.StartCoroutine(BossBehavior());
        }

        IEnumerator BossBehavior()
        {
            var attackActions = new List<Func<IEnumerator>>()
            {
                MissileAttack,
                BeamAttack,
                BeamAttackGuns,
                DirectBeamAttack,
                DirectBeamAttack2,
                SphereAttack,
                ShotgunAttack,
                GunDown,
                SpinFire,
                SpinFire2,
                SpinFire3
            };

            while (true)
            {
                attackActions.Shuffle(Random);

                foreach (var attack in attackActions)
                {
                    yield return attack();
                    yield return new WaitForSeconds(1f);
                }
            }
        }

        void FireShotgun(float angle, int shotsToFire, float spacing, float speed, float phaseSpeed = 0f, float phaseMax = 0f)
        {
            List<float> angles = null;

            angles = GameHelper.SpreadAngle(angle, shotsToFire, spacing);
            foreach (var item in angles)
            {
                Shoot(item, speed, phaseSpeed, phaseMax);
            }
        }

        void FireArc(Vector2 position, float startAngle, float endAngle, int bulletCount, float bulletSpeed)
        {
            if (bulletCount < 2) return;

            float angleStep = (endAngle - startAngle) / (bulletCount - 1);

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = MathHelper.ToRadians(startAngle + i * angleStep);

                Shoot(angle, bulletSpeed);
            }
        }

        void Shoot(float angle, float speed = 5f, float phaseSpeed = 0f, float phaseMax = 0f)
        {
            var projectile = new CircleProjectile(GameHelper.OrbitPos(Transform.Position, angle, 20), angle, speed, 12f);
            projectile.SetPhase(phaseSpeed, phaseMax);
            Scene.World.AddProjectile(projectile, true);
        }

        public override void Update(GameTime gameTime)
        {
            RMM.Update(gameTime);
            Transform.LocalPosition = RMM.Position;
            angle = RMM.MovementAngle;

            if (Flight == ChallengerBossFlightMode.Freeroam && !RMM.Active)
            {
                RMM.SetNewPath2(angle);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, Transform.Position, null, CanBeDamaged ? Color.White : Color.Red, angle, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, 0f);

            Screen.DrawText($"Boss Health: {GetHealth()}/1000", new Vector2(20, 10), Color.White, 12f);
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

        public void GoTowards(Vector2 position, float? arrivalAngle = null)
        {
            RMM.UpdatePosition(Transform.Position);
            IPath path = GameHelper.CreateCatmull(Transform.Position, position, angle, arrivalAngle);
            RMM.SetPath(path);
        }

        public bool DestinationReached()
        {
            return !RMM.Active;
        }

        #region Attacks

        IEnumerator MissileAttack()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(3);

                for (int j = 0; j < 5; j++)
                {
                    ChallengerHomingMissile missile = new ChallengerHomingMissile(Transform.Position, Scene.World.GetRandomPlayer(), MathHelper.ToRadians(Random.Next(0, 360)));

                    Scene.World.AddProjectile(missile, true);
                }
            }
        }

        IEnumerator DirectBeamAttackBase(int totalShots, int extraBeamsPerSide, float angleOffsetDegrees, bool shootAhead, bool shootBehind, bool setTarget, float delay)
        {
            for (int i = 0; i < totalShots; i++)
            {
                Player player = Scene.World.GetRandomPlayer();

                if (player != null)
                {
                    Vector2 targetPlayerPos = player.Transform.Position;
                    float baseAngle = GameHelper.AngleBetween(Transform.Position, targetPlayerPos);

                    List<float> beamAngles = new List<float> { baseAngle };

                    for (int j = 1; j <= extraBeamsPerSide; j++)
                    {
                        float angleRad = MathHelper.ToRadians(angleOffsetDegrees * j);

                        if (shootAhead)
                            beamAngles.Add(baseAngle + angleRad);

                        if (shootBehind)
                            beamAngles.Add(baseAngle - angleRad);
                    }

                    List<ChallengerBeamWarning> warnings = new List<ChallengerBeamWarning>();
                    foreach (var angle in beamAngles)
                    {
                        float angleOffset = baseAngle - angle;

                        var warning = new ChallengerBeamWarning(new Vector2(Transform.Position.X, Transform.Position.Y), angle, 1024f);
                        warning.Follow = this;
                        if (setTarget)
                        {
                            warning.Target = targetPlayerPos;
                            warning.AngleOffset = angleOffset;
                        }
                        warnings.Add(warning);
                        Scene.World.AddWarning(warning, true);
                    }

                    yield return new WaitForSeconds(delay);

                    foreach (var w in warnings)
                    {
                        Scene.World.RemoveWarning(w);
                    }

                    List<float> finalAngles = new List<float> { baseAngle };
                    for (int j = 1; j <= extraBeamsPerSide; j++)
                    {
                        float angleRad = MathHelper.ToRadians(angleOffsetDegrees * j);

                        if (shootAhead)
                            finalAngles.Add(baseAngle + angleRad);
                        if (shootBehind)
                            finalAngles.Add(baseAngle - angleRad);
                    }

                    foreach (var angle in finalAngles)
                    {
                        Scene.World.AddProjectile(new ChallengerBeam(new Vector2(Transform.Position.X, Transform.Position.Y), angle, 1024f), true);
                    }

                    yield return new WaitForSeconds(0.3f);
                }
            }
        }

        IEnumerator DirectBeamAttack()
        {
            Flight = ChallengerBossFlightMode.Locked;

            yield return DirectBeamAttackBase(totalShots: 2, 5, 40f, true, true, false, 2f);

            Flight = ChallengerBossFlightMode.Freeroam;
        }

        IEnumerator DirectBeamAttack2()
        {
            yield return DirectBeamAttackBase(totalShots: 5, 1, 5f, true, true, true, 1f);
        }
        IEnumerator BeamAttack()
        {
            int x = 0;
            int y = 0;

            Flight = ChallengerBossFlightMode.Locked;
            GoTowards(new Vector2(Scene.World.Bounds.Center.X, Scene.World.Bounds.Center.Y));

            yield return new WaitUntil(() => DestinationReached());

            ChallengerBeamWarning beamW = new ChallengerBeamWarning(Transform.Position, MathHelper.ToRadians(-90), 1024f);
            ChallengerBeamWarning beam2W = new ChallengerBeamWarning(Transform.Position, MathHelper.ToRadians(90), 1024f);
            ChallengerBeamWarning beam3W = new ChallengerBeamWarning(Transform.Position, MathHelper.ToRadians(0), 1024f);
            ChallengerBeamWarning beam4W = new ChallengerBeamWarning(Transform.Position, MathHelper.ToRadians(180), 1024f);

            Scene.World.AddWarning(beamW, true);
            Scene.World.AddWarning(beam2W, true);
            Scene.World.AddWarning(beam3W, true);
            Scene.World.AddWarning(beam4W, true);

            yield return new WaitForSeconds(1f);

            Scene.World.RemoveWarning(beamW);
            Scene.World.RemoveWarning(beam2W);
            Scene.World.RemoveWarning(beam3W);
            Scene.World.RemoveWarning(beam4W);

            while (y <= 5)
            {
                ChallengerBeam beam = new ChallengerBeam(Transform.Position, MathHelper.ToRadians(-90), 1024f);
                ChallengerBeam beam2 = new ChallengerBeam(Transform.Position, MathHelper.ToRadians(90), 1024f);
                ChallengerBeam beam3 = new ChallengerBeam(Transform.Position, MathHelper.ToRadians(0), 1024f);
                ChallengerBeam beam4 = new ChallengerBeam(Transform.Position, MathHelper.ToRadians(180), 1024f);

                beam.Locked = true;
                beam2.Locked = true;
                beam3.Locked = true;
                beam4.Locked = true;

                Scene.World.AddProjectile(beam, true);
                Scene.World.AddProjectile(beam2, true);
                Scene.World.AddProjectile(beam3, true);
                Scene.World.AddProjectile(beam4, true);

                for (int i = 0; i < 2; i++)
                {
                    ChallengerHomingMissile missile = new ChallengerHomingMissile(Transform.Position, Scene.World.GetRandomPlayer(), MathHelper.ToRadians(Random.Next(0, 360)));

                    Scene.World.AddProjectile(missile, true);
                }

                float degrees = Random.Next(0, 2) == 0 ? -1 : 1;

                while (x <= 200)
                {
                    yield return new WaitForSeconds(0);

                    beam.Transform.Position = Transform.Position;
                    beam2.Transform.Position = Transform.Position;
                    beam3.Transform.Position = Transform.Position;
                    beam4.Transform.Position = Transform.Position;

                    beam.Angle = beam.Angle + MathHelper.ToRadians(degrees);
                    beam2.Angle = beam2.Angle + MathHelper.ToRadians(degrees);
                    beam3.Angle = beam3.Angle + MathHelper.ToRadians(degrees);
                    beam4.Angle = beam4.Angle + MathHelper.ToRadians(degrees);

                    x++;
                }

                beam.Locked = false;
                beam2.Locked = false;
                beam3.Locked = false;
                beam4.Locked = false;

                y++;
                x = 0;

                Scene.World.AddWarning(beamW, true);
                Scene.World.AddWarning(beam2W, true);
                Scene.World.AddWarning(beam3W, true);
                Scene.World.AddWarning(beam4W, true);

                yield return new WaitForSeconds(1);

                Scene.World.RemoveWarning(beamW);
                Scene.World.RemoveWarning(beam2W);
                Scene.World.RemoveWarning(beam3W);
                Scene.World.RemoveWarning(beam4W);

            }

            Flight = ChallengerBossFlightMode.Freeroam;
        }

        IEnumerator BeamAttackGuns()
        {
            int x = 0;
            int y = 0;

            Flight = ChallengerBossFlightMode.Locked;
            GoTowards(new Vector2(Scene.World.Bounds.Center.X, Scene.World.Bounds.Center.Y));

            yield return new WaitUntil(() => DestinationReached());

            ChallengerBeamWarning beamW = new ChallengerBeamWarning(new Vector2(Transform.Position.X, Transform.Position.Y), MathHelper.ToRadians(-90), 1024f);
            ChallengerBeamWarning beam2W = new ChallengerBeamWarning(new Vector2(Transform.Position.X, Transform.Position.Y), MathHelper.ToRadians(90), 1024f);
            ChallengerBeamWarning beam3W = new ChallengerBeamWarning(new Vector2(Transform.Position.X, Transform.Position.Y), MathHelper.ToRadians(0), 1024f);
            ChallengerBeamWarning beam4W = new ChallengerBeamWarning(new Vector2(Transform.Position.X, Transform.Position.Y), MathHelper.ToRadians(180), 1024f);

            Scene.World.AddWarning(beamW, true);
            Scene.World.AddWarning(beam2W, true);
            Scene.World.AddWarning(beam3W, true);
            Scene.World.AddWarning(beam4W, true);

            yield return new WaitForSeconds(1f);

            Scene.World.RemoveWarning(beamW);
            Scene.World.RemoveWarning(beam2W);
            Scene.World.RemoveWarning(beam3W);
            Scene.World.RemoveWarning(beam4W);

            while (y <= 5)
            {
                ChallengerBeam beam = new ChallengerBeam(Transform.Position, MathHelper.ToRadians(-90), 1024f);
                ChallengerBeam beam2 = new ChallengerBeam(Transform.Position, MathHelper.ToRadians(90), 1024f);
                ChallengerBeam beam3 = new ChallengerBeam(Transform.Position, MathHelper.ToRadians(0), 1024f);
                ChallengerBeam beam4 = new ChallengerBeam(Transform.Position, MathHelper.ToRadians(180), 1024f);

                beam.Locked = true;
                beam2.Locked = true;
                beam3.Locked = true;
                beam4.Locked = true;

                Scene.World.AddProjectile(beam, true);
                Scene.World.AddProjectile(beam2, true);
                Scene.World.AddProjectile(beam3, true);
                Scene.World.AddProjectile(beam4, true);

                Scene.World.StartCoroutine(GunDownForBeam());

                float degrees = Random.Next(0, 2) == 0 ? -1 : 1;

                while (x <= 200)
                {
                    yield return new WaitForSeconds(0);

                    beam.Transform.Position = Transform.Position;
                    beam2.Transform.Position = Transform.Position;
                    beam3.Transform.Position = Transform.Position;
                    beam4.Transform.Position = Transform.Position;

                    beam.Angle = beam.Angle + MathHelper.ToRadians(degrees);
                    beam2.Angle = beam2.Angle + MathHelper.ToRadians(degrees);
                    beam3.Angle = beam3.Angle + MathHelper.ToRadians(degrees);
                    beam4.Angle = beam4.Angle + MathHelper.ToRadians(degrees);

                    x++;
                }

                beam.Locked = false;
                beam2.Locked = false;
                beam3.Locked = false;
                beam4.Locked = false;

                y++;
                x = 0;

                Scene.World.AddWarning(beamW, true);
                Scene.World.AddWarning(beam2W, true);
                Scene.World.AddWarning(beam3W, true);
                Scene.World.AddWarning(beam4W, true);

                yield return new WaitForSeconds(1);

                Scene.World.RemoveWarning(beamW);
                Scene.World.RemoveWarning(beam2W);
                Scene.World.RemoveWarning(beam3W);
                Scene.World.RemoveWarning(beam4W);

            }

            Flight = ChallengerBossFlightMode.Freeroam;
        }
        IEnumerator SphereAttack()
        {
            for (int i = 0; i < 5; i++)
            {
                FireArc(Transform.Position, 0, 360, 10, 5f);
                yield return new WaitForSeconds(0.8f);
            }

            yield return null;
        }
        IEnumerator ShotgunAttack()
        {
            for (int i = 0; i < 10; i++)
            {
                Player player = Scene.World.GetRandomPlayer();
                if (player != null)
                    FireShotgun(GameHelper.AngleBetween(Transform.Position, player.Transform.Position), 5, 40f, 5f);
                yield return new WaitForSeconds(0.5f);
            }

            yield return null;
        }

        IEnumerator GunDown()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Player player = Scene.World.GetRandomPlayer();
                    if (player != null)
                        FireShotgun(GameHelper.AngleBetween(Transform.Position, player.Transform.Position), 2, 10f, 10f);
                    yield return new WaitForSeconds(0.1f);
                }

                yield return new WaitForSeconds(0.5f);
            }

            yield return null;
        }

        IEnumerator GunDownForBeam()
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (j % 4 == 0)
                    {
                        Player player = Scene.World.GetRandomPlayer();
                        if (player != null)
                            FireShotgun(GameHelper.AngleBetween(Transform.Position, player.Transform.Position), 8, 260f, 4f);
                    }
                    yield return new WaitForSeconds(0.3f);
                }

                yield return new WaitForSeconds(0.5f);
            }


            yield return null;
        }

        IEnumerator SpinFireBase(int volleys, float timeBetweenVolleys, int bulletsPerArc, float arcSpread, float bulletSpeed, Action<int>? volleyAction = null)
        {
            Flight = ChallengerBossFlightMode.Locked;
            GoTowards(new Vector2(Scene.World.Bounds.Center.X, Scene.World.Bounds.Center.Y));

            yield return new WaitUntil(() => DestinationReached());
            yield return new WaitForSeconds(0.6f);

            const float rotationSpeed = 15f;
            bool rotateClockwise = Random.Shared.Next(2) == 0;

            float baseAngle = Random.Shared.NextSingle() * 360f;

            for (int i = 0; i < volleys; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    float centerAngle = baseAngle + j * 90f;

                    for (int k = 0; k < bulletsPerArc; k++)
                    {
                        float t = bulletsPerArc == 1 ? 0.5f : (float)k / (bulletsPerArc - 1);

                        float angleOffset = MathHelper.Lerp(-arcSpread / 2f, arcSpread / 2f, t);
                        float finalAngle = centerAngle + angleOffset;

                        Shoot(finalAngle, bulletSpeed);
                    }
                }

                volleyAction?.Invoke(i);

                baseAngle += rotateClockwise ? rotationSpeed : -rotationSpeed;

                yield return new WaitForSeconds(timeBetweenVolleys);
            }

            Flight = ChallengerBossFlightMode.Freeroam;
        }
        IEnumerator SpinFire()
        {
            yield return SpinFireBase(30, 0.2f, 5, 30f, 6f);
        }

        IEnumerator SpinFire2()
        {
            yield return SpinFireBase(10, 1.5f, 5, 30f, 3f,
                i =>
                {
                    if (i % 3 != 0)
                        return;

                    for (int c = 0; c < 2; c++)
                    {
                        var missile = new ChallengerHomingMissile(Transform.Position, Scene.World.GetRandomPlayer(), MathHelper.ToRadians(Random.Next(0, 360)));

                        Scene.World.AddProjectile(missile, true);
                    }
                });
        }

        IEnumerator SpinFire3()
        {
            yield return SpinFireBase(15, 1f, 3, 40f, 3f,
                i =>
                {
                    if (i % 3 != 0)
                        return;

                    Player player = Scene.World.GetRandomPlayer();
                    if (player != null)
                    {
                        FireShotgun(GameHelper.AngleBetween(Transform.Position, player.Transform.Position), 2, 10f, 5f);
                    }
                });
        }

        #endregion
    }
}
