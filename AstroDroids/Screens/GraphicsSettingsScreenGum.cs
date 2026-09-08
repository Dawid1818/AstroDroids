using AstroDroids.Components.Elements;
using AstroDroids.Data;
using AstroDroids.Interfaces;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace AstroDroids.Screens
{
    partial class GraphicsSettingsScreenGum : IMenuPage
    {
        MainMenuScene scene;
        List<DisplayMode> modes = new List<DisplayMode>();
        public void Initialize(MainMenuScene scene, HintedScreenGum hinted)
        {
            this.scene = scene;
            BackBtn.Click += ReturnBtn_Click;

            BackBtn.X = -600;
            ResolutionList.X = -600;
            VideoModeList.X = -600;
            VSyncBox.X = -600;

            hinted.AddHint("T_Navigate", Icon2.IconCategory.ArrowKeys, Icon2.IconCategory.ControllerLeftJoystick, Icon2.IconCategory.MouseNMB);
            hinted.AddHint("T_Select", Icon2.IconCategory.ZKey, Icon2.IconCategory.ControllerA, Icon2.IconCategory.MouseLMB);
            hinted.AddHint("T_Return", Icon2.IconCategory.XKey, Icon2.IconCategory.ControllerB, Icon2.IconCategory.MouseRMB);

            GamepadNavigationMode = Gum.Forms.Controls.GamepadNavigationMode.Spatial;

            modes = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.ToList();

            VideoModeList.LocalizeText = true;
            VideoModeList.AddItem("T_Windowed");
            VideoModeList.AddItem("T_BorderlessFullscreen");
            VideoModeList.AddItem("T_Fullscreen");

            VideoModeList.SelectedIndex = (int)SettingsManager.curSettings.Video.DisplayMode;
            VideoModeList.SelectionChanged += OnDisplayModeSelectionChanged;

            ResolutionList.LocalizeText = false;
            foreach (var mode in modes)
            {
                string resolutionText = $"{mode.Width}x{mode.Height}";
                ResolutionList.AddItem(resolutionText);
            }

            var curRes = SettingsManager.curSettings.Video.Resolution;
            int matchingIndex = modes.FindIndex(m => m.Width == curRes.X && m.Height == curRes.Y);
            ResolutionList.SelectedIndex = matchingIndex >= 0 ? matchingIndex : 0;

            ResolutionList.SelectionChanged += OnResolutionSelectionChanged;

            VSyncBox.IsChecked = SettingsManager.curSettings.Video.VSync;
            VSyncBox.Checked += VSyncBox_Checked;
            VSyncBox.Unchecked += VSyncBox_Checked;
        }

        private void VSyncBox_Checked(object sender, System.EventArgs e)
        {
            SettingsManager.curSettings.Video.VSync = (bool)VSyncBox.IsChecked;
        }

        private void OnResolutionSelectionChanged()
        {
            if (ResolutionList.SelectedIndex >= 0 && ResolutionList.SelectedIndex < modes.Count)
            {
                var selectedMode = modes[ResolutionList.SelectedIndex];
                SettingsManager.curSettings.Video.Resolution = new Point(selectedMode.Width, selectedMode.Height);
            }
        }

        private void OnDisplayModeSelectionChanged()
        {
            if (VideoModeList.SelectedIndex >= 0)
            {
                SettingsManager.curSettings.Video.DisplayMode = (DisplayModeType)VideoModeList.SelectedIndex;

                ResolutionList.IsEnabled = SettingsManager.curSettings.Video.DisplayMode != DisplayModeType.Borderless;
            }
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Uninitialize()
        {

        }

        private void AnimationController_OnCompleted()
        {
            VideoModeList.IsFocused = true;
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
            SettingsManager.ApplyVideoSettings();
            SettingsManager.Save();
            scene.SetPage(new SettingsScreenGum(), false);
        }

        partial void CustomInitialize()
        {

        }

        public void BackPressed()
        {
            SettingsManager.ApplyVideoSettings();
            SettingsManager.Save();
            scene.SetPage(new SettingsScreenGum(), false);
        }
    }
}
