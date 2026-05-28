using AstroDroids.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstroDroids.Drawables
{
    public class AnimatedSprite
    {
        Texture2D texture;

        int columns;

        int frameWidth;
        int frameHeight;
        int padding;

        float timer = 0f;

        int frameCount;
        float frameDuration = 0f;
        int frame = 0;

        public AnimatedSprite(Texture2D texture, int columns, int frameWidth, int frameHeight, int padding, int frameCount, float framesPerSecond)
        {
            this.texture = texture;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.padding = padding;
            this.frameCount = frameCount;
            this.frameDuration = 1f / framesPerSecond;
            this.columns = columns;
        }

        public void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(timer >= frameDuration)
            {
                timer = 0f;
                frame++;

                if(frame >= frameCount)
                {
                    frame = 0;
                }
            }
        }

        public void Draw(Vector2 position, float angle)
        {
            int column = frame % columns;
            int row = frame / columns;

            Rectangle source = new Rectangle(column * (frameWidth + padding), row * (frameHeight + padding), frameWidth, frameHeight);

            Screen.spriteBatch.Draw(texture, position, source, Color.White, angle, new Vector2(frameWidth / 2f, frameHeight / 2f), 1f, SpriteEffects.None, 0f);
        }
    }
}
