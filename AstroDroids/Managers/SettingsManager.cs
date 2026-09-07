using AstroDroids.Data;
using AstroDroids.Graphics;
using Gum.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.IO;

namespace AstroDroids.Managers
{
    public class SettingsManager
    {
        public static string DataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AstroDroids");
        const string Filename = "Settings.adsettings";

        public static SettingsData curSettings { get; private set; }

        public static void Initialize(AstroDroidsGame game)
        {
            if (Directory.Exists(DataDir))
            {
                if (File.Exists(Path.Combine(DataDir, Filename)))
                {
                    Load();
                    ApplyAllSettings();
                }
                else
                {
                    CreateNew(game.GraphicsDevice.Adapter.CurrentDisplayMode);
                    ApplyAllSettings();
                    Save();
                }
            }
            else
            {
                Directory.CreateDirectory(DataDir);
                CreateNew(game.GraphicsDevice.Adapter.CurrentDisplayMode);
                ApplyAllSettings();
                Save();
            }
        }

        static void CreateNew(DisplayMode displayMode)
        {
            curSettings = new SettingsData();
            curSettings.Video.DisplayMode = DisplayModeType.Borderless;
            curSettings.Video.VSync = true;
            curSettings.Video.Resolution = new Point(displayMode.Width, displayMode.Height);
        }

        public static void ApplyAllSettings()
        {
            ApplyVideoSettings();
            ApplyLanguage();
            ApplyVolume();
        }

        public static void Save()
        {
            FileStream str = new FileStream(Path.Combine(DataDir, Filename), FileMode.Create);

            using (BinaryWriter writer = new BinaryWriter(str))
            {
                curSettings.Save(writer);
            }

            str.Close();
        }


        public static void Load()
        {
            FileStream str = new FileStream(Path.Combine(DataDir, Filename), FileMode.Open);

            SettingsData copy = new SettingsData();

            using (BinaryReader reader = new BinaryReader(str))
            {
                copy.Load(reader, 0);
            }

            str.Close();

            curSettings = copy;
        }

        public static void ApplyVideoSettings()
        {
            Screen.ApplyVideoSettings();
        }

        public static void ApplyLanguage()
        {
            Screen.GumUI.LocalizationService.CurrentLanguage = curSettings.LanguageId;
        }

        public static void ApplyVolume()
        {
            SoundManager.MusicVolume = curSettings.MusicVolume;
            SoundManager.SoundVolume = curSettings.SoundVolume;
        }
    }
}
