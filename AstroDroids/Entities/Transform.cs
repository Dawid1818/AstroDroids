using Microsoft.Xna.Framework;

namespace AstroDroids.Entities
{
    public class Transform
    {
        private Vector2 localPosition;
        private Transform parent;

        public Vector2 LocalPosition
        {
            get => localPosition;
            set => localPosition = value;
        }

        public Vector2 Position
        {
            get
            {
                if (parent != null)
                    return parent.Position + localPosition;

                return localPosition;
            }
            set
            {
                if (parent != null)
                    localPosition = value - parent.Position;
                else
                    localPosition = value;
            }
        }

        public Transform(float x, float y)
        {
            localPosition = new Vector2(x, y);
        }

        public Transform(Vector2 position)
        {
            localPosition = position;
        }

        public void Translate(Vector2 translation)
        {
            localPosition += translation;
        }

        public void SetParent(Transform parent)
        {
            this.parent = parent;
        }

        public Transform GetParent()
        {
            return parent;
        }

    }
}
