using AstroDroids.Entities;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace AstroDroids.Levels
{
    public class BackgroundObjectNode : MovableNode
    {
        public double InitialDelay { get; set; } = 0f;
        public float Angle { get; set; } = 0f;
        public string TextureName { get; set; } = string.Empty;
        public bool FlipH { get; set; } = false;
        public bool FlipV { get; set; } = false;

        Texture2D texture;

        public override void Draw(GameTime gameTime)
        {
            if (texture != null)
            {
                SpriteEffects effect = SpriteEffects.None;
                if (FlipH)
                    effect = effect | SpriteEffects.FlipHorizontally;
                if (FlipV)
                    effect = effect | SpriteEffects.FlipVertically;

                Screen.spriteBatch.Draw(texture, HasPath && Path != null && Path.StartPoint != null ? Path.StartPoint : Transform.Position, null, Color.White, MathHelper.ToRadians(Angle), new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, effect, 0f);
            }
        }

        public override void Load(BinaryReader reader, int version)
        {
            Transform.Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());

            InitialDelay = reader.ReadDouble();
            Angle = reader.ReadSingle();
            TextureName = reader.ReadString();

            FlipH = reader.ReadBoolean();
            FlipV = reader.ReadBoolean();

            UpdateTexture();

            base.Load(reader, version);
        }

        public override void Save(BinaryWriter writer)
        {
            writer.Write(Transform.Position.X);
            writer.Write(Transform.Position.Y);

            writer.Write(InitialDelay);
            writer.Write(Angle);
            writer.Write(TextureName);

            writer.Write(FlipH);
            writer.Write(FlipV);

            base.Save(writer);
        }

        internal void UpdateTexture()
        {
            texture = TextureManager.GetBackgroundObject(TextureName);
        }
    }
}
