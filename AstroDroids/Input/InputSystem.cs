using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace AstroDroids.Input
{
    public class InputSystem
    {
        static KeyboardState kState;
        static MouseState mState;

        static KeyboardState oldKState;
        static MouseState oldMState;

        static Dictionary<GameAction, ButtonInputAction> Actions;

        public static void Initialize()
        {
            Actions = new Dictionary<GameAction, ButtonInputAction>
            {
                { GameAction.Up, new ButtonInputAction(Keys.Up) },
                { GameAction.Down, new ButtonInputAction(Keys.Down) },
                { GameAction.Left, new ButtonInputAction(Keys.Left) },
                { GameAction.Right, new ButtonInputAction(Keys.Right) },
                { GameAction.Fire, new ButtonInputAction(Keys.Z) },
            };
        }

        public static void Begin()
        {
            kState = Keyboard.GetState();
            mState = Mouse.GetState();
        }

        public static void End()
        {
            oldKState = kState;
            oldMState = mState;
        }

        public static bool IsActionHeld(GameAction action)
        {
            if(Actions.TryGetValue(action, out ButtonInputAction inputAction))
            {
                if(kState.IsKeyDown(inputAction.KeyboardKey))
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
                if((kState.IsKeyDown(inputAction.KeyboardKey) && oldKState.IsKeyUp(inputAction.KeyboardKey)))
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
    }
}
