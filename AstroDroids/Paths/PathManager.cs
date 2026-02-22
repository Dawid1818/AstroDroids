using Microsoft.Xna.Framework;

namespace AstroDroids.Paths
{
    public class PathManager
    {
        public bool Active { get; set; } = true;
        public PathPoint Position { get; set; } = PathPoint.Zero;
        public double Time { get; set; } = 0f;
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

        public void SetPath(IPath path, float speed)
        {
            Path = path;
            Position = Path.GetPoint(0f);
            Time = 0f;

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
                return;

            if (!Reverse)
                Time += (speed * (float)gameTime.ElapsedGameTime.TotalSeconds) / Path.Length;
            else
                Time -= (speed * (float)gameTime.ElapsedGameTime.TotalSeconds) / Path.Length;

            Position = Path.GetPoint(Time);

            switch (Loop)
            {
                case LoopingMode.Off:
                    if (!Reverse && Time >= 1f)
                    {
                        Active = false;
                    }
                    else if (Reverse && Time <= 0f)
                    {
                        Active = false;
                    }
                    break;
                case LoopingMode.Loop:
                    if (!Reverse)
                    {
                        if (Time >= 1f)
                        {
                            if (MinPath != -1 && Path is CompositePath composite)
                            {
                                Time = (float)MinPath / composite.Decompose().Count;
                            }
                            else
                            {
                                Time = 0f;
                            }
                        }
                    }
                    else if (Reverse)
                    {
                        if (MinPath != -1 && Path is CompositePath composite && Time <= (float)MinPath / composite.Decompose().Count)
                        {
                            Time = 1f;
                        }
                        else if (Time <= 0f)
                        {
                            Time = 1f;
                        }
                    }
                    break;
                case LoopingMode.Oscillate:
                    if (!Reverse)
                    {
                        if (Time >= 1f)
                        {
                            if (MinPath != -1 && Path is CompositePath composite)
                            {
                                Reverse = true;
                                Time = 1f;
                            }
                            else
                            {
                                Reverse = true;
                                Time = 1f;
                            }
                        }
                    }
                    else if (Reverse)
                    {
                        if (MinPath != -1 && Path is CompositePath composite && Time <= (float)MinPath / composite.Decompose().Count)
                        {
                            Reverse = false;
                            Time = (float)MinPath / composite.Decompose().Count;
                        }
                        else if (Time <= 0f)
                        {
                            Reverse = false;
                            Time = 0f;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void Translate(Vector2 delta)
        {
            Path.Translate(delta);
        }
    }
    public enum LoopingMode
    {
        Off,
        Loop,
        Oscillate
    }
}
