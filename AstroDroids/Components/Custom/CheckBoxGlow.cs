using AstroDroids.Managers;

namespace AstroDroids.Components.Custom
{
    partial class CheckBoxGlow
    {
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
            base.UpdateState();
        }
    }
}
