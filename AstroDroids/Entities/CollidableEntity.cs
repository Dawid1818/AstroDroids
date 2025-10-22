using MonoGame.Extended;

namespace AstroDroids.Entities
{
    public class CollidableEntity : Entity
    {
        protected RectangleF Collider;

        public CollidableEntity() : base()
        {
            Collider = new RectangleF(0f, 0f, 32f, 32f);
        }

        public CollidableEntity(RectangleF collider) : base()
        {
            Collider = collider;
        }

        public bool CollidesWith(RectangleF other)
        {
            return Collider.Intersects(other);
        }
    }
}
