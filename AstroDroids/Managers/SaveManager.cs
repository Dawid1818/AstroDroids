using AstroDroids.Data;
using System;
using System.IO;

namespace AstroDroids.Managers
{
    public class SaveManager
    {
        public static string DataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AstroDroids");
        public static string SavesDirectory = Path.Combine(DataDir, "Saves");

        public static SaveData curSave { get; private set; }

        public static void Initialize()
        {
            if (Directory.Exists(SavesDirectory))
            {
                if (File.Exists(Path.Combine(SavesDirectory, "Save.adsave")))
                {
                    LoadGame();
                }
                else
                {
                    curSave = new SaveData();
                    SaveGame();
                }
            }
            else
            {
                Directory.CreateDirectory(SavesDirectory);
                curSave = new SaveData();
                SaveGame();
            }
        }

        public static void SaveGame()
        {
            FileStream str = new FileStream(Path.Combine(SavesDirectory, "Save.adsave"), FileMode.Create);

            using (BinaryWriter writer = new BinaryWriter(str))
            {
                curSave.Save(writer);
            }

            str.Close();
        }


        public static void LoadGame()
        {
            FileStream str = new FileStream(Path.Combine(SavesDirectory, "Save.adsave"), FileMode.Open);

            SaveData copy = new SaveData();

            using (BinaryReader reader = new BinaryReader(str))
            {
                copy.Load(reader, 0);
            }

            str.Close();

            curSave = copy;
        }
    }
}
