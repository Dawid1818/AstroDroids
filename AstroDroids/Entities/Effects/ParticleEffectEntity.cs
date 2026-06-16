using AstroDroids.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;

namespace AstroDroids.Entities.Effects
{
    public class ParticleEffectEntity : Entity
    {
        public ParticleEffect effect { get; set; }

        public ParticleEffectEntity(ParticleEffect effect)
        {
            this.effect = effect;
        }

        public override void Update(GameTime gameTime)
        {
            effect.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(effect);
        }
    }
}
