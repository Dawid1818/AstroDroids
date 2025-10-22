using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace AstroDroids.Entities.Hostile
{
    public class BasicEnemy : Enemy
    {
        public BasicEnemy(Vector2 position) : base(new RectangleF(position.X, position.Y, 32f, 32f), 1)
        {
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), Collider.ToRectangle(), Color.Red);
        }
    }
}
