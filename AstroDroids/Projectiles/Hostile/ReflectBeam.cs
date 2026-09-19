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

namespace AstroDroids.Projectiles.Hostile
{
    public class ReflectBeam : Projectile
    {
        float timer = 0;

        public bool Locked { get; set; } = false;

        float length;
        public float Angle { get { return _angle; } set { _angle = value; col.PointB = GameHelper.OrbitPos(Vector2.Zero, _angle, length); } }
        float _angle;

        CapsuleCollider col;

        List<ReflectBeamSegment> segments = new List<ReflectBeamSegment>();

        Texture2D texture;
        float beamSpeed = 200f;
        float textureOffset = 0f;

        public ReflectBeam(Vector2 position, float angle, float length) : base(position)
        {
            Friendly = false;

            _angle = angle;
            this.length = length;

            texture = TextureManager.Get("Projectiles/ChallengerBeam/ChallengerBeam");

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

            timer += 1f * gameTime.GetElapsedSeconds() * 10f;

            if (Locked && timer >= 5)
                timer = 5;

            if (timer >= 10)
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

            textureOffset -= beamSpeed * gameTime.GetElapsedSeconds();
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
            Color beamColor = Color.Red;

            Rectangle sourceRectangle = new Rectangle((int)textureOffset, 0, (int)length, texture.Height);

            Vector2 origin = new Vector2(0f, texture.Height / 2f);

            float scale;
            if (timer <= 5)
            {
                scale = timer / 5f;
            }
            else
            {
                scale = -(timer - 10f) / 5f;
            }

            Screen.spriteBatch.Draw(texture, basePos, sourceRectangle, new Color(beamColor.R, beamColor.G, beamColor.B, (byte)127), segAngle, origin, new Vector2(1f, 1.4f * scale), SpriteEffects.None, 0f);
            Screen.spriteBatch.Draw(texture, basePos, sourceRectangle, beamColor, segAngle, origin, new Vector2(1f, scale), SpriteEffects.None, 0f);
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
