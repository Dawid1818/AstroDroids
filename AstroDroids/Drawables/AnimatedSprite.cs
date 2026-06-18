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

        public int frameCount { get; private set; }
        float frameDuration = 0f;
        public int frame { get; private set; } = 0;

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

            while (timer >= frameDuration)
            {
                timer -= frameDuration;
                frame = (frame + 1) % frameCount;
            }
        }

        public void Draw(Vector2 position, float angle, float scale)
        {
            Rectangle source = GetCurrentFrameSource();

            Screen.spriteBatch.Draw(texture, position, source, Color.White, angle, new Vector2(frameWidth / 2f, frameHeight / 2f), scale, SpriteEffects.None, 0f);
        }

        public void Draw(Vector2 position, float angle, float scale, Color color)
        {
            Rectangle source = GetCurrentFrameSource();

            Screen.spriteBatch.Draw(texture, position, source, color, angle, new Vector2(frameWidth / 2f, frameHeight / 2f), scale, SpriteEffects.None, 0f);
        }

        public void Draw(Vector2 position, float angle, float scale, Vector2 origin, Color color)
        {
            Rectangle source = GetCurrentFrameSource();

            Screen.spriteBatch.Draw(texture, position, source, color, angle, origin, scale, SpriteEffects.None, 0f);
        }

        public void DrawPartial(Vector2 position, float angle, float scale, Vector2 origin, Color color, float length)
        {
            Rectangle source = GetCurrentFrameSource();

            source.Y += source.Height - (int)length;
            source.Height = (int)length;

            Screen.spriteBatch.Draw(texture, position, source, color, angle, new Vector2(origin.X, length), scale, SpriteEffects.None, 0f);
        }

        public void Draw(Vector2 position, float angle, Vector2 scale, Vector2 origin, Color color)
        {
            Rectangle source = GetCurrentFrameSource();

            Screen.spriteBatch.Draw(texture, position, source, color, angle, origin, scale, SpriteEffects.None, 0f);
        }

        public void DrawPartial(Vector2 position, float angle, Vector2 scale, Vector2 origin, Color color, float length)
        {
            Rectangle source = GetCurrentFrameSource();

            int intLength = (int)length;

            source.Y += source.Height - intLength;
            source.Height = intLength;

            Screen.spriteBatch.Draw(texture, position, source, color, angle, new Vector2(origin.X, intLength), scale, SpriteEffects.None, 0f);
        }

        public Rectangle GetCurrentFrameSource()
        {
            int column = frame % columns;
            int row = frame / columns;

            return new Rectangle(column * (frameWidth + padding), row * (frameHeight + padding), frameWidth, frameHeight);
        }
    }
}
