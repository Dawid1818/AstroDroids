using System;
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

        public PathPoint GetPoint(float t)
        {
            if(paths.Count == 0)
                return PathPoint.Zero;

            float endOfPaths = 1f / paths.Count;

            int pathIndex = Math.Min((int)(t / endOfPaths), paths.Count - 1);

            float localT = (t - (pathIndex * endOfPaths)) / endOfPaths;

            return paths[pathIndex].GetPoint(localT);
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
                    default:
                        break;
                }

                paths.Add(path);
            }
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
    }
}
