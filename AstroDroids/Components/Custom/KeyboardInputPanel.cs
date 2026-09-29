using Gum.Forms.Controls;
using Gum.Forms.Input;
using Gum.Wireframe;
using System;
using System.Collections.Generic;

namespace AstroDroids.Components.Custom
{
    partial class KeyboardInputPanel : IInputReceiver
    {
        public const string CategoryName = "KeyboardInputPanelCategory";
        string lastState = string.Empty;

        public IInputReceiver ParentInputReceiver => this.GetParentInputReceiver();

        Dictionary<(Keys Key, bool Shift), string> allowedKeys = new Dictionary<(Keys Key, bool Shift), string>();

        public Action<string> KeyPressed;
        public Action BackspacePressed;
        public Action LeftPressed;
        public Action RightPressed;
        public Action ResumePressed;

        public void DoKeyboardAction(IInputReceiverKeyboard keyboard)
        {

        }

        public void OnFocusUpdate()
        {
            var gamepads = FrameworkElement.GamePadsForUiControl;

            for (int i = 0; i < gamepads.Count; i++)
            {
                var gamepad = gamepads[i];

                HandleGamepadNavigation(gamepad);
            }

            foreach (var keyboard in KeyboardsForUiControl)
            {
                bool shift = keyboard.IsShiftDown;

                foreach (var key in keyboard.KeysTyped)
                {
                    if (key == Keys.Back)
                    {
                        BackspacePressed?.Invoke();
                        continue;
                    }

                    if (key == Keys.Left)
                    {
                        LeftPressed?.Invoke();
                        continue;
                    }

                    if (key == Keys.Right)
                    {
                        RightPressed?.Invoke();
                        continue;
                    }

                    if (key == Keys.Enter)
                    {
                        ResumePressed?.Invoke();
                        continue;
                    }

                    if (allowedKeys.TryGetValue((key, shift), out string character))
                    {
                        KeyPressed?.Invoke(character);
                    }
                }
            }

            base.HandleKeyboardFocusUpdate();
        }

        public void OnFocusUpdatePreview(RoutedEventArgs args)
        {
        }

        public void OnGainFocus()
        {
            IsFocused = true;
        }

        public void OnLoseFocus()
        {
            IsFocused = false;
        }

        partial void CustomInitialize()
        {
            allowedKeys.Add((Keys.D1, false), "1");
            allowedKeys.Add((Keys.D1, true), "!");

            allowedKeys.Add((Keys.D2, false), "2");

            allowedKeys.Add((Keys.D3, false), "3");

            allowedKeys.Add((Keys.D4, false), "4");

            allowedKeys.Add((Keys.D5, false), "5");

            allowedKeys.Add((Keys.D6, false), "6");

            allowedKeys.Add((Keys.D7, false), "7");
            allowedKeys.Add((Keys.D7, true), "&");

            allowedKeys.Add((Keys.D8, false), "8");

            allowedKeys.Add((Keys.D9, false), "9");
            allowedKeys.Add((Keys.D9, true), "(");

            allowedKeys.Add((Keys.D0, false), "0");
            allowedKeys.Add((Keys.D0, true), ")");

            allowedKeys.Add((Keys.Q, false), "Q");
            allowedKeys.Add((Keys.W, false), "W");
            allowedKeys.Add((Keys.E, false), "E");
            allowedKeys.Add((Keys.R, false), "R");
            allowedKeys.Add((Keys.T, false), "T");
            allowedKeys.Add((Keys.Y, false), "Y");
            allowedKeys.Add((Keys.U, false), "U");
            allowedKeys.Add((Keys.I, false), "I");
            allowedKeys.Add((Keys.O, false), "O");
            allowedKeys.Add((Keys.P, false), "P");

            allowedKeys.Add((Keys.A, false), "A");
            allowedKeys.Add((Keys.S, false), "S");
            allowedKeys.Add((Keys.D, false), "D");
            allowedKeys.Add((Keys.F, false), "F");
            allowedKeys.Add((Keys.G, false), "G");
            allowedKeys.Add((Keys.H, false), "H");
            allowedKeys.Add((Keys.J, false), "J");
            allowedKeys.Add((Keys.K, false), "K");
            allowedKeys.Add((Keys.L, false), "L");

            allowedKeys.Add((Keys.OemMinus, true), "_");

            allowedKeys.Add((Keys.Z, false), "Z");
            allowedKeys.Add((Keys.X, false), "X");
            allowedKeys.Add((Keys.C, false), "C");
            allowedKeys.Add((Keys.V, false), "V");
            allowedKeys.Add((Keys.B, false), "B");
            allowedKeys.Add((Keys.N, false), "N");
            allowedKeys.Add((Keys.M, false), "M");

            allowedKeys.Add((Keys.OemComma, false), ",");
            allowedKeys.Add((Keys.OemPeriod, false), ".");
            allowedKeys.Add((Keys.OemMinus, false), "-");

            allowedKeys.Add((Keys.Space, false), " ");
            allowedKeys.Add((Keys.OemQuestion, true), "?");
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

                Visual.SetProperty(CategoryName + "State", state);
            }

            lastState = state;
        }
    }
}
