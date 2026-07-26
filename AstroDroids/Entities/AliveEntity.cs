using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace AstroDroids.Entities
{
    public class AliveEntity : CollidableEntity
    {
        int StartingHealth;
        int Health;

        public virtual bool CanBeDamaged { get; protected set; } = true;

        public AliveEntity() : base()
        {
            Health = 1;
            StartingHealth = Health;
        }

        public AliveEntity(Transform collider, int health) : base(collider)
        {
            Health = health;
            StartingHealth = health;
        }

        public int GetHealth() { return Health; }
        public int GetStartingHealth() { return StartingHealth; }
        public virtual void SetHealth(int health) { Health = health; }

        public virtual void Damage(int damage, bool produceSound)
        {
            if (!CanBeDamaged)
                return;

            Health -= damage;
            
            if(Health <= 0)
            {
                Destroyed();
            }
        }

        public virtual void Destroyed()
        {
            
        }
    }
}
