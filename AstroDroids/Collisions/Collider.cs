using AstroDroids.Entities;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace AstroDroids.Collisions
{
    public abstract class Collider
    {
        public Vector2 LocalOffset;

        public abstract RectangleF Bounds(Transform transform);

        public abstract bool Intersects(Collider other, Transform myTransform, Transform otherTransform);

        public abstract void DrawDebug(Transform transform);
    }
}
