
using AstroDroids.Curves;
using AstroDroids.Gameplay;
using MonoGame.Extended;

namespace AstroDroids.Entities
{
    public class Enemy : AliveEntity
    {
        protected int Score = 10;

        bool destroyed = false;

        public CompositePath Path { get; set; }

        public Enemy() : base()
        {
        }

        public Enemy(Transform collider, int health, float width, float height) : base(collider, health, width, height)
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
