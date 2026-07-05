using Microsoft.Xna.Framework;

namespace AstroDroids.Paths
{
    public class PathManager
    {
        public bool Active { get; set; } = true;
        public PathPoint Position { get; set; } = PathPoint.Zero;
        public Vector2 Direction { get; set; } = Vector2.Zero;
        public double CurrentDistance { get; set; } = 0f;
        public float Time
        {
            get { return Path != null && Path.Length > 0 ? (float)(CurrentDistance / Path.Length) : 0f; }
        }
        public LoopingMode Loop { get; set; } = LoopingMode.Off;
        public bool Reverse { get; set; } = false;
        public int MinPath = -1;

        float travelTime;
        float speed;

        public float TravelTime 
        {
            get
            {
                return travelTime;
            }
            set
            {
                travelTime = value;
                if (Path != null && Path.Length > 0)
                {
                    speed = (float)Path.Length / travelTime;
                }
            }
        }

        public float Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
                if (Path != null && Path.Length > 0)
                {
                    travelTime = (float)Path.Length / speed;
                }
            }
        }

        IPath Path;

        public PathManager()
        {
            Active = false;
        }

        public PathManager(IPath path, float speed)
        {
            SetPath(path, speed);
        }

        public void SetPath(IPath path, float speed, bool reverse = false)
        {
            Path = path;

            this.Reverse = reverse;

            if (reverse)
            {
                Position = Path.GetPoint(1f);
                CurrentDistance = Path.Length;
            }
            else
            {
                Position = Path.GetPoint(0f);
                CurrentDistance = 0f;
            }
            //Time = 0f;

            Speed = speed;

            Active = true;
        }

        public IPath GetPath()
        {
            return Path;
        }

        public void Update(GameTime gameTime)
        {
            if (!Active || Path == null || Path.Length == 0)
            {
                Active = false;
                return;
            }

            float distanceDelta = speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (!Reverse)
                CurrentDistance += distanceDelta;
            else
                CurrentDistance -= distanceDelta;

            double minDistance = GetMinPathDistance();

            switch (Loop)
            {
                case LoopingMode.Off:
                    if (!Reverse && CurrentDistance >= Path.Length)
                    {
                        CurrentDistance = Path.Length;
                        Active = false;
                    }
                    else if (Reverse && CurrentDistance <= 0f)
                    {
                        CurrentDistance = 0f;
                        Active = false;
                    }
                    break;

                case LoopingMode.Loop:
                    if (!Reverse && CurrentDistance >= Path.Length)
                    {
                        CurrentDistance = minDistance;
                    }
                    else if (Reverse && CurrentDistance <= minDistance)
                    {
                        CurrentDistance = Path.Length;
                    }
                    break;

                case LoopingMode.Oscillate:
                    if (!Reverse && CurrentDistance >= Path.Length)
                    {
                        CurrentDistance = Path.Length;
                        Reverse = true;
                    }
                    else if (Reverse && CurrentDistance <= minDistance)
                    {
                        CurrentDistance = minDistance;
                        Reverse = false;
                    }
                    break;
            }

            double t = Path.GetParameterAtDistance(CurrentDistance);
            Position = Path.GetPoint(t);
            Direction = Path.GetDirection(t);
        }

        public void Translate(Vector2 delta)
        {
            Path.Translate(delta);
        }

        private double GetMinPathDistance()
        {
            if (MinPath != -1 && Path is CompositePath composite)
            {
                return composite.GetSegmentStartDistance(MinPath);
            }
            return 0f;
        }
    }
    public enum LoopingMode
    {
        Off,
        Loop,
        Oscillate
    }
}
