using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace AstroDroids.Entities.Hostile
{
    public class Asteroid : Enemy
    {
        public float t = 0f;

        Texture2D texture;

        float angle = 3.14f;

        float speed = 10f;

        public Asteroid() : base(Vector2.Zero, 2)
        {
            //texture = TextureManager.Get("Ships/Basic/tinyShip9");
            texture = TextureManager.Get("Ships/Basic/tinyShip9Sheet");

            AddCircleCollider(Vector2.Zero, 16f);
        }

        public override void Update(GameTime gameTime)
        {
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
                {
                    Transform.Translate(new Vector2(MathF.Cos(angle) * speed * (float)gameTime.ElapsedGameTime.TotalSeconds, MathF.Sin(angle) * speed * (float)gameTime.ElapsedGameTime.TotalSeconds));
                }

                if (Transform.Position.Y > Scene.World.Bounds.Bottom + texture.Height)
                {
                    Despawn();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, new Vector2(Transform.Position.X, Transform.Position.Y), null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
        }
    }
}
