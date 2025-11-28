using Microsoft.Xna.Framework;

namespace AstroDroids.Entities
{
    public class Transform
    {
        float x;
        float y;

        Transform parent;

        float X
        {
            get
            {
                if (parent != null)
                    return x + parent.X;
                else
                    return x;
            }
            set { x = value; }
        }
        float Y
        {
            get
            {
                if (parent != null)
                    return y + parent.Y;
                else
                    return y;
            }
            set { y = value; }
        }

        public Vector2 Position { get { return new Vector2(X, Y); } set { X = value.X; Y = value.Y; } }
        public Vector2 LocalPosition { get { return new Vector2(x, y); } set { x = value.X; y = value.Y; } }

        public Transform(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void Translate(Vector2 translation)
        {
            x += translation.X;
            y += translation.Y;
        }

        public void SetParent(Transform parent)
        {
            this.parent = parent;
        }
    }
}
