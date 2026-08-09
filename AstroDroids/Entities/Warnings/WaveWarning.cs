using AstroDroids.Levels;
using Microsoft.Xna.Framework;

namespace AstroDroids.Entities.Warnings
{
    public class WaveWarning : Entity
    {
        IWarningShape shape;

        float dashOffset = 0f;
        float time;
        float timeUntilFade = 1f;

        int state = 0;

        public WaveWarning(IWarningShape shape, float time)
        {
            this.shape = shape;
            this.timeUntilFade = time;
        }

        public override void Update(GameTime gameTime)
        {
            if (shape != null)
            {
                dashOffset += (float)gameTime.ElapsedGameTime.TotalSeconds * 0.5f;
            }

            //fading in
            if(state == 0)
            {
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if(time >= 1f)
                {
                    state = 1;
                }
            }//waiting
            else if(state == 1)
            {
                timeUntilFade -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(timeUntilFade <= 0f)
                {
                    state = 2;
                }
            }//fading out
            else if(state == 2)
            {
                time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (time <= 0f)
                {
                    Scene.World.RemoveWarning(this);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (shape != null)
            {
                byte alpha = (byte)((time / 1) * 255);
                shape.Draw(Transform.Position, alpha, dashOffset, gameTime);
            }
        }
    }
}
