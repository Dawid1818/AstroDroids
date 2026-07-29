
using AstroDroids.Components.MenuPages;
using AstroDroids.Drawables;
using AstroDroids.Gameplay;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using AstroDroids.Screens;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AstroDroids.Scenes
{
    public class MainMenuScene : Scene
    {
        MainMenuScreenGum ui;

        CoroutineManager coroutineManager = new CoroutineManager();

        public MainMenuScene()
        {

        }

        public override void Set()
        {
            Screen.GumUI.Root.Children.Clear();

            ui = new MainMenuScreenGum();
            ui.AddToRoot();

            ui.AddChild(new Main());

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
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Screen.ScreenWidth, Screen.ScreenHeight, 0, 0, 1);
            Matrix uv_transform = Screen.GetUVTransform(TextureManager.GetStarfield(), new Vector2(0, 0), 1f, Screen.Viewport);

            Screen.Infinite.Parameters["view_projection"].SetValue(projection);
            Screen.Infinite.Parameters["uv_transform"].SetValue(Matrix.Invert(uv_transform));

            World.Draw(gameTime);
        }

        public override void DrawDebug(GameTime gameTime)
        {
            if (World != null)
                World.DrawDebug();
        }
    }
}
