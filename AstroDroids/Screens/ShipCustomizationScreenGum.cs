using AstroDroids.Components.Controls;
using AstroDroids.Data;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using RenderingLibrary.Graphics;
using System;

namespace AstroDroids.Screens
{
    partial class ShipCustomizationScreenGum
    {
        public event Action<int> PartChanged;
        public event Action<float, float, float> ColorChanged;

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

            RSlider.ValueChangedByUi += Slider_ValueChangedByUi;
            GSlider.ValueChangedByUi += Slider_ValueChangedByUi;
            BSlider.ValueChangedByUi += Slider_ValueChangedByUi;

            BodyBtn.Click += PartBtn_Click;
            WeaponsBtn.Click += PartBtn_Click;
            EnginesBtn.Click += PartBtn_Click;
            CockpitBtn.Click += PartBtn_Click;
            CockpitGlassBtn.Click += PartBtn_Click;
            WingsBtn.Click += PartBtn_Click;
        }

        private void Slider_ValueChangedByUi(object sender, EventArgs e)
        {
            RSlider.Value = (int)RSlider.Value;
            GSlider.Value = (int)GSlider.Value;
            BSlider.Value = (int)BSlider.Value;

            ColorChanged?.Invoke((float)RSlider.Value, (float)GSlider.Value / 100, (float)BSlider.Value / 100);

            RLabel.Text = $"H: {(int)RSlider.Value}";
            GLabel.Text = $"S: {(int)GSlider.Value}";
            BLabel.Text = $"V: {(int)BSlider.Value}";
        }

        private void PartBtn_Click(object sender, System.EventArgs e)
        {
            ButtonStandard s = sender as ButtonStandard;
            int tag = (int)s.Visual.Tag;

            PartChanged?.Invoke(tag);
        }

        public void SetRGB(ShipColor color)
        {
            RSlider.Value = color.Hue;
            GSlider.Value = color.Saturation * 100f;
            BSlider.Value = color.Value * 100f;

            RLabel.Text = $"H: {(int)RSlider.Value}";
            GLabel.Text = $"S: {(int)GSlider.Value}";
            BLabel.Text = $"V: {(int)BSlider.Value}";
        }
    }
}
