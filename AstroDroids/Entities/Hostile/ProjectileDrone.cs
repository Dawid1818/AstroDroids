using AstroDroids.Entities.Friendly;
using AstroDroids.Entities.Warnings;
using AstroDroids.Extensions;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Interfaces;
using AstroDroids.Managers;
using AstroDroids.Paths;
using AstroDroids.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace AstroDroids.Entities.Hostile
{
    public class ProjectileDrone : Enemy
    {
        enum ProjectileDroneState
        {
            Orbiting,
            Idle,
            IdleMoved,
            Moving
        }

        ProjectileDroneState state = ProjectileDroneState.Orbiting;

        Texture2D texture;

        public float angle { get; set; } = 0f;
        public float overridedAngle { get; set; } = 0f;

        Enemy controller;

        float distanceToController;
        float orbitAngle;
        float attackTimer;

        RandomMoveManager RMM;

        public bool angleOverride { get; set; } = false;

        bool controlledExternally = false;

        BeamWarning warning;

        float beamTimer = 0f;

        public ProjectileDrone(Enemy controller, float distance, float startAngle) : base(Vector2.Zero, 5)
        {
            this.controller = controller;
            //texture = TextureManager.Get("Ships/DroneController/ProjectileDrone");
            texture = TextureManager.Get("Ships/DroneController/tinyShip18");
            orbitAngle = startAngle;
            distanceToController = distance;

            if (controller == null)
                state = ProjectileDroneState.Idle;

            AddCircleCollider(Vector2.Zero, 21f);

            controlledExternally = false;
        }

        public ProjectileDrone(Enemy controller) : base(Vector2.Zero, 20)
        {
            this.controller = controller;
            //texture = TextureManager.Get("Ships/DroneController/ProjectileDrone");
            texture = TextureManager.Get("Ships/DroneController/tinyShip18");

            if (controller == null)
                state = ProjectileDroneState.Idle;

            AddCircleCollider(Vector2.Zero, 21f);

            controlledExternally = true;
        }

        public override void Spawned()
        {
            if (controller == null)
                RMM = new RandomMoveManager(Transform.LocalPosition);
        }

        public override void Destroyed()
        {
            if (!destroyed && controller != null && controller is IDroneController dronecontroller)
            {
                dronecontroller.DroneDestroyed(this);
            }

            if (warning != null)
            {
                Scene.World.RemoveWarning(warning);
                warning = null;

                beamTimer = 0f;
            }

            base.Destroyed();
        }

        public override void Update(GameTime gameTime)
        {
            Player player = Scene.World.GetRandomPlayer();

            if (warning == null)
            {
                if (!angleOverride)
                {
                    if (player != null)
                        angle = MathHelperEx.LerpAngle(angle, GameHelper.AngleBetween(Transform.Position, player.GetPosition()), 5f * gameTime.GetElapsedSeconds());
                }
                else
                {
                    angle = MathHelperEx.LerpAngle(angle, overridedAngle, 1.5f * gameTime.GetElapsedSeconds());
                }
            }

            if (warning != null)
            {
                Vector2 forwardPoint = GameHelper.RotateAroundPoint(Transform.Position + new Vector2(texture.Width / 2f, 0), Transform.Position, angle);

                warning.Transform.Position = forwardPoint;

                beamTimer += gameTime.GetElapsedSeconds();

                if (beamTimer >= 1f)
                {
                    beamTimer = 0f;
                    forwardPoint = GameHelper.RotateAroundPoint(Transform.Position + new Vector2(texture.Width / 2f, 0), Transform.Position, angle);
                    Scene.World.AddProjectile(new SpinLaserBeam(forwardPoint, (float)angle), true);
                    Scene.World.RemoveWarning(warning);
                    warning = null;
                }
            }

            if (!controlledExternally)
            {
                switch (state)
                {
                    case ProjectileDroneState.Orbiting:
                        if ((controller == null || controller.destroyed) && state == ProjectileDroneState.Orbiting)
                        {
                            RMM = new RandomMoveManager(Transform.LocalPosition);
                            state = ProjectileDroneState.Idle;
                            return;
                        }

                        Transform.LocalPosition = GameHelper.OrbitPos(controller.Transform.LocalPosition, orbitAngle, distanceToController);

                        orbitAngle += (float)gameTime.ElapsedGameTime.TotalSeconds * 2f;

                        if (orbitAngle > MathHelper.TwoPi)
                        {
                            orbitAngle -= MathHelper.TwoPi;
                        }

                        attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (attackTimer >= 1f)
                        {
                            attackTimer = 0f;

                            if (player != null)
                                Shoot();
                        }
                        break;
                    case ProjectileDroneState.Idle:
                        RMM.SetNewPath(false);
                        state = ProjectileDroneState.Moving;
                        break;
                    case ProjectileDroneState.Moving:
                        RMM.Update(gameTime);
                        Transform.LocalPosition = RMM.Position;
                        if (!RMM.Active)
                        {
                            state = ProjectileDroneState.IdleMoved;
                        }
                        break;
                    case ProjectileDroneState.IdleMoved:
                        state = ProjectileDroneState.Idle;
                        if (player != null)
                            Shoot();
                        break;
                    default:
                        if (RMM == null)
                            RMM = new RandomMoveManager(Transform.LocalPosition);
                        state = ProjectileDroneState.Idle;
                        break;
                }
            }
        }

        public void Shoot()
        {
            Vector2 spawnPos = GameHelper.OrbitPos(Transform.Position, angle, 21);

            Scene.World.AddProjectile(new LaserProjectile(spawnPos, angle), true);
        }

        public void ShootLaser()
        {
            if (warning != null)
                return;

            Vector2 forwardPoint = GameHelper.RotateAroundPoint(Transform.Position + new Vector2(texture.Width / 2f, 0), Transform.Position, angle);
            warning = new BeamWarning(new Transform(forwardPoint.X, forwardPoint.Y), (float)angle, 900);
            Scene.World.AddWarning(warning, true);
            beamTimer = 0f;
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, new Rectangle((int)Transform.Position.X, (int)Transform.Position.Y, texture.Width, texture.Height), null, !Collidable ? Color.Gray : Color.White, angle + 1.571f, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0f);
        }

        public void SetCollidable(bool state)
        {
            Collidable = state;
        }
    }
}
