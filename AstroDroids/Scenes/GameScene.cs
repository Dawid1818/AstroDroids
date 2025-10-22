using AstroDroids.Entities;
using AstroDroids.Entities.Friendly;
using AstroDroids.Gameplay;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;

namespace AstroDroids.Scenes
{
    public class GameScene : Scene
    {
        public GameScene()
        {

        }

        public override void Set()
        {
            LevelManager.LoadLevel(0);

            GameState.NewState();

            World = new GameWorld();

            World.AddPlayer(new Player(0, new Vector2(World.Bounds.Width / 2, World.Bounds.Bottom - 64)));

            LevelManager.StartLevel();
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
