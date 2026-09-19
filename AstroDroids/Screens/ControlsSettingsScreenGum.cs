using AstroDroids.Components.Elements;
using AstroDroids.Interfaces;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using System;

namespace AstroDroids.Screens
{
    partial class ControlsSettingsScreenGum : IMenuPage
    {
        IPageHost scene;
        public bool UpdateWhenTransitioning => false;
        public void Initialize(IPageHost scene, HintedScreenGum hinted)
        {
            this.scene = scene;
            BackBtn.Click += ReturnBtn_Click;
            KeyboardBtn.Click += KeyboardBtn_Click;
            MouseBtn.Click += MouseBtn_Click;
            GamepadBtn.Click += GamepadBtn_Click;

            BackBtn.X = -600;
            KeyboardBtn.X = -600;
            MouseBtn.X = -600;
            GamepadBtn.X = -600;

            hinted.AddHint("T_Navigate", Icon2.IconCategory.ArrowKeys, Icon2.IconCategory.ControllerLeftJoystick, Icon2.IconCategory.MouseNMB);
            hinted.AddHint("T_Select", Icon2.IconCategory.ZKey, Icon2.IconCategory.ControllerA, Icon2.IconCategory.MouseLMB);
            hinted.AddHint("T_Return", Icon2.IconCategory.XKey, Icon2.IconCategory.ControllerB, Icon2.IconCategory.MouseRMB);

            GamepadNavigationMode = Gum.Forms.Controls.GamepadNavigationMode.Spatial;
        }

        private void GamepadBtn_Click(object sender, EventArgs e)
        {
            scene.SetPage(new RebindingSettingsScreenGum(false), false);
        }

        private void MouseBtn_Click(object sender, EventArgs e)
        {

        }

        private void KeyboardBtn_Click(object sender, EventArgs e)
        {
            scene.SetPage(new RebindingSettingsScreenGum(true), false);
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Uninitialize()
        {

        }

        private void AnimationController_OnCompleted()
        {
            KeyboardBtn.IsFocused = true;
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
            //SettingsManager.Save();
            scene.SetPage(new SettingsScreenGum(), false);
        }

        partial void CustomInitialize()
        {

        }

        public void BackPressed()
        {
            //SettingsManager.Save();
            scene.SetPage(new SettingsScreenGum(), false);
        }
    }
}
