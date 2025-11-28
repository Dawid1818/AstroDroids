using AstroDroids.Entities.Friendly;
using AstroDroids.Gameplay;
using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Managers;
using AstroDroids.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

            //LevelManager.LoadLevel(0);

            GameState.NewState();

            World = new GameWorld();

            World.Initialize();

            World.AddPlayer(new Player(0, new Vector2(World.Bounds.Width / 2 - 16, World.Bounds.Bottom - 64)));

            LevelManager.StartLevel();

            Screen.ResetCamera();

            coroutineManager.StartCoroutine(LevelManager.GetLevelScript());
        }

        public override void Update(GameTime gameTime)
        {
            coroutineManager.Update();

            World.Update(gameTime);

            Screen.MoveCamera(new Vector2(0, -2));

            if(InputSystem.GetKeyDown(Keys.Escape) && LevelManager.Playtesting)
            {
                LevelManager.QuitPlaytest();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //Screen.spriteBatch.Draw(TextureManager.GetStarfield(), Vector2.Zero, Color.White);

            Screen.spriteBatch.End();

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Screen.ScreenWidth, Screen.ScreenHeight, 0, 0, 1);
            Matrix uv_transform = Screen.GetUVTransform(TextureManager.GetStarfield(), new Vector2(0, 0), 1f, Screen.Viewport);

            Screen.Infinite.Parameters["view_projection"].SetValue(projection);
            Screen.Infinite.Parameters["uv_transform"].SetValue(Matrix.Invert(uv_transform));

            Screen.spriteBatch.Begin(effect: Screen.Infinite, transformMatrix: Screen.GetCameraMatrix(), samplerState: SamplerState.LinearWrap);
            Screen.spriteBatch.Draw(TextureManager.GetStarfield(), new Rectangle(0, 0, Screen.ScreenWidth, Screen.ScreenHeight), Color.White);
            Screen.spriteBatch.End();

            Screen.spriteBatch.Begin(transformMatrix: Screen.GetCameraMatrix());

            World.Draw(gameTime);
        }
    }
}
