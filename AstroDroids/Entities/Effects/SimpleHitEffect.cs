using AstroDroids.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace AstroDroids.Entities.Effects
{
    public class SimpleHitEffect : Entity
    {
        float timer = 0;

        float finalSize = 16;

        Color color;

        public SimpleHitEffect(Transform transform, Color color) : base(transform)
        {
            this.color = color;
        }

        public override void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (timer >= 1)
            {
                Scene.World.RemoveEffect(this);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            float radius = finalSize * timer;

            float opacity = timer <= 0.5 ? 1f : 1f - timer;

            Screen.spriteBatch.DrawCircle(Transform.Position.X, Transform.Position.Y, radius, 32, new Color(color.R, color.G, color.B, (byte)(opacity * 255)), radius, 0f);
        }
    }
}
