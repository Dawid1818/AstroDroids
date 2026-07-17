using AstroDroids.Coroutines;
using AstroDroids.Entities.Friendly;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Levels;
using AstroDroids.Managers;
using AstroDroids.Paths;
using AstroDroids.Projectiles.Hostile;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AstroDroids.Entities.Hostile.Bosses
{
    public enum LBBossFlightMode
    {
        Freeroam,
        Locked
    }

    public class LBBoss : Enemy
    {
        Texture2D texture;

        float angle;

        RandomMoveManager RMM;

        LBBossFlightMode Flight = LBBossFlightMode.Freeroam;

        CoroutineInstance attackLoop;

        Vector2 targetPos;

        float speed = 50f;

        public LBBoss() : base(Vector2.Zero, 1000)
        {
            AddCircleCollider(Vector2.Zero, 60);
            texture = TextureManager.Get("Ships/LBBoss/ship_020");
        }

        public override void Spawned()
        {
            RMM = new RandomMoveManager(Transform.LocalPosition);
            RMM.maxMoveDistance = 1000;
            //RMM.SetNewPath2(angle);

            attackLoop = Scene.World.StartCoroutine(BossBehavior());

            targetPos = new Vector2(Random.Next(60, Scene.World.Bounds.Width - 60), Random.Next(60, 160));

            angle = MathHelper.ToRadians(-90);
        }

        IEnumerator BossBehavior()
        {
            var attackActions = new List<Func<IEnumerator>>()
            {
                HorizontalGatesAttack,
                TriGatesAttack,
                DiagonalGatesAttack,
                PoweredBarriersAttack,
                PoweredBarriers2Attack,
                SlalomAttack,
                ConstructAttack,
                TunnelAttack
            };

            //attackActions = new List<Func<IEnumerator>>()
            //{
            //    TunnelAttack
            //};

            attackActions = new List<Func<IEnumerator>>()
            {
                DiagonalGatesAttack
            };

            while (true)
            {
                attackActions.Shuffle(Random);

                foreach (var attack in attackActions)
                {
                    yield return attack();
                    yield return new WaitForSeconds(2f);
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
            Vector2 direction = targetPos - Transform.Position;
            float distance = direction.Length();

            const float maxSpeed = 100f;
            const float slowRadius = 60f;

            float dt = gameTime.GetElapsedSeconds();

            if (distance < 1f)
            {
                Transform.Position = targetPos;

                if (Flight == LBBossFlightMode.Freeroam) 
                    targetPos = new Vector2(Random.Next(60, Scene.World.Bounds.Width - 60), Random.Next(60, 160));
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

                float targetAngle = neutralAngle + MathF.Sign(direction.X) * maxBank * t;

                angle = MathHelper.Lerp(angle, targetAngle, 8f * dt);
            }

            //Transform.Position = new Vector2(Transform.Position.X + moveDir.X, Transform.Position.Y + moveDir.Y);

            //RMM.Update(gameTime);
            //Transform.LocalPosition = RMM.Position;
            //angle = RMM.MovementAngle;

            //if (Flight == LBBossFlightMode.Freeroam && !RMM.Active)
            //{
            //    RMM.SetNewPath2(angle);
            //}
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

        void ConnectBarriers(LaserBarrier barrier, LaserBarrier other, bool blockPlayerProjectiles)
        {
            if (barrier == null || other == null)
                return;

            other.AddConnection(barrier);
            barrier.AddConnection(other);

            LaserBarrierBeam beam = new LaserBarrierBeam(barrier.Type == LaserBarrierType.Relay || other.Type == LaserBarrierType.Relay ? LaserBarrierType.Relay : LaserBarrierType.Normal, barrier, other, barrier.Transform.LocalPosition, !barrier.CanBeDamaged, blockPlayerProjectiles);
            Scene.World.AddProjectile(beam, true);
        }

        #region Attacks

        IEnumerator HorizontalGatesAttack()
        {
            var instance = Scene.World.StartCoroutine(GunDown(2));

            for (int i = 0; i < 5; i++)
            {
                float barrierWidth = 16;
                int barrierCount = 10;

                float width = barrierWidth * barrierCount;
                float gap = (Scene.World.Bounds.Width - width) / (barrierCount + 1);

                float x = gap;

                LaserBarrier prev = null;

                int choice = Random.Next(barrierCount);

                for (int j = 0; j < barrierCount; j++)
                {
                    Vector2 desiredPos = new Vector2(x + barrierWidth * 0.5f, -16);

                    LaserBarrier barrier = new LaserBarrier(desiredPos, 0, j == choice ? 1 : -1, new Vector2(0, 2), Levels.LaserBarrierType.Normal);

                    ConnectBarriers(barrier, prev, false);

                    Scene.World.AddEnemy(barrier, false, true);

                    prev = barrier;

                    x += barrierWidth + gap;
                }

                yield return new WaitForSeconds(2f);
            }

            Scene.World.StopCoroutine(instance);
        }

        IEnumerator TriGatesAttack()
        {
            var instance = Scene.World.StartCoroutine(GunDown(1));

            int side = Random.Next(3);

            for (int i = 0; i < 5; i++)
            {
                float barrierWidth = 16;
                int barrierCount = 10;

                if (side == 1 || side == 2)
                    barrierCount -= 3;

                float totalDroneWidth = barrierWidth * barrierCount;

                float gap = 0;

                if (side == 0)
                    gap = (Scene.World.Bounds.Width - totalDroneWidth) / (barrierCount + 1);
                else if (side == 1 || side == 2)
                    gap = (Scene.World.Bounds.Height - totalDroneWidth) / (barrierCount + 1);

                float x = gap;

                LaserBarrier prev = null;

                int choice = Random.Next((barrierCount / 2) - 1, (barrierCount / 2) + 1);

                Vector2 safeSpot = Vector2.Zero;

                for (int j = 0; j < barrierCount; j++)
                {
                    if (choice == j)
                    {
                        x += barrierWidth + gap;
                        prev = null;

                        safeSpot = new Vector2(x + barrierWidth * 0.5f, -16);

                        continue;
                    }

                    Vector2 desiredPos = Vector2.Zero;
                    Vector2 moveDir = Vector2.Zero;
                    float speed = 4f;

                    switch (side) //top
                    {
                        case 0:
                            moveDir = new Vector2(0, speed);
                            desiredPos = new Vector2(x + barrierWidth * 0.5f, -16);
                            break;
                        case 1:
                            moveDir = new Vector2(speed, 0);
                            desiredPos = new Vector2(-16, x + barrierWidth * 0.5f);
                            break;
                        case 2:
                            moveDir = new Vector2(-speed, 0);
                            desiredPos = new Vector2(Scene.World.Bounds.Width + 16, x + barrierWidth * 0.5f);
                            break;
                    }

                    LaserBarrier barrier = new LaserBarrier(desiredPos, 0, j == choice ? 1 : -1, moveDir, Levels.LaserBarrierType.Normal);

                    ConnectBarriers(barrier, prev, false);

                    Scene.World.AddEnemy(barrier, false, true);

                    prev = barrier;
                    x += barrierWidth + gap;
                }

                int oldSide = side;
                side = Random.Next(3);
                float time = 3f;

                //switch (side) //top
                //{
                //    case 0:
                //        moveDir = new Vector2(0, speed);
                //        desiredPos = new Vector2(x + barrierWidth * 0.5f, -16);
                //        break;
                //    case 1:
                //        moveDir = new Vector2(0, -speed);
                //        desiredPos = new Vector2(x + barrierWidth * 0.5f, Scene.World.Bounds.Height + 16);
                //        break;
                //    case 2:
                //        moveDir = new Vector2(speed, 0);
                //        desiredPos = new Vector2(-16, x + barrierWidth * 0.5f);
                //        break;
                //    case 3:
                //        moveDir = new Vector2(-speed, 0);
                //        desiredPos = new Vector2(Scene.World.Bounds.Width + 16, x + barrierWidth * 0.5f);
                //        break;
                //}

                yield return new WaitForSeconds(time);
            }

            Scene.World.StopCoroutine(instance);
        }

        IEnumerator DiagonalGatesAttack()
        {
            var instance = Scene.World.StartCoroutine(GunDown(2));

            for (int i = 0; i < 5; i++)
            {
                int side = Random.Next(4);
                //side = 3;

                float barrierWidth = 16;
                int barrierCount = 10;

                float totalDroneWidth = barrierWidth * barrierCount;

                float gap = 0;

                gap = (900 - totalDroneWidth) / (barrierCount + 1);

                float x = -300;

                LaserBarrier prev = null;

                int choice = Random.Next((barrierCount / 2) - 1, (barrierCount / 2) + 1);

                Vector2 safeSpot = Vector2.Zero;

                for (int j = 0; j < barrierCount; j++)
                {
                    if (choice == j)
                    {
                        x += barrierWidth + gap;
                        prev = null;

                        safeSpot = new Vector2(x + barrierWidth * 0.5f, -16);

                        continue;
                    }

                    Vector2 desiredPos = Vector2.Zero;
                    Vector2 moveDir = Vector2.Zero;
                    float speed = 4f;

                    switch (side) //top left
                    {
                        case 0:
                            moveDir = new Vector2(speed, speed);
                            desiredPos = new Vector2(x + barrierWidth * 0.5f, -(x + barrierWidth * 0.5f));
                            break;
                        case 1: // top right
                            moveDir = new Vector2(-speed, speed);
                            desiredPos = new Vector2(Scene.World.Bounds.Width + (x + barrierWidth * 0.5f), (x + barrierWidth * 0.5f));
                            break;
                        case 2: // bottom right
                            moveDir = new Vector2(-speed, -speed);
                            desiredPos = new Vector2(Scene.World.Bounds.Width + (x + barrierWidth * 0.5f), Scene.World.Bounds.Height + -(x + barrierWidth * 0.5f));
                            break;
                        case 3: // bottom left
                            moveDir = new Vector2(speed, -speed);
                            desiredPos = new Vector2(x + barrierWidth * 0.5f, Scene.World.Bounds.Height + (x + barrierWidth * 0.5f));
                            break;
                    }

                    LaserBarrier barrier = new LaserBarrier(desiredPos, 0, j == choice ? 1 : -1, moveDir, Levels.LaserBarrierType.Normal);

                    ConnectBarriers(barrier, prev, false);

                    Scene.World.AddEnemy(barrier, false, true);

                    prev = barrier;
                    x += barrierWidth + gap;
                }

                float time = 2f;

                yield return new WaitForSeconds(time);
            }

            Scene.World.StopCoroutine(instance);
        }

        IEnumerator PoweredBarriersAttack()
        {
            //var instance = Scene.World.StartCoroutine(GunDown());

            for (int i = 0; i < 5; i++)
            {
                //side = 3;

                LaserBarrier barrier1 = new LaserBarrier(new Vector2(10, -16), 0, -1, new Vector2(0, 2), LaserBarrierType.Relay);
                LaserBarrier barrier2 = new LaserBarrier(new Vector2(Scene.World.Bounds.Width - 10, -16), 0, -1, new Vector2(0, 2), LaserBarrierType.Relay);

                List<LaserBarrier> normals = new List<LaserBarrier>();

                for (int j = 0; j < 3; j++)
                {
                    LaserBarrier normalBarrier = new LaserBarrier(new Vector2(Random.Next(30, Scene.World.Bounds.Width - 30), -50), 0, 1, new Vector2(0, 2), LaserBarrierType.Normal);
                    normals.Add(normalBarrier);
                }

                ConnectBarriers(barrier1, barrier2, false);

                foreach (var item in normals)
                {
                    ConnectBarriers(barrier1, item, false);
                    ConnectBarriers(barrier2, item, false);
                    Scene.World.AddEnemy(item, false, true);
                }

                Scene.World.AddEnemy(barrier1, false, true);
                Scene.World.AddEnemy(barrier2, false, true);

                float time = 2f;

                yield return new WaitForSeconds(time);
            }

            //Scene.World.StopCoroutine(instance);
        }

        List<LaserBarrier> createBox(float posX, float posY, float width, float height)
        {
            List<LaserBarrier> barriers = new List<LaserBarrier>();
            RectangleF rect = new RectangleF(posX, posY, width, height);

            barriers.Add(new LaserBarrier(new Vector2(rect.Left, rect.Top), 0, -1, new Vector2(0, 2), LaserBarrierType.Normal));
            barriers.Add(new LaserBarrier(new Vector2(rect.Right, rect.Top), 0, -1, new Vector2(0, 2), LaserBarrierType.Normal));
            barriers.Add(new LaserBarrier(new Vector2(rect.Right, rect.Bottom), 0, -1, new Vector2(0, 2), LaserBarrierType.Normal));
            barriers.Add(new LaserBarrier(new Vector2(rect.Left, rect.Bottom), 0, -1, new Vector2(0, 2), LaserBarrierType.Normal));

            barriers.Add(new LaserBarrier(new Vector2(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f), 0, 1, new Vector2(0, 2), LaserBarrierType.Normal));

            ConnectBarriers(barriers[0], barriers[1], true);
            ConnectBarriers(barriers[1], barriers[2], true);
            ConnectBarriers(barriers[2], barriers[3], true);
            ConnectBarriers(barriers[3], barriers[0], true);

            Scene.World.AddEnemy(barriers[0], false, true);
            Scene.World.AddEnemy(barriers[1], false, true);
            Scene.World.AddEnemy(barriers[2], false, true);
            Scene.World.AddEnemy(barriers[3], false, true);
            Scene.World.AddEnemy(barriers[4], false, true);

            return barriers;
        }

        IEnumerator PoweredBarriers2Attack()
        {
            var instance = Scene.World.StartCoroutine(GunDown(4));

            for (int i = 0; i < 5; i++)
            {
                //side = 3;

                LaserBarrier barrierLeft = new LaserBarrier(new Vector2(-16, -85), 0, -1, new Vector2(0, 2), LaserBarrierType.Normal);
                LaserBarrier barrierRight = new LaserBarrier(new Vector2(Scene.World.Bounds.Width + 16, -85), 0, -1, new Vector2(0, 2), LaserBarrierType.Normal);

                List<LaserBarrier> box1 = createBox(Random.Next(10, Scene.World.Bounds.Width / 2), -Random.Next(80, 150), 72, 72);
                List<LaserBarrier> box2 = createBox(Random.Next(Scene.World.Bounds.Width / 2, Scene.World.Bounds.Width - 82), -Random.Next(80, 150), 72, 72);

                ConnectBarriers(box1[4], box2[4], false);
                ConnectBarriers(box1[4], barrierLeft, false);
                ConnectBarriers(box2[4], barrierRight, false);

                Scene.World.AddEnemy(barrierLeft, false, true);
                Scene.World.AddEnemy(barrierRight, false, true);

                float time = 3f;

                yield return new WaitForSeconds(time);
            }

            Scene.World.StopCoroutine(instance);
        }

        IEnumerator SlalomAttack()
        {
            //var instance = Scene.World.StartCoroutine(GunDown());

            float stickingDist = 64;

            for (int i = 0; i < 5; i++)
            {
                //side = 3;

                int choice = Random.Next(2);

                int spawnTurrets = Random.Next(2);

                LaserBarrier outsideBarrier;
                LaserBarrier innerBarrier;

                List<LaserBarrier> stickingOut = new List<LaserBarrier>();

                if (choice == 0)
                {
                    outsideBarrier = new LaserBarrier(new Vector2(-16, -85), 0, -1, new Vector2(0, 2), LaserBarrierType.Normal);
                    innerBarrier = new LaserBarrier(new Vector2(Random.Next(16, Scene.World.Bounds.Width / 2), -85), 0, -1, new Vector2(0, 2), LaserBarrierType.Normal);

                    if (spawnTurrets == 1)
                    {
                        stickingOut.Add(new LaserBarrier(innerBarrier.Transform.Position + new Vector2(stickingDist, -stickingDist), 0, -1, new Vector2(0, 2), LaserBarrierType.Normal));
                        stickingOut.Add(new LaserBarrier(innerBarrier.Transform.Position + new Vector2(stickingDist + 20, 0), 0, -1, new Vector2(0, 2), LaserBarrierType.Normal));
                        stickingOut.Add(new LaserBarrier(innerBarrier.Transform.Position + new Vector2(stickingDist, stickingDist), 0, -1, new Vector2(0, 2), LaserBarrierType.Normal));
                    }
                }
                else
                {
                    outsideBarrier = new LaserBarrier(new Vector2(Scene.World.Bounds.Width + 16, -85), 0, -1, new Vector2(0, 2), LaserBarrierType.Normal);
                    innerBarrier = new LaserBarrier(new Vector2(Random.Next(Scene.World.Bounds.Width / 2, Scene.World.Bounds.Width - 16), -85), 0, -1, new Vector2(0, 2), LaserBarrierType.Normal);

                    if (spawnTurrets == 1)
                    {
                        stickingOut.Add(new LaserBarrier(innerBarrier.Transform.Position - new Vector2(stickingDist, -stickingDist), 0, -1, new Vector2(0, 2), LaserBarrierType.Normal));
                        stickingOut.Add(new LaserBarrier(innerBarrier.Transform.Position - new Vector2(stickingDist + 20, 0), 0, -1, new Vector2(0, 2), LaserBarrierType.Normal));
                        stickingOut.Add(new LaserBarrier(innerBarrier.Transform.Position - new Vector2(stickingDist, stickingDist), 0, -1, new Vector2(0, 2), LaserBarrierType.Normal));
                    }
                }

                ConnectBarriers(outsideBarrier, innerBarrier, false);

                Scene.World.AddEnemy(outsideBarrier, false, true);
                Scene.World.AddEnemy(innerBarrier, false, true);
                foreach (var item in stickingOut)
                {
                    ConnectBarriers(innerBarrier, item, false);
                    Scene.World.AddEnemy(item, false, true);

                    TriGunTurret turret = new TriGunTurret();
                    turret.Transform.Position = item.Transform.Position;
                    Scene.World.AddEnemy(turret, false, true);
                }

                float time = 3f;

                if (spawnTurrets == 1)
                    time = 3f;
                else
                    time = 1f;

                yield return new WaitForSeconds(time);
            }

            //Scene.World.StopCoroutine(instance);
        }

        IEnumerator TunnelAttack()
        {
            //var instance = Scene.World.StartCoroutine(GunDown());

            Flight = LBBossFlightMode.Locked;

            float width = 200;
            float xSafe = Random.Next(0, Scene.World.Bounds.Width - (int)width);

            int steps = Random.Next(5, 10);
            float dir = Random.Next(2) == 0 ? 10 : -10;

            float speed = 5f;

            for (int i = 0; i < 100; i++)
            {
                //side = 3;

                if (steps == 0)
                {
                    steps = Random.Next(5, 10);
                    dir = Random.Next(2) == 0 ? 10 : -10;
                }

                xSafe += dir;

                targetPos.X = xSafe + width / 2f;

                if (xSafe <= 0)
                {
                    steps = Random.Next(5, 10);
                    dir = Random.Next(2) == 0 ? 10 : -10;
                    continue;
                }
                else if (xSafe >= Scene.World.Bounds.Width - width)
                {
                    steps = Random.Next(5, 10);
                    dir = Random.Next(2) == 0 ? 10 : -10;
                    continue;
                }

                steps--;

                LaserBarrier leftBarrier1 = new LaserBarrier(new Vector2(-16, 0), 0, -1, new Vector2(0, speed), LaserBarrierType.Normal);
                LaserBarrier leftBarrier2 = new LaserBarrier(new Vector2(xSafe, 0), 0, -1, new Vector2(0, speed), LaserBarrierType.Normal);

                ConnectBarriers(leftBarrier1, leftBarrier2, false);

                LaserBarrier rightBarrier1 = new LaserBarrier(new Vector2(Scene.World.Bounds.Width + 16, 0), 0, -1, new Vector2(0, speed), LaserBarrierType.Normal);
                LaserBarrier rightBarrier2 = new LaserBarrier(new Vector2(xSafe + width, 0), 0, -1, new Vector2(0, speed), LaserBarrierType.Normal);

                ConnectBarriers(rightBarrier1, rightBarrier2, false);

                Scene.World.AddEnemy(leftBarrier1, false, true);
                Scene.World.AddEnemy(leftBarrier2, false, true);
                Scene.World.AddEnemy(rightBarrier1, false, true);
                Scene.World.AddEnemy(rightBarrier2, false, true);

                yield return new WaitForSeconds(0.2f);
            }

            //go towards middle
            while (true)
            {
                xSafe = float.Lerp(xSafe, (Scene.World.Bounds.Width / 2f) - width / 2f, 0.1f);

                targetPos.X = xSafe + width / 2f;

                LaserBarrier leftBarrier1 = new LaserBarrier(new Vector2(-16, 0), 0, -1, new Vector2(0, speed), LaserBarrierType.Normal);
                LaserBarrier leftBarrier2 = new LaserBarrier(new Vector2(xSafe, 0), 0, -1, new Vector2(0, speed), LaserBarrierType.Normal);

                ConnectBarriers(leftBarrier1, leftBarrier2, false);

                LaserBarrier rightBarrier1 = new LaserBarrier(new Vector2(Scene.World.Bounds.Width + 16, 0), 0, -1, new Vector2(0, speed), LaserBarrierType.Normal);
                LaserBarrier rightBarrier2 = new LaserBarrier(new Vector2(xSafe + width, 0), 0, -1, new Vector2(0, speed), LaserBarrierType.Normal);

                ConnectBarriers(rightBarrier1, rightBarrier2, false);

                Scene.World.AddEnemy(leftBarrier1, false, true);
                Scene.World.AddEnemy(leftBarrier2, false, true);
                Scene.World.AddEnemy(rightBarrier1, false, true);
                Scene.World.AddEnemy(rightBarrier2, false, true);

                float test = Math.Abs(xSafe - (Scene.World.Bounds.Width / 2f) - width / 2f);
                if (test <= 2f)
                {
                    break;
                }

                yield return new WaitForSeconds(0.2f);
            }

            targetPos.X = xSafe + width / 2f;

            while (true)
            {
                LaserBarrier leftBarrier1 = new LaserBarrier(new Vector2(-16, 0), 0, -1, new Vector2(0, speed), LaserBarrierType.Normal);
                LaserBarrier leftBarrier2 = new LaserBarrier(new Vector2(xSafe, 0), 0, -1, new Vector2(0, speed), LaserBarrierType.Normal);

                ConnectBarriers(leftBarrier1, leftBarrier2, false);

                LaserBarrier rightBarrier1 = new LaserBarrier(new Vector2(Scene.World.Bounds.Width + 16, 0), 0, -1, new Vector2(0, speed), LaserBarrierType.Normal);
                LaserBarrier rightBarrier2 = new LaserBarrier(new Vector2(xSafe + width, 0), 0, -1, new Vector2(0, speed), LaserBarrierType.Normal);

                ConnectBarriers(rightBarrier1, rightBarrier2, false);

                Scene.World.AddEnemy(leftBarrier1, false, true);
                Scene.World.AddEnemy(leftBarrier2, false, true);
                Scene.World.AddEnemy(rightBarrier1, false, true);
                Scene.World.AddEnemy(rightBarrier2, false, true);

                yield return new WaitForSeconds(0.2f);
            }

            //Scene.World.StopCoroutine(instance);
        }

        List<LaserBarrier> GenerateFlower(LaserBarrier center, float angle, Vector2 moveDir)
        {
            List<LaserBarrier> barriers = new List<LaserBarrier>();

            LaserBarrier barrier = center;
            Vector2 start = barrier.Transform.Position;

            for (int i = 0; i < 5; i++)
            {
                start = GameHelper.OrbitPos(start, MathHelper.ToRadians(angle), 30);
                angle += 25;
                var barrier2 = new LaserBarrier(start, 0, 1, moveDir, LaserBarrierType.Normal);

                ConnectBarriers(barrier, barrier2, false);
                barrier = barrier2;

                barriers.Add(barrier);
            }

            foreach (var item in barriers)
            {
                Scene.World.AddEnemy(item, false, true);
            }

            return barriers;
        }

        IEnumerator Spinny(List<LaserBarrier> barriers, LaserBarrier host)
        {
            while (true)
            {
                foreach (var item in barriers)
                {
                    item.Transform.Position = GameHelper.RotateAroundPoint(item.Transform.Position, host.Transform.Position, 0.1f);
                }

                if (host.destroyed)
                    yield break;

                yield return null;
            }
        }

        IEnumerator ConstructAttack()
        {
            var instance = Scene.World.StartCoroutine(GunDown(3));

            for (int i = 0; i < 6; i++)
            {
                Vector2 start = new Vector2(Random.Next(200, Scene.World.Bounds.Width - 200), -150);
                Vector2 moveDir = new Vector2(0, 2);
                LaserBarrier barrier = new LaserBarrier(start, 0, 1, moveDir, LaserBarrierType.Normal);

                List<LaserBarrier> barriers = new List<LaserBarrier>();

                barriers.AddRange(GenerateFlower(barrier, 0, moveDir));
                barriers.AddRange(GenerateFlower(barrier, 45, moveDir));
                barriers.AddRange(GenerateFlower(barrier, 90, moveDir));
                barriers.AddRange(GenerateFlower(barrier, 135, moveDir));
                barriers.AddRange(GenerateFlower(barrier, 180, moveDir));
                barriers.AddRange(GenerateFlower(barrier, 225, moveDir));
                barriers.AddRange(GenerateFlower(barrier, 270, moveDir));
                barriers.AddRange(GenerateFlower(barrier, 315, moveDir));

                Scene.World.StartCoroutine(Spinny(barriers, barrier));

                Scene.World.AddEnemy(barrier, false, true);

                yield return new WaitForSeconds(3f);
            }

            Scene.World.StopCoroutine(instance);

            yield return null;
        }

        IEnumerator GunDown(int bullets)
        {
            //for (int i = 0; i < 10; i++)
            while (true)
            {
                for (int j = 0; j < bullets; j++)
                {
                    Player player = Scene.World.GetRandomPlayer();
                    if (player != null)
                        FireShotgun(GameHelper.AngleBetween(Transform.Position, player.Transform.Position), 2, 10f, 6f);
                    yield return new WaitForSeconds(0.1f);
                }

                yield return new WaitForSeconds(1f);
            }

            yield return null;
        }

        #endregion
    }
}
