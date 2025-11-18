using MonoGame.Extended;

namespace AstroDroids.Entities
{
    public class AliveEntity : CollidableEntity
    {
        int Health;

        public AliveEntity() : base()
        {
            Health = 1;
        }

        public AliveEntity(Transform collider, int health) : base(collider)
        {
            Health = health;
        }

        public int GetHealth() { return Health; }
        public void SetHealth(int health) { Health = health; }

        public virtual void Damage(int damage, bool produceSound)
        {
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
