using AstroDroids.Entities.Friendly;
using AstroDroids.Gameplay;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using AstroDroids.Screens;
using Microsoft.Xna.Framework;
using MonoGameGum;

namespace AstroDroids.Scenes
{
    public class GameScene : Scene
    {
        GameScreenGum ui;

        CoroutineManager coroutineManager = new CoroutineManager();

        public GameScene()
        {

        }

        public override void Set()
        {
            ui = new GameScreenGum();
            ui.AddToRoot();

            LevelManager.LoadLevel(0);

            GameState.NewState();

            World = new GameWorld();

            World.AddPlayer(new Player(0, new Vector2(World.Bounds.Width / 2 - 16, World.Bounds.Bottom - 64)));

            LevelManager.StartLevel();

            coroutineManager.StartCoroutine(LevelManager.GetLevelScript());
        }

        public override void Update(GameTime gameTime)
        {
            coroutineManager.Update();

            World.Update(gameTime);

            Screen.MoveCamera(new Vector2(0, -2));
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(TextureManager.GetStarfield(), Vector2.Zero, Color.White);

            World.Draw(gameTime);
        }
    }
}
