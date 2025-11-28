using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;

namespace AstroDroids.Entities
{
    public class Entity
    {
        public Transform Transform { get; set; }
        protected Scene Scene { get { return SceneManager.GetScene(); } }

        public Entity()
        {
            Transform = new Transform(0, 0);
        }

        public Entity(Transform transform)
        {
            Transform = transform;
        }

        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }
    }
}
