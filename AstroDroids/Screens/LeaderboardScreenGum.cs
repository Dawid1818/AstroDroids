using AstroDroids.Interfaces;
using AstroDroids.Scenes;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

namespace AstroDroids.Screens
{
    partial class LeaderboardScreenGum : IMenuPage
    {
        MainMenuScene scene;
        public void Initialize(MainMenuScene scene, HintedScreenGum hinted)
        {
            this.scene = scene;
            ReturnBtn.IsFocused = true;

            ReturnBtn.Click += ReturnBtn_Click;
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
