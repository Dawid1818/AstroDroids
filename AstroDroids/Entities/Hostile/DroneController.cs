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

        public DroneController() : base(new Transform(0, 0), 1, 49f, 69f)
        {
            texture = TextureManager.Get("Ships/DroneController/DroneController");

            AddCircleCollider(Vector2.Zero, 35);
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
            Screen.spriteBatch.Draw(texture, ToRectangle(), null, Color.White, MathHelper.ToRadians(180), new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0f);
        }
    }
}
