using Microsoft.Xna.Framework.Input;

namespace AstroDroids.Input
{
    public class ButtonInputAction
    {
        public Keys KeyboardKey { get; set; }
        public Buttons GamepadButton { get; set; }

        public ButtonInputAction(Keys keyboardKey, Buttons gamepadButton)
        {
            KeyboardKey = keyboardKey;
            GamepadButton = gamepadButton;
        }
    }
}
