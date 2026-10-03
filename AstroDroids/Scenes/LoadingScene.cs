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

        string gameTitle = "Astrodroids";
        float gameTextSize = 98f;
        Vector2 gameTileTextMeasurement;

        Texture2D starfield;

        public LoadingScene()
        {
            loadingTextMeasurement = Screen.MeasureText(loadingText, loadingTextSize);
            gameTileTextMeasurement = Screen.MeasureText(gameTitle, gameTextSize);

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
            Screen.DrawText(gameTitle, new Vector2(Screen.ScreenWidth / 2f - gameTileTextMeasurement.X / 2f, Screen.ScreenHeight / 2f - gameTileTextMeasurement.Y / 2f), Color.White, gameTextSize);
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
