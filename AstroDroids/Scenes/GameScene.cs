using AstroDroids.Entities;
using AstroDroids.Gameplay;
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
        public GameScene()
        {
            GameState.NewState();

            World = new GameWorld();

            World.Player = new Player(Vector2.Zero);
        }

        public override void Update(GameTime gameTime)
        {
            World.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            World.Draw(gameTime);
        }
    }
}
