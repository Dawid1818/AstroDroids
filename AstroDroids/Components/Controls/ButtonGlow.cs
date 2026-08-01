using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

namespace AstroDroids.Components.Controls
{
    partial class ButtonGlow
    {
        string lastState = string.Empty;

        partial void CustomInitialize()
        {
            //Click += (not, used) =>
            //{
            //    Visual.PlayAnimation(GlowActive);
            //};
        }

        public override void UpdateState()
        {
            if(Visual.AnimationController.CurrentAnimation != null && Visual.AnimationController.CurrentAnimation.Name == "GlowActive")
            {
                return;
            }

            var state = base.GetDesiredState();

            bool isFocused = (state == "Focused" || state == "HighlightedFocused");
            bool wasntFocused = (lastState != "Focused" && lastState != "HighlightedFocused");

            if (state == "Highlighted" || state == "HighlightedFocused")
            {
                if (wasntFocused)
                {
                    Visual.PlayAnimation(GlowFocused);
                    lastState = "Focused";
                }
                return;
            }

            if (isFocused)
            {
                if(wasntFocused)
                    Visual.PlayAnimation(GlowFocused);
            }
            else
            {
                Visual.StopAnimation();

                Visual.SetProperty(ButtonCategoryName + "State", state);
            }

            lastState = state;
        }
    }
}
