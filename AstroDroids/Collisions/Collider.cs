using AstroDroids.Entities;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;

namespace AstroDroids.Collisions
{
    public abstract class Collider
    {
        public Vector2 LocalOffset;

        public abstract RectangleF Bounds(Transform transform);

        public abstract bool Intersects(Collider other, Transform myTransform, Transform otherTransform);
        public abstract bool Intersects(Rectangle other, Transform transform);

        public abstract void DrawDebug(Transform transform);
    }
}
