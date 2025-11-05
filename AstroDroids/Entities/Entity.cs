using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;

namespace AstroDroids.Entities
{
    public class Entity
    {
        protected Scene Scene { get { return SceneManager.GetScene(); } }

        public Entity()
        {

        }

        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }
    }
}
