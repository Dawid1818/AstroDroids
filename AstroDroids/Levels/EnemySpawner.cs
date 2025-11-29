using AstroDroids.Curves;
using AstroDroids.Entities;
using AstroDroids.Interfaces;
using Microsoft.Xna.Framework;
using System.IO;

namespace AstroDroids.Levels
{
    public class EnemySpawner : Entity, ISaveable
    {
        public string EnemyId { get; set; } = string.Empty;
        public bool FollowsCamera { get; set; } = false;
        public bool HasPath { get; set; } = false;
        public CompositePath Path { get; set; } = null;
        public int EnemyCount { get; set; } = 1;
        public float DelayBetweenEnemies { get; set; } = 1f;

        public void Load(BinaryReader reader, int version)
        {
            Transform.Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());

            HasPath = reader.ReadBoolean();


            EnemyId = reader.ReadString();
            FollowsCamera = reader.ReadBoolean();

            EnemyCount = reader.ReadInt32();
            DelayBetweenEnemies = reader.ReadSingle();

            if (HasPath)
            {
                Path = new CompositePath();
                Path.Load(reader, version);
            }
            else
                Path = null;
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Transform.Position.X);
            writer.Write(Transform.Position.Y);

            writer.Write(HasPath);

            writer.Write(EnemyId);
            writer.Write(FollowsCamera);

            writer.Write(EnemyCount);
            writer.Write(DelayBetweenEnemies);

            if(HasPath)
                Path.Save(writer);
        }
    }
}
