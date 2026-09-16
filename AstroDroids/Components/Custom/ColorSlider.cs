using AstroDroids.Managers;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;
using System;

namespace AstroDroids.Components.Custom
{
    partial class ColorSlider
    {
        partial void CustomInitialize()
        {
            ThumbInstance.FocusUpdate += ThumbInstance_FocusUpdate;

            GotFocus += (not, used) =>
            {
                SoundManager.PlaySound(Sounds.UI_ButtonFocus);
            };
        }

        private void ThumbInstance_FocusUpdate(IInputReceiver obj)
        {
            OnFocusUpdate();
        }
    }
}
