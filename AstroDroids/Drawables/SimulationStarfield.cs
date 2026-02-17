using AstroDroids.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;

namespace AstroDroids.Drawables
{
    internal class SimulationStarfield : Starfield
    {
        int gridSize = 64;

        Vector2 offset1 = Vector2.Zero;
        Vector2 offset2 = Vector2.Zero;

        Color color1 = new Color(0, 0, 127, 127);
        Color color2 = new Color(0, 50, 127, 127);

        public SimulationStarfield()
        {

        }

        public override void Update()
        {
            offset1.X += 0.2f;
            offset1.Y += 0.2f;

            offset2.X += 0.3f;
            offset2.Y += 0.3f;
        }

        public override void Draw()
        {
            Matrix camMatrix = Screen.GetCameraMatrix();
            Screen.spriteBatch.Begin();

            Matrix invCam = Matrix.Invert(camMatrix);

            Screen.spriteBatch.FillRectangle(new RectangleF(0, 0, Screen.ActualScreenWidth, Screen.ActualScreenHeight), Color.Black);

            DrawGrid(offset1, color1);
            DrawGrid(offset2, color2);

            Screen.spriteBatch.End();
        }

        void DrawGrid(Vector2 offset, Color color)
        {
            int startX = (int)Math.Floor(0d / gridSize) * gridSize;
            int endX = (int)Math.Ceiling((float)Screen.ActualScreenWidth / gridSize) * gridSize;
            int startY = (int)Math.Floor(0d / gridSize) * gridSize;
            int endY = (int)Math.Ceiling((float)Screen.ActualScreenHeight / gridSize) * gridSize;

            int wrappedXOffset = (((int)offset.X % gridSize) + gridSize) % gridSize;
            int wrappedYOffset = (((int)offset.Y % gridSize) + gridSize) % gridSize;

            for (int x = startX - gridSize + wrappedXOffset; x <= endX + gridSize; x += gridSize)
            {
                Screen.spriteBatch.DrawLine(new Vector2(x, startY), new Vector2(x, endY), color, 2);
            }

            for (int y = startY - gridSize + wrappedYOffset; y <= endY + gridSize; y += gridSize)
            {
                Screen.spriteBatch.DrawLine(new Vector2(startX, y), new Vector2(endX, y), color, 2);
            }
        }
    }
}
