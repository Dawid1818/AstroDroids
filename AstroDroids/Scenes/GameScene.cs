using AstroDroids.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroDroids.Scenes
{
    public class GameScene : Scene
    {
        Player player;
        public GameScene()
        {
            player = new Player(Vector2.Zero);
        }

        public override void Update(GameTime gameTime)
        {
            player.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            player.Draw(gameTime);
        }
    }
}
