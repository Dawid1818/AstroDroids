using Microsoft.Xna.Framework;
using System.IO;

namespace AstroDroids.Paths
{
    public class PathManager
    {
        public bool Active { get; set; } = true;
        public PathPoint Position { get; set; } = PathPoint.Zero;
        public float Speed { get; set; } = 1;
        public float Time { get; set; } = 0f;
        public LoopingMode Loop { get; set; } = LoopingMode.Off;
        public bool Reverse { get; set; } = false;
        public int MinPath = -1;

        IPath Path;

        public PathManager()
        {
            Active = false;
        }

        public PathManager(IPath path)
        {
            SetPath(path);
        }

        public void SetPath(IPath path)
        {
            Path = path;
            Position = Path.GetPoint(0f);
            Time = 0f;
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            if (!Active || Path == null)
                return;

            if (!Reverse)
                Time += (float)gameTime.ElapsedGameTime.TotalSeconds * Speed;
            else
                Time -= (float)gameTime.ElapsedGameTime.TotalSeconds * Speed;

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
    }
    public enum LoopingMode
    {
        Off,
        Loop,
        Oscillate
    }
}
