using AstroDroids.Drawables;
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
using System.Collections.Generic;

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

            if(World == null)
                World = new GameWorld();

            World.Initialize();

            if (LevelManager.CurrentLevel.BackgroundId == 0)
            {
                World.Starfield = new SimulationStarfield();
            }
            else
            {
                List<Texture2D> starfields = TextureManager.GetStarfields();
                World.Starfield = new ImageStarfield(starfields[LevelManager.CurrentLevel.BackgroundId - 1]);
            }

            World.AddPlayer(new Player(0, new Vector2(World.Bounds.Width / 2 - 16, World.Bounds.Bottom - 64)));

            LevelManager.StartLevel();

            Screen.ResetCamera();

            coroutineManager.StartCoroutine(LevelManager.GetLevelScript());
        }

        public override void Update(GameTime gameTime)
        {
            coroutineManager.Update();

            World.Update(gameTime);

            if(InputSystem.GetKeyDown(Keys.Escape) && LevelManager.Playtesting)
            {
                LevelManager.QuitPlaytest();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Screen.ScreenWidth, Screen.ScreenHeight, 0, 0, 1);
            Matrix uv_transform = Screen.GetUVTransform(TextureManager.GetStarfield(), new Vector2(0, 0), 1f, Screen.Viewport);

            Screen.Infinite.Parameters["view_projection"].SetValue(projection);
            Screen.Infinite.Parameters["uv_transform"].SetValue(Matrix.Invert(uv_transform));

            World.Draw(gameTime);
        }
    }
}
