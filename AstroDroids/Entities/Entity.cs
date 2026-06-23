using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using System;

namespace AstroDroids.Entities
{
    public class Entity
    {
        public Transform Transform { get; set; }
        protected Scene Scene { get { return SceneManager.GetScene(); } }

        protected Random Random { get { return AstroDroidsGame.rnd; } }

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

        public virtual void DrawDebug(GameTime gameTime) { }

        protected void DefaultMove()
        {
            Transform.Position = new Vector2(Transform.Position.X, Transform.Position.Y + (float)Scene.World.speed);
        }
    }
}
