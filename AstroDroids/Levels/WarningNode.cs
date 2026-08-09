using Apos.Shapes;
using AstroDroids.Entities;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Interfaces;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using System.IO;
using Numeric = System.Numerics;

namespace AstroDroids.Levels
{
    public enum WarningShape
    {
        Circle,
        Line,
        Rectangle
    }

    public interface IWarningShape : ISaveable
    {
        void Draw(Vector2 position, byte alpha, float dashOffset, GameTime gameTime);
        void DrawEditor();
    }

    public class WarningRectangle : IWarningShape
    {
        public Vector2 Size { get; set; }
        public float Angle { get; set; }
        public void Draw(Vector2 position, byte alpha, float dashOffset, GameTime gameTime)
        {
            Color clr = new Color(Color.Red.R, Color.Red.G, Color.Red.B, alpha);

            Screen.shapeBatch.FillRectangle(position, Size, new Gradient(position + new Vector2(Size.X / 2f, Size.Y / 2f), clr, position + new Vector2(Size.X, Size.Y / 2f), new Color(Color.Red.R, Color.Red.G, Color.Red.B, (byte)0), Gradient.Shape.Bilinear), rotation: Angle);
            Screen.shapeBatch.BorderRectangle(position, Size, clr, 1f, rotation: Angle, dash: new DashStyle(24f, 16f, dashOffset));
        }

        public void DrawEditor()
        {
            Numeric.Vector2 size = new Numeric.Vector2(Size.X, Size.Y);

            if (ImGui.InputFloat2("Size", ref size))
            {
                Size = new Vector2(size.X, size.Y);
            }

            float angle = Angle;
            if (ImGui.InputFloat("Angle", ref angle))
            {
                Angle = angle;
            }
        }

        public void Load(BinaryReader reader, int version)
        {
            Size = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            Angle = reader.ReadSingle();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Size.X);
            writer.Write(Size.Y);
            writer.Write(Angle);
        }
    }

    public class WarningCircle : IWarningShape
    {
        public float Radius { get; set; }
        public void Draw(Vector2 position, byte alpha, float dashOffset, GameTime gameTime)
        {
            Color clr = new Color(Color.Red.R, Color.Red.G, Color.Red.B, alpha);
            Screen.shapeBatch.FillCircle(position, Radius, new Gradient(position, clr, position + new Vector2(Radius), new Color(Color.Red.R, Color.Red.G, Color.Red.B, (byte)0), Gradient.Shape.Radial));
            Screen.shapeBatch.BorderCircle(position, Radius, new Gradient(position, clr, position + new Vector2(Radius), new Color(Color.Red.R, Color.Red.G, Color.Red.B, (byte)0), Gradient.Shape.Radial), dash: new DashStyle(24f, 16f, dashOffset));
        }

        public void DrawEditor()
        {
            float radius = Radius;
            if (ImGui.InputFloat("Radius", ref radius))
            {
                Radius = radius;
            }
        }

        public void Load(BinaryReader reader, int version)
        {
            Radius = reader.ReadSingle();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Radius);
        }
    }

    public class WarningLine : IWarningShape
    {
        public float Radius { get; set; }
        public float Angle { get; set; }
        public float Length { get; set; }
        public void Draw(Vector2 position, byte alpha, float dashOffset, GameTime gameTime)
        {
            Color clr = new Color(Color.Red.R, Color.Red.G, Color.Red.B, alpha);

            Vector2 targetPosition = position + GameHelper.DirFromAngle(Angle) * Length;
            Vector2 halfTargetPosition = position + GameHelper.DirFromAngle(Angle) * Length / 2f;
            Screen.shapeBatch.FillLine(position, targetPosition, Radius, new Gradient(halfTargetPosition, clr, halfTargetPosition + GameHelper.DirFromAngle(Angle) * Length / 2f, new Color(Color.Red.R, Color.Red.G, Color.Red.B, (byte)127), Gradient.Shape.Radial));
            Screen.shapeBatch.BorderLine(position, targetPosition, Radius, new Gradient(halfTargetPosition, clr, halfTargetPosition + GameHelper.DirFromAngle(Angle) * Length / 2f, new Color(Color.Red.R, Color.Red.G, Color.Red.B, (byte)127), Gradient.Shape.Radial), dash: new DashStyle(24f, 16f, dashOffset));
        }

        public void DrawEditor()
        {
            float radius = Radius;
            if (ImGui.InputFloat("Radius", ref radius))
            {
                Radius = radius;
            }

            float angle = Angle;
            if (ImGui.InputFloat("Angle", ref angle))
            {
                Angle = angle;
            }

            float length = Length;
            if (ImGui.InputFloat("Length", ref length))
            {
                Length = length;
            }
        }

        public void Load(BinaryReader reader, int version)
        {
            Radius = reader.ReadSingle();
            Angle = reader.ReadSingle();
            Length = reader.ReadSingle();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Radius);
            writer.Write(Angle);
            writer.Write(Length);
        }
    }

    public class WarningNode : MovableNode
    {
        public double InitialDelay { get; set; } = 0f;

        public float TimeUntilFade { get; set; } = 1f;

        public WarningShape Shape { get; set; } = WarningShape.Circle;

        public IWarningShape ShapeObj { get; set; } = new WarningCircle();

        float dashOffset = 0f;

        public override void Draw(GameTime gameTime)
        {
            dashOffset += (float)gameTime.ElapsedGameTime.TotalSeconds * 0.5f;
            ShapeObj.Draw(Transform.Position, 255, dashOffset, gameTime);
        }

        public override void Load(BinaryReader reader, int version)
        {
            Transform.Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());

            InitialDelay = reader.ReadDouble();
            TimeUntilFade = reader.ReadSingle();

            Shape = (WarningShape)reader.ReadInt32();
            switch (Shape)
            {
                case WarningShape.Circle:
                    ShapeObj = new WarningCircle();
                    break;
                case WarningShape.Line:
                    ShapeObj = new WarningLine();
                    break;
                case WarningShape.Rectangle:
                    ShapeObj = new WarningRectangle();
                    break;
                default:
                    break;
            }

            if (ShapeObj != null)
                ShapeObj.Load(reader, version);
        }

        public override void Save(BinaryWriter writer)
        {
            writer.Write(Transform.Position.X);
            writer.Write(Transform.Position.Y);

            writer.Write(InitialDelay);
            writer.Write(TimeUntilFade);

            writer.Write((int)Shape);
            ShapeObj.Save(writer);
        }
    }
}
