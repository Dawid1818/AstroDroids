using System.IO;

namespace AstroDroids.Interfaces
{
    public interface ISaveable
    {
        //int FileFormatVersion { get; set; }

        public void Save(BinaryWriter writer);

        public void Load(BinaryReader reader, int version);
    }
}
