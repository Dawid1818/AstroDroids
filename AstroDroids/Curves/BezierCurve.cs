using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace AstroDroids.Curves
{
    public class BezierCurve
    {
        List<Vector2> Points = new List<Vector2>();

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

            t = Math.Max(0, Math.Min(1, t));

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
    }
}
