using AstroDroids.Entities;
using Microsoft.Xna.Framework;

namespace AstroDroids.Projectiles
{
    public class Projectile : CollidableEntity
    {
        public bool Friendly { get; internal set; } = false;
        public bool BlocksPlayerProjectiles { get; internal set; } = false;

        public Projectile(Vector2 position) : base(new Transform(position))
        {
        }

        public void Despawn()
        {
            Scene.World.RemoveProjectile(this);
        }
    }
}
