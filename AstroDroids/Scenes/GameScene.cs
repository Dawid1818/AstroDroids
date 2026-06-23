using AstroDroids.Drawables;
using AstroDroids.Entities.Friendly;
using AstroDroids.Gameplay;
using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Managers;
using AstroDroids.Screens;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace AstroDroids.Scenes
{
    public class GameScene : Scene
    {
        GameScreenGum ui;

        CoroutineManager coroutineManager = new CoroutineManager();

        bool paused = false;

        float yPos = 0f;

        public GameScene()
        {

        }

        public override void Set()
        {
            ui = new GameScreenGum();
            ui.AddToRoot();

            //LevelManager.LoadLevel(0);

            if (World == null)
                World = new GameWorld();

            GameState.NewState();

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
            if (InputSystem.GetKeyDown(Keys.P))
            {
                paused = !paused;
            }

            if (!paused)
            {
                coroutineManager.Update();

                World.Update(gameTime);

                yPos -= (float)gameTime.ElapsedGameTime.TotalSeconds * 50f;
            }

            if (InputSystem.GetKeyDown(Keys.F5))
            {
                GameState.Firepower += 1;
                if (GameState.Firepower > 5)
                {
                    GameState.Firepower = 1;
                }
            }

            if (InputSystem.GetKeyDown(Keys.Escape) && LevelManager.Playtesting)
            {
                LevelManager.QuitPlaytest();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Screen.ScreenWidth, Screen.ScreenHeight, 0, 0, 1);
            Matrix uv_transform = Screen.GetUVTransform(TextureManager.GetStarfield(), new Vector2(0, -yPos), 1f, Screen.Viewport);

            Screen.Infinite.Parameters["view_projection"].SetValue(projection);
            Screen.Infinite.Parameters["uv_transform"].SetValue(Matrix.Invert(uv_transform));

            World.Draw(gameTime);

            if (paused)
            {
                Screen.spriteBatch.Begin();
                Screen.DrawText("Paused", new Vector2(10, 10), Color.White, 12f);
                Screen.spriteBatch.End();
            }
        }
    }
}
