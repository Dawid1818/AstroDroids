using AstroDroids.Coroutines;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace AstroDroids.Scenes
{
    internal class LoadingScene : Scene
    {
        CoroutineManager coroutineManager = new CoroutineManager();
        Task soundLoadTask;
        float LoadingPercentage = 0f;


        Vector2 loadingTextMeasurement;
        float loadingTextSize = 18f;
        string loadingText = "Loading...";

        Texture2D starfield;
        Texture2D gameLogo;
        Texture2D loadingBar;

        public LoadingScene()
        {
            loadingTextMeasurement = Screen.MeasureText(loadingText, loadingTextSize);

            gameLogo = TextureManager.Get("UI/GameLogo");
            starfield = TextureManager.GetStarfield();

            loadingBarColor = new Color(255, 255, 255, 127);
            loadingBar = TextureManager.Get("UI/LoadingBar");
        }

        public override void Set()
        {
            var progressHandler = new Action<float>(percentage =>
            {
                LoadingPercentage = percentage;
            });

            soundLoadTask = SoundManager.InitializeAsync(AstroDroidsGame.Instance, progressHandler);
        }

        public override void Update(GameTime gameTime)
        {
            coroutineManager.Update(gameTime);

            if (soundLoadTask != null && soundLoadTask.IsCompleted)
            {
                soundLoadTask = null;
                TransitionToScene(new MainMenuScene());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointWrap);
            Screen.spriteBatch.Draw(starfield, new Rectangle(0, 0, Screen.ScreenWidth, Screen.ScreenHeight), Color.White);
            Screen.DrawText(loadingText, new Vector2(Screen.ScreenWidth - loadingTextMeasurement.X - 10, Screen.ScreenHeight - loadingTextMeasurement.Y - 30), Color.White, loadingTextSize);
            Screen.spriteBatch.Draw(gameLogo, new Vector2(Screen.ScreenWidth / 2f - gameLogo.Width / 2f, Screen.ScreenHeight / 2f - gameLogo.Height / 2f), Color.White);

            int currentWidth = (int)(Screen.ScreenWidth * LoadingPercentage);
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle(0, Screen.ScreenHeight - 25, Screen.ScreenWidth, 30), Color.Black);
            Screen.spriteBatch.Draw(loadingBar, new Rectangle(0, Screen.ScreenHeight - 20, currentWidth, 20), new Rectangle(0, 0, currentWidth, loadingBar.Height), Color.White);

            Screen.spriteBatch.End();
        }

        public override void DrawDebug(GameTime gameTime)
        {

        }

        IEnumerator TransitionToSceneCoroutine(Scene scene)
        {
            TransitionManager.SetState(TransitionState.In);

            yield return new WaitUntil(() => TransitionManager.State == TransitionState.Out);

            SceneManager.SetScene(scene);
        }

        public void TransitionToScene(Scene scene)
        {
            coroutineManager.StartCoroutine(TransitionToSceneCoroutine(scene));
        }

    }
}
