using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace AstroDroids.Entities.Warnings
{
    public struct ReflectBeamSegment
    {
        public Vector2 Position { get; set; }
        public float Length { get; set; }
        public float Angle { get; set; }
    }
    public class ReflectBeamWarning : Entity
    {
        float angle;
        int length;

        List<ReflectBeamSegment> segments = new List<ReflectBeamSegment>();

        public ReflectBeamWarning(Transform transform, float angle, int length) : base(transform)
        {
            UpdateParameters(angle, length);
        }

        public void UpdateParameters(float angle, int length)
        {
            this.angle = angle;
            this.length = length;
        }

        public override void Update(GameTime gameTime)
        {
            segments.Clear();

            Vector2 currentPos = Transform.Position;
            float currentAngle = angle;
            float remainingLength = length;

            int maxBounces = 10;

            while (remainingLength > 0 && maxBounces > 0)
            {
                Vector2 dir = GameHelper.DirFromAngle(currentAngle);
                float distToWall = float.MaxValue;

                if (dir.X > 0)
                {
                    distToWall = (Scene.World.Bounds.Width - currentPos.X) / dir.X;
                }
                else if (dir.X < 0)
                {
                    distToWall = (0f - currentPos.X) / dir.X;
                }

                float drawLength = Math.Min(remainingLength, distToWall);

                segments.Add(new ReflectBeamSegment() { Position = currentPos, Angle = currentAngle, Length = drawLength });

                if (distToWall < remainingLength)
                {
                    currentPos += dir * distToWall;
                    remainingLength -= distToWall;

                    currentAngle = (float)Math.Atan2(dir.Y, -dir.X);
                }
                else
                {
                    break;
                }

                maxBounces--;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var item in segments)
            {
                DrawSegment(item.Position, item.Angle, item.Length);
            }
        }

        private void DrawSegment(Vector2 basePos, float segAngle, float segLength)
        {
            float halfThickness = 16f;

            Vector2 dir = GameHelper.DirFromAngle(segAngle);
            Vector2 perp = new Vector2(-dir.Y, dir.X);

            Vector2 upperPos = basePos + perp * halfThickness;
            Vector2 lowerPos = basePos - perp * halfThickness;

            var pixel = TextureManager.GetPixelTexture();

            Screen.spriteBatch.Draw(pixel, new Rectangle((int)basePos.X, (int)basePos.Y, (int)segLength, 32), null, new Color(255, 0, 0, 127), segAngle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
            Screen.spriteBatch.Draw(pixel, new Rectangle((int)upperPos.X, (int)upperPos.Y, (int)segLength, 4), null, Color.Red, segAngle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
            Screen.spriteBatch.Draw(pixel, new Rectangle((int)lowerPos.X, (int)lowerPos.Y, (int)segLength, 4), null, Color.Red, segAngle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
        }
    }
}
