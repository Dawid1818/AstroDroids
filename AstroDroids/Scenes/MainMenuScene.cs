using AstroDroids.Drawables;
using AstroDroids.Gameplay;
using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Interfaces;
using AstroDroids.Managers;
using AstroDroids.Screens;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace AstroDroids.Scenes
{
    public class MainMenuScene : Scene
    {
        IMenuPage menuPage;

        CoroutineManager coroutineManager = new CoroutineManager();

        HintedScreenGum ui;

        float yPos = 0f;
        float xPos = 0f;

        InputMethod inputMethod;

        public MainMenuScene()
        {

        }

        public override void Set()
        {
            inputMethod = InputSystem.GetLastInputMethod();

            Screen.GumUI.Root.Children.Clear();

            ui = new HintedScreenGum();
            ui.AddToRoot();

            MainMenuScreenGum page = new MainMenuScreenGum();
            SetPage(page);

            if (World == null)
                World = new GameWorld();

            List<Texture2D> starfields = TextureManager.GetStarfields();
            World.Starfield = new ImageStarfield(starfields[0]);

            Screen.ResetCamera();
        }

        public override void Update(GameTime gameTime)
        {
            coroutineManager.Update();

            World.Update(gameTime);

            yPos -= (float)gameTime.ElapsedGameTime.TotalSeconds * 50f;

            if (InputSystem.GetKeyDown(Keys.R))
            {
                SceneManager.SetScene(new MainMenuScene());
            }

            if (menuPage != null)
            {
                if (InputSystem.IsActionDown(GameAction.NextWeapon) || InputSystem.GetRMBDown() || InputSystem.GetButtonDown(Buttons.B))
                {
                    menuPage.BackPressed();
                }
            }

            InputMethod newInputMethod = InputSystem.GetLastInputMethod();

            if (inputMethod != newInputMethod)
            {
                inputMethod = newInputMethod;
                ui.InputMethodChanged(inputMethod);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Screen.ScreenWidth, Screen.ScreenHeight, 0, 0, 1);
            Matrix uv_transform = Screen.GetUVTransform(TextureManager.GetStarfield(), new Vector2(-xPos, -yPos), 1f, Screen.Viewport);

            Screen.Infinite.Parameters["view_projection"].SetValue(projection);
            Screen.Infinite.Parameters["uv_transform"].SetValue(Matrix.Invert(uv_transform));

            World.Draw(gameTime);
        }

        public override void DrawDebug(GameTime gameTime)
        {
            if (World != null)
                World.DrawDebug();
        }

        public void SetPage(FrameworkElement page)
        {
            this.menuPage = null;
            ui.HostPane.Children.Clear();
            ui.ClearHints();

            if (page is IMenuPage menuPage)
            {
                menuPage.Initialize(this, ui);
                this.menuPage = menuPage;
            }

            ui.HostPane.AddChild(page);
        }
    }
}
