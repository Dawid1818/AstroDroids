using AstroDroids.Managers;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

namespace AstroDroids.Components.Custom
{
    partial class SliderGlow
    {
        partial void CustomInitialize()
        {
            GotFocus += (not, used) =>
            {
                SoundManager.PlaySound(Sounds.UI_ButtonFocus);
            };
        }
    }
}
