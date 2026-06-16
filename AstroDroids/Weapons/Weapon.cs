using AstroDroids.Entities.Friendly;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;

namespace AstroDroids.Weapons
{
    public class Weapon
    {
        protected Scene Scene { get { return SceneManager.GetScene(); } }

        public virtual void Update(Player player, GameTime gameTime)
        {

        }

        public virtual void DrawEffects(Player player, GameTime gameTime)
        {

        }

        public virtual void ResetState()
        {

        }
    }
}
