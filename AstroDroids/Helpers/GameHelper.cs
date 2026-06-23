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
        public static float WrapValue(float x, float x_min, float x_max)
        {
            return (((x - x_min) % (x_max - x_min)) + (x_max - x_min)) % (x_max - x_min) + x_min;
        }

        public static CatmullRomPath FlattenComposite(List<CatmullRomPath> subPaths)
        {
            List<PathPoint> points = new List<PathPoint>();

            for (int i = 0; i < subPaths.Count; i++)
            {
                CatmullRomPath current = subPaths[i];
                int startIndex = (i == 0) ? 0 : 1;

                for (int j = startIndex; j < current.GetPointCount(); j++)
                {
                    points.Add(current.GetPointAtIndex(j));
                }
            }

            return new CatmullRomPath(points);
        }

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

        public static CatmullRomPath CreateCatmull(Vector2 start, Vector2 end)
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

            return new CatmullRomPath(points);
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

            Vector2 perp = new Vector2(-endDir.Y, endDir.X) * delta;

            Vector2 cp1 = start + startDir * distance * 0.2f;
            Vector2 cp2 = end - endDir * distance * 0.2f;

            return new BezierPath(new List<PathPoint>() { start, cp1, cp2, end });
        }

        public static CatmullRomPath CreateCatmull(Vector2 start, Vector2 end, float currentAngle)
        {
            Vector2 startDir = DirFromAngle(currentAngle);
            Vector2 delta = end - start;
            Vector2 dir = delta.LengthSquared() < 0.000001f ? Vector2.Zero : Vector2.Normalize(delta);

            float distance = Vector2.Distance(start, end);

            float dot = MathHelper.Clamp(Vector2.Dot(startDir, dir), -1f, 1f);
            float turnAngle = MathF.Acos(dot);
            float turnFactor = turnAngle / MathF.PI;

            Vector2 perp = new Vector2(-startDir.Y, startDir.X);

            float side = Vector2.Dot(perp, dir);
            float sideSign = MathF.Abs(side) > 0.0001f ? MathF.Sign(side) : 1f;

            float minHandle = 30f;
            float maxHandle = 140f;
            float handleLength = MathHelper.Lerp(minHandle, maxHandle, turnFactor);
            handleLength = MathF.Min(handleLength, distance * 0.5f);

            float bowAmount = MathHelper.Lerp(0f, handleLength * 1.2f, turnFactor) * sideSign;

            Vector2 phantom = start - startDir * handleLength;
            Vector2 cp1 = start + startDir * handleLength;

            float aheadBlend = MathHelper.Lerp(0.5f, 0.1f, turnFactor);
            Vector2 cp2 = (start + dir * distance * aheadBlend) + (startDir * handleLength * 0.8f) + perp * bowAmount;

            List<PathPoint> points = new List<PathPoint>() { start, cp1, cp2, end };

            var path = new CatmullRomPath(points);
            path.SetPhantomStart(phantom);
            return path;
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

        public static Vector2 OrbitEllipsePos(Vector2 center, float orbitAngle, float distanceX, float distanceY, float ellipseRot)
        {
            float cosRot = MathF.Cos(ellipseRot);
            float sinRot = MathF.Sin(ellipseRot);

            float localX = MathF.Cos(orbitAngle) * distanceX;
            float localY = MathF.Sin(orbitAngle) * distanceY;

            float rotatedX = localX * cosRot - localY * sinRot;
            float rotatedY = localX * sinRot + localY * cosRot;

            return new Vector2(center.X + rotatedX, center.Y + rotatedY);
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

        public static Vector2 RandomPosition(Rectangle bounds)
        {
            return new Vector2(AstroDroidsGame.rnd.Next(bounds.X, bounds.Width + bounds.X), AstroDroidsGame.rnd.Next(bounds.Y, bounds.Height + bounds.Y));
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
