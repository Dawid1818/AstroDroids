using AstroDroids.Entities.Neutral;
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
        protected Scene Scene { get { return SceneManager.GetScene(); } }

        public List<EnemySpawner> Spawners { get; private set; } = new List<EnemySpawner>();
        public List<EventNode> Events { get; private set; } = new List<EventNode>();

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

        public void RemoveSpawner(EnemySpawner spawner)
        {
            Spawners.Remove(spawner);
        }

        public void RemoveEvent(EventNode eventN)
        {
            Events.Remove(eventN);
        }

        public void Save(BinaryWriter writer)
        {
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
        }

        public void Load(BinaryReader reader, int version)
        {
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
        }
    }
}
