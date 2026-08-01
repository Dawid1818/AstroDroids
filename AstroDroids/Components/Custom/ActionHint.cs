using AstroDroids.Components.Elements;
using AstroDroids.Input;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using RenderingLibrary.Graphics;

namespace AstroDroids.Components.Custom
{
    public partial class ActionHint
    {
        string stringID = string.Empty;
        Icon2.IconCategory keyboardIcon = Icon2.IconCategory.ZKey;
        Icon2.IconCategory gamepadIcon = Icon2.IconCategory.ControllerA;
        Icon2.IconCategory mouseIcon = Icon2.IconCategory.Mouse;

        partial void CustomInitialize()
        {

        }

        public void Setup(string stringID, Icon2.IconCategory keyboardIcon, Icon2.IconCategory gamepadIcon, Icon2.IconCategory mouseIcon)
        {
            this.stringID = stringID;
            this.keyboardIcon = keyboardIcon;
            this.gamepadIcon = gamepadIcon;
            this.mouseIcon = mouseIcon;

            InputMethodChanged(InputSystem.GetLastInputMethod());
            Label.Text = stringID;
        }

        public void InputMethodChanged(InputMethod method)
        {
            switch (method)
            {
                case InputMethod.Keyboard:
                    ActionIcon.IconCategoryState = keyboardIcon;
                    ActionIconShadow.IconCategoryState = keyboardIcon;
                    break;
                case InputMethod.Mouse:
                    ActionIcon.IconCategoryState = mouseIcon;
                    ActionIconShadow.IconCategoryState = mouseIcon;
                    break;
                case InputMethod.Gamepad:
                    ActionIcon.IconCategoryState = gamepadIcon;
                    ActionIconShadow.IconCategoryState = gamepadIcon;
                    break;
                default:
                    break;
            }
        }
    }
}
