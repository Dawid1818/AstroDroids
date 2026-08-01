using AstroDroids.Components.Elements;
using AstroDroids.Gameplay;
using AstroDroids.Interfaces;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using System;

namespace AstroDroids.Screens
{
    partial class GamemodeScreenGum : IMenuPage
    {
        MainMenuScene scene;
        public void Initialize(MainMenuScene scene, HintedScreenGum hinted)
        {
            this.scene = scene;

            ReturnBtn.Click += ReturnBtn_Click;
            BossRushBtn.Click += BossRushBtn_Click;

            hinted.AddHint("T_Navigate", Icon2.IconCategory.ArrowKeys, Icon2.IconCategory.ControllerLeftJoystick, Icon2.IconCategory.MouseNMB);
            hinted.AddHint("T_Select", Icon2.IconCategory.ZKey, Icon2.IconCategory.ControllerA, Icon2.IconCategory.MouseLMB);
            hinted.AddHint("T_Return", Icon2.IconCategory.XKey, Icon2.IconCategory.ControllerB, Icon2.IconCategory.MouseRMB);

            Visual.PlayAnimation(Enter);
            Visual.AnimationController.OnCompleted += AnimationController_OnCompleted;
        }

        private void BossRushBtn_Click(object sender, EventArgs e)
        {
            GameStateManager.NewState(GameDatabase.GetMission(MissionType.BossRush));

            scene.TransitionToScene(new GameScene());
        }

        private void AnimationController_OnCompleted()
        {
            ReturnBtn.IsFocused = true;
            Visual.AnimationController.OnCompleted -= AnimationController_OnCompleted;
        }

        public void Uninitialize()
        {

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
            scene.SetPage(new MainMenuScreenGum());
        }

        partial void CustomInitialize()
        {

        }

        public void BackPressed()
        {
            scene.SetPage(new MainMenuScreenGum());
        }
    }
}
