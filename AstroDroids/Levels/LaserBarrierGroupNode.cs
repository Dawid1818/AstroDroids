using AstroDroids.Entities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace AstroDroids.Levels
{
    public class LaserBarrierGroupNode : MovableNode
    {
        public double InitialDelay { get; set; } = 0f;
        public int AvailableId { get; set; } = 0;
        public Dictionary<int, LaserBarrierNode> Nodes { get; private set; } = new Dictionary<int, LaserBarrierNode>();
        public List<LaserBarrierConnection> Connections { get; private set; } = new List<LaserBarrierConnection>();

        public Vector2 MoveSpeed { get; set; } = new Vector2(0, 2);

        public override void Load(BinaryReader reader, int version)
        {
            Transform.Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());

            InitialDelay = reader.ReadDouble();

            AvailableId = reader.ReadInt32();

            Nodes = new Dictionary<int, LaserBarrierNode>();
            int nodeCount = reader.ReadInt32();
            for (int i = 0; i < nodeCount; i++)
            {
                LaserBarrierNode node = new LaserBarrierNode();
                int id = reader.ReadInt32();
                node.Load(reader, version);
                Nodes.Add(id, node);
            }

            Connections = new List<LaserBarrierConnection>();
            int connectionCount = reader.ReadInt32();
            for (int i = 0; i < connectionCount; i++)
            {
                LaserBarrierConnection connection = new LaserBarrierConnection();
                connection.Load(reader, version);
                Connections.Add(connection);
            }

            base.Load(reader, version);

            if(!HasPath)
            {
                MoveSpeed = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            }
            else
            {
                MoveSpeed = new Vector2(0, 2);
            }
        }

        public override void Save(BinaryWriter writer)
        {
            writer.Write(Transform.Position.X);
            writer.Write(Transform.Position.Y);

            writer.Write(InitialDelay);

            writer.Write(AvailableId);

            writer.Write(Nodes.Count);
            foreach (var pair in Nodes)
            {
                writer.Write(pair.Key);
                pair.Value.Save(writer);
            }

            writer.Write(Connections.Count);
            foreach (var item in Connections)
            {
                item.Save(writer);
            }

            base.Save(writer);

            if(!HasPath)
            {
                writer.Write(MoveSpeed.X);
                writer.Write(MoveSpeed.Y);
            }
            else
            {
                MoveSpeed = new Vector2(0, 2);
            }
        }

        public void Translate(Vector2 delta)
        {
            foreach (var item in Nodes.Values)
            {
                item.Position += delta;
            }
        }
    }
}
