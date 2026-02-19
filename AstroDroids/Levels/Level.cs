using AstroDroids.Entities.Hostile;
using AstroDroids.Entities.Neutral;
using AstroDroids.Extensions;
using AstroDroids.Interfaces;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AstroDroids.Levels
{
    public class Level : ISaveable
    {
        public const string Magic = "adlvl";
        public string Name { get; set; } = string.Empty;
        public int BackgroundId { get; set; } = 0;

        protected Scene Scene { get { return SceneManager.GetScene(); } }
        public List<AttackWave> AttackWaves { get; private set; } = new List<AttackWave>();

        public virtual void StartLevel()
        {

        }

        public virtual IEnumerator LevelScript()
        {
            yield break;
        }

        protected EntityGroup CreateGroup(Vector2 position, int rows, int cols, float cellWidth, float cellHeight, float spacing)
        {
            EntityGroup group = new EntityGroup(position, rows, cols, cellWidth, cellHeight, spacing);

            Scene.World.AddEntityGroup(group);
            return group; 
        }

        public AttackWave CreateAttackWave()
        {
            AttackWave wave = new AttackWave();
            AttackWaves.Add(wave);
            return wave;
        }

        public void RemoveAttackWave(AttackWave wave)
        {
            AttackWaves.Remove(wave);
        }

        public void Save(BinaryWriter writer)
        {
            writer.WriteFixedString(Magic);

            //file format version placeholder
            writer.Write(2);

            writer.Write(Name);
            writer.Write(BackgroundId);

            writer.Write(AttackWaves.Count);
            foreach (var spawner in AttackWaves)
            {
                spawner.Save(writer);
            }
        }

        public void Load(BinaryReader reader, int version)
        {
            if(reader.ReadFixedString(Magic.Length) != Magic)
            {
                throw new InvalidDataException("Invalid level file, Magic string doesn't match.");
            }

            //file format version placeholder
            int actualVersion = reader.ReadInt32();

            Name = reader.ReadString();
            BackgroundId = reader.ReadInt32();

            AttackWaves = new List<AttackWave>();
            int wavesCount = reader.ReadInt32();
            for (int i = 0; i < wavesCount; i++)
            {
                AttackWave wave = new AttackWave();
                wave.Load(reader, version);
                AttackWaves.Add(wave);
            }
        }
    }
}
