using AstroDroids.Graphics;
using Gum.Forms;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameGum.Input;
using System;
using System.Collections.Generic;

namespace AstroDroids.Input
{
    public class InputSystem
    {
        static KeyboardState kState;
        static MouseState mState;

        static KeyboardState oldKState;
        static MouseState oldMState;

        static GamePadState gState;
        static GamePadState oldGState;

        static Dictionary<GameAction, ButtonInputAction> Actions;

        static int scroll = 0;
        static int oldScroll = 0;

        static InputMethod lastInputMethod = InputMethod.Keyboard;

        static Cursor defaultCursor;
        static DisabledCursor disabledCursor;

        public static void Initialize()
        {
            Actions = new Dictionary<GameAction, ButtonInputAction>
            {
                { GameAction.Up, new ButtonInputAction(Keys.Up) },
                { GameAction.Down, new ButtonInputAction(Keys.Down) },
                { GameAction.Left, new ButtonInputAction(Keys.Left) },
                { GameAction.Right, new ButtonInputAction(Keys.Right) },
                { GameAction.Fire, new ButtonInputAction(Keys.Z) },
                { GameAction.NextWeapon, new ButtonInputAction(Keys.X) },
                { GameAction.Focus, new ButtonInputAction(Keys.C) },
            };

            defaultCursor = new Cursor(AstroDroidsGame.Instance.Window);
            disabledCursor = new DisabledCursor();
        }

        public static void Begin()
        {
            kState = Keyboard.GetState();
            mState = Mouse.GetState();
            gState = GamePad.GetState(0);

            scroll = mState.ScrollWheelValue;

            if (GetAnyKey())
            {
                lastInputMethod = InputMethod.Keyboard;
            }

            if (GetLMB() || GetRMB() || mState.X != oldMState.X || mState.Y != oldMState.Y)
            {
                lastInputMethod = InputMethod.Mouse;
            }

            if (GamePadInputChanged())
            {
                lastInputMethod = InputMethod.Gamepad;
            }
        }

        static bool GamePadInputChanged()
        {
            return gState.IsConnected && gState.PacketNumber != oldGState.PacketNumber;
        }

        public static void End()
        {
            oldKState = kState;
            oldMState = mState;
            oldGState = gState;

            oldScroll = scroll;
        }

        public static int GetScrollDelta()
        {
            return scroll - oldScroll;
        }

        public static bool IsActionHeld(GameAction action)
        {
            if (Actions.TryGetValue(action, out ButtonInputAction inputAction))
            {
                if (kState.IsKeyDown(inputAction.KeyboardKey))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsActionDown(GameAction action)
        {
            if (Actions.TryGetValue(action, out ButtonInputAction inputAction))
            {
                if ((kState.IsKeyDown(inputAction.KeyboardKey) && oldKState.IsKeyUp(inputAction.KeyboardKey)))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsActionUp(GameAction action)
        {
            if (Actions.TryGetValue(action, out ButtonInputAction inputAction))
            {
                if ((kState.IsKeyUp(inputAction.KeyboardKey) && oldKState.IsKeyDown(inputAction.KeyboardKey)))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool GetKey(Keys key)
        {
            return kState.IsKeyDown(key);
        }

        public static bool GetKeyDown(Keys key)
        {
            return kState.IsKeyDown(key) && oldKState.IsKeyUp(key);
        }

        public static bool GetKeyUp(Keys key)
        {
            return kState.IsKeyUp(key) && oldKState.IsKeyDown(key);
        }

        public static bool GetAnyKey()
        {
            return kState.GetPressedKeyCount() > 0;
        }

        public static bool GetButtonDown(Buttons button)
        {
            return gState.IsConnected && gState.IsButtonDown(button) && oldGState.IsButtonUp(button);
        }

        public static bool GetButtonUp(Buttons button)
        {
            return gState.IsConnected && gState.IsButtonUp(button) && oldGState.IsButtonDown(button);
        }

        public static bool GetButton(Buttons button)
        {
            return gState.IsConnected && gState.IsButtonDown(button);
        }

        public static Vector2 GetMousePos()
        {
            return new Vector2(mState.X, mState.Y);
        }

        public static bool GetLMB()
        {
            return mState.LeftButton == ButtonState.Pressed;
        }

        public static bool GetLMBDown()
        {
            return mState.LeftButton == ButtonState.Pressed && oldMState.LeftButton == ButtonState.Released;
        }

        public static bool GetLMBUp()
        {
            return mState.LeftButton == ButtonState.Released && oldMState.LeftButton == ButtonState.Pressed;
        }

        public static bool GetRMB()
        {
            return mState.RightButton == ButtonState.Pressed;
        }

        public static bool GetRMBDown()
        {
            return mState.RightButton == ButtonState.Pressed && oldMState.RightButton == ButtonState.Released;
        }

        public static bool GetRMBUp()
        {
            return mState.RightButton == ButtonState.Released && oldMState.RightButton == ButtonState.Pressed;
        }

        public static bool GetMMB()
        {
            return mState.MiddleButton == ButtonState.Pressed;
        }

        public static bool GetMMBDown()
        {
            return mState.MiddleButton == ButtonState.Pressed && oldMState.MiddleButton == ButtonState.Released;
        }

        public static bool GetMMBUp()
        {
            return mState.MiddleButton == ButtonState.Released && oldMState.MiddleButton == ButtonState.Pressed;
        }

        public static InputMethod GetLastInputMethod()
        {
            return lastInputMethod;
        }

        internal static void ClearUIKeys()
        {
            FrameworkElement.ClickCombos.Clear();
            FrameworkElement.TabKeyCombos.Clear();
            FrameworkElement.TabReverseKeyCombos.Clear();
        }

        internal static void AddUIKeys()
        {
            FrameworkElement.ClickCombos.Add(new KeyCombo() { PushedKey = Gum.Forms.Input.Keys.Z, HeldKey = null, IsTriggeredOnRepeat = false });
            FrameworkElement.TabKeyCombos.Add(new KeyCombo() { PushedKey = Gum.Forms.Input.Keys.Down, HeldKey = null, IsTriggeredOnRepeat = true });
            FrameworkElement.TabReverseKeyCombos.Add(new KeyCombo() { PushedKey = Gum.Forms.Input.Keys.Up, HeldKey = null, IsTriggeredOnRepeat = true });
        }

        internal static void DisableUIMouse()
        {
            FormsUtilities.SetCursor(disabledCursor);
        }

        internal static void EnableUIMouse()
        {
            FormsUtilities.SetCursor(defaultCursor);
        }
    }
}