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
            VolumeSlider.Value = volume * 100;
            VolumeSlider.SliderPercent = volume * 100f;
            ValueLabel.Text = ((int)VolumeSlider.Value).ToString() + "%";
        }

        public float GetValue()
        {
            return (float)VolumeSlider.Value / 100f;
        }
    }
}
