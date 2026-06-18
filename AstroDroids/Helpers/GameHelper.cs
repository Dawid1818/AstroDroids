using AstroDroids.Graphics;
using AstroDroids.Paths;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace AstroDroids.Helpers
{
    public class GameHelper
    {
        public static BezierPath CreateBezier(Vector2 start, Vector2 end)
        {
            Vector2 delta = end - start;

            Vector2 dir;

            if (delta.LengthSquared() < 0.000001f)
            {
                dir = Vector2.Zero;
            }
            else
            {
                dir = Vector2.Normalize(delta);
            }

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

        public static BezierPath CreateBezier(Vector2 start, Vector2 end, float currentAngle)
        {
            Vector2 startDir = DirFromAngle(currentAngle);

            Vector2 endDir;

            Vector2 delta = end - start;

            if (delta.LengthSquared() < 0.000001f)
                endDir = startDir;
            else
                endDir = Vector2.Normalize(delta);

            float distance = Vector2.Distance(start, end);

            Vector2 cp1 = start + startDir * distance * 0.35f;
            Vector2 cp2 = end - endDir * distance * 0.35f;

            return new BezierPath(new List<PathPoint>() { start, cp1, cp2, end });
        }

        public static float AngleBetween(Vector2 p1, Vector2 p2)
        {
            return (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
        }

        public static Vector2 DirFromAngle(float angle)
        {
            return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        }

        public static float AngleFromDir(Vector2 dir)
        {
            return MathF.Atan2(dir.Y, dir.X);
        }

        public static Vector2 OrbitPos(Vector2 center, float orbitAngle, float distance)
        {
            return center + DirFromAngle(orbitAngle) * distance;
        }

        public static Vector2 RotateAroundPoint(Vector2 point, Vector2 center, float angle)
        {
            Vector2 offset = point - center;

            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);
            Vector2 rotatedOffset = new Vector2(offset.X * cos - offset.Y * sin, offset.X * sin + offset.Y * cos);

            return center + rotatedOffset;
        }

        public static List<float> SpreadAngle(float startAngle, float amount, float spacing)
        {
            List<float> angles = new List<float>();

            float spacingRad = MathHelper.ToRadians(spacing);
            float angle = startAngle - spacingRad * (amount - 1) / 2f;

            for (int i = 0; i < amount; i++)
            {
                angles.Add(angle);
                angle += spacingRad;
            }

            return angles;
        }

        public static void DrawNode(string label, Vector2 position, Color color, Color borderColor, float fontSize = 24)
        {
            Screen.spriteBatch.DrawCircle(position, 16f, 16, color, 16f);
            Screen.spriteBatch.DrawCircle(position, 16f, 16, borderColor, 1f);
            Vector2 measurement = Screen.MeasureText(label, fontSize);
            Screen.DrawText(label, position - new Vector2(measurement.X / 2f, measurement.Y / 2f), Color.White, fontSize);
        }
    }
}
