using AstroDroids.Interfaces;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace AstroDroids.Levels
{
    public class AttackWave : ISaveable
    {
        public string Name { get; set; } = "New Attack Wave";
        public double Delay { get; set; } = 0;
        public List<EnemySpawner> Spawners { get; private set; } = new List<EnemySpawner>();
        public List<EventNode> Events { get; private set; } = new List<EventNode>();
        public List<LaserBarrierGroupNode> LaserBarriers { get; private set; } = new List<LaserBarrierGroupNode>();
        public bool WaitForPreviousWave { get; set; } = false;

        public void Load(BinaryReader reader, int version)
        {
            Name = reader.ReadString();

            Delay = reader.ReadDouble();
            WaitForPreviousWave = reader.ReadBoolean();

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

            LaserBarriers = new List<LaserBarrierGroupNode>();
            int barriersCount = reader.ReadInt32();
            for (int i = 0; i < barriersCount; i++)
            {
                LaserBarrierGroupNode barrierN = new LaserBarrierGroupNode();
                barrierN.Load(reader, version);
                LaserBarriers.Add(barrierN);
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Name);

            writer.Write(Delay);
            writer.Write(WaitForPreviousWave);

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
    }
}
