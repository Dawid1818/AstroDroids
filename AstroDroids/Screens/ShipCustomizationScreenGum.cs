using AstroDroids.Components.Controls;
using AstroDroids.Components.Elements;
using AstroDroids.Data;
using AstroDroids.Entities.Friendly;
using AstroDroids.Graphics;
using AstroDroids.Interfaces;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace AstroDroids.Screens
{
    partial class ShipCustomizationScreenGum : IMenuPage
    {
        MainMenuScene scene;
        Player player;

        int curPart = 0;

        private const int TrackWidth = 256;
        private const int TrackHeight = 16;

        Texture2D saturationTrack = new Texture2D(Screen.GetGraphicsManager().GraphicsDevice, TrackWidth, TrackHeight);
        Color[] satColors = new Color[TrackWidth * TrackHeight];

        Texture2D hueTrack = new Texture2D(Screen.GetGraphicsManager().GraphicsDevice, TrackWidth, TrackHeight);
        Color[] hueColors = new Color[TrackWidth * TrackHeight];

        Texture2D valueTrack = new Texture2D(Screen.GetGraphicsManager().GraphicsDevice, TrackWidth, TrackHeight);
        Color[] valueColors = new Color[TrackWidth * TrackHeight];

        partial void CustomInitialize()
        {
            BodyBtn.Visual.Tag = 0;
            WeaponsBtn.Visual.Tag = 1;
            EnginesBtn.Visual.Tag = 2;
            CockpitBtn.Visual.Tag = 3;
            CockpitGlassBtn.Visual.Tag = 4;
            WingsBtn.Visual.Tag = 5;

            RSlider.Maximum = 360;
            GSlider.Maximum = 100;
            BSlider.Maximum = 100;

            SatTrack.Texture = saturationTrack;
            ValTrack.Texture = valueTrack;
            HueTrack.Texture = hueTrack;

            ShipColor targetColor = SaveManager.curSave.Ship.GetColorByPart(0);
            UpdateTracks(targetColor.Hue, targetColor.Saturation, targetColor.Value);
            SetRGB(targetColor);
        }

        void UpdateTracks(float currentHue, float currentSat, float currentValue)
        {
            for (int x = 0; x < TrackWidth; x++)
            {
                float t = x / (float)(TrackWidth - 1);

                float hueAtX = t * 360f;
                Color hueCol = Color.FromHSV(hueAtX, currentSat, currentValue);

                Color satCol = Color.FromHSV(currentHue, t, currentValue);

                Color valCol = Color.FromHSV(currentHue, currentSat, t);

                for (int y = 0; y < TrackHeight; y++)
                {
                    int index = x + (y * TrackWidth);
                    hueColors[index] = hueCol;
                    satColors[index] = satCol;
                    valueColors[index] = valCol;
                }
            }

            if(hueTrack != null)
                hueTrack.SetData(hueColors);
            if(saturationTrack != null)
            saturationTrack.SetData(satColors);
            if(valueTrack != null)
                valueTrack.SetData(valueColors);
        }


        private void Slider_ValueChangedByUi(object sender, EventArgs e)
        {
            RSlider.Value = (int)RSlider.Value;
            GSlider.Value = (int)GSlider.Value;
            BSlider.Value = (int)BSlider.Value;

            SaveManager.curSave.Ship.SetColorByPart(curPart, new ShipColor() { Hue = (float)RSlider.Value, Saturation = (float)GSlider.Value / 100, Value = (float)BSlider.Value / 100 });
            scene.World.GetPlayers()[0].ApplyCustomization(SaveManager.curSave.Ship);
            UpdateTracks((float)RSlider.Value, (float)GSlider.Value / 100, (float)BSlider.Value / 100);

            RLabel.SetTextNoTranslate($"H: {(int)RSlider.Value}");
            GLabel.SetTextNoTranslate($"S: {(int)GSlider.Value}");
            BLabel.SetTextNoTranslate($"V: {(int)BSlider.Value}");
        }

        private void PartBtn_Click(object sender, System.EventArgs e)
        {
            //ButtonGlow s = sender as ButtonGlow;
            //int tag = (int)s.Visual.Tag;

            //curPart = tag;
            //ShipColor targetColor = SaveManager.curSave.Ship.GetColorByPart(curPart);
            //UpdateTracks(targetColor.Hue, targetColor.Saturation, targetColor.Value);
            //SetRGB(targetColor);

            RSlider.IsFocused = true;
        }

        public void SetRGB(ShipColor color)
        {
            RSlider.Value = color.Hue;
            GSlider.Value = color.Saturation * 100f;
            BSlider.Value = color.Value * 100f;

            RLabel.SetTextNoTranslate($"H: {(int)RSlider.Value}");
            GLabel.SetTextNoTranslate($"S: {(int)GSlider.Value}");
            BLabel.SetTextNoTranslate($"V: {(int)BSlider.Value}");
        }

        public void Initialize(MainMenuScene scene, HintedScreenGum hinted)
        {
            this.scene = scene;

            BodyBtn.IsFocused = true;

            player = new Player(0, new Vector2(scene.World.Bounds.Width / 2 - 16, scene.World.Bounds.Height / 2 - 16));
            player.LockMovement = true;
            scene.World.AddPlayer(player);

            RSlider.ValueChangedByUi += Slider_ValueChangedByUi;
            GSlider.ValueChangedByUi += Slider_ValueChangedByUi;
            BSlider.ValueChangedByUi += Slider_ValueChangedByUi;

            BodyBtn.Click += PartBtn_Click;
            WeaponsBtn.Click += PartBtn_Click;
            EnginesBtn.Click += PartBtn_Click;
            CockpitBtn.Click += PartBtn_Click;
            CockpitGlassBtn.Click += PartBtn_Click;
            WingsBtn.Click += PartBtn_Click;
            ReturnBtn.Click += ReturnBtn_Click;

            //BodyBtn.SpatialNavigationUp = BodyBtn;
            BodyBtn.SpatialNavigationRight = BodyBtn;
            WeaponsBtn.SpatialNavigationRight = WeaponsBtn;
            EnginesBtn.SpatialNavigationRight = EnginesBtn;
            CockpitBtn.SpatialNavigationRight = CockpitBtn;
            CockpitGlassBtn.SpatialNavigationRight = CockpitGlassBtn;
            WingsBtn.SpatialNavigationRight = WingsBtn;
            ReturnBtn.SpatialNavigationRight = ReturnBtn;

            hinted.AddHint("T_Navigate", Icon2.IconCategory.ArrowKeys, Icon2.IconCategory.ControllerLeftJoystick, Icon2.IconCategory.MouseNMB);
            hinted.AddHint("T_Select", Icon2.IconCategory.ZKey, Icon2.IconCategory.ControllerA, Icon2.IconCategory.MouseLMB);
            hinted.AddHint("T_Return", Icon2.IconCategory.XKey, Icon2.IconCategory.ControllerB, Icon2.IconCategory.MouseRMB);

            GamepadNavigationMode = Gum.Forms.Controls.GamepadNavigationMode.Spatial;
            //RSlider.SpatialNavigationUp = BodyBtn;

            BodyBtn.GotFocus += BodyBtn_GotFocus;
            WeaponsBtn.GotFocus += BodyBtn_GotFocus;
            EnginesBtn.GotFocus += BodyBtn_GotFocus;
            CockpitBtn.GotFocus += BodyBtn_GotFocus;
            CockpitGlassBtn.GotFocus += BodyBtn_GotFocus;
            WingsBtn.GotFocus += BodyBtn_GotFocus;
        }

        private void BodyBtn_GotFocus(object sender, EventArgs e)
        {
            ButtonGlow s = sender as ButtonGlow;
            int tag = (int)s.Visual.Tag;

            curPart = tag;
            ShipColor targetColor = SaveManager.curSave.Ship.GetColorByPart(curPart);
            UpdateTracks(targetColor.Hue, targetColor.Saturation, targetColor.Value);
            SetRGB(targetColor);
        }

        public void Uninitialize()
        {
            if (saturationTrack != null)
                saturationTrack.Dispose();

            if (hueTrack != null)
                hueTrack.Dispose();

            if (valueTrack != null)
                valueTrack.Dispose();


            scene.World.RemovePlayer(player);
        }

        public void TransitionOut()
        {

        }

        public bool TransitionFinished()
        {
            return true;
        }

        void Return()
        {
            SaveManager.SaveGame();
            scene.SetPage(new MainMenuScreenGum());
        }

        private void ReturnBtn_Click(object sender, EventArgs e)
        {
            Return();
        }

        public void BackPressed()
        {
            if(RSlider.IsFocused || GSlider.IsFocused || BSlider.IsFocused)
            {
                switch (curPart)
                {
                    case 0:
                        BodyBtn.IsFocused = true;
                        break;
                    case 1:
                        WeaponsBtn.IsFocused = true;
                        break;
                    case 2:
                        EnginesBtn.IsFocused = true;
                        break;
                    case 3:
                        CockpitBtn.IsFocused = true;
                        break;
                    case 4:
                        CockpitGlassBtn.IsFocused = true;
                        break;
                    case 5:
                        WingsBtn.IsFocused = true;
                        break;
                    default:
                        BodyBtn.IsFocused = true;
                        break;
                }
                return;
            }

            Return();
        }
    }
}
