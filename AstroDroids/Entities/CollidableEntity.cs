using AstroDroids.Collisions;
using AstroDroids.Gameplay;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Collections.Generic;

namespace AstroDroids.Entities
{
    public class CollidableEntity : Entity
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public float Left { get { return Transform.Position.X; } }
        public float Right { get { return Transform.Position.X + Width; } }
        public float LocalLeft { get { return Transform.LocalPosition.X; } }
        public float LocalRight { get { return Transform.LocalPosition.X + Width; } }
        public float Top { get { return Transform.Position.Y; } }
        public float LocalTop { get { return Transform.LocalPosition.Y; } }
        public float Bottom { get { return Transform.Position.Y + Height; } }
        public float LocalBottom { get { return Transform.LocalPosition.Y + Height; } }
        public Vector2 Center { get { return new Vector2(Transform.Position.X + Width / 2f, Transform.Position.Y + Height / 2f); } }
        public Vector2 LocalCenter { get { return new Vector2(Transform.LocalPosition.X + Width / 2f, Transform.LocalPosition.Y + Height / 2f); } }

        public List<Collider> Colliders { get; private set; } = new List<Collider>();
        //public List<BoundingCircle2D> Colliders { get; private set; } = new List<BoundingCircle2D>();

        public CollidableEntity() : base()
        {

        }

        public CollidableEntity(Transform transform) : base(transform)
        {

        }

        public CollidableEntity(Transform transform, float width, float height) : base(transform)
        {
            Width = width;
            Height = height;
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle((int)Transform.Position.X, (int)Transform.Position.Y, (int)Width, (int)Height);
        }

        public RectangleF ToRectangleF()
        {
            return new RectangleF(Transform.Position.X, Transform.Position.Y, Width, Height);
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

        //Original collision code before new collision shapes were added
        //public bool Intersects(CollidableEntity other)
        //{
        //    return ToRectangleF().Intersects(other.ToRectangleF());
        //}

        protected void AddCircleCollider(Vector2 offset, float radius)
        {
            Colliders.Add(new CircleCollider(offset, radius));
        }

        protected void AddCapsuleCollider(Vector2 PointA, Vector2 PointB, float radius)
        {
            Colliders.Add(new CapsuleCollider(PointA, PointB, radius));
        }

        public static Vector2 ClampPosition(Vector2 position, GameWorld world)
        {
            float clampedX = MathHelper.Clamp(position.X, world.Bounds.Left, world.Bounds.Right);
            float clampedY = MathHelper.Clamp(position.Y, world.Bounds.Top, world.Bounds.Bottom);
            return new Vector2(clampedX, clampedY);
        }
    }
}
