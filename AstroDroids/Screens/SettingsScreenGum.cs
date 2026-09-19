using AstroDroids.Components.Elements;
using AstroDroids.Graphics;
using AstroDroids.Interfaces;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using RenderingLibrary.Graphics;
using System;

namespace AstroDroids.Screens
{
    partial class SettingsScreenGum : IMenuPage
    {
        IPageHost scene;
        public bool UpdateWhenTransitioning => false;
        public void Initialize(IPageHost scene, HintedScreenGum hinted)
        {
            this.scene = scene;
            BackBtn.Click += ReturnBtn_Click;
            ControlsBtn.Click += ControlsBtn_Click;
            AudioBtn.Click += AudioBtn_Click;
            GraphicsBtn.Click += GraphicsBtn_Click;

            BackBtn.X = -600;
            ControlsBtn.X = -600;
            AudioBtn.X = -600;
            GraphicsBtn.X = -600;
            LanguageList.X = -600;

            LanguageList.LocalizeText = true;
            LanguageList.AddItem("T_English");
            LanguageList.AddItem("T_Polish");

            LanguageList.SelectionChanged += LanguageList_SelectionChanged;
            LanguageList.SelectedIndex = SettingsManager.curSettings.LanguageId - 1;

            hinted.AddHint("T_Navigate", Icon2.IconCategory.ArrowKeys, Icon2.IconCategory.ControllerLeftJoystick, Icon2.IconCategory.MouseNMB);
            hinted.AddHint("T_Select", Icon2.IconCategory.ZKey, Icon2.IconCategory.ControllerA, Icon2.IconCategory.MouseLMB);
            hinted.AddHint("T_Return", Icon2.IconCategory.XKey, Icon2.IconCategory.ControllerB, Icon2.IconCategory.MouseRMB);

            GamepadNavigationMode = Gum.Forms.Controls.GamepadNavigationMode.Spatial;
        }

        private void LanguageList_SelectionChanged()
        {
            SettingsManager.curSettings.LanguageId = LanguageList.SelectedIndex + 1;
            SettingsManager.ApplyLanguage();
        }

        private void AudioBtn_Click(object sender, EventArgs e)
        {
            scene.SetPage(new AudioSettingsScreenGum(), false);
        }

        private void LanguageBtn_Click(object sender, System.EventArgs e)
        {
            Screen.GumUI.LocalizationService.CurrentLanguage = 1;
        }

        private void GraphicsBtn_Click(object sender, System.EventArgs e)
        {
            scene.SetPage(new GraphicsSettingsScreenGum(), false);
        }

        private void ControlsBtn_Click(object sender, System.EventArgs e)
        {
            scene.SetPage(new ControlsSettingsScreenGum(), false);
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Uninitialize()
        {

        }

        private void AnimationController_OnCompleted()
        {
            ControlsBtn.IsFocused = true;
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
            SettingsManager.Save();
            if (scene is GameScene gs)
            {
                gs.HideHinted();
            }
            else
            {
                scene.SetPage(new MainMenuScreenGum(), false);
            }
        }

        partial void CustomInitialize()
        {
            
        }

        public void BackPressed()
        {
            SettingsManager.Save();

            if (scene is GameScene gs)
            {
                gs.HideHinted();
            }
            else
            {
                scene.SetPage(new MainMenuScreenGum(), false);
            }
        }
    }
}
