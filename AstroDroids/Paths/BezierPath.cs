using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace AstroDroids.Paths
{
    public class BezierPath : IPath
    {
        List<PathPoint> Points = new List<PathPoint>();

        public PathPoint[] KeyPoints
        {
            get
            {
                return Points.ToArray();
            }
        }

        public PathPoint StartPoint { get => Points[0]; set => Points[0] = value; }
        public PathPoint EndPoint { get => Points[3]; set => Points[3] = value; }

        public double Length { get; private set; }

        public BezierPath()
        {
            Points = new List<PathPoint>() { PathPoint.Zero, PathPoint.Zero, PathPoint.Zero, PathPoint.Zero };
            RecalculateLength();
        }

        public BezierPath(List<PathPoint> points)
        {
            Points = points;
            RecalculateLength();
        }

        public PathPoint GetPoint(double t)
        {
            if (Points.Count == 0)
            {
                return PathPoint.Zero;
            }

            t = Math.Clamp(t, 0f, 1f);

            PathPoint result = Vector2.Zero;

            for (int i = 0; i < Points.Count; i++)
            {
                var binom = binomialCoefficient(Points.Count - 1, i);
                var term = Math.Pow(1 - t, Points.Count - 1 - i) * Math.Pow(t, i);
                result += (float)(binom * term) * Points[i];
            }

            return result;
        }

        public int GetPointCount()
        {
            return Points.Count;
        }

        public Vector2 GetPointAtIndex(int index)
        {
            if (index < 0 || index >= Points.Count)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }

            return Points[index];
        }

        public void SetPointAtIndex(int index, Vector2 point)
        {
            if (index < 0 || index >= Points.Count)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }
            Points[index] = point;
        }

        float binomialCoefficient(float n, float k)
        {
            if (k > n)
            {
                throw new Exception("n must be greater or equal k");
            }

            if (k == 0 || k == n)
            {
                return 1;
            }

            return binomialCoefficient(n - 1, k - 1) + binomialCoefficient(n - 1, k);
        }

        public Vector2 GetDirection(float t)
        {
            int n = Points.Count - 1;
            if (n == 0)
            {
                return Vector2.Zero;
            }

            t = Math.Clamp(t, 0f, 1f);

            Vector2 result = Vector2.Zero;

            for (int i = 0; i < n; i++)
            {
                float binom = binomialCoefficient(n - 1, i);
                float term = (float)(Math.Pow(1 - t, n - 1 - i) * Math.Pow(t, i));
                Vector2 diff = Points[i + 1] - Points[i];
                result += binom * term * diff;
            }

            result *= n;

            if (result != Vector2.Zero)
                result.Normalize();

            return result;
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Points.Count);
            foreach (var item in Points)
            {
                writer.Write(item.X);
                writer.Write(item.Y);
            }

            writer.Write(Length);
        }

        public void Load(BinaryReader reader, int version)
        {
            int pointCount = reader.ReadInt32();
            Points.Clear();
            for (int i = 0; i < pointCount; i++)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                Points.Add(new Vector2(x, y));
            }

            Length = reader.ReadDouble();
        }

        public void Translate(PathPoint dist)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                Points[i].X += dist.X;
                Points[i].Y += dist.Y;
            }
        }

        public void RecalculateLength()
        {
            Length = 0;

            float t = 0f;
            PathPoint lastPos = GetPoint(t);
            while (t < 1f)
            {
                t += 0.01f;
                PathPoint nextPos = GetPoint(t);
                Length += nextPos.DistanceFrom(lastPos);
                lastPos = nextPos;
            }
        }
    }
}
