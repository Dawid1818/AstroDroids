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
            int column = frame % columns;
            int row = frame / columns;

            Rectangle source = new Rectangle(column * (frameWidth + padding), row * (frameHeight + padding), frameWidth, frameHeight);

            Screen.spriteBatch.Draw(texture, position, source, Color.White, angle, new Vector2(frameWidth / 2f, frameHeight / 2f), scale, SpriteEffects.None, 0f);
        }
    }
}
