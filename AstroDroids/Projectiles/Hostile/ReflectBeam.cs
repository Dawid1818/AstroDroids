using AstroDroids.Collisions;
using AstroDroids.Entities;
using AstroDroids.Entities.Friendly;
using AstroDroids.Entities.Warnings;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroDroids.Projectiles.Hostile
{
    public class ReflectBeam : Projectile
    {
        int timer = 0;

        public bool Locked { get; set; } = false;

        float length;
        public float Angle { get { return _angle; } set { _angle = value; col.PointB = GameHelper.OrbitPos(Vector2.Zero, _angle, length); } }
        float _angle;

        CapsuleCollider col;

        List<ReflectBeamSegment> segments = new List<ReflectBeamSegment>();

        public ReflectBeam(Vector2 position, float angle, float length) : base(position)
        {
            Friendly = false;

            _angle = angle;
            this.length = length;

            col = AddCapsuleCollider(Vector2.Zero, GameHelper.OrbitPos(Vector2.Zero, angle, length), 15f);
        }

        public override void Update(GameTime gameTime)
        {
            segments.Clear();

            Vector2 currentPos = Transform.Position;
            float currentAngle = Angle;
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

            timer += 1;

            if (Locked && timer >= 5)
                timer = 5;

            if (timer >= 21)
            {
                Despawn();
            }

            foreach (var segment in segments)
            {
                Vector2 dir = GameHelper.DirFromAngle(segment.Angle);
                List<Player> players = Raycast.FireCapsule(segment.Position, segment.Position + dir * segment.Length, 16).OfType<Player>().ToList();

                foreach (var player in players)
                {
                    player.Damage(1, false);
                }
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

            Vector2 dir = GameHelper.DirFromAngle(Angle);
            Vector2 perp = new Vector2(-dir.Y, dir.X);

            Vector2 upperPos = basePos + perp * halfThickness;
            Vector2 lowerPos = basePos - perp * halfThickness;

            var pixel = TextureManager.GetPixelTexture();

            Screen.spriteBatch.Draw(pixel, new Rectangle((int)basePos.X, (int)basePos.Y, (int)length, 32), null, new Color(255, 0, 0, 255), segAngle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
            Screen.spriteBatch.Draw(pixel, new Rectangle((int)upperPos.X, (int)upperPos.Y, (int)length, 4), null, Color.Red, segAngle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
            Screen.spriteBatch.Draw(pixel, new Rectangle((int)lowerPos.X, (int)lowerPos.Y, (int)length, 4), null, Color.Red, segAngle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
        }

        public override void DrawDebug(GameTime gameTime)
        {
            base.DrawDebug(gameTime);

            foreach (var segment in segments)
            {
                Vector2 dir = GameHelper.DirFromAngle(segment.Angle);
                Vector2 pointB = segment.Position + dir * segment.Length;

                Screen.spriteBatch.DrawCircle(segment.Position, 16, 16, Color.Yellow);
                Screen.spriteBatch.DrawCircle(pointB, 16, 16, Color.Yellow);
                Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle((int)segment.Position.X, (int)segment.Position.Y, (int)Vector2.Distance(segment.Position, pointB), (int)(16 * 2f)), null, new Color(Color.Yellow.R, Color.Yellow.G, Color.Yellow.B, 0.5f), GameHelper.AngleBetween(segment.Position, pointB), new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
            }
        }
    }
}
