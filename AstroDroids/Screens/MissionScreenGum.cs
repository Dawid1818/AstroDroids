using AstroDroids.Components.Custom;
using AstroDroids.Gameplay;
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
using System;
using System.Collections.Generic;

namespace AstroDroids.Screens
{
    partial class MissionScreenGum : IMenuPage
    {
        MainMenuScene scene;

        List<LevelCard> cards = new List<LevelCard>();

        int selectedLevel = 0;

        public void BackPressed()
        {
            Return();
        }

        public void Initialize(MainMenuScene scene, HintedScreenGum hinted)
        {
            this.scene = scene;

            ReturnBtn.Click += ReturnBtn_Click;
            PlayBtn.Click += PlayBtn_Click;
            PrevLevelBtn.Click += PrevLevelBtn_Click;
            NextLevelBtn.Click += NextLevelBtn_Click;

            ReturnBtn.SpatialNavigationUp = PrevLevelBtn;
            PlayBtn.SpatialNavigationUp = NextLevelBtn;
            PrevLevelBtn.SpatialNavigationRight = NextLevelBtn;
            PrevLevelBtn.SpatialNavigationDown = ReturnBtn;
            NextLevelBtn.SpatialNavigationLeft = PrevLevelBtn;
            NextLevelBtn.SpatialNavigationDown = PlayBtn;

            GamepadNavigationMode = Gum.Forms.Controls.GamepadNavigationMode.Spatial;

            Level1Card.TextInstance.Text = "T_Level1";
            Level2Card.TextInstance.Text = "T_Level2";
            Level3Card.TextInstance.Text = "T_Level3";
            Level4Card.TextInstance.Text = "T_Level4";
            Level5Card.TextInstance.Text = "T_Level5";

            cards.Add(Level1Card);
            cards.Add(Level2Card);
            cards.Add(Level3Card);
            cards.Add(Level4Card);
            cards.Add(Level5Card);

            ReturnBtn.IsFocused = true;
        }

        private void NextLevelBtn_Click(object sender, System.EventArgs e)
        {
            selectedLevel++;

            if (selectedLevel > 1)
                selectedLevel = 1;
        }

        private void PrevLevelBtn_Click(object sender, System.EventArgs e)
        {
            selectedLevel--;

            if (selectedLevel < 0)
                selectedLevel = 0;
        }

        public void Update(GameTime gameTime)
        {
            float lerpSpeed = 10f;

            for (int i = 0; i < cards.Count; i++)
            {
                LevelCard card = cards[i];

                int targetX = 220 * (i - selectedLevel);

                card.X = MathHelper.Lerp(card.X, targetX, lerpSpeed * gameTime.GetElapsedSeconds());
            }
        }

        private void PlayBtn_Click(object sender, System.EventArgs e)
        {
            GameStateManager.NewState(GameDatabase.GetMission(MissionType.Story), selectedLevel);
            scene.TransitionToScene(new GameScene());
        }

        private void ReturnBtn_Click(object sender, System.EventArgs e)
        {
            Return();
        }

        public bool TransitionFinished()
        {
            return true;
        }

        public void TransitionOut()
        {

        }

        public void Uninitialize()
        {

        }

        partial void CustomInitialize()
        {
        
        }

        void Return()
        {
            scene.SetPage(new MainMenuScreenGum());
        }
    }
}
