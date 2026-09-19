using AstroDroids.Components.Elements;
using AstroDroids.Interfaces;
using Microsoft.Xna.Framework;
using System;

namespace AstroDroids.Screens
{
    partial class MainMenuScreenGum : IMenuPage
    {
        IPageHost scene;
        public bool UpdateWhenTransitioning => false;
        public void Initialize(IPageHost scene, HintedScreenGum hinted)
        {
            this.scene = scene;

            PlayBtn.Click += PlayBtn_Click;
            CustomizeBtn.Click += CustomizeBtn_Click;
            SettingsBtn.Click += SettingsBtn_Click;
            LeaderboardBtn.Click += LeaderboardBtn_Click;
            ExitBtn.Click += ExitBtn_Click;

            //for animation purposes
            PlayBtn.X = -600;
            CustomizeBtn.X = -600;
            SettingsBtn.X = -600;
            LeaderboardBtn.X = -600;
            CreditsBtn.X = -600;
            ExitBtn.X = -600;

            hinted.AddHint("T_Navigate", Icon2.IconCategory.ArrowKeys, Icon2.IconCategory.ControllerLeftJoystick, Icon2.IconCategory.MouseNMB);
            hinted.AddHint("T_Select", Icon2.IconCategory.ZKey, Icon2.IconCategory.ControllerA, Icon2.IconCategory.MouseLMB);

            GamepadNavigationMode = Gum.Forms.Controls.GamepadNavigationMode.Spatial;
        }

        public void Update(GameTime gameTime)
        {

        }

        private void AnimationController_OnCompleted()
        {
            PlayBtn.IsFocused = true;
            Visual.AnimationController.OnCompleted -= AnimationController_OnCompleted;
        }

        public void TransitionOut()
        {
            //AnimationsState = Animations.Arrived
            Visual.PlayAnimation(Leave);
        }

        public void TransitionIn()
        {
            Visual.PlayAnimation(Enter);
            Visual.AnimationController.OnCompleted += AnimationController_OnCompleted;
        }

        public void Uninitialize()
        {

        }

        public bool TransitionFinished()
        {
            return Visual.AnimationController.IsStopped;
        }

        private void PlayBtn_Click(object sender, EventArgs e)
        {
            scene.SetPage(new GamemodeScreenGum(), true);
        }

        private void CustomizeBtn_Click(object sender, EventArgs e)
        {
            scene.SetPage(new ShipCustomizationScreenGum(), true);
        }

        private void SettingsBtn_Click(object sender, EventArgs e)
        {
            scene.SetPage(new SettingsScreenGum(), false);
        }


        private void LeaderboardBtn_Click(object sender, EventArgs e)
        {
            //SoundManager.PlaySound("ShieldOff", AstroDroidsGame.rnd.NextSingle() * 2f);
            scene.SetPage(new LeaderboardScreenGum(), true);
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            scene.TransitionClose();
        }

        partial void CustomInitialize()
        {

        }

        public void BackPressed()
        {
            //scene.SetPage(new MainMenuScreenGum());
        }
    }
}
