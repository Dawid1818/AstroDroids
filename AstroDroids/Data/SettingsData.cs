using AstroDroids.Extensions;
using AstroDroids.Input;
using AstroDroids.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
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
        public const int FileVersion = 2;

        public const string Magic = "adsettings";

        public VideoSettings Video { get; set; } = new VideoSettings();
        public int LanguageId { get; set; } = 1;
        public float MusicVolume { get; set; } = 1f;
        public float SoundVolume { get; set; } = 1f;
        public Dictionary<GameAction, ButtonInputAction> Actions { get; set; } = new Dictionary<GameAction, ButtonInputAction>();

        public float MouseSensitivty { get; set; } = 1f;

        public void Load(BinaryReader reader, int version)
        {
            if (reader.ReadFixedString(Magic.Length) != Magic)
            {
                throw new InvalidDataException("Invalid settings data file, Magic string doesn't match.");
            }

            int actualVersion = reader.ReadInt32();

            Video = new VideoSettings();
            Video.Load(reader, actualVersion);
            MusicVolume = reader.ReadSingle();
            SoundVolume = reader.ReadSingle();
            LanguageId = reader.ReadInt32();

            Actions.Clear();
            if (actualVersion >= 1)
            {
                int actionsAmount = reader.ReadInt32();
                for (int i = 0; i < actionsAmount; i++)
                {
                    GameAction ga = (GameAction)reader.ReadInt32();
                    Keys key = (Keys)reader.ReadInt32();
                    Buttons button = (Buttons)reader.ReadInt32();

                    Actions.Add(ga, new ButtonInputAction(key, button));
                }
            }
            else
            {
                Actions = InputSystem.CreateDefaultActions();
            }

            if(actualVersion >= 2)
            {
                MouseSensitivty = reader.ReadSingle();
            }
            else
            {
                MouseSensitivty = 1f;
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.WriteFixedString(Magic);

            writer.Write(FileVersion);

            Video.Save(writer);
            writer.Write(MusicVolume);
            writer.Write(SoundVolume);
            writer.Write(LanguageId);

            writer.Write(Actions.Count);
            foreach (var item in Actions)
            {
                writer.Write((int)item.Key);
                writer.Write((int)item.Value.KeyboardKey);
                writer.Write((int)item.Value.GamepadButton);
            }

            writer.Write(MouseSensitivty);
        }
    }
}
