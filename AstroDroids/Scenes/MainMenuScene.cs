using AstroDroids.Coroutines;
using AstroDroids.Drawables;
using AstroDroids.Gameplay;
using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Interfaces;
using AstroDroids.Managers;
using AstroDroids.Screens;
using Gum.Forms.Controls;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections;
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

        bool transitioning = false;

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

            SoundManager.PlayMusic("subspace_loop");
        }

        public override void Update(GameTime gameTime)
        {
            InputSystem.SetMouseLock(false);

            coroutineManager.Update(gameTime);

            World.Update(gameTime);

            yPos -= (float)gameTime.ElapsedGameTime.TotalSeconds * 50f;

            if (InputSystem.GetKeyDown(Keys.R))
            {
                SceneManager.SetScene(new MainMenuScene());
            }

            if (menuPage != null && !transitioning)
            {
                if (InputSystem.IsActionDown(GameAction.NextWeapon) || InputSystem.GetRMBDown() || InputSystem.GetButtonDown(Buttons.B))
                {
                    menuPage.BackPressed();
                }

                menuPage.Update(gameTime);
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

        IEnumerator PageTransition(FrameworkElement page)
        {
            transitioning = true;
            InputSystem.ClearUIKeys();
            InputSystem.DisableUIMouse();
            InteractiveGue.CurrentInputReceiver = null;

            if (this.menuPage != null)
            {
                this.menuPage.TransitionOut();
                //(this.menuPage as FrameworkElement).Visual.AnimationController.OnCompleted += () => { transitioning = false; };
                yield return new WaitUntil(this.menuPage.TransitionFinished);
            }

            if (this.menuPage != null)
            {
                this.menuPage.Uninitialize();
            }
            this.menuPage = null;
            ui.HostPane.Children.Clear();
            ui.ClearHints();

            ui.HostPane.AddChild(page);

            if (page is IMenuPage menuPage)
            {
                menuPage.Initialize(this, ui);
                this.menuPage = menuPage;

                menuPage.TransitionIn();
                yield return new WaitUntil(this.menuPage.TransitionFinished);
            }

            transitioning = false;
            InputSystem.AddUIKeys();
            InputSystem.EnableUIMouse();

            yield return null;
        }

        IEnumerator TransitionToSceneCoroutine(Scene scene)
        {
            transitioning = true;
            InputSystem.ClearUIKeys();
            InputSystem.DisableUIMouse();
            InteractiveGue.CurrentInputReceiver = null;

            TransitionManager.SetState(TransitionState.In);

            yield return new WaitUntil(() => TransitionManager.State == TransitionState.Out);

            Screen.GumUI.Root.Children.Clear();

            SceneManager.SetScene(scene);

            transitioning = false;
            InputSystem.AddUIKeys();
            InputSystem.EnableUIMouse();
        }

        IEnumerator TransitionCloseGame()
        {
            transitioning = true;
            InputSystem.ClearUIKeys();
            InputSystem.DisableUIMouse();
            InteractiveGue.CurrentInputReceiver = null;

            TransitionManager.SetState(TransitionState.In);

            yield return new WaitUntil(() => TransitionManager.State == TransitionState.Out);

            AstroDroidsGame.Instance.Exit();
        }

        public void SetPage(FrameworkElement page)
        {
            coroutineManager.StartCoroutine(PageTransition(page));
        }

        public void TransitionToScene(Scene scene)
        {
            coroutineManager.StartCoroutine(TransitionToSceneCoroutine(scene));
        }

        public void TransitionClose()
        {
            coroutineManager.StartCoroutine(TransitionCloseGame());
        }
    }
}
