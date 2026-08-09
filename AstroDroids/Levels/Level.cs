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
        public const int FileVersion = 8;
        public string Name { get; set; } = string.Empty;
        public int EventHandlerId { get; set; } = 0;
        public int BackgroundId { get; set; } = 0;
        public int MusicId { get; set; } = 0;
        public int BossMusicId { get; set; } = 0;

        protected Scene Scene { get { return SceneManager.GetScene(); } }
        public List<AttackWave> AttackWaves { get; private set; } = new List<AttackWave>();
        public List<NamedPath> Paths { get; private set; } = new List<NamedPath>();

        //runtime only
        LevelEventHandler eventHandler = new LevelEventHandler();

        public virtual void StartLevel()
        {

        }

        public void RegisterEvents()
        {
            if (eventHandler != null)
            {
                eventHandler.RegisterEvents();
            }
        }

        public Dictionary<int, LevelEvent> GetEvents()
        {
            if (eventHandler != null)
            {
                return eventHandler.GetEvents();
            }
            else
            {
                return new Dictionary<int, LevelEvent>();
            }
        }

        public virtual IEnumerator LevelScript()
        {
            yield break;
        }

        public void RunEvent(int id)
        {
            if (eventHandler != null)
                eventHandler.RunEvent(id);
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

        public NamedPath CreatePath()
        {
            NamedPath wave = new NamedPath();
            Paths.Add(wave);
            return wave;
        }

        public void RemoveAttackWave(AttackWave wave)
        {
            AttackWaves.Remove(wave);
        }

        public void RemovePath(NamedPath path)
        {
            Paths.Remove(path);
        }

        public void Save(BinaryWriter writer)
        {
            writer.WriteFixedString(Magic);

            //file format version placeholder
            writer.Write(FileVersion);

            writer.Write(Name);

            writer.Write(EventHandlerId);

            writer.Write(BackgroundId);

            writer.Write(MusicId);
            writer.Write(BossMusicId);

            writer.Write(AttackWaves.Count);
            foreach (var spawner in AttackWaves)
            {
                spawner.Save(writer);
            }

            writer.Write(Paths.Count);
            foreach (var path in Paths)
            {
                path.Save(writer);
            }
        }

        public void Load(BinaryReader reader, int version)
        {
            if (reader.ReadFixedString(Magic.Length) != Magic)
            {
                throw new InvalidDataException("Invalid level file, Magic string doesn't match.");
            }

            //file format version placeholder
            int actualVersion = reader.ReadInt32();

            Name = reader.ReadString();

            if (actualVersion >= 4)
            {
                EventHandlerId = reader.ReadInt32();

                eventHandler = GameDatabase.CreateEventHandler(EventHandlerId);
            }
            else
            {
                eventHandler = new LevelEventHandler();
            }

            BackgroundId = reader.ReadInt32();

            if (actualVersion >= 5)
            {
                MusicId = reader.ReadInt32();
            }
            else
            {
                MusicId = 0;
            }

            if (actualVersion >= 7)
            {
                BossMusicId = reader.ReadInt32();
            }
            else
            {
                BossMusicId = 0;
            }

            AttackWaves = new List<AttackWave>();
            int wavesCount = reader.ReadInt32();
            for (int i = 0; i < wavesCount; i++)
            {
                AttackWave wave = new AttackWave();
                wave.Load(reader, actualVersion);
                AttackWaves.Add(wave);
            }

            Paths = new List<NamedPath>();
            if (actualVersion >= 3)
            {
                int pathCount = reader.ReadInt32();
                for (int i = 0; i < pathCount; i++)
                {
                    NamedPath path = new NamedPath();
                    path.Load(reader, actualVersion);
                    Paths.Add(path);
                }
            }
        }

        internal void ReloadEventHandler()
        {
            eventHandler = GameDatabase.CreateEventHandler(EventHandlerId);
        }
    }
}
