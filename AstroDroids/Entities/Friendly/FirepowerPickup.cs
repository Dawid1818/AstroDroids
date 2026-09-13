using AstroDroids.Drawables;
using AstroDroids.Gameplay;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroDroids.Entities.Friendly
{
    public class FirepowerPickup : CollidableEntity
    {
        public FirepowerPickup(Vector2 position) : base(new Transform(position))
        {
            AddCircleCollider(Vector2.Zero, 15f);
        }

        public override void Update(GameTime gameTime)
        {
            List<Player> players = Scene.World.GetPlayers();
            foreach (Player p in players)
            {
                if(Intersects(p))
                {
                    Scene.World.RemovePowerup(this);
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
        }
    }
}
