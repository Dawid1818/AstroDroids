using AstroDroids.Editors;
using AstroDroids.Entities.Friendly;
using AstroDroids.Extensions;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Input;
using AstroDroids.Managers;
using AstroDroids.Paths;
using AstroDroids.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AstroDroids.Entities.Hostile.Bosses
{
    enum SnakeBossTurretAimStyle
    {
        AtPlayer,
        AtPosition,
        AtAngle,
        RelativeToBody
    }
    public class SnakeBossSegment : Enemy
    {
        SnakeBoss boss;
        SnakeBossSegment parentSegment;
        Texture2D texture;
        Texture2D turretTexture;

        Vector2 turretMountPoint = new Vector2(0, 0);

        public float angle { get; private set; }
        public float turretAngle { get; private set; }

        SnakeBossTurretAimStyle aimStyle = SnakeBossTurretAimStyle.RelativeToBody;
        float targetOffsetAngle;
        Vector2 targetAimPosition;
        float targetAimAngle;

        public float leftSideAngle { get { return angle + MathHelper.ToRadians(-90); } }
        public float rightSideAngle { get { return angle + MathHelper.ToRadians(90); } }
        public float behindAngle { get { return angle + MathHelper.ToRadians(180); } }

        int historyOffset;

        RandomMoveManager RMM;

        float speed = 100f;
        float turnSpeed = 5f;

        bool followPlayer = true;

        SnakeBossMovementStyle style;

        public float TravelProgress { get { return RMM.GetProgress(); } }

        public SnakeBossSegment(SnakeBoss boss, SnakeBossSegment parentSegment, int historyOffset) : base(Vector2.Zero, 100)
        {
            this.boss = boss;
            this.parentSegment = parentSegment;
            if (parentSegment == null)
                texture = TextureManager.Get("Ships/SnakeBoss/SnakeBoss");
            else
                texture = TextureManager.Get("Ships/SnakeBoss/SnakeBossNocockpit");

            turretTexture = TextureManager.Get("Ships/SnakeBoss/SnakeBossTurret");
            AddCircleCollider(Vector2.Zero, 42);
            this.historyOffset = historyOffset;
        }

        public override void Spawned()
        {
            RMM = new RandomMoveManager(Transform.LocalPosition);
            RMM.maxMoveDistance = 1000;
            RMM.SetNewPath2(angle);
        }

        public void GoTargetPlayer()
        {
            style = SnakeBossMovementStyle.TowardsPlayer;
        }

        public void GoFollowPath(IPath path, bool reverse = false)
        {
            style = SnakeBossMovementStyle.Path;
            RMM.UpdatePosition(Transform.Position);
            RMM.SetPath(path, reverse);
        }

        public void GoRandom()
        {
            style = SnakeBossMovementStyle.Path;
            RMM.UpdatePosition(Transform.Position);
            RMM.SetNewPath2(angle);
        }

        public void GoTowards(IPath targetPath, bool reverse = false)
        {
            float arrivalAngle;
            if (!reverse)
                arrivalAngle = GameHelper.AngleFromDir(targetPath.GetDirection(0f));
            else
                arrivalAngle = GameHelper.AngleFromDir(targetPath.GetDirection(1f)) + MathF.PI;

            style = SnakeBossMovementStyle.Path;
            RMM.UpdatePosition(Transform.Position);
            IPath path = GameHelper.CreateCatmull(Transform.Position, !reverse ? targetPath.StartPoint : targetPath.EndPoint, angle, arrivalAngle);
            RMM.SetPath(path);
        }

        public void GoTowards(Vector2 position, float? arrivalAngle = null)
        {
            style = SnakeBossMovementStyle.Path;
            RMM.UpdatePosition(Transform.Position);
            IPath path = GameHelper.CreateCatmull(Transform.Position, position, angle, arrivalAngle);
            RMM.SetPath(path);
        }

        public override void Update(GameTime gameTime)
        {
            Player player = Scene.World.GetRandomPlayer();

            if (parentSegment != null)
            {
                if (boss.PositionHistory.Count > historyOffset + 1)
                {
                    Transform.Position = boss.PositionHistory[historyOffset];

                    Vector2 current = boss.PositionHistory[historyOffset];

                    Vector2 next = boss.PositionHistory[historyOffset + 1];

                    Vector2 dir = current - next;

                    if (dir.LengthSquared() > 0.001f)
                        angle = MathF.Atan2(dir.Y, dir.X);
                }
            }
            else
            {
                HeadLogic(gameTime);
            }

            switch (aimStyle)
            {
                case SnakeBossTurretAimStyle.AtPlayer:
                    if (player != null)
                    {
                        Vector2 desiredDirection = (player.Transform.Position - Transform.Position);
                        desiredDirection.Normalize();

                        var tarAngle = GameHelper.AngleFromDir(desiredDirection);

                        turretAngle = MathHelperEx.LerpAngle(turretAngle, tarAngle + targetOffsetAngle, turnSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                    break;
                case SnakeBossTurretAimStyle.AtPosition:
                    {
                        Vector2 desiredDirection = (targetAimPosition - Transform.Position);
                        desiredDirection.Normalize();

                        Vector2 lerp = Vector2.Lerp(GameHelper.DirFromAngle(turretAngle), desiredDirection, turnSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        lerp.Normalize();
                        turretAngle = GameHelper.AngleFromDir(lerp);
                    }
                    break;
                case SnakeBossTurretAimStyle.AtAngle:
                    {
                        turretAngle = MathHelperEx.LerpAngle(turretAngle, targetAimAngle, turnSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                    break;
                case SnakeBossTurretAimStyle.RelativeToBody:
                    {
                        turretAngle = MathHelperEx.LerpAngle(turretAngle, angle + targetAimAngle, turnSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                    break;
                default:
                    break;
            }
        }

        void HeadLogic(GameTime gameTime)
        {
            Vector2 oldPos = Transform.Position;

            Player player = Scene.World.GetRandomPlayer();

            if (InputSystem.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.I))
            {
                followPlayer = !followPlayer;

                if (!followPlayer)
                {
                    RMM.UpdatePosition(Transform.Position);
                    RMM.SetNewPath2(angle);
                }
            }

            switch (style)
            {
                case SnakeBossMovementStyle.None:
                    break;
                case SnakeBossMovementStyle.Path:
                    RMM.Update(gameTime);
                    Transform.LocalPosition = RMM.Position;
                    angle = RMM.MovementAngle;

                    break;
                case SnakeBossMovementStyle.TowardsPlayer:
                    if (player != null)
                    {
                        Vector2 desiredDirection = (player.Transform.Position - Transform.Position);
                        desiredDirection.Normalize();

                        Vector2 lerp = Vector2.Lerp(GameHelper.DirFromAngle(angle), desiredDirection, turnSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        lerp.Normalize();
                        angle = GameHelper.AngleFromDir(lerp);

                        Transform.Translate(new Vector2(MathF.Cos(angle) * speed * (float)gameTime.ElapsedGameTime.TotalSeconds, MathF.Sin(angle) * speed * (float)gameTime.ElapsedGameTime.TotalSeconds));
                    }
                    break;
                default:
                    break;
            }

            if (Transform.Position != oldPos)
            {
                boss.PositionHistory.Insert(0, Transform.Position);

                if (boss.PositionHistory.Count > 1000)
                    boss.PositionHistory.RemoveAt(boss.PositionHistory.Count - 1);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, Transform.Position, null, CanBeDamaged ? Color.White : Color.Red, angle + MathHelper.ToRadians(90), new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, 0f);
            Screen.spriteBatch.Draw(turretTexture, Transform.Position + turretMountPoint, null, CanBeDamaged ? Color.White : Color.Red, turretAngle + MathHelper.ToRadians(90), new Vector2(turretTexture.Width / 2f, turretTexture.Height / 2f), 1f, SpriteEffects.None, 0f);

            if (parentSegment == null && AstroDroidsGame.Debug)
            {
                if (style == SnakeBossMovementStyle.Path)
                    PathVisualizer.DrawPath(RMM.GetPath(), true);
            }
        }

        public override void Destroyed()
        {
            CanBeDamaged = false;
            SetHealth(0);

            //can change sprite to damaged state

            //original behavior
            //if (destroyed) return;

            //Scene.World.AddEffect(new StandardExplosion(new Transform(Transform.Position.X, Transform.Position.Y), 0.6f));

            //GameState.AddScore(Score);
            //Despawn();

            //destroyed = true;
        }

        public bool DestinationReached()
        {
            return !RMM.Active;
        }

        public IEnumerator FireSequence2(int shotsToFire, float spacing, float speed, float phaseSpeed, float phaseMax)
        {
            List<float> angles = null;

            angles = GameHelper.SpreadAngle(turretAngle, shotsToFire, spacing);
            foreach (var item in angles)
            {
                Shoot(item, speed, phaseSpeed, phaseMax);
            }

            yield return null;
        }

        public void AimAtPlayer(float offsetAngle = 0f)
        {
            aimStyle = SnakeBossTurretAimStyle.AtPlayer;
            targetOffsetAngle = offsetAngle;
        }

        public void AimAtAngle(float angle)
        {
            aimStyle = SnakeBossTurretAimStyle.AtAngle;
            targetAimAngle = angle;
        }

        public void AimAtPosition(Vector2 pos)
        {
            aimStyle = SnakeBossTurretAimStyle.AtPosition;
            targetAimPosition = pos;
        }

        public void AimRelatively(float angle)
        {
            aimStyle = SnakeBossTurretAimStyle.RelativeToBody;
            targetAimAngle = angle;
        }

        public void Fire(int shots, float spacing, float speed = 5f, float phaseSpeed = 0f, float phaseMax = 0f)
        {
            Scene.World.StartCoroutine(FireSequence2(shots, spacing, speed, phaseSpeed, phaseMax));
        }

        void Shoot(float angle, float speed = 5f, float phaseSpeed = 0f, float phaseMax = 0f)
        {
            var projectile = new CircleProjectile(GameHelper.OrbitPos(Transform.Position, angle, 20), angle, speed, 12f);
            projectile.SetPhase(phaseSpeed, phaseMax);
            Scene.World.AddProjectile(projectile, true);
        }
    }
}
