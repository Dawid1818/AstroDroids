using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
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

            SceneManager.SetScene(new GameScene());
            //SceneManager.SetScene(new CurveEditorScene());
        }

        protected override void Update(GameTime gameTime)
        {
            InputSystem.Begin();

            SceneManager.Update(gameTime);

            Screen.Update(gameTime);

            InputSystem.End();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Screen.spriteBatch.Begin(transformMatrix: Screen.GetCameraMatrix());

            SceneManager.Draw(gameTime);

            Screen.spriteBatch.End();

            Screen.Draw(gameTime);

            base.Draw(gameTime);
        }

        public GraphicsDeviceManager GetGraphicsDeviceManager()
        {
            return _graphics;
        }
    }
}
