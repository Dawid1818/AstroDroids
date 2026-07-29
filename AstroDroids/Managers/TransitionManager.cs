using AstroDroids.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;

namespace AstroDroids.Managers
{
    public enum TransitionState
    {
        Idle,
        In,
        Out
    }

    public class TransitionManager
    {
        static TransitionState state = TransitionState.Idle;
        static float progress = 0f;

        static List<Texture2D> transitionTxts = new List<Texture2D>();
        static Texture2D transitionTxt;

        static EffectParameter progressParam;
        static EffectParameter edgeWidthParam;
        static EffectParameter inverseParam;

        public static void Initialize()
        {
            transitionTxts.Add(TextureManager.Get("Transitions/CircleTransition"));
            transitionTxts.Add(TextureManager.Get("Transitions/HexagonTransition"));
            transitionTxt = transitionTxts[0];

            edgeWidthParam = Screen.Transition.Parameters["EdgeWidth"];
            progressParam = Screen.Transition.Parameters["Progress"];
            inverseParam = Screen.Transition.Parameters["Inverse"];
        }

        public static void Update(GameTime gameTime)
        {
            if (state == TransitionState.Idle)
                return;

            float dt = gameTime.GetElapsedSeconds();

            progress += dt;

            if (progress >= 2f)
            {
                progress = 2f;
                SetState(state == TransitionState.In ? TransitionState.Out : TransitionState.Idle);
            }
        }

        public static void Draw(GameTime gameTime)
        {
            if (state == TransitionState.Idle)
                return;

            edgeWidthParam.SetValue(0.2f);
            progressParam.SetValue(1f - progress);
            inverseParam.SetValue(state == TransitionState.Out);

            Screen.spriteBatch.Begin(effect: Screen.Transition);
            Screen.spriteBatch.Draw(transitionTxt, Vector2.Zero, Color.White);
            Screen.spriteBatch.End();
        }

        public static void SetState(TransitionState newState)
        {
            if(newState == TransitionState.In)
            {
                transitionTxt = transitionTxts[AstroDroidsGame.rnd.Next(transitionTxts.Count)];
            }

            state = newState;
            progress = 0f;
        }
    }
}
