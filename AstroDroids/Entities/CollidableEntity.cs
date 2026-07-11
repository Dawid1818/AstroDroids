using AstroDroids.Collisions;
using AstroDroids.Gameplay;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using System.Collections.Generic;

namespace AstroDroids.Entities
{
    public class CollidableEntity : Entity
    {
        public float Width { get; private set; } = 0;
        public float Height { get; private set; } = 0;

        public virtual bool Collidable { get; protected set; } = true;

        public List<Collider> Colliders { get; private set; } = new List<Collider>();

        public CollidableEntity() : base()
        {

        }

        public CollidableEntity(Transform transform) : base(transform)
        {

        }

        public Rectangle ToRectangle()
        {
            return new Rectangle((int)Transform.Position.X, (int)Transform.Position.Y, (int)Width, (int)Height);
        }

        public RectangleF ToRectangleF()
        {
            return new RectangleF(Transform.Position.X, Transform.Position.Y, Width, Height);
        }

        public bool Intersects(Rectangle other)
        {
            foreach (var item in Colliders)
            {
                if(item.Intersects(other, Transform))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Intersects(CircleF other)
        {
            foreach (var item in Colliders)
            {
                if (item.Intersects(other, Transform))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Intersects(CollidableEntity other)
        {
            foreach (var item in Colliders)
            {
                foreach (var otherCol in other.Colliders)
                {
                    if (otherCol.Intersects(item, other.Transform, Transform))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected CircleCollider AddCircleCollider(Vector2 offset, float radius)
        {
            CircleCollider collider = new CircleCollider(offset, radius);
            Colliders.Add(collider);
            RecalculateBounds();
            return collider;
        }

        protected CapsuleCollider AddCapsuleCollider(Vector2 PointA, Vector2 PointB, float radius)
        {
            CapsuleCollider collider = new CapsuleCollider(PointA, PointB, radius);
            Colliders.Add(collider);
            RecalculateBounds();
            return collider;
        }

        void RecalculateBounds()
        {
            if (Colliders.Count == 0)
            {
                Width = 0;
                Height = 0;
                return;
            }

            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (var item in Colliders)
            {
                RectangleF bounds = item.Bounds(Transform);
                if (bounds.Right > maxX)
                    maxX = bounds.Right;

                if (bounds.Left < minX)
                    minX = bounds.Left;

                if (bounds.Bottom > maxY)
                    maxY = bounds.Bottom;

                if (bounds.Top < minY)
                    minY = bounds.Top;
            }

            Width = maxX - minX;
            Height = maxY - minY;
        }

        public static Vector2 ClampPosition(Vector2 position, GameWorld world)
        {
            float clampedX = MathHelper.Clamp(position.X, world.Bounds.Left, world.Bounds.Right);
            float clampedY = MathHelper.Clamp(position.Y, world.Bounds.Top, world.Bounds.Bottom);
            return new Vector2(clampedX, clampedY);
        }

        public override void DrawDebug(GameTime gameTime)
        {
            base.DrawDebug(gameTime);

            foreach (var col in Colliders)
            {
                col.DrawDebug(Transform);
            }
        }
    }
}
