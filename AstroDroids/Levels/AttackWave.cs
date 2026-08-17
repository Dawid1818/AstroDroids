using AstroDroids.Entities;
using AstroDroids.Interfaces;
using AstroDroids.Paths;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace AstroDroids.Levels
{
    public enum WaveWaitStyle
    {
        None,
        WaitForPreviousWave,
        WaitForAllEnemiesDefeated
    }

    public class AttackWave : ISaveable
    {
        public string Name { get; set; } = "New Attack Wave";
        public double Delay { get; set; } = 0;
        public List<EnemySpawner> Spawners { get; private set; } = new List<EnemySpawner>();
        public List<EventNode> Events { get; private set; } = new List<EventNode>();
        public List<LaserBarrierGroupNode> LaserBarriers { get; private set; } = new List<LaserBarrierGroupNode>();
        public List<BackgroundObjectNode> BackgroundObjects { get; private set; } = new List<BackgroundObjectNode>();
        public List<WarningNode> Warnings { get; private set; } = new List<WarningNode>();
        public WaveWaitStyle WaitStyle { get; set; } = WaveWaitStyle.WaitForAllEnemiesDefeated;


        //camera path stuff
        public bool HasPath { get; set; } = false;
        public CompositePath Path { get; set; } = null;
        public float PathSpeed { get; set; } = 1f;
        public LoopingMode PathLoop { get; set; } = LoopingMode.Off;
        public int MinPath { get; set; } = -1;

        public void Load(BinaryReader reader, int version)
        {
            Name = reader.ReadString();

            Delay = reader.ReadDouble();
            WaitStyle = (WaveWaitStyle)reader.ReadInt32();

            if (version >= 11)
            {
                HasPath = reader.ReadBoolean();

                if (HasPath)
                {
                    Path = new CompositePath();
                    Path.Load(reader, version);
                    PathSpeed = reader.ReadSingle();
                    PathLoop = (LoopingMode)reader.ReadInt32();
                    MinPath = reader.ReadInt32();
                }
                else
                {
                    Path = null;
                }
            }
            else
            {
                HasPath = false;
                Path = null;
                PathSpeed = 1f;
                PathLoop = LoopingMode.Off;
                MinPath = -1;
            }

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

            BackgroundObjects = new List<BackgroundObjectNode>();
            int bgObjectsCount = reader.ReadInt32();
            for (int i = 0; i < bgObjectsCount; i++)
            {
                BackgroundObjectNode bgObjN = new BackgroundObjectNode();
                bgObjN.Load(reader, version);
                BackgroundObjects.Add(bgObjN);
            }

            Warnings = new List<WarningNode>();
            if (version >= 8)
            {
                int warningCount = reader.ReadInt32();
                for (int i = 0; i < warningCount; i++)
                {
                    WarningNode warningN = new WarningNode();
                    warningN.Load(reader, version);
                    Warnings.Add(warningN);
                }
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Name);

            writer.Write(Delay);
            writer.Write((int)WaitStyle);


            writer.Write(HasPath);

            if (HasPath)
            {
                Path.Save(writer);
                writer.Write(PathSpeed);
                writer.Write((int)PathLoop);
                writer.Write(MinPath);
            }


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

            writer.Write(BackgroundObjects.Count);
            foreach (var bgObjN in BackgroundObjects)
            {
                bgObjN.Save(writer);
            }

            writer.Write(Warnings.Count);
            foreach (var warningN in Warnings)
            {
                warningN.Save(writer);
            }
        }

        public EnemySpawner CreateSpawner(Vector2 Position)
        {
            EnemySpawner spawner = new EnemySpawner() { Transform = new Transform(Position.X, Position.Y), SpawnPosition = Position };
            Spawners.Add(spawner);
            return spawner;
        }

        public EventNode CreateEvent(Vector2 Position)
        {
            EventNode node = new EventNode() { Transform = new Transform(Position.X, Position.Y) };
            Events.Add(node);
            return node;
        }

        public LaserBarrierGroupNode CreateLaserBarrier(Vector2 Position)
        {
            LaserBarrierGroupNode node = new LaserBarrierGroupNode() { Transform = new Transform(Position.X, Position.Y) };

            LaserBarriers.Add(node);

            return node;
        }

        public BackgroundObjectNode CreateBackgroundObject(Vector2 Position)
        {
            BackgroundObjectNode bgobject = new BackgroundObjectNode() { Transform = new Transform(Position.X, Position.Y) };
            BackgroundObjects.Add(bgobject);
            return bgobject;
        }

        public WarningNode CreateWarning(Vector2 Position)
        {
            WarningNode warningN = new WarningNode() { Transform = new Transform(Position.X, Position.Y) };
            Warnings.Add(warningN);
            return warningN;
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

        public void RemoveBackgroundObject(BackgroundObjectNode bgobject)
        {
            BackgroundObjects.Remove(bgobject);
        }

        public void RemoveWarning(WarningNode warningN)
        {
            Warnings.Remove(warningN);
        }
    }
}
