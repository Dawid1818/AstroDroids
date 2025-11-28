using AstroDroids.Curves;
using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Scenes;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;

namespace AstroDroids.Editors
{
    public class PathEditor
    {
        BezierCurve curve;

        LevelEditorScene scene;

        int draggedPointIndex = -1;
        bool isDraggingPoint = false;

        public PathEditor(LevelEditorScene scene)
        {
            this.scene = scene;
        }

        public void SetPath(BezierCurve curve)
        {
            this.curve = curve;
        }

        public void Update(GameTime gameTime)
        {
            Vector2 mousePos = Screen.ScreenToWorldSpaceMouse();

            if (InputSystem.GetLMB())
            {
                if (!isDraggingPoint)
                {
                    for (int i = 0; i < curve.GetPointCount(); i++)
                    {
                        Vector2 point = curve.GetPointAtIndex(i);
                        RectangleF col = new RectangleF(point.X - 16f, point.Y - 16f, 32f, 32f);
                        if (col.Contains(Screen.ScreenToWorldSpaceMouse()))
                        {
                            isDraggingPoint = true;
                            draggedPointIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    if (scene.DrawGrid)
                    {
                        mousePos.X = (int)Math.Floor(mousePos.X / scene.gridSize) * scene.gridSize;
                        mousePos.Y = (int)Math.Floor(mousePos.Y / scene.gridSize) * scene.gridSize;
                    }

                    curve.SetPointAtIndex(draggedPointIndex, mousePos);
                    //UpdateUI();
                }
            }
            else if (isDraggingPoint)
            {
                isDraggingPoint = false;
                draggedPointIndex = -1;
            }
        }

        public void Draw(GameTime gameTime)
        {
            float t = 0f;
            Vector2 lastPos = curve.GetPoint(t);
            while (t < 1f)
            {
                t += 0.01f;
                Vector2 nextPos = curve.GetPoint(t);
                Screen.spriteBatch.DrawLine(lastPos, nextPos, Color.Green, 4f);
                lastPos = nextPos;
            }

            for (int i = 0; i < curve.GetPointCount(); i++)
            {
                Vector2 point = curve.GetPointAtIndex(i);

                Screen.spriteBatch.DrawCircle(point, 16f, 16, Color.Red, 16f);
            }
        }

        public void DrawImGui(GameTime gameTime)
        {
            ImGui.Begin("Path Editor");

            if(ImGui.Button("Return"))
            {
                curve = null;
                scene.ReturnFromEditor();
            }

            ImGui.End();
        }
    }
}
