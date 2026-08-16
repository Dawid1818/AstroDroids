using AstroDroids.Entities;
using AstroDroids.Interfaces;
using Microsoft.Xna.Framework;
using System.IO;

namespace AstroDroids.Levels
{
    public enum LaserBarrierType
    {
        Normal,
        Relay
    }

    public class LaserBarrierNode : ISaveable
    {
        public int Id { get; set; }
        public Vector2 Position { get; set; }
        public int Health { get; set; } = 1;
        public LaserBarrierType Type { get; set; } = LaserBarrierType.Normal;

        public bool HasEnemy { get; set; } = false;
        public EnemySpawnEntry Enemy { get; set; } = null;

        public void Load(BinaryReader reader, int version)
        {
            Id = reader.ReadInt32();
            Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            Health = reader.ReadInt32();

            Type = (LaserBarrierType)reader.ReadInt32();
            HasEnemy = reader.ReadBoolean();

            if(HasEnemy)
            {
                Enemy = new EnemySpawnEntry();
                Enemy.Load(reader, version);
            }
            else
            {
                Enemy = null;
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Health);

            writer.Write((int)Type);
            writer.Write(HasEnemy);

            if(HasEnemy)
            {
                Enemy.Save(writer);
            }
        }
    }
}
