using AstroDroids.Components.Controls;
using System;

namespace AstroDroids.Components.Custom
{
    partial class KeyboardGlow
    {
        public Action<string> KeyPressed;
        public Action BackspacePressed;
        public Action LeftPressed;
        public Action RightPressed;
        public Action ResumePressed;

        partial void CustomInitialize()
        {
            //setup spatial navigation
            SetupKeyNav(Key1, null, Key2, null, KeyQ);
            SetupKeyNav(Key2, Key1, Key3, null, KeyW);
            SetupKeyNav(Key3, Key2, Key4, null, KeyE);
            SetupKeyNav(Key4, Key3, Key5, null, KeyR);
            SetupKeyNav(Key5, Key4, Key6, null, KeyT);
            SetupKeyNav(Key6, Key5, Key7, null, KeyY);
            SetupKeyNav(Key7, Key6, Key8, null, KeyU);
            SetupKeyNav(Key8, Key7, Key9, null, KeyI);
            SetupKeyNav(Key9, Key8, Key0, null, KeyO);
            SetupKeyNav(Key0, Key9, KeyBackspace, null, KeyP);
            SetupKeyNav(KeyBackspace, Key0, null, null, KeyLeft);

            SetupKeyNav(KeyQ, null, KeyW, Key1, KeyA);
            SetupKeyNav(KeyW, KeyQ, KeyE, Key2, KeyS);
            SetupKeyNav(KeyE, KeyW, KeyR, Key3, KeyD);
            SetupKeyNav(KeyR, KeyE, KeyT, Key4, KeyF);
            SetupKeyNav(KeyT, KeyR, KeyY, Key5, KeyG);
            SetupKeyNav(KeyY, KeyT, KeyU, Key6, KeyH);
            SetupKeyNav(KeyU, KeyY, KeyI, Key7, KeyJ);
            SetupKeyNav(KeyI, KeyU, KeyO, Key8, KeyK);
            SetupKeyNav(KeyO, KeyI, KeyP, Key9, KeyL);
            SetupKeyNav(KeyP, KeyO, KeyLeft, Key0, KeyUnderscore);
            SetupKeyNav(KeyLeft, KeyP, null, KeyBackspace, KeyRight);

            SetupKeyNav(KeyA, null, KeyS, KeyQ, KeyZ);
            SetupKeyNav(KeyS, KeyA, KeyD, KeyW, KeyX);
            SetupKeyNav(KeyD, KeyS, KeyF, KeyE, KeyC);
            SetupKeyNav(KeyF, KeyD, KeyG, KeyR, KeyV);
            SetupKeyNav(KeyG, KeyF, KeyH, KeyT, KeyB);
            SetupKeyNav(KeyH, KeyG, KeyJ, KeyY, KeyN);
            SetupKeyNav(KeyJ, KeyH, KeyK, KeyU, KeyM);
            SetupKeyNav(KeyK, KeyJ, KeyL, KeyI, KeyComma);
            SetupKeyNav(KeyL, KeyK, KeyUnderscore, KeyO, KeyPeriod);
            SetupKeyNav(KeyUnderscore, KeyL, KeyRight, KeyP, KeyHyphen);
            SetupKeyNav(KeyRight, KeyUnderscore, null, KeyLeft, KeyReturn);

            SetupKeyNav(KeyZ, null, KeyX, KeyA, KeyParenLeft);
            SetupKeyNav(KeyX, KeyZ, KeyC, KeyS, KeyParenRight);
            SetupKeyNav(KeyC, KeyX, KeyV, KeyD, KeySpace);
            SetupKeyNav(KeyV, KeyC, KeyB, KeyF, KeySpace);
            SetupKeyNav(KeyB, KeyV, KeyN, KeyG, KeySpace);
            SetupKeyNav(KeyN, KeyB, KeyM, KeyH, KeySpace);
            SetupKeyNav(KeyM, KeyN, KeyComma, KeyJ, KeySpace);
            SetupKeyNav(KeyComma, KeyM, KeyPeriod, KeyK, KeyQuestion);
            SetupKeyNav(KeyPeriod, KeyComma, KeyHyphen, KeyL, KeyBang);
            SetupKeyNav(KeyHyphen, KeyPeriod, KeyReturn, KeyUnderscore, KeyAmpersand);
            SetupKeyNav(KeyReturn, KeyHyphen, null, KeyRight, null);

            SetupKeyNav(KeyParenLeft, null, KeyParenRight, KeyZ, null);
            SetupKeyNav(KeyParenRight, KeyParenLeft, KeySpace, KeyX, null);
            SetupKeyNav(KeySpace, KeyParenRight, KeyQuestion, KeyB, null);
            SetupKeyNav(KeyQuestion, KeySpace, KeyBang, KeyComma, null);
            SetupKeyNav(KeyBang, KeyQuestion, KeyAmpersand, KeyPeriod, null);
            SetupKeyNav(KeyAmpersand, KeyBang, KeyReturn, KeyHyphen, null);

            //assign key codes
            Key1.Visual.Tag = "1";
            Key2.Visual.Tag = "2";
            Key3.Visual.Tag = "3";
            Key4.Visual.Tag = "4";
            Key5.Visual.Tag = "5";
            Key6.Visual.Tag = "6";
            Key7.Visual.Tag = "7";
            Key8.Visual.Tag = "8";
            Key9.Visual.Tag = "9";
            Key0.Visual.Tag = "0";

            KeyQ.Visual.Tag = "Q";
            KeyW.Visual.Tag = "W";
            KeyE.Visual.Tag = "E";
            KeyR.Visual.Tag = "R";
            KeyT.Visual.Tag = "T";
            KeyY.Visual.Tag = "Y";
            KeyU.Visual.Tag = "U";
            KeyI.Visual.Tag = "I";
            KeyO.Visual.Tag = "O";
            KeyP.Visual.Tag = "P";

            KeyA.Visual.Tag = "A";
            KeyS.Visual.Tag = "S";
            KeyD.Visual.Tag = "D";
            KeyF.Visual.Tag = "F";
            KeyG.Visual.Tag = "G";
            KeyH.Visual.Tag = "H";
            KeyJ.Visual.Tag = "J";
            KeyK.Visual.Tag = "K";
            KeyL.Visual.Tag = "L";
            KeyUnderscore.Visual.Tag = "_";

            KeyZ.Visual.Tag = "Z";
            KeyX.Visual.Tag = "X";
            KeyC.Visual.Tag = "C";
            KeyV.Visual.Tag = "V";
            KeyB.Visual.Tag = "B";
            KeyN.Visual.Tag = "N";
            KeyM.Visual.Tag = "M";
            KeyComma.Visual.Tag = ",";
            KeyPeriod.Visual.Tag = ".";
            KeyHyphen.Visual.Tag = "-";

            KeyParenLeft.Visual.Tag = "(";
            KeyParenRight.Visual.Tag = ")";
            KeySpace.Visual.Tag = " ";
            KeyQuestion.Visual.Tag = "?";
            KeyBang.Visual.Tag = "!";
            KeyAmpersand.Visual.Tag = "&";

            //assign events
            Key1.Click += Key_Click;
            Key2.Click += Key_Click;
            Key3.Click += Key_Click;
            Key4.Click += Key_Click;
            Key5.Click += Key_Click;
            Key6.Click += Key_Click;
            Key7.Click += Key_Click;
            Key8.Click += Key_Click;
            Key9.Click += Key_Click;
            Key0.Click += Key_Click;
            KeyQ.Click += Key_Click;
            KeyW.Click += Key_Click;
            KeyE.Click += Key_Click;
            KeyR.Click += Key_Click;
            KeyT.Click += Key_Click;
            KeyY.Click += Key_Click;
            KeyU.Click += Key_Click;
            KeyI.Click += Key_Click;
            KeyO.Click += Key_Click;
            KeyP.Click += Key_Click;
            KeyA.Click += Key_Click;
            KeyS.Click += Key_Click;
            KeyD.Click += Key_Click;
            KeyF.Click += Key_Click;
            KeyG.Click += Key_Click;
            KeyH.Click += Key_Click;
            KeyJ.Click += Key_Click;
            KeyK.Click += Key_Click;
            KeyL.Click += Key_Click;
            KeyUnderscore.Click += Key_Click;
            KeyZ.Click += Key_Click;
            KeyX.Click += Key_Click;
            KeyC.Click += Key_Click;
            KeyV.Click += Key_Click;
            KeyB.Click += Key_Click;
            KeyN.Click += Key_Click;
            KeyM.Click += Key_Click;
            KeyComma.Click += Key_Click;
            KeyPeriod.Click += Key_Click;
            KeyHyphen.Click += Key_Click;
            KeyParenLeft.Click += Key_Click;
            KeyParenRight.Click += Key_Click;
            KeySpace.Click += Key_Click;
            KeyQuestion.Click += Key_Click;
            KeyBang.Click += Key_Click;
            KeyAmpersand.Click += Key_Click;

            KeyBackspace.Click += KeyBackspace_Click;
            KeyLeft.Click += KeyLeft_Click;
            KeyRight.Click += KeyRight_Click;
            KeyReturn.Click += KeyReturn_Click;
        }

        private void KeyReturn_Click(object sender, System.EventArgs e)
        {
            ResumePressed?.Invoke();
        }

        private void KeyRight_Click(object sender, System.EventArgs e)
        {
            RightPressed?.Invoke();
        }

        private void KeyLeft_Click(object sender, System.EventArgs e)
        {
            LeftPressed?.Invoke();
        }

        private void KeyBackspace_Click(object sender, System.EventArgs e)
        {
            BackspacePressed?.Invoke();
        }

        private void Key_Click(object sender, System.EventArgs e)
        {
            KeyboardKeyGlow btn = (KeyboardKeyGlow)sender;
            KeyPressed?.Invoke((string)btn.Visual.Tag);
        }

        void SetupKeyNav(KeyboardKeyGlow key, KeyboardKeyGlow left, KeyboardKeyGlow right, KeyboardKeyGlow up, KeyboardKeyGlow down)
        {
            key.SpatialNavigationLeft = left;
            key.SpatialNavigationRight = right;
            key.SpatialNavigationDown = down;
            key.SpatialNavigationUp = up;
        }
    }
}
