using AstroDroids.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace AstroDroids.Curves
{
    public class BezierCurve : ISaveable
    {
        List<Vector2> Points = new List<Vector2>();

        public BezierCurve()
        {

        }

        public BezierCurve(List<Vector2> points)
        {
            Points = points;
        }

        public Vector2 GetPoint(float t)
        {
            if(Points.Count == 0)
            {                 
                return Vector2.Zero;
            }

            t = Math.Clamp(t, 0f, 1f);

            Vector2 result = Vector2.Zero;

            for (int i = 0; i < Points.Count; i++) { 
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
        }
    }
}
