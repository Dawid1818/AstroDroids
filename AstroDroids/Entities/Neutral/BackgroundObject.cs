using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;

namespace AstroDroids.Entities.Neutral
{
    public class BackgroundObject : Entity
    {
        public BackgroundObject()
        {

        }

        public override void Update(GameTime gameTime)
        {
            DefaultMove();
        }
        public override void Draw(GameTime gameTime)
        {
            //placeholder
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle((int)Transform.Position.X, (int)Transform.Position.Y, 64, 64), Color.White);
        }

        protected void Despawn()
        {
            Scene.World.RemoveBackgroundObject(this);
        }
    }
}
