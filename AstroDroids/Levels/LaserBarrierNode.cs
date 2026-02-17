using AstroDroids.Interfaces;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace AstroDroids.Levels
{
    public class LaserBarrierNode : ISaveable
    {
        public int Id { get; set; }
        public Vector2 Position { get; set; }
        public List<int> Connections { get; private set; } = new List<int>();
        public int Health { get; set; } = 1;

        public void Load(BinaryReader reader, int version)
        {
            Id = reader.ReadInt32();
            Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            Health = reader.ReadInt32();

            Connections = new List<int>();
            int connectionCount = reader.ReadInt32();
            for (int i = 0; i < connectionCount; i++)
            {
                Connections.Add(reader.ReadInt32());
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Health);
            writer.Write(Connections.Count);
            foreach (int connection in Connections)
            {
                writer.Write(connection);
            }
        }
    }
}
