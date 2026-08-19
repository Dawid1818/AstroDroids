using AstroDroids.Interfaces;
using AstroDroids.Levels;
using System.IO;

namespace AstroDroids.Helpers
{
    public static class FileSaver
    {
        public static void SaveObject(ISaveable item, string location)
        {
            FileStream str = File.Create(location);

            using (BinaryWriter writer = new BinaryWriter(str))
            {
                item.Save(writer);
            }
            str.Close();
        }

        public static void SaveObject(ISaveable item, Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                item.Save(writer);
            }
        }

        public static ISaveable CloneObject(ISaveable item, ISaveable target)
        {
            Stream stream = new MemoryStream();

            using (BinaryWriter writer = new BinaryWriter(stream))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                item.Save(writer);
                stream.Position = 0;
                target.Load(reader, Level.FileVersion);
            }

            return target;
        }

        public static ISaveable RestoreObject(ISaveable item, string location)
        {
            Stream str = new FileStream(location, FileMode.Open);

            using (BinaryReader reader = new BinaryReader(str))
            {
                item.Load(reader, Level.FileVersion);
            }

            str.Close();

            return item;
        }

        public static ISaveable RestoreObject(ISaveable item, Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                item.Load(reader, Level.FileVersion);

                return item;
            }
        }
    }
}
