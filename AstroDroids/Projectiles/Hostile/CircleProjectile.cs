using AstroDroids.Collisions;
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
        float decay = 0f;
        CircleCollider col;

        public CircleProjectile(Vector2 position, float angle, float speed, float size) : base(position)
        {
            Friendly = false;

            this.angle = angle;
            this.speed = speed;
            this.size = size;

            col = AddCircleCollider(Vector2.Zero, size);

            actualPosition = position;
        }

        public CircleProjectile(Vector2 position, float angle, float speed, float size, float decay) : base(position)
        {
            Friendly = false;

            this.angle = angle;
            this.speed = speed;
            this.size = size;
            this.decay = decay;

            col = AddCircleCollider(Vector2.Zero, size);

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

            foreach (var item in Scene.World.Neutrals)
            {
                if (item.Intersects(this))
                {
                    item.Damage(1, false);
                    item.Push(GameHelper.DirectionFromTo(item.Transform.Position, Transform.Position));
                    Despawn();

                    return;
                }
            }

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
            size -= decay * gameTime.GetElapsedSeconds();

            col.Radius = size;

            if(size <= 0)
            {
                Despawn();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.shapeBatch.DrawCircle(Transform.Position, size - 3, Color.DarkOrange, Color.OrangeRed, 1);
            Screen.shapeBatch.BorderCircleBlurred(Transform.Position, size, Color.OrangeRed, 3, 3);
        }
    }
}
