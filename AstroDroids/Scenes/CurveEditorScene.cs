using AstroDroids.Components.Controls;
using AstroDroids.Curves;
using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGameGum;
using System;
using System.Collections.Generic;

namespace AstroDroids.Scenes
{
    public class CurveEditorScene : Scene
    {
        BezierCurve curve;

        int draggedPointIndex = -1;
        bool isDraggingPoint = false;

        CurveEditorScreenGum ui;

        bool disableUIEvents = false;

        float cameraSpeed = 5f;

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

            ui.StartPointXBox.Visual.Tag = 0;
            ui.KeyPoint1XBox.Visual.Tag = 1;
            ui.KeyPoint2XBox.Visual.Tag = 2;
            ui.EndPointXBox.Visual.Tag = 3;

            ui.StartPointYBox.Visual.Tag = 0;
            ui.KeyPoint1YBox.Visual.Tag = 1;
            ui.KeyPoint2YBox.Visual.Tag = 2;
            ui.EndPointYBox.Visual.Tag = 3;

            ui.StartPointXBox.TextChanged += XBox_TextChanged;
            ui.KeyPoint1XBox.TextChanged += XBox_TextChanged;
            ui.KeyPoint2XBox.TextChanged += XBox_TextChanged;
            ui.EndPointXBox.TextChanged += XBox_TextChanged;

            ui.StartPointYBox.TextChanged += YBox_TextChanged;
            ui.KeyPoint1YBox.TextChanged += YBox_TextChanged;
            ui.KeyPoint2YBox.TextChanged += YBox_TextChanged;
            ui.EndPointYBox.TextChanged += YBox_TextChanged;

            UpdateUI();
        }

        private void XBox_TextChanged(object sender, System.EventArgs e)
        {
            if (disableUIEvents)
                return;

            TextBox box = (TextBox)sender;

            if(float.TryParse(box.Text, out float value))
            {
                curve.SetPointAtIndex((int)box.Visual.Tag, new Vector2(value, curve.GetPointAtIndex((int)box.Visual.Tag).Y));
            }
        }

        private void YBox_TextChanged(object sender, System.EventArgs e)
        {
            if (disableUIEvents)
                return;

            TextBox box = (TextBox)sender;

            if (float.TryParse(box.Text, out float value))
            {
                curve.SetPointAtIndex((int)box.Visual.Tag, new Vector2(curve.GetPointAtIndex((int)box.Visual.Tag).X, value));
            }
        }

        public void UpdateUI()
        {
            disableUIEvents = true;

            Vector2 point = curve.GetPointAtIndex(0);
            ui.StartPointXBox.Text = point.X.ToString();
            ui.StartPointYBox.Text = point.Y.ToString();

            point = curve.GetPointAtIndex(1);
            ui.KeyPoint1XBox.Text = point.X.ToString();
            ui.KeyPoint1YBox.Text = point.Y.ToString();

            point = curve.GetPointAtIndex(2);
            ui.KeyPoint2XBox.Text = point.X.ToString();
            ui.KeyPoint2YBox.Text = point.Y.ToString();

            point = curve.GetPointAtIndex(3);
            ui.EndPointXBox.Text = point.X.ToString();
            ui.EndPointYBox.Text = point.Y.ToString();

            disableUIEvents = false;
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
                    curve.SetPointAtIndex(draggedPointIndex, Screen.ScreenToWorldSpaceMouse());
                    UpdateUI();
                }
            }
            else if(isDraggingPoint)
            {
                isDraggingPoint = false;
                draggedPointIndex = -1;
            }

            Vector2 cameraTranslation = Vector2.Zero;

            if (InputSystem.GetKey(Keys.W))
                cameraTranslation.Y -= cameraSpeed;

            if (InputSystem.GetKey(Keys.S))
                cameraTranslation.Y += cameraSpeed;

            if (InputSystem.GetKey(Keys.A))
                cameraTranslation.X -= cameraSpeed;

            if (InputSystem.GetKey(Keys.D))
                cameraTranslation.X += cameraSpeed;

            Screen.MoveCamera(cameraTranslation);
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
