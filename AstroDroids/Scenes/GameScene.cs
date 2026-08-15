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
using Microsoft.Xna.Framework.Media;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        bool gameLost = false;
        bool levelFinished = false;

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

            if (World == null)
                World = new GameWorld();

            if (GameStateManager.MissionInitialized())
            {
                if (GameStateManager.GetMissionType() != MissionType.Editor)
                {
                    List<string> levels = GameStateManager.GetLevels();
                    LevelManager.LoadLevel(levels[GameStateManager.GetLevelIndex()]);
                }
            }
            else
            {
                GameStateManager.NewState(new GameMission() { Type = MissionType.Editor });
            }

            World.Initialize();

            if (LevelManager.CurrentLevel != null)
            {
                if (LevelManager.CurrentLevel.BackgroundId == 0)
                {
                    World.Starfield = new SimulationStarfield();
                }
                else
                {
                    List<Texture2D> starfields = TextureManager.GetStarfields();
                    World.Starfield = new ImageStarfield(starfields[LevelManager.CurrentLevel.BackgroundId - 1]);
                }
            }
            else
            {
                List<Texture2D> starfields = TextureManager.GetStarfields();
                World.Starfield = new ImageStarfield(starfields[0]);
            }

            World.AddPlayer(new Player(0, new Vector2(World.Bounds.Width / 2 - 16, World.Bounds.Bottom - 64)));

            LevelManager.StartLevel();

            SoundManager.PlayMusic(GameDatabase.GetMusic(LevelManager.CurrentLevel.MusicId));

            Screen.ResetCamera();

            //coroutineManager.StartCoroutine(LevelManager.GetLevelScript());
        }

        public override void Update(GameTime gameTime)
        {
            if(GameStateManager.GetLives() <= 0 && !gameLost)
            {
                gameLost = true;
                coroutineManager.StartCoroutine(gameOverSequence());
            }

            if (InputSystem.GetKeyDown(Keys.P))
            {
                debugPaused = !debugPaused;
            }

            if((InputSystem.GetKeyDown(Keys.Escape) || InputSystem.GetButtonDown(Buttons.Start)) && !transitioning && !gameLost && !levelFinished)
            {
                SetPauseState(!paused);
            }

            if (!debugPaused)
            {
                coroutineManager.Update(gameTime);

                if (!paused)
                {
                    InputSystem.SetMouseLock(true);

                    World.Update(gameTime);

                    yPos -= (float)gameTime.ElapsedGameTime.TotalSeconds * 50f;
                }
                else
                {
                    InputSystem.SetMouseLock(false);
                }
            }
            else
            {
                InputSystem.SetMouseLock(false);
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

            if (InputSystem.GetKeyDown(Keys.F6))
            {
                GameStateManager.Lives += 1;
                if (GameStateManager.Lives > 99)
                {
                    GameStateManager.Lives = 99;
                }
            }

            if (InputSystem.GetKeyDown(Keys.F7))
            {
                foreach (var item in World.Enemies.ToList())
                {
                    if (item.CanBeDamaged)
                        item.Damage(20000, false);
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
            InputSystem.SetMouseLock(false);

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

        IEnumerator gameOverSequence()
        {
            ui.MissionStatusContainer.Visible = true;
            ui.MissionStatusLabel.Text = "T_GameOver";
            ui.Visual.PlayAnimation(ui.ShowMissionStatus);

            yield return new WaitUntil(() => ui.Visual.AnimationController.IsStopped);

            yield return new WaitForSeconds(2f);

            SaveAndQuit();
        }

        IEnumerator missionFinishedSequence()
        {
            bool nextLevel = false;
            SoundManager.PlayMusic("ZeroRanger - Hyyeeaaaarh", false);
            ui.MissionStatusContainer.Visible = true;

            if (!LevelManager.Playtesting && GameStateManager.GetLevels().Count - 1 > GameStateManager.GetLevelIndex())
            {
                nextLevel = true;
                GameStateManager.IncreaseLevelIndex();
                ui.MissionStatusLabel.Text = "T_LevelComplete";
            }
            else
            {
                ui.MissionStatusLabel.Text = "T_MissionComplete";
            }
            ui.Visual.PlayAnimation(ui.ShowMissionStatus);

            yield return new WaitUntil(() => ui.Visual.AnimationController.IsStopped && SoundManager.CurrentMusic == "ZeroRanger - Hyyeeaaaarh" && (MediaPlayer.State == MediaState.Stopped || MediaPlayer.PlayPosition.Seconds > 18));

            yield return new WaitForSeconds(2f);

            if (!nextLevel)
            {
                SaveAndQuit();
            }
            else
            {
                coroutineManager.StartCoroutine(TransitionToSceneCoroutine(new GameScene()));
            }
        }

        IEnumerator TransitionToSceneCoroutine(Scene scene)
        {
            transitioning = true;
            InputSystem.ClearUIKeys();
            InputSystem.DisableUIMouse();
            InteractiveGue.CurrentInputReceiver = null;

            TransitionManager.SetState(TransitionState.In);

            yield return new WaitUntil(() => TransitionManager.State == TransitionState.Out || TransitionManager.State == TransitionState.Idle);

            Screen.GumUI.Root.Children.Clear();

            SceneManager.SetScene(scene);

            transitioning = false;
            InputSystem.AddUIKeys();
            InputSystem.EnableUIMouse();
        }

        public void FinishLevel()
        {
            if (levelFinished || gameLost)
                return;

            levelFinished = true;
            coroutineManager.StartCoroutine(missionFinishedSequence());
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
