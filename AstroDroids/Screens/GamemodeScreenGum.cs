using AstroDroids.Components.Elements;
using AstroDroids.Interfaces;
using AstroDroids.Scenes;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

namespace AstroDroids.Screens
{
    partial class GamemodeScreenGum : IMenuPage
    {
        MainMenuScene scene;
        public void Initialize(MainMenuScene scene, HintedScreenGum hinted)
        {
            this.scene = scene;
            ReturnBtn.IsFocused = true;

            ReturnBtn.Click += ReturnBtn_Click;

            hinted.AddHint("T_Navigate", Icon2.IconCategory.ArrowKeys, Icon2.IconCategory.ControllerLeftJoystick, Icon2.IconCategory.MouseNMB);
            hinted.AddHint("T_Select", Icon2.IconCategory.ZKey, Icon2.IconCategory.ControllerA, Icon2.IconCategory.MouseLMB);
            hinted.AddHint("T_Return", Icon2.IconCategory.XKey, Icon2.IconCategory.ControllerB, Icon2.IconCategory.MouseRMB);
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
