using AstroDroids.Entities;
using AstroDroids.Interfaces;
using AstroDroids.Paths;
using System.IO;

namespace AstroDroids.Levels
{
    public class MovableNode : Entity, ISaveable
    {
        public bool HasPath { get; set; } = false;
        public CompositePath Path { get; set; } = null;
        public float PathSpeed { get; set; } = 1f;
        public LoopingMode PathLoop { get; set; } = LoopingMode.Off;
        public int MinPath { get; set; } = -1;
        public bool FollowsCamera { get; set; } = false;

        public virtual void Load(BinaryReader reader, int version)
        {
            FollowsCamera = reader.ReadBoolean();

            HasPath = reader.ReadBoolean();

            if (HasPath)
            {
                Path = new CompositePath();
                Path.Load(reader, version);
                PathSpeed = reader.ReadSingle();
                PathLoop = (LoopingMode)reader.ReadInt32();
                MinPath = reader.ReadInt32();
            }
            else
            {
                Path = null;
            }
        }

        public virtual void Save(BinaryWriter writer)
        {
            writer.Write(FollowsCamera);

            writer.Write(HasPath);

            if (HasPath)
            {
                Path.Save(writer);
                writer.Write(PathSpeed);
                writer.Write((int)PathLoop);
                writer.Write(MinPath);
            }
        }
    }
}
