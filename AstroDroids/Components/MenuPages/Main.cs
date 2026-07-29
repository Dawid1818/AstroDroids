using AstroDroids.Graphics;
using AstroDroids.Managers;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

namespace AstroDroids.Components.MenuPages
{
    partial class Main
    {
        partial void CustomInitialize()
        {
            ButtonStandardInstance.Click += ButtonStandardInstance_Click;
        }

        private void ButtonStandardInstance_Click(object sender, System.EventArgs e)
        {
            TransitionManager.SetState(TransitionState.In);
        }
    }
}
