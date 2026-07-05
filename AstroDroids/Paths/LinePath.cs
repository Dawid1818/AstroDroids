using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace AstroDroids.Paths
{
    public class LinePath : IPath
    {
        public List<PathPoint> KeyPoints { get; private set; } = new List<PathPoint>();

        public int MinimumPoints { get; } = 2;

        private List<double> segmentLengths;
        private List<double> cumulativeLengths;

        public PathPoint StartPoint { get => KeyPoints[0]; set => KeyPoints[0] = value; }
        public PathPoint EndPoint { get => KeyPoints[KeyPoints.Count - 1]; set => KeyPoints[KeyPoints.Count - 1] = value; }

        public double Length
        {
            get; private set;
        }

        public LinePath()
        {
            KeyPoints.Add(PathPoint.Zero);
            KeyPoints.Add(PathPoint.Zero);

            RecalculateLength();
        }

        public LinePath(PathPoint point1, PathPoint point2)
        {
            KeyPoints.Add(point1);
            KeyPoints.Add(point2);

            RecalculateLength();
        }

        public PathPoint GetPoint(double t)
        {
            t = Math.Clamp(t, 0.0, 1.0);

            double targetDistance = t * Length;
            return GetPointAtDistance(targetDistance);
        }

        public int GetPointCount()
        {
            return KeyPoints.Count;
        }

        public void Load(BinaryReader reader, int version)
        {
            //Point1 = new PathPoint(reader.ReadSingle(), reader.ReadSingle());
            //Point2 = new PathPoint(reader.ReadSingle(), reader.ReadSingle());

            KeyPoints.Clear();

            int pointCount = reader.ReadInt32();
            for (int i = 0; i < pointCount; i++)
            {
                PathPoint point = new PathPoint();
                point.Load(reader, version);
                KeyPoints.Add(point);
            }

            Length = reader.ReadDouble();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(KeyPoints.Count);
            foreach (var item in KeyPoints)
            {
                item.Save(writer);
            }

            writer.Write(Length);
        }

        public void Translate(PathPoint dist)
        {
            foreach (var item in KeyPoints)
            {
                item.X += dist.X;
                item.Y += dist.Y;
            }
        }

        public void RecalculateLength()
        {
            //Length = Point2.DistanceFrom(Point1);

            segmentLengths = new List<double>(KeyPoints.Count - 1);
            cumulativeLengths = new List<double>(KeyPoints.Count) { 0.0 };

            double total = 0.0;
            for (int i = 0; i < KeyPoints.Count - 1; i++)
            {
                double segLen = KeyPoints[i + 1].DistanceFrom(KeyPoints[i]);
                segmentLengths.Add(segLen);
                total += segLen;
                cumulativeLengths.Add(total);
            }

            Length = total;
        }

        public Vector2 GetDirection(double t)
        {
            t = Math.Clamp(t, 0.0, 1.0);
            int seg = GetSegmentIndex(t * Length, out _);

            Vector2 dir = KeyPoints[seg + 1] - KeyPoints[seg];
            if (dir != Vector2.Zero)
                dir.Normalize();

            return dir;
        }

        public PathPoint GetPointAtDistance(double distance)
        {
            distance = Math.Clamp(distance, 0.0, Length);

            int seg = GetSegmentIndex(distance, out double localDistance);
            double segLen = segmentLengths[seg];

            if (segLen <= 0) return KeyPoints[seg];

            double localT = localDistance / segLen;
            var p1 = KeyPoints[seg];
            var p2 = KeyPoints[seg + 1];

            return new PathPoint(p1.X + (p2.X - p1.X) * (float)localT, p1.Y + (p2.Y - p1.Y) * (float)localT);
        }

        public double GetParameterAtDistance(double targetDistance)
        {
            if (Length <= 0) return 0.0;

            double t = targetDistance / Length;

            return Math.Clamp(t, 0.0, 1.0);
        }

        private int GetSegmentIndex(double distance, out double localDistance)
        {
            for (int i = 0; i < segmentLengths.Count - 1; i++)
            {
                if (distance <= cumulativeLengths[i + 1])
                {
                    localDistance = distance - cumulativeLengths[i];
                    return i;
                }
            }

            int last = segmentLengths.Count - 1;
            localDistance = distance - cumulativeLengths[last];
            return last;
        }
    }
}
