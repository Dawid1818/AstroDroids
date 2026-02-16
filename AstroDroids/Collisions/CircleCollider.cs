using AstroDroids.Entities;
using AstroDroids.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace AstroDroids.Collisions
{
    public class CircleCollider : Collider
    {
        public float Radius;

        public CircleCollider(Vector2 localOffset, float radius)
        {
            LocalOffset = localOffset;
            Radius = radius;
        }

        public BoundingCircle2D GetWorldShape(Transform transform)
        {
            return new BoundingCircle2D(transform.Position + LocalOffset, Radius);
        }

        public override bool Intersects(Collider other, Transform myTransform, Transform otherTransform)
        {
            if (other is CircleCollider circle)
                return GetWorldShape(myTransform).Intersects(circle.GetWorldShape(otherTransform));

            if (other is CapsuleCollider capsule)
                return GetWorldShape(myTransform).Intersects(capsule.GetWorldShape(otherTransform));

            return false;
        }

        public override void DrawDebug(Transform transform)
        {
            Screen.spriteBatch.DrawCircle(transform.Position + LocalOffset, Radius, 16, Color.Yellow);
        }
    }
}
