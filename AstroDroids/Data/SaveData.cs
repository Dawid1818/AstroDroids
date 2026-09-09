using AstroDroids.Extensions;
using AstroDroids.Gameplay;
using AstroDroids.Interfaces;
using System.IO;

namespace AstroDroids.Data
{
    public class SaveData : ISaveable
    {
        public const string Magic = "adsave";
        public const int FileVersion = 1;

        public ShipCustomization Ship { get; set; } = new ShipCustomization();
        public MissionProgress MissionProgress { get; set; }

        public void Load(BinaryReader reader, int version)
        {
            if (reader.ReadFixedString(Magic.Length) != Magic)
            {
                throw new InvalidDataException("Invalid save data file, Magic string doesn't match.");
            }

            int actualVersion = reader.ReadInt32();

            Ship = new ShipCustomization();
            Ship.Load(reader, version);

            if(actualVersion >= 1)
            {
                bool hasMission = reader.ReadBoolean();
                if (hasMission)
                {
                    MissionProgress = new MissionProgress();
                    MissionProgress.Load(reader, actualVersion);
                }
                else
                {
                    MissionProgress = null;
                }
            }
            else
            {
                MissionProgress = null;
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.WriteFixedString(Magic);

            writer.Write(FileVersion);

            Ship.Save(writer);

            if(MissionProgress != null)
            {
                writer.Write(true);
                MissionProgress.Save(writer);
            }
            else
            {
                writer.Write(false);
            }
        }
    }
}
