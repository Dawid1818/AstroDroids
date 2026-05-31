using AstroDroids.Graphics;
using AstroDroids.Managers;
using AstroDroids.Paths;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstroDroids.Entities.Hostile
{
    public class DroneController : Enemy
    {
        Texture2D texture;

        RandomMoveManager RMM;

        public DroneController() : base(Vector2.Zero, 20)
        {
            //texture = TextureManager.Get("Ships/DroneController/DroneController");
            texture = TextureManager.Get("Ships/DroneController/DroneControllerv2");

            AddCircleCollider(Vector2.Zero, 21);
        }

        public override void Spawned()
        {
            ProjectileDrone drone1 = new ProjectileDrone(this, 50, 90);
            drone1.Transform.Position = Transform.Position + new Vector2(-50, 0);
            Scene.World.AddEnemy(drone1, true);

            ProjectileDrone drone2 = new ProjectileDrone(this, 50, -90);
            drone1.Transform.Position = Transform.Position + new Vector2(50, 0);
            Scene.World.AddEnemy(drone2, true);

            ProjectileDrone drone3 = new ProjectileDrone(this, 50, 0);
            drone1.Transform.Position = Transform.Position + new Vector2(0, 50);
            Scene.World.AddEnemy(drone3, true);

            RMM = new RandomMoveManager(Transform.LocalPosition);
        }

        public override void Destroyed()
        {
            base.Destroyed();
        }

        public override void Update(GameTime gameTime)
        {
            if (PathManager != null && PathManager.Active)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
                RMM.UpdatePosition(PathManager.Position);
            }
            else
            {
                RMM.Update(gameTime);
                Transform.LocalPosition = RMM.Position;
                if (!RMM.Active)
                {
                    RMM.SetNewPath(true);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, new Rectangle((int)Transform.Position.X, (int)Transform.Position.Y, texture.Width, texture.Height), null, Color.White, MathHelper.ToRadians(180), new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0f);
        }
    }
}
