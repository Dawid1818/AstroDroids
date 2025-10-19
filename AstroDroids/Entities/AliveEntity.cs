using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace AstroDroids.Entities
{
    public class AliveEntity : Entity
    {
        protected RectangleF Collider;
        int Health;

        public AliveEntity()
        {
            Collider = new RectangleF(0f, 0f, 32f, 32f);
            Health = 1;
        }

        public AliveEntity(RectangleF collider, int health)
        {
            Collider = collider;
            Health = health;
        }

        public int GetHealth() { return Health; }
        public void SetHealth(int health) { Health = health; }

        public virtual void Damage(int damage, bool produceSound)
        {
            Health -= damage;
        }
    }
}
