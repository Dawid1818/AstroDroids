using AstroDroids.Extensions;
using AstroDroids.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace AstroDroids.Data
{
    public class VideoSettings : ISaveable
    {
        public DisplayModeType DisplayMode { get; set; }
        public Point Resolution { get; set; }
        public bool VSync { get; set; }

        public void Load(BinaryReader reader, int version)
        {
            DisplayMode = (DisplayModeType)reader.ReadInt32();
            Resolution = new Point(reader.ReadInt32(), reader.ReadInt32());
            VSync = reader.ReadBoolean();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write((int)DisplayMode);
            writer.Write(Resolution.X);
            writer.Write(Resolution.Y);
            writer.Write(VSync);
        }
    }

    public enum DisplayModeType
    {
        Windowed,
        Borderless,
        Exclusive
    }

    public class SettingsData : ISaveable
    {
        public const int FileVersion = 0;

        public const string Magic = "adsettings";

        public VideoSettings Video { get; set; } = new VideoSettings();
        public int LanguageId { get; set; } = 1;

        public void Load(BinaryReader reader, int version)
        {
            if (reader.ReadFixedString(Magic.Length) != Magic)
            {
                throw new InvalidDataException("Invalid settings data file, Magic string doesn't match.");
            }

            int actualVersion = reader.ReadInt32();

            Video = new VideoSettings();
            Video.Load(reader, actualVersion);
            LanguageId = reader.ReadInt32();
        }

        public void Save(BinaryWriter writer)
        {
            writer.WriteFixedString(Magic);

            writer.Write(FileVersion);

            Video.Save(writer);
            writer.Write(LanguageId);
        }
    }
}
