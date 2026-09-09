using AstroDroids.Data;
using System;
using System.IO;

namespace AstroDroids.Managers
{
    public class SaveManager
    {
        public static string DataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AstroDroids");

        public static SaveData curSave { get; private set; }

        public static void Initialize()
        {
            if (Directory.Exists(DataDir))
            {
                if (File.Exists(Path.Combine(DataDir, "Save.adsave")))
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
                Directory.CreateDirectory(DataDir);
                curSave = new SaveData();
                SaveGame();
            }
        }

        public static void SaveGame()
        {
            FileStream str = new FileStream(Path.Combine(DataDir, "Save.adsave"), FileMode.Create);

            using (BinaryWriter writer = new BinaryWriter(str))
            {
                curSave.Save(writer);
            }

            str.Close();
        }


        public static void LoadGame()
        {
            FileStream str = new FileStream(Path.Combine(DataDir, "Save.adsave"), FileMode.Open);

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
