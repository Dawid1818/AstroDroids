using AstroDroids.Interfaces;
using AstroDroids.Managers;
using System;
using System.IO;

namespace AstroDroids.Entities
{
    public class EnemySpawnEntry : ISaveable
    {
        public int EnemyID { get; set; }
        public IEnemySpawnData SpawnData { get; set; }

        public void Load(BinaryReader reader, int version)
        {
            EnemyID = reader.ReadInt32();
            SpawnData = (IEnemySpawnData)Activator.CreateInstance(EntityDatabase.GetEnemySpawnDataType(EnemyID));
            SpawnData.Load(reader, version);
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(EnemyID);
            SpawnData.Save(writer);
        }
    }
}
