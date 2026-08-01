using AstroDroids.ErrorHandling;
using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.InteropServices;

namespace AstroDroids
{
    public class AstroDroidsGame : Game
    {
        public static AstroDroidsGame Instance { get; private set; }
        public GraphicsDeviceManager Graphics { get; private set; }

        public static bool Debug { get; set; } = false;

        public static Random rnd { get; private set; } = new Random();

        [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_MaximizeWindow(IntPtr window);

        public AstroDroidsGame()
        {
            Instance = this;
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Graphics.PreferredBackBufferWidth = Screen.ScreenWidth;
            Graphics.PreferredBackBufferHeight = Screen.ScreenHeight;
            Window.AllowUserResizing = true;
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            base.Initialize();

            NativeErrorHandler.Setup();

            Window.Position = new Point(0, 0);
            SDL_MaximizeWindow(Window.Handle);
        }

        protected override void LoadContent()
        {
            InputSystem.Initialize();
            TextureManager.Initialize(this);
            SoundManager.Initialize();
            Screen.Initialize(this);
            GameDatabase.Initialize();
            LevelManager.Initialize();
            TransitionManager.Initialize();

            //SceneManager.SetScene(new GameScene());
            //SceneManager.SetScene(new LevelEditorScene());
            //SceneManager.SetScene(new ShipEditorScene());
            SceneManager.SetScene(new MainMenuScene());
        }

        protected override void Update(GameTime gameTime)
        {
            InputSystem.Begin();

            if(InputSystem.GetKeyDown(Keys.F1))
                SceneManager.SetScene(new LevelEditorScene());

            if (InputSystem.GetKeyDown(Keys.F2))
                Debug = !Debug;

            SceneManager.Update(gameTime);

            Screen.Update(gameTime);

            InputSystem.End();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Screen.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
