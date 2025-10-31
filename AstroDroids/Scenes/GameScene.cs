using AstroDroids.Entities.Friendly;
using AstroDroids.Gameplay;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;

namespace AstroDroids.Scenes
{
    public class GameScene : Scene
    {
        CoroutineManager coroutineManager = new CoroutineManager();

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

            coroutineManager.StartCoroutine(LevelManager.GetLevelScript());
        }

        public override void Update(GameTime gameTime)
        {
            coroutineManager.Update();

            World.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(TextureManager.GetStarfield(), Vector2.Zero, Color.White);

            World.Draw(gameTime);
        }
    }
}
