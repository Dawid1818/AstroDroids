using AstroDroids.Interfaces;
using System.IO;

namespace AstroDroids.Levels
{
    public class LaserBarrierConnection : ISaveable
    {
        public int FirstBarrierID;
        public int SecondBarrierID;

        public bool BlocksPlayerProjectiles = false;

        public void Save(BinaryWriter writer)
        {
            writer.Write(FirstBarrierID);
            writer.Write(SecondBarrierID);

            writer.Write(BlocksPlayerProjectiles);
        }

        public void Load(BinaryReader reader, int version)
        {
            FirstBarrierID = reader.ReadInt32();
            SecondBarrierID = reader.ReadInt32();

            BlocksPlayerProjectiles = reader.ReadBoolean();
        }
    }
}
