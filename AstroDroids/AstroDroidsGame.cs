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
        public GraphicsDeviceManager Graphics { get; private set; }

        public static bool Debug { get; set; } = true;

        public AstroDroidsGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Graphics.PreferredBackBufferWidth = Screen.ScreenWidth;
            Graphics.PreferredBackBufferHeight = Screen.ScreenHeight;
            Window.AllowUserResizing = true;
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
            LevelManager.Initialize();

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
            GraphicsDevice.Clear(Color.Black);

            //Screen.spriteBatch.Begin(transformMatrix: Screen.GetCameraMatrix());

            Screen.DrawImGuiBefore(gameTime);

            SceneManager.Draw(gameTime);

            //Screen.spriteBatch.End();

            Screen.DrawImGuiAfter();

            Screen.DrawGum(gameTime);

            base.Draw(gameTime);
        }
    }
}
