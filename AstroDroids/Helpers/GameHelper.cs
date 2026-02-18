using AstroDroids.Paths;
using Microsoft.Xna.Framework;
using System;
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

        public static float AngleBetween(Vector2 p1, Vector2 p2)
        {
            return (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
        }

        public static Vector2 DirFromAngle(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static float AngleFromDir(Vector2 dir)
        {
            return (float)Math.Atan2(dir.Y, dir.X);
        }

        public static Vector2 OrbitPos(Vector2 center, float orbitAngle, float distance)
        {
            return center + DirFromAngle(orbitAngle) * distance;
        }

        public static Vector2 RotateAroundPoint(Vector2 point, Vector2 center, float angle)
        {
            Vector2 offset = point - center;

            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            Vector2 rotatedOffset = new Vector2(offset.X * cos - offset.Y * sin, offset.X * sin + offset.Y * cos);

            return center + rotatedOffset;
        }
    }
}
