using AstroDroids.Components.Custom;
using AstroDroids.Components.Elements;
using AstroDroids.Input;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;
using System.Collections.Generic;

namespace AstroDroids.Screens
{
    public partial class HintedScreenGum
    {
        List<ActionHint> hints = new List<ActionHint>();

        partial void CustomInitialize()
        {
            ClearHints();
        }

        public void AddHint(string stringID, Icon2.IconCategory keyboardIcon, Icon2.IconCategory gamepadIcon, Icon2.IconCategory mouseIcon)
        {
            ActionHint hint = new ActionHint();
            hint.Setup(stringID, keyboardIcon, gamepadIcon, mouseIcon);
            hints.Add(hint);
            ActionHintsPanel.AddChild(hint);
        }

        public void ClearHints()
        {
            ActionHintsPanel.Children.Clear();
            hints.Clear();
        }

        public void InputMethodChanged(InputMethod method)
        {
            foreach (var item in hints)
            {
                item.InputMethodChanged(method);
            }
        }
    }
}
