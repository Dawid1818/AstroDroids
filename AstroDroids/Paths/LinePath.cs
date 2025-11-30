using System.IO;

namespace AstroDroids.Paths
{
    public class LinePath : IPath
    {
        public PathPoint Point1 { get; set; }
        public PathPoint Point2 { get; set; }

        public PathPoint StartPoint
        {
            get { return Point1; }
            set { Point1 = value; }
        }

        public PathPoint EndPoint
        {
            get { return Point2; }
            set { Point2 = value; }
        }

        public PathPoint[] KeyPoints
        {
            get
            {
                return new PathPoint[] { Point1, Point2 };
            }
        }

        public double Length
        {
            get; private set;
        }

        public LinePath()
        {
            Point1 = new PathPoint(0f, 0f);
            Point2 = new PathPoint(0f, 0f);

            Length = Point2.DistanceFrom(Point1);
        }

        public LinePath(PathPoint point1, PathPoint point2)
        {
            Point1 = point1;
            Point2 = point2;

            Length = Point2.DistanceFrom(Point1);
        }

        public PathPoint GetPoint(float t)
        {
            return new PathPoint(
                Point1.X + (Point2.X - Point1.X) * t,
                Point1.Y + (Point2.Y - Point1.Y) * t
            );
        }

        public int GetPointCount()
        {
            return 2;
        }

        public void Load(BinaryReader reader, int version)
        {
            Point1 = new PathPoint(reader.ReadSingle(), reader.ReadSingle());
            Point2 = new PathPoint(reader.ReadSingle(), reader.ReadSingle());
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Point1.X);
            writer.Write(Point1.Y);

            writer.Write(Point2.X);
            writer.Write(Point2.Y);
        }

        public void Translate(PathPoint dist)
        {
            Point1 += dist;
            Point2 += dist;
        }
    }
}
