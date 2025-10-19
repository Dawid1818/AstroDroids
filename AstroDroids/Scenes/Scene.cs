using AstroDroids.Gameplay;
using Microsoft.Xna.Framework;

namespace AstroDroids.Scenes
{
    public class Scene
    {
        public GameWorld World { get; protected set; }

        public Scene() { }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime)
        {

        }
    }
}
