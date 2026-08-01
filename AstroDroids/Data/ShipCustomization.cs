
using AstroDroids.Interfaces;
using Microsoft.Xna.Framework;
using System.IO;

namespace AstroDroids.Data
{
    public class ShipColor : ISaveable
    {
        public float Hue { get; set; }
        public float Saturation { get; set; }
        public float Value { get; set; }

        public Color ToColor()
        {
            return Color.FromHSV(Hue, Saturation, Value);
        }

        public static ShipColor FromColor(Color color)
        {
            color.ToHSV(out float h, out float s, out float v);
            return new ShipColor() { Hue = h, Saturation = s, Value = v };
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Hue);
            writer.Write(Saturation);
            writer.Write(Value);
        }

        public void Load(BinaryReader reader, int version)
        {
            Hue = reader.ReadSingle();
            Saturation = reader.ReadSingle();
            Value = reader.ReadSingle();
        }

    }

    public class ShipCustomization : ISaveable
    {
        private ShipColor[] colors = new ShipColor[6];

        public ShipColor BodyColor { get => colors[0]; set => colors[0] = value; }
        public ShipColor WeaponsColor { get => colors[1]; set => colors[1] = value; }
        public ShipColor EnginesColor { get => colors[2]; set => colors[2] = value; }
        public ShipColor CockpitColor { get => colors[3]; set => colors[3] = value; }
        public ShipColor CockpitGlassColor { get => colors[4]; set => colors[4] = value; }
        public ShipColor WingsColor { get => colors[5]; set => colors[5] = value; }

        public ShipCustomization()
        {
            for (int i = 0; i < 6; i++)
            {
                colors[i] = ShipColor.FromColor(Color.White);
            }
        }

        public ShipColor GetColorByPart(int part) => colors[part];
        public void SetColorByPart(int part, ShipColor color)
        {
            colors[part] = color;
        }

        public void Save(BinaryWriter writer)
        {
            BodyColor.Save(writer);
            WeaponsColor.Save(writer);
            EnginesColor.Save(writer);
            CockpitColor.Save(writer);
            CockpitGlassColor.Save(writer);
            WingsColor.Save(writer);
        }

        public void Load(BinaryReader reader, int version)
        {
            BodyColor = new ShipColor();
            BodyColor.Load(reader, version);

            WeaponsColor = new ShipColor();
            WeaponsColor.Load(reader, version);

            EnginesColor = new ShipColor();
            EnginesColor.Load(reader, version);

            CockpitColor = new ShipColor();
            CockpitColor.Load(reader, version);

            CockpitGlassColor = new ShipColor();
            CockpitGlassColor.Load(reader, version);

            WingsColor = new ShipColor();
            WingsColor.Load(reader, version);
        }
    }
}
