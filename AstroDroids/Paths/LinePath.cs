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

            RecalculateLength();
        }

        public LinePath(PathPoint point1, PathPoint point2)
        {
            Point1 = point1;
            Point2 = point2;

            RecalculateLength();
        }

        public PathPoint GetPoint(double t)
        {
            return new PathPoint(
                Point1.X + (Point2.X - Point1.X) * (float)t,
                Point1.Y + (Point2.Y - Point1.Y) * (float)t
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

            Length = reader.ReadDouble();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Point1.X);
            writer.Write(Point1.Y);

            writer.Write(Point2.X);
            writer.Write(Point2.Y);

            writer.Write(Length);
        }

        public void Translate(PathPoint dist)
        {
            Point1.X += dist.X;
            Point1.Y += dist.Y;
            Point2.X += dist.X;
            Point2.Y += dist.Y;
        }

        public void RecalculateLength()
        {
            Length = Point2.DistanceFrom(Point1);
        }
    }
}
