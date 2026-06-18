using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AstroDroids.Entities.Hostile.Bosses
{
    public class SnakeBoss : Enemy
    {
        public float t = 0f;

        List<SnakeBossSegment> segments = new List<SnakeBossSegment>();

        bool eliminated = false;

        public List<Vector2> PositionHistory { get; } = new();

        const int MaxHistory = 1000;

        public SnakeBoss() : base(Vector2.Zero, 2)
        {

        }

        public override void Spawned()
        {
            for (int i = 0; i < 10; i++)
            {
                SnakeBossSegment segment;

                if (i == 0)
                    segment = new SnakeBossSegment(this, null, i * 40);
                else
                    segment = new SnakeBossSegment(this, segments[i - 1], i * 40);

                segment.Transform.Position = Transform.Position + new Vector2(-(i * 20), 0);
                Scene.World.AddEnemy(segment, true);
                segments.Add(segment);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!segments.Any(x => x.GetHealth() > 0))
            {
                //boss got defeated?
                //maybe each segment should explode one after another for dramatic effect
                foreach (var item in segments)
                {
                    item.Despawn();
                }
                Despawn();
            }
        }

        public override void Draw(GameTime gameTime)
        {

        }
    }
}
