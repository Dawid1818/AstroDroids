using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace AstroDroids.Entities
{
    public class AliveEntity : CollidableEntity
    {
        float StartingHealth;
        float Health;

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

        public float GetHealth() { return Health; }
        public float GetStartingHealth() { return StartingHealth; }
        public virtual void SetHealth(float health) { Health = health; }
        public virtual void SetStartingHealth(float health) { StartingHealth = health; }

        public virtual void Damage(float damage, bool produceSound)
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
