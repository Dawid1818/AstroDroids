using AstroDroids.Scenes;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;
using System;

namespace AstroDroids.Screens
{
    partial class GameScreenGum
    {
        GameScene scene;
        internal void Initialize(GameScene gameScene)
        {
            this.scene = gameScene;

            ResumeBtn.Click += ResumeBtn_Click;
            QuitBtn.Click += QuitBtn_Click;
        }

        private void QuitBtn_Click(object sender, EventArgs e)
        {
            scene.SaveAndQuit();
        }

        private void ResumeBtn_Click(object sender, EventArgs e)
        {
            scene.SetPauseState(false);
        }

        partial void CustomInitialize()
        {
        
        }
    }
}
