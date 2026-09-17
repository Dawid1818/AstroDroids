using AstroDroids.Gameplay;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AstroDroids.Entities.Friendly
{
    public class FirepowerPickup : CollidableEntity
    {
        Texture2D texture;
        public FirepowerPickup(Vector2 position) : base(new Transform(position))
        {
            texture = TextureManager.Get("Powerups/Firepower");
            AddCircleCollider(Vector2.Zero, 15f);
        }

        public override void Update(GameTime gameTime)
        {
            List<Player> players = Scene.World.GetPlayers();
            foreach (Player p in players)
            {
                if (Intersects(p))
                {
                    Scene.World.RemovePowerup(this);
                    SoundManager.PlaySound("Firepower");
                    GameStateManager.IncreaseFirepower();
                    break;
                }
            }

            if (!Intersects(Scene.World.Bounds))
            {
                Scene.World.RemovePowerup(this);
            }

            DefaultMove();
        }

        public override void Draw(GameTime gameTime)
        {
            float size = 15f;
            Screen.shapeBatch.DrawCircle(Transform.Position, size - 3, Color.DarkBlue, Color.Cyan, 1);
            Screen.shapeBatch.BorderCircleBlurred(Transform.Position, size, Color.Cyan, 3, 3);
            Screen.spriteBatch.Draw(texture, Transform.Position, null, Color.White, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 0.3f, SpriteEffects.None, 0f);
        }
    }
}
