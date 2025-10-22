using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace AstroDroids.Projectiles
{
    public class BasicProjectile : Projectile
    {
        public BasicProjectile(Vector2 position) : base(new RectangleF((int)position.X, (int)position.Y, 16, 16))
        {

        }

        public override void Update(GameTime gameTime)
        {
            Collider.Y -= 5f;

            if(Collider.Y + 16 < 0)
            {
                Despawn();
            }
            else
            {
                foreach (var enemy in Scene.World.Enemies)
                {
                    if (enemy.CollidesWith(Collider))
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
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), Collider.ToRectangle(), Color.White);
        }
    }
}
