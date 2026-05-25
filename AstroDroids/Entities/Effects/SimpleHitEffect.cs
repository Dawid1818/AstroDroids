using AstroDroids.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroDroids.Entities.Effects
{
    public class SimpleHitEffect : Entity
    {
        float timer = 0;

        float finalSize = 16;

        public SimpleHitEffect(Transform transform) : base(transform)
        {

        }

        public override void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if(timer >= 1)
            {
                Scene.World.RemoveEffect(this);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            float radius = finalSize * timer;

            float opacity = timer <= 0.5 ? 1f : 1f - timer;

            Screen.spriteBatch.DrawCircle(Transform.Position.X, Transform.Position.Y, radius, 32, new Color(Color.Cyan.R, Color.Cyan.G, Color.Cyan.B, opacity), radius, 0f);
        }
    }
}
