using AstroDroids.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace AstroDroids.Paths
{
    public class PathPoint : ISaveable
    {
        public float X { get; set; }
        public float Y { get; set; }

        public static PathPoint Zero => new PathPoint(0f, 0f);

        public PathPoint() { }
        public PathPoint(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
        }

        public void Load(BinaryReader reader, int version)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
        }

        public static PathPoint operator+ (PathPoint a, PathPoint b) => new PathPoint(a.X + b.X, a.Y + b.Y);
        public static PathPoint operator+ (PathPoint a, Vector2 b) => new PathPoint(a.X + b.X, a.Y + b.Y);
        public static PathPoint operator- (PathPoint a, PathPoint b) => new PathPoint(a.X - b.X, a.Y - b.Y);
        public static PathPoint operator* (PathPoint a, float scalar) => new PathPoint(a.X * scalar, a.Y * scalar);
        public static PathPoint operator* (float scalar, PathPoint a) => new PathPoint(a.X * scalar, a.Y * scalar);


        public static implicit operator PathPoint (Vector2 point) => new PathPoint(point.X, point.Y);
        public static implicit operator Vector2 (PathPoint point) => new Vector2(point.X, point.Y);

        public double DistanceFrom(PathPoint point)
        {
            double x = X - point.X;
            double y = Y - point.Y;
            return Math.Sqrt(x * x + y * y);
        }
    }
}
