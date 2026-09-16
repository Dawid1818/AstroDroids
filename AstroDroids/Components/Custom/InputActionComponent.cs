using AstroDroids.Input;
using AstroDroids.Managers;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.Forms.Data;
using Gum.Input;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;
using System;

namespace AstroDroids.Components.Custom
{
    partial class InputActionComponent : IInputReceiver
    {
        string lastState = string.Empty;
        Action<InputActionComponent> rebindAction;
        public GameAction ActionType { get; private set; }
        public ButtonInputAction ActionBinding { get; private set; }

        public IInputReceiver ParentInputReceiver => this.GetParentInputReceiver();
        public const string CategoryName = "InputActionCategory";

        bool rebinding = false;
        bool rebindedThisFrame = false;
        bool isKeyboard = true;

        partial void CustomInitialize()
        {
            Visual.Click += Visual_Click;

            GotFocus += (not, used) =>
            {
                SoundManager.PlaySound(Sounds.UI_ButtonFocus);
            };
        }

        private void Visual_Click(object sender, EventArgs e)
        {
            SoundManager.PlaySound(Sounds.UI_Accept);
            rebinding = true;
            ItemLabel.SetTextNoTranslate("?");
            rebindAction?.Invoke(this);
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

                //var valueBeforeGamepad = selectedIndex;

                //if (gamepad.ButtonRepeatRate(GamepadButton.DPadLeft) ||
                //    gamepad.LeftStick.AsDPadPushedRepeatRate(DPadDirection.Left))
                //{
                //    this.SelectedIndex -= 1;
                //}
                //else if (gamepad.ButtonRepeatRate(GamepadButton.DPadRight) ||
                //    gamepad.LeftStick.AsDPadPushedRepeatRate(DPadDirection.Right))
                //{
                //    this.SelectedIndex += 1;
                //}

                //if (valueBeforeGamepad != this.SelectedIndex)
                //{
                //    SelectionChanged?.Invoke();
                //}
            }

            //foreach (var keyboard in KeyboardsForUiControl)
            //{
            //    var valueBeforeKeyboard = selectedIndex;

            //    if (keyboard.KeyTyped(Gum.Forms.Input.Keys.Right) == true)
            //    {
            //        this.SelectedIndex += 1;
            //    }
            //    if (keyboard.KeyTyped(Gum.Forms.Input.Keys.Left) == true)
            //    {
            //        this.SelectedIndex -= 1;
            //    }

            //    if (valueBeforeKeyboard != selectedIndex)
            //    {
            //        SelectionChanged?.Invoke();
            //    }
            //}

            if (InputSystem.IsActionDown(GameAction.Fire) && !rebinding && !rebindedThisFrame)
            {
                rebinding = true;
                ItemLabel.SetTextNoTranslate("?");
                rebindAction?.Invoke(this);
                SoundManager.PlaySound(Sounds.UI_Accept);
            }

            rebindedThisFrame = false;

            base.HandleKeyboardFocusUpdate();
        }

        public void StopRebinding()
        {
            rebindedThisFrame = true;
            rebinding = false;
            updateLabel();
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

        internal void Set(Action<InputActionComponent> rebindAction, bool isKeyboard, GameAction type, ButtonInputAction binding)
        {
            this.isKeyboard = isKeyboard;
            this.rebindAction = rebindAction;
            ActionType = type;
            ActionBinding = binding;

            LeftLabel.SetTextNoTranslate(type.ToString());

            updateLabel();
        }

        void updateLabel()
        {
            if(isKeyboard)
                ItemLabel.SetTextNoTranslate($"{ActionBinding.KeyboardKey}");
            else
                ItemLabel.SetTextNoTranslate($"{ActionBinding.GamepadButton}");
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
