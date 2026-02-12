using AstroDroids.Paths;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AstroDroids.Helpers
{
    public class GameHelper
    {
        public static BezierPath CreateBezier(Vector2 start, Vector2 end)
        {
            Vector2 dir = Vector2.Normalize(end - start);
            Vector2 perp = new Vector2(-dir.Y, dir.X);

            float distance = Vector2.Distance(start, end);

            Vector2 cp1 = start + dir * distance * 0.25f + perp * 45f;
            Vector2 cp2 = start + dir * distance * 0.75f + perp * 45f;

            List<PathPoint> points = new List<PathPoint>()
                {
                    start,
                    cp1,
                    cp2,
                    end
                };

            return new BezierPath(points);
        }
    }
}
