using AstroDroids.Interfaces;
using AstroDroids.Paths;
using System.IO;

namespace AstroDroids.Levels
{
    public class NamedPath : ISaveable
    {
        public string Name { get; set; } = "New Path";
        public CompositePath Path { get; set; } = new CompositePath();

        public NamedPath() { }

        public void Load(BinaryReader reader, int version)
        {
            Name = reader.ReadString();
            Path = new CompositePath();
            Path.Load(reader, version);
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Name);
            Path.Save(writer);
        }
    }
}
