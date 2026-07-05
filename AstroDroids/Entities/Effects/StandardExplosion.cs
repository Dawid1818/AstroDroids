using AstroDroids.Drawables;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroDroids.Entities.Effects
{
    public class StandardExplosion : Entity
    {
        AnimatedSprite sprite;
        float scale = 1f;
        float angle = 0f;
        public StandardExplosion(Transform transform, float scale) : base(transform)
        {
            sprite = new AnimatedSprite(TextureManager.Get("Effects/StandardExplosion/Explosion"), 8, 128, 128, 0, 61, 150f);
            this.scale = scale;

            angle = (float)(Random.NextDouble() * Math.Tau);
        }

        public override void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);

            if(sprite.frame == sprite.frameCount - 1)
            {
                Scene.World.RemoveEffect(this);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            sprite.Draw(Transform.Position, angle, scale);
        }
    }
}
