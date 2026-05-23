using AstroDroids.Paths;
using AstroDroids.Entities.Neutral;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AstroDroids.Helpers;

namespace AstroDroids.Entities.Hostile
{
    public class BasicEnemy : Enemy
    {
        public float t = 0f;

        Texture2D texture;

        float angle = 3.14f;

        public BasicEnemy() : base(new Transform(0, 0), 1)
        {
            texture = TextureManager.Get("Ships/Basic/tinyShip9");

            AddCircleCollider(Vector2.Zero, 16f);
        }

        public override void Update(GameTime gameTime)
        {
            if(PathManager != null)
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
                if(!FollowsCamera)
                    DefaultMove();
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
            Screen.spriteBatch.Draw(texture, new Vector2(Transform.Position.X, Transform.Position.Y), null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
        }
    }
}
