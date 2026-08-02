
using Microsoft.Xna.Framework;

namespace AstroDroids.Coroutines
{
    public class Coroutine
    {
        public virtual bool Execute(GameTime gameTime)
        {
            return false;
        }
    }
}
