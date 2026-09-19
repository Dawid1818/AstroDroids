using AstroDroids.Components.Elements;
using AstroDroids.Gameplay;
using AstroDroids.Interfaces;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using System;

namespace AstroDroids.Screens
{
    partial class ContinueScreenGum : IMenuPage
    {
        IPageHost scene;
        public bool UpdateWhenTransitioning => false;
        public void Initialize(IPageHost scene, HintedScreenGum hinted)
        {
            this.scene = scene;

            BackBtn.Click += BackBtn_Click;
            ContinueBtn.Click += ContinueBtn_Click;
            NewGameBtn.Click += NewGameBtn_Click;

            BackBtn.X = -600;
            ContinueBtn.X = -600;
            NewGameBtn.X = -600;

            hinted.AddHint("T_Navigate", Icon2.IconCategory.ArrowKeys, Icon2.IconCategory.ControllerLeftJoystick, Icon2.IconCategory.MouseNMB);
            hinted.AddHint("T_Select", Icon2.IconCategory.ZKey, Icon2.IconCategory.ControllerA, Icon2.IconCategory.MouseLMB);
            hinted.AddHint("T_Return", Icon2.IconCategory.XKey, Icon2.IconCategory.ControllerB, Icon2.IconCategory.MouseRMB);

            GamepadNavigationMode = Gum.Forms.Controls.GamepadNavigationMode.Spatial;
        }

        private void NewGameBtn_Click(object sender, EventArgs e)
        {
            GameStateManager.ClearState();
            scene.SetPage(new MissionScreenGum(), true);
        }

        private void ContinueBtn_Click(object sender, EventArgs e)
        {
            GameStateManager.LoadState(GameDatabase.GetMission(MissionType.Story));

            scene.TransitionToScene(new GameScene());
        }

        public void Update(GameTime gameTime)
        {

        }

        private void AnimationController_OnCompleted()
        {
            ContinueBtn.IsFocused = true;
            Visual.AnimationController.OnCompleted -= AnimationController_OnCompleted;
        }

        public void Uninitialize()
        {

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

        private void BackBtn_Click(object sender, System.EventArgs e)
        {
            scene.SetPage(new GamemodeScreenGum(), true);
        }

        partial void CustomInitialize()
        {

        }

        public void BackPressed()
        {
            scene.SetPage(new GamemodeScreenGum(), true);
        }
    }
}
