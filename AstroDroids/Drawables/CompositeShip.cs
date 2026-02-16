using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            AddPart(TextureManager.Get("Ships/Player/PlayerShip_Hull"), new Vector2(0, 0), Color.Green);
            AddPart(TextureManager.Get("Ships/Player/PlayerShip_HullOutline"), new Vector2(0, 0), Color.White);
            AddPart(TextureManager.Get("Ships/Player/PlayerShip_CockpitGlass"), new Vector2(0, 0), Color.Red);
            AddPart(TextureManager.Get("Ships/Player/PlayerShip_CockpitOutline"), new Vector2(0, 0), Color.White);
            AddPart(TextureManager.Get("Ships/Player/PlayerShip_Decals"), new Vector2(0, 0), Color.Yellow);
            AddPart(TextureManager.Get("Ships/Player/PlayerShip_DecalsOutline"), new Vector2(0, 0), Color.White);
            AddPart(TextureManager.Get("Ships/Player/PlayerShip_Engines"), new Vector2(0, 0), Color.Orange);
            AddPart(TextureManager.Get("Ships/Player/PlayerShip_EnginesOutline"), new Vector2(0, 0), Color.White);
            AddPart(TextureManager.Get("Ships/Player/PlayerShip_WeaponPods"), new Vector2(0, 0), Color.Pink);
            AddPart(TextureManager.Get("Ships/Player/PlayerShip_WeaponPodsOutline"), new Vector2(0, 0), Color.White);
        }

        void AddPart(Texture2D texture, Vector2 offset, Color color)
        {
            Parts.Add(new ShipPart(texture, offset, color));
        }

        public void Draw(Vector2 Position, float angle)
        {
            foreach (var part in Parts)
            {
                DrawPart(part, Position + part.Offset, angle);
            }
        }

        void DrawPart(ShipPart part, Vector2 position, float angle)
        {
            Screen.spriteBatch.Draw(part.Texture, position, null, part.Color, angle, new Vector2(part.Texture.Width / 2, part.Texture.Height / 2), 1f, SpriteEffects.None, 0f);
        }
    }
}
