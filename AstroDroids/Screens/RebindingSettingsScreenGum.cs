using AstroDroids.Components.Custom;
using AstroDroids.Components.Elements;
using AstroDroids.Input;
using AstroDroids.Interfaces;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace AstroDroids.Screens
{
    partial class RebindingSettingsScreenGum : IMenuPage
    {
        IPageHost scene;

        Dictionary<GameAction, ButtonInputAction> actions { get { return SettingsManager.curSettings.Actions; } }

        public bool UpdateWhenTransitioning => false;

        InputActionComponent rebindingWith;

        bool isKeyboard = true;

        public RebindingSettingsScreenGum(bool isKeyboard)
        {
            this.isKeyboard = isKeyboard;
        }

        public void Initialize(IPageHost scene, HintedScreenGum hinted)
        {
            this.scene = scene;
            BackBtn.Click += ReturnBtn_Click;

            BackBtn.X = -600;
            InputActionCom1.X = -600;
            InputActionCom2.X = -600;
            InputActionCom3.X = -600;
            InputActionCom4.X = -600;
            InputActionCom5.X = -600;
            InputActionCom6.X = -600;
            InputActionCom7.X = -600;

            InputActionCom1.Set(StartRebind, isKeyboard, GameAction.Up, actions[GameAction.Up]);
            InputActionCom2.Set(StartRebind, isKeyboard, GameAction.Down, actions[GameAction.Down]);
            InputActionCom3.Set(StartRebind, isKeyboard, GameAction.Left, actions[GameAction.Left]);
            InputActionCom4.Set(StartRebind, isKeyboard, GameAction.Right, actions[GameAction.Right]);
            InputActionCom5.Set(StartRebind, isKeyboard, GameAction.Fire, actions[GameAction.Fire]);
            InputActionCom6.Set(StartRebind, isKeyboard, GameAction.NextWeapon, actions[GameAction.NextWeapon]);
            InputActionCom7.Set(StartRebind, isKeyboard, GameAction.Focus, actions[GameAction.Focus]);

            hinted.AddHint("T_Navigate", Icon2.IconCategory.ArrowKeys, Icon2.IconCategory.ControllerLeftJoystick, Icon2.IconCategory.MouseNMB);
            hinted.AddHint("T_Select", Icon2.IconCategory.ZKey, Icon2.IconCategory.ControllerA, Icon2.IconCategory.MouseLMB);
            hinted.AddHint("T_Return", Icon2.IconCategory.XKey, Icon2.IconCategory.ControllerB, Icon2.IconCategory.MouseRMB);

            GamepadNavigationMode = Gum.Forms.Controls.GamepadNavigationMode.Spatial;
        }

        public void StartRebind(InputActionComponent comp)
        {
            rebindingWith = comp;
            InputSystem.ClearUIKeys();
            InputSystem.DisableUIMouse();
        }

        public void Update(GameTime gameTime)
        {
            if (rebindingWith != null)
            {
                if (isKeyboard)
                {
                    Keys[] pressed = InputSystem.GetAllPressedKeys();
                    if (pressed.Length > 0)
                    {
                        foreach (var item in pressed)
                        {
                            if (InputSystem.GetKeyDown(item))
                            {
                                rebindingWith.ActionBinding.KeyboardKey = item;
                                rebindingWith.StopRebinding();
                                rebindingWith = null;
                                InputSystem.AddUIKeys();
                                InputSystem.EnableUIMouse();
                                break;
                            }
                        }
                    }
                }
                else
                {
                    List<Buttons> pressed = InputSystem.GetAllPressedGamepadButtons();
                    if(pressed.Count > 0)
                    {
                        foreach (var item in pressed)
                        {
                            if (InputSystem.GetButtonDown(item))
                            {
                                rebindingWith.ActionBinding.GamepadButton = item;
                                rebindingWith.StopRebinding();
                                rebindingWith = null;
                                InputSystem.AddUIKeys();
                                InputSystem.EnableUIMouse();
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void Uninitialize()
        {

        }

        private void AnimationController_OnCompleted()
        {
            InputActionCom1.IsFocused = true;
            Visual.AnimationController.OnCompleted -= AnimationController_OnCompleted;
        }

        public void TransitionIn()
        {
            Visual.PlayAnimation(Enter);
            Visual.AnimationController.OnCompleted += AnimationController_OnCompleted;
        }

        public void TransitionOut()
        {
            Visual.PlayAnimation(Leave);
        }

        public bool TransitionFinished()
        {
            return Visual.AnimationController.IsStopped;
        }

        private void ReturnBtn_Click(object sender, System.EventArgs e)
        {
            if (rebindingWith != null)
                return;

            SettingsManager.ApplyRebinds();
            SettingsManager.Save();
            scene.SetPage(new ControlsSettingsScreenGum(), false);
        }

        partial void CustomInitialize()
        {

        }

        public void BackPressed()
        {
            if (rebindingWith != null)
                return;

            SettingsManager.ApplyRebinds();
            SettingsManager.Save();
            scene.SetPage(new ControlsSettingsScreenGum(), false);
        }
    }
}
