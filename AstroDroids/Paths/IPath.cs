using AstroDroids.Interfaces;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AstroDroids.Paths
{
    public interface IPath : ISaveable
    {
        public List<PathPoint> KeyPoints { get; }
        public PathPoint StartPoint { get; }
        public PathPoint EndPoint { get; }

        public int MinimumPoints { get; }

        public double Length { get; }
        public PathPoint GetPoint(double t);

        public void Translate(PathPoint dist);
        public void RecalculateLength();
        public Vector2 GetDirection(double t);
        PathPoint GetPointAtDistance(double distance);
        double GetParameterAtDistance(double distance);
    }
}
