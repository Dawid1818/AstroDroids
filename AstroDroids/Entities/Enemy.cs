
using AstroDroids.Entities.Effects;
using AstroDroids.Gameplay;
using AstroDroids.Paths;
using Microsoft.Xna.Framework;
using System.IO;

namespace AstroDroids.Entities
{
    public class DefaultSpawnData : IEnemySpawnData
    {
        public void DrawEditor()
        {

        }

        public void Load(BinaryReader reader, int version)
        {

        }

        public void Save(BinaryWriter writer)
        {

        }
    }
    public class Enemy : AliveEntity
    {
        protected int Score = 10;

        public bool destroyed { get; private set; } = false;

        public PathManager PathManager { get; set; }
        public bool FollowsCamera { get; set; }

        public Enemy() : base()
        {
        }

        public Enemy(Vector2 position, int health) : base(new Transform(position), health)
        {
        }

        public virtual void Spawned()
        {

        }

        public virtual void ApplySpawnData(IEnemySpawnData spawnData)
        {
        }

        public override void Destroyed()
        {
            if (destroyed) return;

            Scene.World.AddEffect(new StandardExplosion(new Transform(Transform.Position.X, Transform.Position.Y), 0.6f));

            GameState.AddScore(Score);
            Despawn();

            destroyed = true;
        }

        public void Despawn()
        {
            Scene.World.RemoveEnemy(this);
        }
    }
}
