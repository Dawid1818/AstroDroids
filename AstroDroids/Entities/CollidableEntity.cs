using Microsoft.Xna.Framework;
using MonoGame.Extended;

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
            return ToRectangleF().Intersects(other.ToRectangleF());
        }

        public Vector2 ClampPosition(Vector2 position)
        {
            float clampedX = MathHelper.Clamp(position.X, Scene.World.Bounds.Left, Scene.World.Bounds.Right - Width);
            float clampedY = MathHelper.Clamp(position.Y, Scene.World.Bounds.Top, Scene.World.Bounds.Bottom - Height);
            return new Vector2(clampedX, clampedY);
        }
    }
}
