using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;

namespace AstroDroids.Projectiles
{
    public class BasicProjectile : Projectile
    {
        Vector2 position;

        public BasicProjectile(Vector2 position)
        {
            this.position = position;
        }

        public override void Update(GameTime gameTime)
        {
            position.Y -= 5f;

            if(position.Y + 16 < 0)
            {
                Scene.World.RemoveProjectile(this);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle((int)position.X, (int)position.Y, 16, 16), Color.White);
        }
    }
}
