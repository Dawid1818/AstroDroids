using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.Input;
using Gum.Managers;
using Gum.Wireframe;
using RenderingLibrary.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AstroDroids.Components.Custom
{
    partial class HorizontalList : IInputReceiver
    {
        int selectedIndex = 0;
        List<string> Items = new List<string>();

        public bool LocalizeText { get { return ItemLabel.LocalizeText; } set { ItemLabel.LocalizeText = value; }  }

        public Action SelectionChanged;

        public int SelectedIndex 
        { 
            get { return selectedIndex; } 
            set 
            { 
                if(value < 0) 
                    selectedIndex = 0;
                else if (value >= Items.Count)
                    selectedIndex = Items.Count - 1;
                else
                {
                    selectedIndex = value;
                }
                UpdateDisplay();
            }
        }
        public IInputReceiver ParentInputReceiver => this.GetParentInputReceiver();

        void UpdateDisplay()
        {
            if (Items.Count == 0)
            {
                ItemLabel.Text = string.Empty;
            }
            else
            {
                if (selectedIndex >= 0 && selectedIndex < Items.Count)
                {
                    ItemLabel.Text = Items[selectedIndex];
                }
                else
                {
                    ItemLabel.Text = string.Empty;
                }
            }
        }

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

                var valueBeforeGamepad = selectedIndex;

                if (gamepad.ButtonRepeatRate(GamepadButton.DPadLeft) ||
                    gamepad.LeftStick.AsDPadPushedRepeatRate(DPadDirection.Left))
                {
                    this.SelectedIndex -= 1;
                }
                else if (gamepad.ButtonRepeatRate(GamepadButton.DPadRight) ||
                    gamepad.LeftStick.AsDPadPushedRepeatRate(DPadDirection.Right))
                {
                    this.SelectedIndex += 1;
                }

                if (valueBeforeGamepad != this.SelectedIndex)
                {
                    SelectionChanged?.Invoke();
                }
            }

            foreach (var keyboard in KeyboardsForUiControl)
            {
                var valueBeforeKeyboard = selectedIndex;

                if (keyboard.KeyTyped(Gum.Forms.Input.Keys.Right) == true)
                {
                    this.SelectedIndex += 1;
                }
                if (keyboard.KeyTyped(Gum.Forms.Input.Keys.Left) == true)
                {
                    this.SelectedIndex -= 1;
                }

                if (valueBeforeKeyboard != selectedIndex)
                {
                    SelectionChanged?.Invoke();
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
        
        }

        public void AddItem(string item)
        {
            Items.Add(item);
            UpdateDisplay();
        }

        public void ClearItems()
        {
            Items.Clear();
            selectedIndex = 0;
            UpdateDisplay();
        }

        public void RemoveItem(string item)
        {
            Items.Remove(item);
            if (selectedIndex >= Items.Count)
            {
                selectedIndex = Items.Count - 1;
            }
            UpdateDisplay();
        }

        public void RemoveItemAt(int index)
        {
            if (index >= 0 && index < Items.Count)
            {
                Items.RemoveAt(index);
                if (selectedIndex >= Items.Count)
                {
                    selectedIndex = Items.Count - 1;
                }
                UpdateDisplay();
            }
        }
    }
}
