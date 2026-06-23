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
            int segmentCount = 10;
            for (int i = 0; i < segmentCount; i++)
            {
                SnakeBossSegment segment;

                if (i == 0)
                    segment = new SnakeBossSegment(this, null, i * 40);
                else
                    segment = new SnakeBossSegment(this, segments[i - 1], i * 40);

                segment.Transform.Position = Transform.Position + new Vector2(-(i * 20), 0);
                segments.Add(segment);
            }

            for (int i = 0; i < MaxHistory; i++)
            {
                PositionHistory.Add(Transform.Position + new Vector2(-(i), 0));
            }

            for (int i = segmentCount - 1; i >= 0; i--)
            {
                Scene.World.AddEnemy(segments[i], true);
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
