using AstroDroids.Drawables;
using AstroDroids.Entities.Friendly;
using AstroDroids.Gameplay;
using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Managers;
using AstroDroids.Screens;
using Gum.Forms;
using Gum.Forms.Controls;
using Gum.GueDeriving;
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

        public void DisplayBossWarning()
        {
            ui.BossWarning.Visible = true;
            ui.Visual.PlayAnimation(ui.Show);
        }

        public void HideBossWarning()
        {
            ui.Visual.PlayAnimation(ui.Hide);
        }

        public void DisableBossWarning()
        {
            ui.BossWarning.Visible = false;
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

            ui.ScoreLabel.Text = GameState.GetScore().ToString();
            ui.LivesLabel.Text = GameState.GetLives().ToString();
            ui.PowerLabel.Text = $"{GameState.GetFirepower()}/5";
            ui.WeaponPanelIcon.Texture = GameState.GetWeaponIcon();

            if (World.BossEntity != null)
            {
                ui.BossPanel.Visible = true;
                ui.BossHPBar.BarPercent = (float)World.BossEntity.GetHealth() / World.BossEntity.GetStartingHealth() * 100f;
            }
            else
            {
                ui.BossPanel.Visible = false;
            }

            //if (World.Enemies.Count > 0)
            //{
            //    int totalHealth = World.Enemies[0].GetHealth();

            //    ui.BossHPBar.BarPercent = totalHealth / 1000f * 100f;
            //}

            byte bottomAlpha = 255;
            byte scoreAlpha = 255;
            byte bossAlpha = 255;


            foreach (var item in World.GetPlayers())
            {
                float dist = Vector2.Distance(item.Transform.Position, World.Bounds.BottomLeft);

                float alpha = MathHelper.Clamp((dist - 100f) / 100f, 0f, 1f);

                byte bottomAlphac = (byte)(alpha * 255f);

                if (bottomAlphac < bottomAlpha)
                    bottomAlpha = bottomAlphac;


                dist = Vector2.Distance(item.Transform.Position, World.Bounds.TopLeft);

                alpha = MathHelper.Clamp((dist - 100f) / 100f, 0f, 1f);

                byte scoreAlphac = (byte)(alpha * 255f);

                if (scoreAlphac < scoreAlpha)
                    scoreAlpha = scoreAlphac;


                dist = Vector2.Distance(item.Transform.Position, new Vector2(World.Bounds.Center.X, World.Bounds.Top));

                alpha = MathHelper.Clamp((dist - 100f) / 100f, 0f, 1f);

                byte bossAlphac = (byte)(alpha * 255f);

                if (bossAlphac < bossAlpha)
                    bossAlpha = bossAlphac;

            }

            ui.WeaponPanelBG.Alpha = bottomAlpha;
            ui.WeaponPanelIcon.Alpha = bottomAlpha;

            ui.BottomPanelBG.Alpha = bottomAlpha;
            ui.ShipIcon.Alpha = bottomAlpha;
            ui.PowerIcon.Alpha = bottomAlpha;

            (ui.PowerLabel.Visual as TextRuntime).Alpha = bottomAlpha;
            (ui.LivesLabel.Visual as TextRuntime).Alpha = bottomAlpha;


            ui.ScorePanelBG.Alpha = scoreAlpha;
            (ui.ScoreLabel.Visual as TextRuntime).Alpha = scoreAlpha;


            ui.BossPanelBG.Alpha = bossAlpha;
            ui.BossHPBar.FindVisual<NineSliceRuntime>("Background").Alpha = bossAlpha;
            ui.BossHPBar.FindVisual<NineSliceRuntime>("BarContainer").Alpha = bossAlpha;
            ui.BossHPBar.FindVisual<NineSliceRuntime>("Bar").Alpha = bossAlpha;

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

            if (ui.BossWarning.Visible)
            {
                ui.BossWarningBottomLines.FindVisual<SpriteRuntime>("LinesSprite").TextureLeft += 1;
                ui.BossWarningTopLines.FindVisual<SpriteRuntime>("LinesSprite").TextureLeft += 1;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Screen.ScreenWidth, Screen.ScreenHeight, 0, 0, 1);
            Matrix uv_transform = Screen.GetUVTransform(TextureManager.GetStarfield(), new Vector2(0, -yPos), 1f, Screen.Viewport);

            Screen.Infinite.Parameters["view_projection"].SetValue(projection);
            Screen.Infinite.Parameters["uv_transform"].SetValue(Matrix.Invert(uv_transform));

            World.Draw(gameTime);
        }

        public override void DrawDebug(GameTime gameTime)
        {
            if (paused)
            {
                Screen.spriteBatch.Begin();
                Screen.DrawText("Paused", new Vector2(120, 10), Color.White, 12f);
                Screen.spriteBatch.End();
            }

            if (World != null)
                World.DrawDebug();
        }
    }
}
