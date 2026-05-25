using AstroDroids.Input;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using FontStashSharp;
using Gum.DataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGameGum;
using System;
using System.IO;
using System.Reflection.Metadata.Ecma335;

namespace AstroDroids.Graphics
{
    public static class Screen
    {
        public const int ScreenWidth = 800;
        public const int ScreenHeight = 600;

        public static int ActualScreenWidth { get { return !(SceneManager.GetScene() is LevelEditorScene) ? ScreenWidth : gameWnd.ClientBounds.Width; } }
        public static int ActualScreenHeight { get { return !(SceneManager.GetScene() is LevelEditorScene) ? ScreenHeight : gameWnd.ClientBounds.Height; } }

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

        static FontSystem fontSystem;


        public static RenderTarget2D RenderTarget;
        public static Rectangle DestinationRectangle;

        public static void Initialize(AstroDroidsGame game)
        {
            graphicsManager = game.Graphics;
            spriteBatch = new SpriteBatch(game.GraphicsDevice);

            gumProject = GumUI.Initialize(game, "GumProject/AstroDroidsGum.gumx");

            CameraPosition = new Vector2(graphicsManager.GraphicsDevice.Viewport.Width / 2f, graphicsManager.GraphicsDevice.Viewport.Height / 2f);

            imGuiRenderer = new ImGuiRenderer(game);

            Infinite = game.Content.Load<Effect>("Shaders/Infinite");

            fontSystem = new FontSystem();
            fontSystem.AddFont(File.ReadAllBytes("Content/Fonts/VCR_OSD_MONO_1.001.ttf"));

            gameWnd = game.Window;

            RenderTarget = new RenderTarget2D(game.GraphicsDevice, ScreenWidth, ScreenHeight);
            game.Window.ClientSizeChanged += (_, _) => UpdateViewport();
        }

        static void UpdateViewport()
        {
            int windowWidth = graphicsManager.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int windowHeight = graphicsManager.GraphicsDevice.PresentationParameters.BackBufferHeight;

            float scaleX = (float)windowWidth / ScreenWidth;
            float scaleY = (float)windowHeight / ScreenHeight;

            float scale = MathF.Min(scaleX, scaleY);

            int width = (int)(ScreenWidth * scale);
            int height = (int)(ScreenHeight * scale);

            int x = (windowWidth - width) / 2;
            int y = (windowHeight - height) / 2;

            DestinationRectangle = new Rectangle(x, y, width, height);
        }   

        public static void DrawText(string text, Vector2 position, Color color, float size)
        {
            SpriteFontBase font = fontSystem.GetFont(size);
            spriteBatch.DrawString(font, text, position, color, effect: FontSystemEffect.Stroked, effectAmount: 1);
        }

        public static Vector2 MeasureText(string text, float size)
        {
            SpriteFontBase font = fontSystem.GetFont(size);
            return font.MeasureString(text, effect: FontSystemEffect.Stroked, effectAmount: 1);
        }

        public static void Update(GameTime gameTime)
        {
            GumUI.Update(gameTime);
        }

        public static void Draw(GameTime gameTime)
        {
            bool useVirtualResolution = !(SceneManager.GetScene() is LevelEditorScene);

            if (useVirtualResolution)
                graphicsManager.GraphicsDevice.SetRenderTarget(RenderTarget);

            graphicsManager.GraphicsDevice.Clear(Color.Black);

            //Screen.spriteBatch.Begin(transformMatrix: Screen.GetCameraMatrix());

            DrawImGuiBefore(gameTime);

            SceneManager.Draw(gameTime);

            //Screen.spriteBatch.End();

            DrawImGuiAfter();

            DrawGum(gameTime);

            if (useVirtualResolution)
            {
                graphicsManager.GraphicsDevice.SetRenderTarget(null);

                graphicsManager.GraphicsDevice.Clear(Color.Black);

                spriteBatch.Begin(samplerState: SamplerState.PointClamp);

                spriteBatch.Draw(RenderTarget, DestinationRectangle, Color.White);

                spriteBatch.End();

                spriteBatch.Begin();

                spriteBatch.DrawRectangle(new Rectangle(0, 0, DestinationRectangle.Left, gameWnd.ClientBounds.Height), Color.Blue);

                spriteBatch.DrawRectangle(new Rectangle(DestinationRectangle.Right, 0, gameWnd.ClientBounds.Width - DestinationRectangle.Right, gameWnd.ClientBounds.Height), Color.Blue);

                spriteBatch.End();
            }
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

        public static GraphicsDeviceManager GetGraphicsManager()
        {
            return graphicsManager;
        }

        public static Vector2 ScreenToWorldSpace(Vector2 point)
        {
            Matrix invertedMatrix = Matrix.Invert(GetCameraMatrix());
            return Vector2.Transform(point, invertedMatrix);
        }

        public static Vector2 ScreenToWorldSpaceMouse()
        {
            var mousePos = InputSystem.GetMousePos();
            return ScreenToWorldSpace(mousePos);
        }

        public static Vector2 VirtualScreenToWorldSpaceMouse()
        {
            Vector2 virtualMouse = GetVirtualMousePosition();
            return VirtualScreenToWorldSpace(virtualMouse);
        }

        public static Vector2 VirtualScreenToWorldSpace(Vector2 point)
        {
            Matrix invertedMatrix = Matrix.Invert(GetCameraMatrix());
            return Vector2.Transform(point, invertedMatrix);
        }

        public static Vector2 GetVirtualMousePosition()
        {
            MouseState mouse = Mouse.GetState();

            Rectangle dest = DestinationRectangle;

            float x = (mouse.X - dest.X) * ScreenWidth / (float)dest.Width;

            float y = (mouse.Y - dest.Y) * ScreenHeight / (float)dest.Height;

            return new Vector2(x, y);
        }

        public static void ResetCamera()
        {
            CameraPosition = new Vector2(ScreenWidth / 2f, ScreenHeight / 2f);
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

        public static ImGuiRenderer GetImGuiRenderer()
        {
            return imGuiRenderer;
        }
    }
}
