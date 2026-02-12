using AstroDroids.Entities;

namespace AstroDroids.Projectiles
{
    public class Projectile : CollidableEntity
    {
        public Projectile(Transform collider, float width, float height) : base(collider, width, height)
        {
        }

        protected void Despawn()
        {
            Scene.World.RemoveProjectile(this);
        }
    }
}
