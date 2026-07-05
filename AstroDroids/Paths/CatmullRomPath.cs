using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace AstroDroids.Paths
{
    public class CatmullRomPath : IPath
    {
        private List<double> arcLengths = new List<double>();
        private const int LUT_RESOLUTION = 100;
        public List<PathPoint> KeyPoints { get; private set; } = new List<PathPoint>();
        public int MinimumPoints { get; } = 4;

        private PathPoint? phantomStart = null;
        private PathPoint? phantomEnd = null;

        public void SetPhantomStart(PathPoint p) 
        { 
            phantomStart = p;
            RecalculateLength();
        }
        public void SetPhantomEnd(PathPoint p)
        {
            phantomEnd = p;
            RecalculateLength();
        }
        public PathPoint StartPoint { get => KeyPoints[0]; set => KeyPoints[0] = value; }
        public PathPoint EndPoint { get => KeyPoints[KeyPoints.Count-1]; set => KeyPoints[KeyPoints.Count-1] = value; }

        public double Length { get; private set; }

        public CatmullRomPath()
        {
            KeyPoints = new List<PathPoint>() { PathPoint.Zero, PathPoint.Zero, PathPoint.Zero, PathPoint.Zero };
            RecalculateLength();
        }

        public CatmullRomPath(List<PathPoint> points)
        {
            KeyPoints = points;
            RecalculateLength();
        }

        public PathPoint GetPoint(double t)
        {
            int numPoints = KeyPoints.Count;
            if (numPoints == 0) return PathPoint.Zero;
            if (numPoints == 1) return KeyPoints[0];

            t = Math.Clamp(t, 0.0, 1.0);

            int numSegments = numPoints - 1;
            double globalT = t * numSegments;
            int index = (int)Math.Floor(globalT);

            if (index >= numSegments)
            {
                index = numSegments - 1;
            }
            double u = globalT - index;

            Vector2 p0 = (index - 1 < 0) ? (phantomStart ?? KeyPoints[0]) : KeyPoints[index - 1];
            Vector2 p1 = KeyPoints[index];
            Vector2 p2 = KeyPoints[index + 1];
            Vector2 p3 = (index + 2 >= numPoints) ? (phantomEnd ?? KeyPoints[numPoints - 1]) : KeyPoints[index + 2];

            if (p1 == p2) return p1;

            const double alpha = 0.5;

            double t0 = 0.0;
            double t1 = t0 + Math.Pow(Math.Max(Vector2.Distance(p0, p1), 0.0001f), alpha);
            double t2 = t1 + Math.Pow(Math.Max(Vector2.Distance(p1, p2), 0.0001f), alpha);
            double t3 = t2 + Math.Pow(Math.Max(Vector2.Distance(p2, p3), 0.0001f), alpha);

            double currTime = t1 + u * (t2 - t1);

            Vector2 a1 = Vector2.Lerp(p0, p1, (float)((currTime - t0) / (t1 - t0)));
            Vector2 a2 = Vector2.Lerp(p1, p2, (float)((currTime - t1) / (t2 - t1)));
            Vector2 a3 = Vector2.Lerp(p2, p3, (float)((currTime - t2) / (t3 - t2)));

            Vector2 b1 = Vector2.Lerp(a1, a2, (float)((currTime - t0) / (t2 - t0)));
            Vector2 b2 = Vector2.Lerp(a2, a3, (float)((currTime - t1) / (t3 - t1)));

            Vector2 finalPosition = Vector2.Lerp(b1, b2, (float)((currTime - t1) / (t2 - t1)));

            return finalPosition;
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

        public Vector2 GetDirection(double t)
        {
            double delta = 0.005;
            double tAhead = Math.Min(t + delta, 1.0);
            double tBehind = Math.Max(t - delta, 0.0);

            Vector2 posAhead = GetPoint(tAhead);
            Vector2 posBehind = GetPoint(tBehind);

            Vector2 direction = posAhead - posBehind;
            if (direction != Vector2.Zero)
            {
                return Vector2.Normalize(direction);
            }
            return Vector2.Zero;
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

        public PathPoint GetPointAtDistance(double distance)
        {
            double t = GetParameterAtDistance(distance);
            return GetPoint(t);
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
    }
}
