using AstroDroids.Input;
using Gum.DataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;

namespace AstroDroids.Graphics
{
    public static class Screen
    {
        public const int ScreenWidth = 800;
        public const int ScreenHeight = 600;

        public static int ActualScreenWidth { get { return gameWnd.ClientBounds.Width; } }
        public static int ActualScreenHeight { get { return gameWnd.ClientBounds.Height; } }

        public static SpriteBatch spriteBatch { get; private set; }
        public static GumService GumUI => GumService.Default;
        static GumProjectSave gumProject;

        static GameWindow gameWnd;
        static ImGuiRenderer imGuiRenderer;
        static GraphicsDeviceManager graphicsManager;
        static Vector2 CameraPosition = new Vector2(0, 0);
        static float ScreenScale = 1.0f;

        public static Effect Infinite { get; private set; }
        public static Viewport Viewport { get { return graphicsManager.GraphicsDevice.Viewport; } }

        public static void Initialize(AstroDroidsGame game)
        {
            graphicsManager = game.Graphics;
            spriteBatch = new SpriteBatch(game.GraphicsDevice);

            gumProject = GumUI.Initialize(game, "GumProject/AstroDroidsGum.gumx");

            CameraPosition = new Vector2(graphicsManager.GraphicsDevice.Viewport.Width / 2f, graphicsManager.GraphicsDevice.Viewport.Height / 2f);

            imGuiRenderer = new ImGuiRenderer(game);

            Infinite = game.Content.Load<Effect>("Shaders/Infinite");

            gameWnd = game.Window;
        }

        public static void Update(GameTime gameTime)
        {
            GumUI.Update(gameTime);
        }

        public static void DrawGum(GameTime gameTime)
        {
            GumUI.Draw();
        }

        public static Matrix GetCameraMatrix()
        {
            Vector2 screenCenter = new Vector2(graphicsManager.GraphicsDevice.Viewport.Width / 2f, graphicsManager.GraphicsDevice.Viewport.Height / 2f);

            return Matrix.CreateTranslation(-CameraPosition.X, -CameraPosition.Y, 0)
             * Matrix.CreateScale(ScreenScale)
             * Matrix.CreateTranslation(screenCenter.X, screenCenter.Y, 0);
        }

        public static Vector2 ScreenToWorldSpace(Vector2 point)
        {
            Vector2 vec = new Vector2(point.X, point.Y);
            Matrix invertedMatrix = Matrix.Invert(GetCameraMatrix());
            return Vector2.Transform(vec, invertedMatrix);
        }

        public static Vector2 ScreenToWorldSpaceMouse()
        {
            var mousePos = InputSystem.GetMousePos();
            return ScreenToWorldSpace(mousePos);
        }

        public static void ResetCamera()
        {
            CameraPosition = new Vector2(graphicsManager.GraphicsDevice.Viewport.Width / 2f, graphicsManager.GraphicsDevice.Viewport.Height / 2f);
            ScreenScale = 1.0f;
        }

        public static void MoveCamera(Vector2 vec)
        {
            CameraPosition += vec;
        }

        public static void SetCameraPosition(Vector2 vec)
        {
            CameraPosition = vec;
        }

        public static void ZoomCamera(float val)
        {
            ScreenScale += val;
            if (ScreenScale <= 0.2f)
                ScreenScale = 0.2f;
        }

        public static float GetCameraZoom()
        {
            return ScreenScale;
        }

        public static void SetCameraZoom(float val)
        {
            ScreenScale = val;
            if (ScreenScale <= 0.2f)
                ScreenScale = 0.2f;
        }

        public static Vector2 GetCameraPosition()
        {
            return CameraPosition;
        }

        internal static void DrawImGuiBefore(GameTime gameTime)
        {
            imGuiRenderer.BeforeLayout(gameTime);
        }

        internal static void DrawImGuiAfter()
        {
            imGuiRenderer.AfterLayout();
        }

        internal static Matrix GetUVTransform(Texture2D t, Vector2 offset, float scale, Viewport v)
        {
            return
                Matrix.CreateScale(t.Width, t.Height, 1f) *
                Matrix.CreateScale(scale, scale, 1f) *
                Matrix.CreateTranslation(offset.X, offset.Y, 0f) *
                Screen.GetCameraMatrix() *
                Matrix.CreateScale(1f / v.Width, 1f / v.Height, 1f);
        }
    }
}
