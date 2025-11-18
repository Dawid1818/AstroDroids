
using AstroDroids.Gameplay;
using MonoGame.Extended;

namespace AstroDroids.Entities
{
    public class Enemy : AliveEntity
    {
        protected int Score = 10;

        bool destroyed = false;

        public Enemy() : base()
        {
        }

        public Enemy(Transform collider, int health) : base(collider, health)
        {
        }

        public override void Destroyed()
        {
            if(destroyed) return;

            //spawn explosion later
            GameState.AddScore(Score);
            Despawn();

            destroyed = true;
        }

        protected void Despawn()
        {
            Scene.World.RemoveEnemy(this);
        }
    }
}
