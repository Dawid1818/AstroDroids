using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace AstroDroids.Entities.Warnings
{
    public class BeamWarning : Entity
    {
        float angle;
        int length;
        public BeamWarning(Transform transform, float angle, int length) : base(transform)
        {
            this.angle = angle;
            this.length = length;
        }

        public override void Draw(GameTime gameTime)
        {
            float halfThickness = 16f;

            Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            Vector2 perp = new Vector2(-dir.Y, dir.X);
            Vector2 basePos = Transform.Position;
            Vector2 upperPos = basePos + perp * halfThickness;
            Vector2 lowerPos = basePos - perp * halfThickness;


            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle((int)basePos.X, (int)basePos.Y, length, 32), null, new Color(255, 0, 0, 127), angle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle((int)upperPos.X, (int)upperPos.Y, length, 4), null, Color.Red, angle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle((int)lowerPos.X, (int)lowerPos.Y, length, 4), null, Color.Red, angle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);

        }
    }
}
