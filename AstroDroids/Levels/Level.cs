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

        public List<EnemySpawner> Spawners { get; private set; } = new List<EnemySpawner>();
        public List<EventNode> Events { get; private set; } = new List<EventNode>();
        public List<LaserBarrierGroupNode> LaserBarriers { get; private set; } = new List<LaserBarrierGroupNode>();

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

        public void CreateSpawner(Vector2 Position)
        {
            //Spawners.Add(new EnemySpawner() { Transform = new Entities.Transform(Position.X, Position.Y), Curve = new Curves.BezierPath(new List<Vector2>() { Position, Position, Position, Position }) });
            Spawners.Add(new EnemySpawner() { Transform = new Entities.Transform(Position.X, Position.Y), SpawnPosition = Position });
        }

        public void CreateEvent(Vector2 Position)
        {
            Events.Add(new EventNode() { Transform = new Entities.Transform(Position.X, Position.Y) });
        }

        public void CreateLaserBarrier(Vector2 Position)
        {
            LaserBarriers.Add(new LaserBarrierGroupNode() { Transform = new Entities.Transform(Position.X, Position.Y) });
        }

        public void RemoveSpawner(EnemySpawner spawner)
        {
            Spawners.Remove(spawner);
        }

        public void RemoveEvent(EventNode eventN)
        {
            Events.Remove(eventN);
        }

        public void RemoveLaserBarrier(LaserBarrierGroupNode barrierN)
        {
            LaserBarriers.Remove(barrierN);
        }

        public void Save(BinaryWriter writer)
        {
            writer.WriteFixedString(Magic);

            //file format version placeholder
            writer.Write(1);

            writer.Write(Name);
            writer.Write(BackgroundId);

            writer.Write(Spawners.Count);
            foreach (var spawner in Spawners)
            {
                spawner.Save(writer);
            }

            writer.Write(Events.Count);
            foreach (var eventN in Events)
            {
                eventN.Save(writer);
            }

            writer.Write(LaserBarriers.Count);
            foreach (var barrierN in LaserBarriers)
            {
                barrierN.Save(writer);
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

            Spawners = new List<EnemySpawner>();
            int spawnerCount = reader.ReadInt32();
            for (int i = 0; i < spawnerCount; i++)
            {
                EnemySpawner spawner = new EnemySpawner();
                spawner.Load(reader, version);
                Spawners.Add(spawner);
            }

            Events = new List<EventNode>();
            int eventCount = reader.ReadInt32();
            for (int i = 0; i < eventCount; i++)
            {
                EventNode eventN = new EventNode();
                eventN.Load(reader, version);
                Events.Add(eventN);
            }

            if (actualVersion >= 1)
            {
                LaserBarriers = new List<LaserBarrierGroupNode>();
                int barriersCount = reader.ReadInt32();
                for (int i = 0; i < barriersCount; i++)
                {
                    LaserBarrierGroupNode barrierN = new LaserBarrierGroupNode();
                    barrierN.Load(reader, version);
                    LaserBarriers.Add(barrierN);
                }
            }
        }
    }
}
