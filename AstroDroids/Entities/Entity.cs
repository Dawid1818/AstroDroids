using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroDroids.Entities
{
    public class Entity
    {
        protected RectangleF Collider;
        int Health;

        public Entity()
        {
            Collider = new RectangleF(0f, 0f, 32f, 32f);
            Health = 1;
        }

        public Entity(RectangleF collider, int health)
        {
            Collider = collider;
            Health = health;
        }

        public int GetHealth() { return Health; }
        public void SetHealth(int health) { Health = health; }

        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }

        public virtual void Damage(int damage, bool produceSound)
        {
            Health -= damage;
        }
    }
}
