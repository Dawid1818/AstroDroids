using AstroDroids.Entities;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
