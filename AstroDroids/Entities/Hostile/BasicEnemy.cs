using AstroDroids.Drawables;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Paths;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstroDroids.Entities.Hostile
{
    public class BasicEnemy : Enemy
    {
        public float t = 0f;

        Texture2D texture;
        AnimatedSprite sprite;

        float angle = 3.14f;

        public BasicEnemy() : base(Vector2.Zero, 2)
        {
            //texture = TextureManager.Get("Ships/Basic/tinyShip9");
            texture = TextureManager.Get("Ships/Basic/tinyShip9Sheet");

            sprite = new AnimatedSprite(texture, 5, 34, 30, 1, 5, 10f);

            AddCircleCollider(Vector2.Zero, 16f);
        }

        public override void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);

            if (PathManager != null)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
                angle = GameHelper.AngleFromDir(PathManager.Direction) + 1.571f;

                if (!PathManager.Active)
                {
                    Despawn();
                }
            }
            else
            {
                if (!FollowsCamera)
                    DefaultMove();

                if (Transform.Position.Y > Scene.World.Bounds.Bottom + texture.Height)
                {
                    Despawn();
                }
            }

            //if (Path != null)
            //{
            //    if (t < 1f)
            //    {
            //        t += 0.01f;
            //        //Path.SetPointAtIndex(3, cell.Position);
            //        Transform.Position = PathManager.GetPoint(t);

            //        //Vector2 dir = Path.GetDirection(t);

            //        //angle = (float)Math.Atan2(dir.Y, dir.X) + 1.571f;
            //    }
            //    else
            //    {
            //        //Transform.Position = cell.Position;
            //        //angle = 3.142f;
            //    }
            //}
        }

        public override void Draw(GameTime gameTime)
        {
            sprite.Draw(new Vector2(Transform.Position.X, Transform.Position.Y), angle, 1f);
            //Screen.spriteBatch.Draw(texture, new Vector2(Transform.Position.X, Transform.Position.Y), null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
        }
    }
}
