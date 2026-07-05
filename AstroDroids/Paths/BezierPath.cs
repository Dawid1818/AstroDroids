using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace AstroDroids.Paths
{
    public class BezierPath : IPath
    {
        private List<double> arcLengths = new List<double>();
        private const int LUT_RESOLUTION = 100;

        public int MinimumPoints { get; } = 4;

        public List<PathPoint> KeyPoints { get; private set; } = new List<PathPoint>();

        public PathPoint StartPoint { get => KeyPoints[0]; set => KeyPoints[0] = value; }
        public PathPoint EndPoint { get => KeyPoints[KeyPoints.Count - 1]; set => KeyPoints[KeyPoints.Count - 1] = value; }

        public double Length { get; private set; }

        public BezierPath()
        {
            KeyPoints = new List<PathPoint>() { PathPoint.Zero, PathPoint.Zero, PathPoint.Zero, PathPoint.Zero };
            RecalculateLength();
        }

        public BezierPath(List<PathPoint> points)
        {
            KeyPoints = points;
            RecalculateLength();
        }

        public PathPoint GetPoint(double t)
        {
            if (KeyPoints.Count == 0)
            {
                return PathPoint.Zero;
            }

            t = Math.Clamp(t, 0f, 1f);

            PathPoint result = Vector2.Zero;

            for (int i = 0; i < KeyPoints.Count; i++)
            {
                var binom = binomialCoefficient(KeyPoints.Count - 1, i);
                var term = Math.Pow(1 - t, KeyPoints.Count - 1 - i) * Math.Pow(t, i);
                result += (float)(binom * term) * KeyPoints[i];
            }

            return result;
        }

        public int GetPointCount()
        {
            return KeyPoints.Count;
        }

        public Vector2 GetPointAtIndex(int index)
        {
            if (index < 0 || index >= KeyPoints.Count)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }

            return KeyPoints[index];
        }

        public void SetPointAtIndex(int index, Vector2 point)
        {
            if (index < 0 || index >= KeyPoints.Count)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }
            KeyPoints[index] = point;
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

        public Vector2 GetDirection(double t)
        {
            int n = KeyPoints.Count - 1;
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
                Vector2 diff = KeyPoints[i + 1] - KeyPoints[i];
                result += binom * term * diff;
            }

            result *= n;

            if (result != Vector2.Zero)
                result.Normalize();

            return result;
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(KeyPoints.Count);
            foreach (var item in KeyPoints)
            {
                writer.Write(item.X);
                writer.Write(item.Y);
            }

            writer.Write(Length);
        }

        public void Load(BinaryReader reader, int version)
        {
            int pointCount = reader.ReadInt32();
            KeyPoints.Clear();
            for (int i = 0; i < pointCount; i++)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                KeyPoints.Add(new Vector2(x, y));
            }

            Length = reader.ReadDouble();
        }

        public void Translate(PathPoint dist)
        {
            for (int i = 0; i < KeyPoints.Count; i++)
            {
                KeyPoints[i].X += dist.X;
                KeyPoints[i].Y += dist.Y;
            }
        }

        public void RecalculateLength()
        {
            Length = 0;
            arcLengths.Clear();
            arcLengths.Add(0);

            float step = 1f / LUT_RESOLUTION;
            PathPoint lastPos = GetPoint(0f);

            for (int i = 1; i <= LUT_RESOLUTION; i++)
            {
                float t = i * step;
                PathPoint nextPos = GetPoint(t);
                Length += nextPos.DistanceFrom(lastPos);
                arcLengths.Add(Length);
                lastPos = nextPos;
            }

            if (double.IsNaN(Length) || double.IsInfinity(Length))
                Length = 0;
        }

        public double GetParameterAtDistance(double targetDistance)
        {
            if (targetDistance <= 0) return 0f;
            if (targetDistance >= Length) return 1f;

            for (int i = 0; i < arcLengths.Count - 1; i++)
            {
                if (targetDistance >= arcLengths[i] && targetDistance <= arcLengths[i + 1])
                {
                    double segmentLength = arcLengths[i + 1] - arcLengths[i];
                    double segmentFraction = (targetDistance - arcLengths[i]) / segmentLength;

                    double tStart = (double)i / LUT_RESOLUTION;
                    double tEnd = (double)(i + 1) / LUT_RESOLUTION;

                    return tStart + (segmentFraction * (tEnd - tStart));
                }
            }
            return 1f;
        }

        public PathPoint GetPointAtDistance(double distance)
        {
            double t = GetParameterAtDistance(distance);
            return GetPoint(t);
        }
    }
}
