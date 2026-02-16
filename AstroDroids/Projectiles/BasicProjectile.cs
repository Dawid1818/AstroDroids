using AstroDroids.Entities;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;

namespace AstroDroids.Projectiles
{
    public class BasicProjectile : Projectile
    {
        public BasicProjectile(Vector2 position) : base(new Transform(position.X, position.Y), 16, 16)
        {
            AddCircleCollider(Vector2.Zero, 8f);
        }

        public override void Update(GameTime gameTime)
        {
            Transform.Translate(new Vector2(0f, -5f));

            if (Transform.LocalPosition.Y + 16 < 0)
            {
                Despawn();
            }
            else
            {
                foreach (var enemy in Scene.World.Enemies)
                {
                    if (enemy.Intersects(this))
                    {
                        Despawn();
                        enemy.Damage(1, true);
                        break;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), ToRectangle(), null, Color.White, 0f, new Vector2(0.5f, 0.5f), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }
    }
}
