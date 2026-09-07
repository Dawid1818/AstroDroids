using AstroDroids.Components.Elements;
using AstroDroids.Interfaces;
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
    partial class AudioSettingsScreenGum : IMenuPage
    {
        MainMenuScene scene;
        public void Initialize(MainMenuScene scene, HintedScreenGum hinted)
        {
            this.scene = scene;
            BackBtn.Click += ReturnBtn_Click;

            BackBtn.X = -600;
            MusicVolumeControl.X = -600;
            SoundEffectsVolumeControl.X = -600;

            hinted.AddHint("T_Navigate", Icon2.IconCategory.ArrowKeys, Icon2.IconCategory.ControllerLeftJoystick, Icon2.IconCategory.MouseNMB);
            hinted.AddHint("T_Select", Icon2.IconCategory.ZKey, Icon2.IconCategory.ControllerA, Icon2.IconCategory.MouseLMB);
            hinted.AddHint("T_Return", Icon2.IconCategory.XKey, Icon2.IconCategory.ControllerB, Icon2.IconCategory.MouseRMB);

            GamepadNavigationMode = Gum.Forms.Controls.GamepadNavigationMode.Spatial;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Uninitialize()
        {

        }

        private void AnimationController_OnCompleted()
        {
            MusicVolumeControl.IsFocused = true;
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
            scene.SetPage(new SettingsScreenGum());
        }

        partial void CustomInitialize()
        {

        }

        public void BackPressed()
        {
            scene.SetPage(new SettingsScreenGum());
        }
    }
}
