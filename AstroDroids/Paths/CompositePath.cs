using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AstroDroids.Paths
{
    public class CompositePath : IPath
    {
        List<IPath> paths = new List<IPath>();

        public PathPoint[] KeyPoints => new PathPoint[] { StartPoint, EndPoint };

        public PathPoint StartPoint { get; private set; }
        public PathPoint EndPoint { get; private set; }

        public double Length { get; private set; }

        public PathPoint GetPoint(double t)
        {
            if (paths.Count == 0)
                return PathPoint.Zero;

            double targetDistance = t * Length;

            double accumulated = 0f;

            foreach (var path in paths)
            {
                if (accumulated + path.Length >= targetDistance)
                {
                    double localDistance = targetDistance - accumulated;
                    double localT = localDistance / path.Length;

                    if (double.IsNaN(localT))
                        return PathPoint.Zero;
                    return path.GetPoint(localT);
                }

                accumulated += path.Length;
            }

            return paths[paths.Count - 1].GetPoint(1);
        }

        public void Load(BinaryReader reader, int version)
        {
            int pathCount = reader.ReadInt32();
            paths = new List<IPath>();
            for (int i = 0; i < pathCount; i++)
            {
                int pathType = reader.ReadInt32();
                IPath path = null;

                switch (pathType)
                {
                    case 0:
                        path = new CompositePath();
                        path.Load(reader, version);
                        break;
                    case 1:
                        path = new LinePath();
                        path.Load(reader, version);
                        break;
                    case 2:
                        path = new BezierPath();
                        path.Load(reader, version);
                        break;
                    case 3:
                        path = new CatmullRomPath();
                        path.Load(reader, version);
                        break;
                    default:
                        break;
                }

                paths.Add(path);
            }

            RecalculateLength();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(paths.Count);
            foreach (var path in paths)
            {
                if (path is CompositePath)
                    writer.Write(0);
                else if (path is LinePath)
                    writer.Write(1);
                else if (path is BezierPath)
                    writer.Write(2);
                else if (path is CatmullRomPath)
                    writer.Write(3);

                path.Save(writer);
            }
        }

        public void Add(IPath path)
        {
            paths.Add(path);

            if (paths.Count > 0)
            {
                StartPoint = paths.First().StartPoint;
                EndPoint = paths.Last().EndPoint;
            }
            else
            {
                StartPoint = PathPoint.Zero;
                EndPoint = PathPoint.Zero;
            }
        }

        public void Remove(IPath path)
        {
            paths.Remove(path);

            if (paths.Count > 0)
            {
                StartPoint = paths.First().StartPoint;
                EndPoint = paths.Last().EndPoint;
            }
            else
            {
                StartPoint = PathPoint.Zero;
                EndPoint = PathPoint.Zero;
            }
        }

        public List<IPath> Decompose()
        {
            return paths;
        }

        public void Translate(PathPoint dist)
        {
            foreach (var path in paths)
            {
                path.Translate(dist);
            }
        }

        public void RecalculateLength()
        {
            Length = 0;
            foreach (var path in paths)
            {
                path.RecalculateLength();
                Length += path.Length;
            }
        }

        public Vector2 GetDirection(double t)
        {
            if (paths.Count == 0)
                return PathPoint.Zero;

            double targetDistance = t * Length;

            double accumulated = 0f;

            double blendDistanceWindow = 60.0;

            for (int i = 0; i < paths.Count; i++)
            {
                var currentPath = paths[i];

                if (accumulated + currentPath.Length >= targetDistance)
                {
                    double localDistance = targetDistance - accumulated;
                    double localT = localDistance / currentPath.Length;

                    Vector2 currentDir = currentPath.GetDirection(localT);

                    if (i < paths.Count - 1)
                    {
                        double distanceToEndOfSegment = currentPath.Length - localDistance;

                        if (distanceToEndOfSegment < blendDistanceWindow)
                        {
                            float blendFactor = (float)(1.0 - (distanceToEndOfSegment / blendDistanceWindow));

                            blendFactor = blendFactor * blendFactor * (3.0f - 2.0f * blendFactor);

                            Vector2 nextDir = paths[i + 1].GetDirection(0.0);

                            Vector2 blendedDir = Vector2.Lerp(currentDir, nextDir, blendFactor);

                            return blendedDir != Vector2.Zero ? Vector2.Normalize(blendedDir) : Vector2.Zero;
                        }
                    }

                    return currentDir;
                }

                accumulated += currentPath.Length;
            }

            return paths[paths.Count - 1].GetDirection(1);
        }

        public float GetSegmentStartTime(int index)
        {
            if (paths.Count == 0)
                return 0f;

            double accumulated = 0;

            for (int i = 0; i < index; i++)
                accumulated += paths[i].Length;

            return (float)(accumulated / Length);
        }

        public PathPoint GetPointAtDistance(double distance)
        {
            if (paths.Count == 0) return PathPoint.Zero;
            if (distance <= 0) return paths[0].GetPointAtDistance(0);

            double remainingDistance = distance;

            foreach (var path in paths)
            {
                if (remainingDistance <= path.Length)
                {
                    return path.GetPointAtDistance(remainingDistance);
                }
                remainingDistance -= path.Length;
            }

            return paths[paths.Count - 1].GetPointAtDistance(paths[paths.Count - 1].Length);
        }

        public double GetParameterAtDistance(double targetDistance)
        {
            if (Length <= 0 || paths.Count == 0) return 0.0;
            if (targetDistance <= 0) return 0.0;
            if (targetDistance >= Length) return 1.0;

            double accumulatedDistance = 0.0;

            for (int i = 0; i < paths.Count; i++)
            {
                var path = paths[i];

                if (accumulatedDistance + path.Length >= targetDistance)
                {
                    double localDistance = targetDistance - accumulatedDistance;

                    double localT = path.GetParameterAtDistance(localDistance);

                    double pathStartGlobalT = accumulatedDistance / Length;
                    double pathEndGlobalT = (accumulatedDistance + path.Length) / Length;

                    return MathHelper.Lerp((float)pathStartGlobalT, (float)pathEndGlobalT, (float)localT);
                }

                accumulatedDistance += path.Length;
            }

            return 1.0;
        }

        public double GetSegmentStartDistance(int index)
        {
            if (paths.Count == 0 || index <= 0)
                return 0.0;

            if (index >= paths.Count)
                return Length;

            double accumulated = 0.0;
            for (int i = 0; i < index; i++)
            {
                accumulated += paths[i].Length;
            }

            return accumulated;
        }
    }
}
