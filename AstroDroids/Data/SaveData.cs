using AstroDroids.Extensions;
using AstroDroids.Interfaces;
using System.IO;

namespace AstroDroids.Data
{
    public class SaveData : ISaveable
    {
        public const string Magic = "adsave";

        public ShipCustomization Ship { get; set; } = new ShipCustomization();

        public void Load(BinaryReader reader, int version)
        {
            if (reader.ReadFixedString(Magic.Length) != Magic)
            {
                throw new InvalidDataException("Invalid save data file, Magic string doesn't match.");
            }

            int actualVersion = reader.ReadInt32();

            Ship = new ShipCustomization();
            Ship.Load(reader, version);
        }

        public void Save(BinaryWriter writer)
        {
            writer.WriteFixedString(Magic);

            //file format version placeholder
            writer.Write(0);

            Ship.Save(writer);
        }
    }
}
