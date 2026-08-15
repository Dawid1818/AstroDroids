using AstroDroids.Interfaces;
using AstroDroids.Scenes;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
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

        public void Update(GameTime gameTime)
        {

        }

        public void Uninitialize()
        {

        }

        public void TransitionIn()
        {

        }

        public void TransitionOut()
        {

        }

        public bool TransitionFinished()
        {
            return true;
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
