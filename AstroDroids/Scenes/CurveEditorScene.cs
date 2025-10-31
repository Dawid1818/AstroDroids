using AstroDroids.Curves;
using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Screens;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGameGum;
using System.Collections.Generic;

namespace AstroDroids.Scenes
{
    public class CurveEditorScene : Scene
    {
        BezierCurve curve;

        int draggedPointIndex = -1;
        bool isDraggingPoint = false;

        CurveEditorScreenGum ui;

        public CurveEditorScene()
        {
            curve = new BezierCurve(new List<Vector2>() 
            { 
                new Vector2(10, 10),
                new Vector2(200, 300),
                new Vector2(300, 100),
                new Vector2(400, 400)
            });
        }

        public override void Set()
        {
            ui = new CurveEditorScreenGum();
            ui.AddToRoot();

            ui.StartPointText.Text = $"Start Point: {curve.GetPointAtIndex(0)}";
            ui.KeyPoint1Text.Text = $"Key Point 1: {curve.GetPointAtIndex(1)}";
            ui.KeyPoint2Text.Text = $"Key Point 2: {curve.GetPointAtIndex(2)}";
            ui.EndPointText.Text = $"End Point: {curve.GetPointAtIndex(3)}";
        }

        public override void Update(GameTime gameTime)
        {
            if (InputSystem.GetLMB())
            {
                if (!isDraggingPoint)
                {
                    for (int i = 0; i < curve.GetPointCount(); i++)
                    {
                        Vector2 point = curve.GetPointAtIndex(i);
                        RectangleF col = new RectangleF(point.X - 16f, point.Y - 16f, 32f, 32f);
                        if (col.Contains(InputSystem.GetMousePos()))
                        {
                            isDraggingPoint = true;
                            draggedPointIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    curve.SetPointAtIndex(draggedPointIndex, InputSystem.GetMousePos());
                }
            }
            else if(isDraggingPoint)
            {
                isDraggingPoint = false;
                draggedPointIndex = -1;
            }

            ui.StartPointText.Text = $"Start Point: {curve.GetPointAtIndex(0)}";
            ui.KeyPoint1Text.Text = $"Key Point 1: {curve.GetPointAtIndex(1)}";
            ui.KeyPoint2Text.Text = $"Key Point 2: {curve.GetPointAtIndex(2)}";
            ui.EndPointText.Text = $"End Point: {curve.GetPointAtIndex(3)}";
        }

        public override void Draw(GameTime gameTime)
        {
            float t = 0f;
            Vector2 lastPos = curve.GetPoint(t);
            while(t < 1f)
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
    }
}
