using AstroDroids.Entities;
using AstroDroids.Paths;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace AstroDroids.Levels
{
    public class EnemySpawner : MovableNode
    {
        public List<int> EnemyIDs = new List<int>();
        public PathPoint SpawnPosition { get; set; } = null;
        public float DelayBetweenEnemies { get; set; } = 1f;
        public double InitialDelay { get; set; } = 0f;

        public override void Load(BinaryReader reader, int version)
        {
            Transform.Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());

            InitialDelay = reader.ReadDouble();

            HasPath = reader.ReadBoolean();

            EnemyIDs.Clear();
            int enemyCount = reader.ReadInt32();
            for (int i = 0; i < enemyCount; i++)
            {
                EnemyIDs.Add(reader.ReadInt32());
            }

            DelayBetweenEnemies = reader.ReadSingle();

            base.Load(reader, version);

            if (!HasPath)
            {
                SpawnPosition = new PathPoint(reader.ReadSingle(), reader.ReadSingle());
            }
        }

        public override void Save(BinaryWriter writer)
        {
            writer.Write(Transform.Position.X);
            writer.Write(Transform.Position.Y);

            writer.Write(InitialDelay);

            writer.Write(HasPath);

            writer.Write(EnemyIDs.Count);
            foreach (var id in EnemyIDs)
            {
                writer.Write(id);
            }

            writer.Write(DelayBetweenEnemies);

            base.Save(writer);

            if (!HasPath)
            {
                writer.Write(SpawnPosition.X);
                writer.Write(SpawnPosition.Y);
            }
        }
    }
}
