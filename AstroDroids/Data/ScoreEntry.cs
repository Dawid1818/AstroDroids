using AstroDroids.Interfaces;
using System.IO;

namespace AstroDroids.Data
{
    public class ScoreEntry : ISaveable
    {
        public string Name { get; set; } = string.Empty;
        public int Score { get; set; } = 0;

        public bool Victory { get; set; } = false;

        public void Load(BinaryReader reader, int version)
        {
            Name = reader.ReadString();
            Score = reader.ReadInt32();
            Victory = reader.ReadBoolean();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(Score);
            writer.Write(Victory);
        }
    }
}
