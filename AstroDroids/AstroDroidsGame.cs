using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AstroDroids
{
    public class AstroDroidsGame : Game
    {
        private GraphicsDeviceManager _graphics;

        public AstroDroidsGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = Screen.ScreenWidth;
            _graphics.PreferredBackBufferHeight = Screen.ScreenHeight;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            InputSystem.Initialize();
            TextureManager.Initialize(this);
            SoundManager.Initialize();
            Screen.Initialize(this);

            //SceneManager.SetScene(new GameScene());
            SceneManager.SetScene(new LevelEditorScene());
        }

        protected override void Update(GameTime gameTime)
        {
            InputSystem.Begin();

            if(InputSystem.GetKeyDown(Keys.F1))
                SceneManager.SetScene(new LevelEditorScene());

            SceneManager.Update(gameTime);

            Screen.Update(gameTime);

            InputSystem.End();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Screen.spriteBatch.Begin(transformMatrix: Screen.GetCameraMatrix());

            Screen.DrawImGuiBefore(gameTime);

            SceneManager.Draw(gameTime);

            Screen.spriteBatch.End();

            Screen.DrawImGuiAfter();

            Screen.DrawGum(gameTime);

            base.Draw(gameTime);
        }

        public GraphicsDeviceManager GetGraphicsDeviceManager()
        {
            return _graphics;
        }
    }
}
