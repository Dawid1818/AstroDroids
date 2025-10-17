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
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            TextureManager.Initialize(this);
            SoundManager.Initialize();
            Screen.Initialize(this);

            SceneManager.SetScene(new MainMenuScene());
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

            SceneManager.Draw(gameTime);

            Screen.Draw(gameTime);

            base.Draw(gameTime);
        }

        public GraphicsDeviceManager GetGraphicsDeviceManager()
        {
            return _graphics;
        }
    }
}
