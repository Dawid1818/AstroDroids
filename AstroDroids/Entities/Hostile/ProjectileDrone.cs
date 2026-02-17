using AstroDroids.Entities.Friendly;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Paths;
using AstroDroids.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        float angle = 0f;

        DroneController controller;

        float distanceToController;
        float orbitAngle;
        float attackTimer;

        RandomMoveManager RMM;

        public ProjectileDrone(DroneController controller, float distance, float startAngle) : base(new Transform(0, 0), 1, 42f, 40f)
        {
            this.controller = controller;
            texture = TextureManager.Get("Ships/DroneController/ProjectileDrone");
            orbitAngle = startAngle;
            distanceToController = distance;

            if (controller == null)
                state = ProjectileDroneState.Idle;

            AddCircleCollider(Vector2.Zero, 21f);
        }

        public override void Spawned()
        {
            if (controller == null)
                RMM = new RandomMoveManager(Transform.LocalPosition);
        }

        public override void Destroyed()
        {
            base.Destroyed();
        }

        public override void Update(GameTime gameTime)
        {
            Player player = Scene.World.GetRandomPlayer();

            if (player != null)
            {
                angle = GameHelper.AngleBetween(Transform.LocalPosition, player.GetLocalPosition());
            }

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

                    attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (attackTimer >= 1f)
                    {
                        attackTimer = 0f;

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
                    if(player != null)
                        Shoot();
                    break;
                default:
                    if(RMM == null)
                        RMM = new RandomMoveManager(Transform.LocalPosition);
                    state = ProjectileDroneState.Idle;
                    break;
            }
        }

        void Shoot()
        {
            Vector2 spawnPos = GameHelper.OrbitPos(Transform.Position, angle, 21);

            Scene.World.AddProjectile(new LaserProjectile(new Transform(spawnPos.X, spawnPos.Y), 32, 16, angle), true);
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, ToRectangle(), null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0f);
        }
    }
}
