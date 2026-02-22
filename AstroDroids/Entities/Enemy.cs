
using AstroDroids.Gameplay;
using AstroDroids.Paths;

namespace AstroDroids.Entities
{
    public class Enemy : AliveEntity
    {
        protected int Score = 10;

        public bool destroyed { get; private set; } = false;

        public PathManager PathManager { get; set; }
        public bool FollowsCamera { get; set; }

        public Enemy() : base()
        {
        }

        public Enemy(Transform collider, int health, float width, float height) : base(collider, health, width, height)
        {
        }

        public virtual void Spawned()
        {

        }

        public override void Destroyed()
        {
            if (destroyed) return;

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
