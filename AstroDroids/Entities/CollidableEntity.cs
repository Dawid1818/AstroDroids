using AstroDroids.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Runtime.Serialization;

namespace AstroDroids.Entities
{
    public class Transform
    {
        float x;
        float y;

        float width;
        float height;

        bool followsCamera = true;

        public float X { get {
                if (followsCamera)
                    return x + (Screen.GetCameraPosition().X - Screen.ScreenWidth / 2);
                else
                    return x;
            } set { x = value; } }
        public float Y { get {
                if (followsCamera)
                    return y + (Screen.GetCameraPosition().Y - Screen.ScreenHeight / 2);
                else
                    return y; 
            } set { y = value; } }

        public float Width { get { return width; } set { width = value; } }
        public float Height { get { return height; } set { height = value; } }

        public Vector2 Position { get { return new Vector2(X, Y); } set { X = value.X; Y = value.Y; } }
        public float Right { get { return X + width; } }
        public float Bottom { get { return Y + Height; } }

        public Transform(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Intersects(Transform other)
        {
            return ToRectangleF().Intersects(other.ToRectangleF());
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
        }

        public RectangleF ToRectangleF()
        {
            return new RectangleF(X, Y, Width, Height);
        }
    }

    public class CollidableEntity : Entity
    {
        protected Transform Transform;
        //protected RectangleF Collider;

        public CollidableEntity() : base()
        {
            Transform = new Transform(0f, 0f, 32f, 32f);
            //Collider = new RectangleF(0f, 0f, 32f, 32f);
        }

        //public CollidableEntity(RectangleF collider) : base()
        //{
        //    Collider = collider;
        //}

        public CollidableEntity(Transform transform) : base()
        {
            Transform = transform;
        }

        //public bool CollidesWith(RectangleF other)
        //{
        //    return Collider.Intersects(other);
        //}

        public bool CollidesWith(Transform other)
        {
            return Transform.Intersects(other);
        }
    }
}
