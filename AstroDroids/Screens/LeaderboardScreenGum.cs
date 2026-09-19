using AstroDroids.Components.Custom;
using AstroDroids.Components.Elements;
using AstroDroids.Interfaces;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using RenderingLibrary.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace AstroDroids.Screens
{
    partial class LeaderboardScreenGum : IMenuPage
    {
        IPageHost scene;
        List<LeaderboardItem> items = new List<LeaderboardItem>();

        bool entering = true;
        int element = 0;

        float timer = 0f;

        public bool UpdateWhenTransitioning => true;

        public void Initialize(IPageHost scene, HintedScreenGum hinted)
        {
            this.scene = scene;

            ReturnBtn.Click += ReturnBtn_Click;

            hinted.AddHint("T_Navigate", Icon2.IconCategory.ArrowKeys, Icon2.IconCategory.ControllerLeftJoystick, Icon2.IconCategory.MouseNMB);
            hinted.AddHint("T_Select", Icon2.IconCategory.ZKey, Icon2.IconCategory.ControllerA, Icon2.IconCategory.MouseLMB);
            hinted.AddHint("T_Return", Icon2.IconCategory.XKey, Icon2.IconCategory.ControllerB, Icon2.IconCategory.MouseRMB);

            GamepadNavigationMode = Gum.Forms.Controls.GamepadNavigationMode.Spatial;
        }

        public void Update(GameTime gameTime)
        {
            if(element < SaveManager.curSave.Scores.Count)
            {
                timer -= gameTime.GetElapsedSeconds();

                if(timer <= 0)
                {
                    timer = 0.2f;

                    if (entering)
                    {
                        var entry = new LeaderboardItem();
                        entry.Setup(element + 1, SaveManager.curSave.Scores[element]);
                        entry.Visual.PlayAnimation(entry.SlideIn);
                        LeaderboardItems.AddChild(entry);
                        items.Add(entry);
                    }
                    else
                    {
                        if(element < items.Count)
                            items[element].Visual.PlayAnimation(items[element].SlideOutRight);
                    }

                    element++;
                }
            }
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
            element = 0;
            entering = false;
            timer = 0f;
            Visual.PlayAnimation(Leave);
        }

        public bool TransitionFinished()
        {
            if (entering)
                return Visual.AnimationController.IsStopped;
            else
                return Visual.AnimationController.IsStopped && element == SaveManager.curSave.Scores.Count && items.All(x => x.Visual.AnimationController.IsStopped);
        }

        private void ReturnBtn_Click(object sender, System.EventArgs e)
        {
            scene.SetPage(new MainMenuScreenGum(), false);
        }

        partial void CustomInitialize()
        {
        
        }

        public void BackPressed()
        {
            scene.SetPage(new MainMenuScreenGum(), false);
        }
    }
}
