using AstroDroids.Entities;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace AstroDroids.Collisions
{
    public class CapsuleCollider : Collider
    {
        public Vector2 PointB;
        public float Radius;

        public CapsuleCollider(Vector2 PointA, Vector2 PointB, float radius)
        {
            LocalOffset = PointA;
            this.PointB = PointB;
            Radius = radius;
        }

        public BoundingCapsule2D GetWorldShape(Transform transform)
        {
            return new BoundingCapsule2D(transform.Position + LocalOffset, transform.Position + PointB, Radius);
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
            var worldShape = GetWorldShape(transform);
            Screen.spriteBatch.DrawCircle(worldShape.PointA, worldShape.Radius, 16, Color.Yellow);
            Screen.spriteBatch.DrawCircle(worldShape.PointB, worldShape.Radius, 16, Color.Yellow);
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle((int)worldShape.PointA.X, (int)worldShape.PointA.Y, (int)Vector2.Distance(worldShape.PointA, worldShape.PointB), (int)(worldShape.Radius * 2f)), null, Color.Yellow, GameHelper.AngleBetween(worldShape.PointA, worldShape.PointB), new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
        }
    }
}
