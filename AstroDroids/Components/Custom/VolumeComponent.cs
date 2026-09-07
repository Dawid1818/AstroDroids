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
        partial void CustomInitialize()
        {
            VolumeSlider.ValueChangedByUi += VolumeSlider_ValueChangedByUi;
        }

        private void VolumeSlider_ValueChangedByUi(object sender, EventArgs e)
        {
            ValueLabel.Text = ((int)VolumeSlider.Value).ToString() + "%";
        }
    }
}
