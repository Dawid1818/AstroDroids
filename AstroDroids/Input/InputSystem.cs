using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroDroids.Input
{
    public class InputSystem
    {
        static KeyboardState kState;
        static MouseState mState;

        static KeyboardState oldKState;
        static MouseState oldMState;
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
