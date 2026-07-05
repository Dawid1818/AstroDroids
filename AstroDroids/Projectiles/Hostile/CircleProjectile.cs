using AstroDroids.Entities;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;

namespace AstroDroids.Projectiles.Hostile
{
    public class CircleProjectile : Projectile
    {
        float t = 0f;
        float angle;
        Vector2 movementDirection { get { return GameHelper.DirFromAngle(angle); } }
        float speed = 10f;
        float size = 16f;

        Vector2 actualPosition;
        float phase = 0f;
        float phaseSpeed = 0f;
        float phaseMax = 0f;
        float perpSpeed = 0f;

        public CircleProjectile(Vector2 position, float angle, float speed, float size) : base(position)
        {
            this.angle = angle;
            this.speed = speed;
            this.size = size;

            AddCircleCollider(Vector2.Zero, size);

            actualPosition = position;
        }

        public void SetPerpSpeed(float speed)
        {
            perpSpeed = speed;
        }

        public void SetPhase(float phaseSpeed, float phaseMax)
        {
            this.phaseMax = phaseMax;
            this.phaseSpeed = phaseSpeed;
        }

        public override void Update(GameTime gameTime)
        {
            if (t >= 3)
                Despawn();

            Vector2 perpendicular = new(-movementDirection.Y, movementDirection.X);

            angle += perpSpeed * gameTime.GetElapsedSeconds();

            actualPosition += (movementDirection * speed);
            Vector2 offset = perpendicular * MathF.Cos(phase) * phaseMax;
            Transform.LocalPosition = actualPosition + offset;

            foreach (var item in Scene.World.GetPlayers())
            {
                if (item.Intersects(this))
                {
                    item.Damage(1, false);
                    Despawn();

                    return;
                }
            }

            t += gameTime.GetElapsedSeconds();
            phase += gameTime.GetElapsedSeconds() * phaseSpeed;
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.DrawCircle(Transform.Position, size, 16, Color.OrangeRed, size);
            Screen.spriteBatch.DrawCircle(Transform.Position, size - 4f, 16, Color.DarkOrange, size - 4);
        }
    }
}
