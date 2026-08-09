using AstroDroids.Collisions;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Collections.Generic;

namespace AstroDroids.Projectiles.Hostile
{
    public class SentinelLaser : Projectile
    {
        float t = 0f;
        public float Angle { get; set; }
        Vector2 movementDirection { get { return GameHelper.DirFromAngle(Angle); } }
        public float Speed { get; set; } = 10f;
        public float Size { get; set; } = 0f;

        Vector2 actualPosition;
        CircleCollider col;

        public SentinelLaser(Vector2 position, float angle, float speed, float size) : base(position)
        {
            Friendly = false;

            Angle = angle;
            Speed = speed;
            Size = size;
            Size = 0f;

            //col = AddCircleCollider(Vector2.Zero, size);
            AddCapsuleCollider(Vector2.Zero, new Vector2(0, 20), 5);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Intersects(Scene.World.Bounds))
            {
                if (t >= 10f)
                    Despawn();

                t += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            actualPosition = (movementDirection * Speed);
            Transform.LocalPosition += actualPosition;

            foreach (var item in Scene.World.Projectiles)
            {
                if(item is SentinelCircleProjectile circleProj)
                {
                    if (item.Intersects(this))
                    {
                        if (circleProj.Speed != 0 && circleProj.CanDetonate)
                        {
                            Despawn();
                            circleProj.Detonate();
                            return;
                        }
                    }
                }
            }

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
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 forward = Transform.Position + GameHelper.DirFromAngle(Angle) * 20;
            Screen.shapeBatch.DrawLine(Transform.Position, forward, 5, Color.DarkRed, Color.Red);
            Screen.shapeBatch.BorderLineBlurred(Transform.Position, forward, 8, Color.Red, 5f, 20);

            //Screen.shapeBatch.DrawCircle(Transform.Position, Size - 3, Color.DarkRed, Color.Red, 1);
            //Screen.shapeBatch.BorderCircleBlurred(Transform.Position, Size, Color.Red, 3, 3);
        }
    }
}
