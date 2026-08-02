using Microsoft.Xna.Framework;
using System;

namespace AstroDroids.Coroutines
{
    public class WaitForSeconds : Coroutine
    {
        private readonly TimeSpan waitTime;
        private TimeSpan elapsed;

        public WaitForSeconds(double seconds)
        {
            waitTime = TimeSpan.FromSeconds(seconds);
        }

        public override bool Execute(GameTime gameTime)
        {
            elapsed += gameTime.ElapsedGameTime;
            return elapsed >= waitTime;
        }
    }
}
