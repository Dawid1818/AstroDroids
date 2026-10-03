using AstroDroids.Components.Elements;
using AstroDroids.Interfaces;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;

namespace AstroDroids.Screens
{
    partial class MouseSettingsScreenGum : IMenuPage
    {
        IPageHost scene;
        public bool UpdateWhenTransitioning => false;
        public void Initialize(IPageHost scene, HintedScreenGum hinted)
        {
            this.scene = scene;
            BackBtn.Click += ReturnBtn_Click;

            BackBtn.X = -600;
            MouseSensitivitySlider.X = -600;

            MouseSensitivitySlider.SetMinMax(10, 100);

            MouseSensitivitySlider.SetValue(SettingsManager.curSettings.MouseSensitivty);

            MouseSensitivitySlider.ValueChanged += MouseSensitivityChanged;

            hinted.AddHint("T_Navigate", Icon2.IconCategory.ArrowKeys, Icon2.IconCategory.ControllerLeftJoystick, Icon2.IconCategory.MouseNMB);
            hinted.AddHint("T_Select", Icon2.IconCategory.ZKey, Icon2.IconCategory.ControllerA, Icon2.IconCategory.MouseLMB);
            hinted.AddHint("T_Return", Icon2.IconCategory.XKey, Icon2.IconCategory.ControllerB, Icon2.IconCategory.MouseRMB);

            GamepadNavigationMode = Gum.Forms.Controls.GamepadNavigationMode.Spatial;
        }

        private void MouseSensitivityChanged()
        {
            SettingsManager.curSettings.MouseSensitivty = MouseSensitivitySlider.GetValue();
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Uninitialize()
        {

        }

        private void AnimationController_OnCompleted()
        {
            MouseSensitivitySlider.IsFocused = true;
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
            scene.SetPage(new ControlsSettingsScreenGum(), false);
        }

        partial void CustomInitialize()
        {

        }

        public void BackPressed()
        {
            SettingsManager.Save();
            scene.SetPage(new ControlsSettingsScreenGum(), false);
        }
    }
}
