using Microsoft.Xna.Framework.Input;

namespace AstroDroids.Input
{
    public class ButtonInputAction
    {
        public Keys KeyboardKey { get; set; }

        public ButtonInputAction(Keys keyboardKey)
        {
            KeyboardKey = keyboardKey;
        }
    }
}
