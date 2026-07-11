using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstroDroids.Entities.Warnings
{
    internal class ChallengerBeamWarning : Entity
    {
        public bool Locked { get; set; } = false;

        public Entity Follow { get; set; }
        public Vector2 Target { get; set; }
        public float AngleOffset { get; set; } = 0f;

        float length;

        float angle;

        public ChallengerBeamWarning(Vector2 position, float angle, float length) : base(position)
        {
            this.angle = angle;
            this.length = length;
        }

        public override void Update(GameTime gameTime)
        {
            if (Follow != null)
            {
                Transform.Position = Follow.Transform.Position;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            float halfThickness = 15f;

            Vector2 dir = GameHelper.DirFromAngle(angle);
            Vector2 perp = new Vector2(-dir.Y, dir.X);
            Vector2 basePos = Transform.Position;
            Vector2 upperPos = basePos + perp * halfThickness;
            Vector2 lowerPos = basePos - perp * halfThickness;

            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle((int)basePos.X, (int)basePos.Y, (int)length, 32), null, new Color(255, 0, 0, 127), angle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle((int)upperPos.X, (int)upperPos.Y, (int)length, 4), null, Color.Red, angle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle((int)lowerPos.X, (int)lowerPos.Y, (int)length, 4), null, Color.Red, angle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
        }
    }
}
