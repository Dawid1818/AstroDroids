using AstroDroids.Coroutines;
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
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections;
using System.Collections.Generic;

namespace AstroDroids.Scenes
{
    public class GameScene : Scene
    {
        GameScreenGum ui;

        CoroutineManager coroutineManager = new CoroutineManager();

        bool debugPaused = false;

        bool paused = false;

        float yPos = 0f;

        bool transitioning = false;

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
            Screen.GumUI.Root.Children.Clear();

            ui = new GameScreenGum();
            ui.Initialize(this);
            ui.AddToRoot();

            if(GameStateManager.GetMissionType() != MissionType.Editor)
            {
                List<string> levels = GameStateManager.GetLevels();
                LevelManager.LoadLevel(levels[GameStateManager.GetLevelIndex()]);
            }

            if (World == null)
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

            //coroutineManager.StartCoroutine(LevelManager.GetLevelScript());
        }

        public override void Update(GameTime gameTime)
        {
            if (InputSystem.GetKeyDown(Keys.P))
            {
                debugPaused = !debugPaused;
            }

            if(InputSystem.GetKeyDown(Keys.Escape) && !transitioning)
            {
                SetPauseState(!paused);
            }

            coroutineManager.Update();

            if (!debugPaused && !paused)
            {
                World.Update(gameTime);

                yPos -= (float)gameTime.ElapsedGameTime.TotalSeconds * 50f;
            }

            ui.ScoreLabel.Text = GameStateManager.GetScore().ToString();
            ui.LivesLabel.Text = GameStateManager.GetLives().ToString();
            ui.PowerLabel.Text = $"{GameStateManager.GetFirepower()}/5";
            ui.WeaponPanelIcon.Texture = GameStateManager.GetWeaponIcon();

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
                GameStateManager.Firepower += 1;
                if (GameStateManager.GetFirepower() > 5)
                {
                    GameStateManager.Firepower = 1;
                }
            }

            //if (InputSystem.GetKeyDown(Keys.Escape) && LevelManager.Playtesting)
            //{
            //    LevelManager.QuitPlaytest();
            //}

            if (ui.BossWarning.Visible)
            {
                ui.BossWarningBottomLines.FindVisual<SpriteRuntime>("LinesSprite").TextureLeft += 1;
                ui.BossWarningTopLines.FindVisual<SpriteRuntime>("LinesSprite").TextureLeft += 1;
            }
        }

        public void SaveAndQuit()
        {
            if (LevelManager.Playtesting)
            {
                LevelManager.QuitPlaytest();
            }
            else
            {
                coroutineManager.StartCoroutine(TransitionToSceneCoroutine(new MainMenuScene()));
            }
        }

        public void SetPauseState(bool paused)
        {
            this.paused = paused;
            ui.PauseMenu.Visible = paused;

            if(paused)
            {
                ui.ResumeBtn.IsFocused = true;
            }
            else
            {
                InteractiveGue.CurrentInputReceiver = null;
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

        public override void DrawDebug(GameTime gameTime)
        {
            if (debugPaused)
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
