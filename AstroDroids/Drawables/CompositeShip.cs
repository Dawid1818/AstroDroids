using AstroDroids.Data;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace AstroDroids.Drawables
{
    public class ShipPart
    {
        public Vector2 Offset { get; set; }
        public Texture2D Texture { get; set; }
        public Color Color { get; set; }

        public ShipPart(Texture2D texture, Vector2 offset, Color color)
        {
            Texture = texture;
            Offset = offset;
            Color = color;
        }
    }

    public class CompositeShip
    {
        List<ShipPart> Parts = new List<ShipPart>();

        public CompositeShip()
        {
            AddPart(TextureManager.Get("Ships/Player/PlayerShip_Body"), new Vector2(0, 0), Color.White);
            AddPart(TextureManager.Get("Ships/Player/PlayerShip_Weapons"), new Vector2(0, 0), Color.White);
            AddPart(TextureManager.Get("Ships/Player/PlayerShip_Engines"), new Vector2(0, 0), Color.White);
            AddPart(TextureManager.Get("Ships/Player/PlayerShip_Cockpit"), new Vector2(0, 0), Color.White);
            AddPart(TextureManager.Get("Ships/Player/PlayerShip_CockpitGlass"), new Vector2(0, 0), Color.White);
            AddPart(TextureManager.Get("Ships/Player/PlayerShip_Wings"), new Vector2(0, 0), Color.White);
        }

        void AddPart(Texture2D texture, Vector2 offset, Color color)
        {
            Parts.Add(new ShipPart(texture, offset, color));
        }

        public void Draw(Vector2 Position, float angle, float scale)
        {
            Screen.spriteBatch.End();
            Screen.spriteBatch.Begin(effect: Screen.Test, transformMatrix: Screen.GetCameraMatrix(), samplerState: SamplerState.LinearWrap);

            foreach (var part in Parts)
            {
                DrawPart(part, Position + part.Offset, angle, scale);
            }

            Screen.spriteBatch.End();

            Screen.spriteBatch.Begin(transformMatrix: Screen.GetCameraMatrix(), blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointWrap);
        }

        void DrawPart(ShipPart part, Vector2 position, float angle, float scale)
        {
            Screen.spriteBatch.Draw(part.Texture, position, null, part.Color, angle, new Vector2(part.Texture.Width / 2, part.Texture.Height / 2), scale, SpriteEffects.None, 0f);
        }

        internal void ApplyCustomization(ShipCustomization customization)
        {
            Parts[0].Color = customization.BodyColor.ToColor();
            Parts[1].Color = customization.WeaponsColor.ToColor();
            Parts[2].Color = customization.EnginesColor.ToColor();
            Parts[3].Color = customization.CockpitColor.ToColor();
            Parts[4].Color = customization.CockpitGlassColor.ToColor();
            Parts[5].Color = customization.WingsColor.ToColor();
        }
    }
}
