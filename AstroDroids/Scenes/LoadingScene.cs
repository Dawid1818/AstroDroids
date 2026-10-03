using AstroDroids.Coroutines;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Threading.Tasks;

namespace AstroDroids.Scenes
{
    internal class LoadingScene : Scene
    {
        CoroutineManager coroutineManager = new CoroutineManager();
        Task soundLoadTask;

        Vector2 loadingTextMeasurement;
        float loadingTextSize = 18f;
        string loadingText = "Loading...";

        Texture2D starfield;
        Texture2D gameLogo;

        public LoadingScene()
        {
            loadingTextMeasurement = Screen.MeasureText(loadingText, loadingTextSize);

            gameLogo = TextureManager.Get("UI/GameLogo");
            starfield = TextureManager.GetStarfield();
        }

        public override void Set()
        {
            soundLoadTask = SoundManager.InitializeAsync(AstroDroidsGame.Instance);
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
            Screen.DrawText(loadingText, new Vector2(Screen.ScreenWidth - loadingTextMeasurement.X - 10, Screen.ScreenHeight - loadingTextMeasurement.Y - 10), Color.White, loadingTextSize);
            Screen.spriteBatch.Draw(gameLogo, new Vector2(Screen.ScreenWidth / 2f - gameLogo.Width / 2f, Screen.ScreenHeight / 2f - gameLogo.Height / 2f), Color.White);
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
