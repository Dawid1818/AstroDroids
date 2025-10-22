using AstroDroids.Entities;
using MonoGame.Extended;

namespace AstroDroids.Projectiles
{
    public class Projectile : CollidableEntity
    {
        public Projectile(RectangleF collider) : base(collider)
        {
        }

        protected void Despawn()
        {
            Scene.World.RemoveProjectile(this);
        }
    }
}
