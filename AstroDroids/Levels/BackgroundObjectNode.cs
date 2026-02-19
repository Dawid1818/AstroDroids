using AstroDroids.Entities;
using AstroDroids.Interfaces;
using Microsoft.Xna.Framework;
using System.IO;

namespace AstroDroids.Levels
{
    public class BackgroundObjectNode : Entity, ISaveable
    {
        public double InitialDelay { get; set; } = 0f;

        public void Load(BinaryReader reader, int version)
        {
            Transform.Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());

            InitialDelay = reader.ReadDouble();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Transform.Position.X);
            writer.Write(Transform.Position.Y);

            writer.Write(InitialDelay);
        }
    }
}
