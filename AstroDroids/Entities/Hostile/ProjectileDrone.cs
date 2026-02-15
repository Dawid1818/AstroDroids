using AstroDroids.Entities.Friendly;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstroDroids.Entities.Hostile
{
    public class ProjectileDrone : Enemy
    {
        Texture2D texture;

        float angle = 0f;

        DroneController controller;

        float distanceToController;
        float orbitAngle;
        float attackTimer;

        public ProjectileDrone(DroneController controller, float distance, float startAngle) : base(new Transform(0, 0), 1, 42f, 40f)
        {
            this.controller = controller;
            texture = TextureManager.Get("Ships/DroneController/ProjectileDrone");
            orbitAngle = startAngle;
            distanceToController = distance;
        }

        public override void Spawned()
        {

        }

        public override void Destroyed()
        {
            base.Destroyed();
        }

        public override void Update(GameTime gameTime)
        {
            if (controller == null || controller.destroyed)
                return;

            Transform.LocalPosition = GameHelper.OrbitPos(controller.Transform.LocalPosition, orbitAngle, distanceToController);

            orbitAngle += (float)gameTime.ElapsedGameTime.TotalSeconds * 2f;

            Player player = Scene.World.GetRandomPlayer();

            if (player != null)
            {
                angle = GameHelper.AngleBetween(Transform.LocalPosition, player.LocalCenter);
            }

            attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (attackTimer >= 1f)
            {
                attackTimer = 0f;

                Vector2 spawnPos = GameHelper.OrbitPos(Transform.LocalPosition, angle, 21);

                Scene.World.AddProjectile(new LaserProjectile(new Transform(spawnPos.X, spawnPos.Y), 32, 16, angle));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, ToRectangle(), null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0f);
        }
    }
}
