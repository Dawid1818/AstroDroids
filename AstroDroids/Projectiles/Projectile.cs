using AstroDroids.Entities;
using Microsoft.Xna.Framework;

namespace AstroDroids.Projectiles
{
    public class Projectile : CollidableEntity
    {
        public Projectile(Vector2 position) : base(new Transform(position))
        {
        }

        protected void Despawn()
        {
            Scene.World.RemoveProjectile(this);
        }
    }
}
