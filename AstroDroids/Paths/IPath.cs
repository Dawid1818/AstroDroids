using AstroDroids.Interfaces;
using Microsoft.Xna.Framework;

namespace AstroDroids.Paths
{
    public interface IPath : ISaveable
    {
        public PathPoint[] KeyPoints { get; }
        public PathPoint StartPoint { get; }
        public PathPoint EndPoint { get; }

        public double Length { get; }
        public PathPoint GetPoint(double t);

        public void Translate(PathPoint dist);
        public void RecalculateLength();
    }
}
