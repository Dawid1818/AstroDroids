using AstroDroids.Managers;

namespace AstroDroids.Components.Controls
{
    partial class ButtonIconGlow
    {
        string lastState = string.Empty;

        partial void CustomInitialize()
        {
            Click += (not, used) =>
            {
                SoundManager.PlaySound(Sounds.UI_Accept);
            };

            GotFocus += (not, used) =>
            {
                SoundManager.PlaySound(Sounds.UI_ButtonFocus);
            };
        }

        public override void UpdateState()
        {
            if (Visual.AnimationController.CurrentAnimation != null && Visual.AnimationController.CurrentAnimation.Name == "GlowActive")
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
                if (wasntFocused)
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
