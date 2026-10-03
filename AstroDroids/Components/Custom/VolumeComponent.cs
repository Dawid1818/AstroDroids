using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;
using System;

namespace AstroDroids.Components.Custom
{
    partial class VolumeComponent
    {
        public Action ValueChanged;

        partial void CustomInitialize()
        {
            VolumeSlider.ValueChangedByUi += VolumeSlider_ValueChangedByUi;
            VolumeSlider.ThumbInstance.FocusUpdate += ThumbInstance_FocusUpdate;
        }

        public void SetMinMax(float minval, float maxval)
        {
            VolumeSlider.Minimum = minval;
            VolumeSlider.Maximum = maxval;
        }

        private void ThumbInstance_FocusUpdate(IInputReceiver obj)
        {
            VolumeSlider.OnFocusUpdate();
        }

        private void VolumeSlider_ValueChangedByUi(object sender, EventArgs e)
        {
            ValueLabel.Text = ((int)VolumeSlider.Value).ToString() + "%";
            ValueChanged?.Invoke();
        }

        public void SetValue(float volume)
        {
            VolumeSlider.Value = volume;
            VolumeSlider.SliderPercent = (volume / (float)VolumeSlider.Maximum) * 100f;
            ValueLabel.Text = ((int)VolumeSlider.Value).ToString() + "%";
        }

        public float GetValue()
        {
            return (float)VolumeSlider.Value;
        }
    }
}
