
using Microsoft.Xna.Framework;

namespace AstroDroids.Data
{
    public struct ShipColor
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
    }

    public class ShipCustomization
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
    }
}
