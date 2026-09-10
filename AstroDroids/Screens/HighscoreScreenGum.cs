using AstroDroids.Components.Elements;
using AstroDroids.Data;
using AstroDroids.Gameplay;
using AstroDroids.Interfaces;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using System;

namespace AstroDroids.Screens
{
    partial class HighscoreScreenGum : IMenuPage
    {
        MainMenuScene scene;

        public bool UpdateWhenTransitioning => true;

        string playerName = string.Empty;

        MissionProgress progress;
        public void Initialize(MainMenuScene scene, HintedScreenGum hinted)
        {
            this.scene = scene;

            ReturnBtn.Click += ReturnBtn_Click;

            GlowKeyboard.KeyPressed += GlowKeyboard_KeyPressed;
            GlowKeyboard.BackspacePressed += GlowKeyboard_BackspacePressed;
            GlowKeyboard.LeftPressed += GlowKeyboard_LeftPressed;
            GlowKeyboard.RightPressed += GlowKeyboard_RightPressed;
            GlowKeyboard.ResumePressed += GlowKeyboard_ResumePressed;

            ReturnBtn.X = -600;

            NameLabel.SetTextNoTranslate(playerName);

            GamepadNavigationMode = Gum.Forms.Controls.GamepadNavigationMode.Spatial;

            hinted.AddHint("T_Navigate", Icon2.IconCategory.ArrowKeys, Icon2.IconCategory.ControllerLeftJoystick, Icon2.IconCategory.MouseNMB);
            hinted.AddHint("T_Select", Icon2.IconCategory.ZKey, Icon2.IconCategory.ControllerA, Icon2.IconCategory.MouseLMB);
            hinted.AddHint("T_Return", Icon2.IconCategory.XKey, Icon2.IconCategory.ControllerB, Icon2.IconCategory.MouseRMB);
        }

        public void Setup(MissionProgress progress)
        {
            this.progress = progress;
            if (progress.Victory)
            {
                ResultLabel.Text = "T_Victory";
            }
            else
            {
                ResultLabel.Text = "T_Defeat";
            }

            ScoreDisplay.SetTextNoTranslate(progress.Score.ToString());

            playerName = SaveManager.curSave.PlayerName;
            NameLabel.SetTextNoTranslate(playerName);
        }

        private void GlowKeyboard_ResumePressed()
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                return;
            }

            SaveManager.curSave.PlayerName = playerName;

            SaveManager.curSave.Scores.Add(new ScoreEntry()
            {
                Name = playerName,
                Score = progress.Score,
                Victory = progress.Victory
            });

            SaveManager.curSave.Scores.Sort((a, b) => b.Score.CompareTo(a.Score));
            while (SaveManager.curSave.Scores.Count > 10)
            {
                SaveManager.curSave.Scores.RemoveAt(SaveManager.curSave.Scores.Count - 1);
            }

            SaveManager.SaveGame();

            scene.SetPage(new LeaderboardScreenGum(), true);
        }

        private void GlowKeyboard_RightPressed()
        {

        }

        private void GlowKeyboard_LeftPressed()
        {

        }

        private void GlowKeyboard_BackspacePressed()
        {
            playerName = playerName.Substring(0, Math.Max(0, playerName.Length - 1));
            NameLabel.SetTextNoTranslate(playerName);
        }

        private void GlowKeyboard_KeyPressed(string key)
        {
            if ((key == " " && playerName.Length == 0) || playerName.Length >= 16)
            {
                return;
            }
            playerName += key;
            NameLabel.SetTextNoTranslate(playerName);
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Uninitialize()
        {

        }

        private void AnimationController_OnCompleted()
        {
            ReturnBtn.IsFocused = true;
            Visual.AnimationController.OnCompleted -= AnimationController_OnCompleted;
        }

        public void TransitionIn()
        {
            Visual.PlayAnimation(Enter);
            Visual.AnimationController.OnCompleted += AnimationController_OnCompleted;
        }

        public void TransitionOut()
        {
            Visual.PlayAnimation(Leave);
        }

        public bool TransitionFinished()
        {
            return Visual.AnimationController.IsStopped;
        }

        private void ReturnBtn_Click(object sender, System.EventArgs e)
        {
            scene.SetPage(new MainMenuScreenGum(), false);
        }

        partial void CustomInitialize()
        {
            GlowKeyboard.KeyParenLeft.SpatialNavigationDown = ReturnBtn;
            GlowKeyboard.KeyParenRight.SpatialNavigationDown = ReturnBtn;
            GlowKeyboard.KeySpace.SpatialNavigationDown = ReturnBtn;
            GlowKeyboard.KeyQuestion.SpatialNavigationDown = ReturnBtn;
            GlowKeyboard.KeyBang.SpatialNavigationDown = ReturnBtn;
            GlowKeyboard.KeyAmpersand.SpatialNavigationDown = ReturnBtn;
            GlowKeyboard.KeyReturn.SpatialNavigationDown = ReturnBtn;
        }

        public void BackPressed()
        {
            scene.SetPage(new MainMenuScreenGum(), false);
        }
    }
}
