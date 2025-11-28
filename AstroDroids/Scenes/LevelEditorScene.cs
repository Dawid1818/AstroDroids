using AstroDroids.Editors;
using AstroDroids.Entities;
using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Levels;
using AstroDroids.Managers;
using AstroDroids.Projectiles;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using Numeric = System.Numerics;

namespace AstroDroids.Scenes
{
    enum EditorMode
    {
        Main,
        Path
    }

    public class LevelEditorScene : Scene
    {
        EditorMode mode = EditorMode.Main;
        Level level { get { return LevelManager.CurrentLevel; } set { LevelManager.CurrentLevel = value; } }

        float cameraMoveSpeed = 5f;
        float cameraZoomSpeed = 0.2f;

        bool drawGrid = false;
        public bool DrawGrid { get { return drawGrid; } private set { drawGrid = value; } }

        public int gridSize = 32;

        PathEditor curveEditor;

        bool isDraggingNode;
        Entity selectedNode;

        bool savedCamera = false;
        Vector2 savedCameraPos;
        float savedCameraZoom;

        public LevelEditorScene()
        {
            //curve = new BezierCurve(new List<Vector2>() 
            //{ 
            //    new Vector2(10, 10),
            //    new Vector2(200, 300),
            //    new Vector2(300, 100),
            //    new Vector2(400, 400)
            //});

            curveEditor = new PathEditor(this);

            level = new Level();
        }

        public override void Set()
        {
            Screen.GumUI.Root.Children.Clear();
        }

        public override void Update(GameTime gameTime)
        {
            switch (mode)
            {
                case EditorMode.Main:
                    MainUpdate();
                    break;
                case EditorMode.Path:
                    curveEditor.Update(gameTime);
                    break;
                default:
                    break;
            }

            Vector2 cameraTranslation = Vector2.Zero;

            if (InputSystem.GetKey(Keys.W))
                cameraTranslation.Y -= cameraMoveSpeed;

            if (InputSystem.GetKey(Keys.S))
                cameraTranslation.Y += cameraMoveSpeed;

            if (InputSystem.GetKey(Keys.A))
                cameraTranslation.X -= cameraMoveSpeed;

            if (InputSystem.GetKey(Keys.D))
                cameraTranslation.X += cameraMoveSpeed;

            Screen.MoveCamera(cameraTranslation);

            int scrollDelta = InputSystem.GetScrollDelta();

            if(scrollDelta > 0)
            {
                Screen.ZoomCamera(cameraZoomSpeed);
            }
            else if(scrollDelta < 0)
            {
                Screen.ZoomCamera(-cameraZoomSpeed);
            }
        }

        public override void Draw(GameTime gameTime)
        {
           // Screen.spriteBatch.Draw(TextureManager.GetStarfield(), Vector2.Zero, Color.White);
            Vector2 cameraPos = Screen.GetCameraPosition();

            Screen.spriteBatch.End();

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Screen.ScreenWidth, Screen.ScreenHeight, 0, 0, 1);
            Matrix uv_transform = Screen.GetUVTransform(TextureManager.GetStarfield(), new Vector2(0, 0), 1f, Screen.Viewport);

            Screen.Infinite.Parameters["view_projection"].SetValue(projection);
            Screen.Infinite.Parameters["uv_transform"].SetValue(Matrix.Invert(uv_transform));

            Screen.spriteBatch.Begin(effect: Screen.Infinite, transformMatrix: Screen.GetCameraMatrix(), samplerState: SamplerState.LinearWrap);
            Screen.spriteBatch.Draw(TextureManager.GetStarfield(), new Rectangle(0, 0, Screen.ScreenWidth, Screen.ScreenHeight), Color.White);
            Screen.spriteBatch.End();

            Screen.spriteBatch.Begin(transformMatrix: Screen.GetCameraMatrix());
            Matrix cam = Screen.GetCameraMatrix();
            Matrix invCam = Matrix.Invert(cam);

            Vector2 topLeft = Vector2.Transform(Vector2.Zero, invCam);
            Vector2 bottomRight = Vector2.Transform(new Vector2(Screen.ScreenWidth, Screen.ScreenHeight), invCam);

            if (drawGrid)
            {
                int startX = (int)Math.Floor(topLeft.X / gridSize) * gridSize;
                int endX = (int)Math.Ceiling(bottomRight.X / gridSize) * gridSize;
                int startY = (int)Math.Floor(topLeft.Y / gridSize) * gridSize;
                int endY = (int)Math.Ceiling(bottomRight.Y / gridSize) * gridSize;

                for (int x = startX; x <= endX; x += gridSize)
                    Screen.spriteBatch.DrawLine(new Vector2(x, startY), new Vector2(x, endY), Color.DarkGray, 2);

                for (int y = startY; y <= endY; y += gridSize)
                    Screen.spriteBatch.DrawLine(new Vector2(startX, y), new Vector2(endX, y), Color.DarkGray, 2);
            }

            Screen.spriteBatch.DrawRectangle(0, 0, 800, 600, Color.White, 5);

            Screen.spriteBatch.DrawLine(new Vector2(0, topLeft.Y), new Vector2(0, bottomRight.Y), Color.White, 5f);
            Screen.spriteBatch.DrawLine(new Vector2(800, topLeft.Y), new Vector2(800, bottomRight.Y), Color.White, 5f);

            switch (mode)
            {
                case EditorMode.Main:
                    MainDraw();
                    break;
                case EditorMode.Path:
                    curveEditor.Draw(gameTime);
                    break;
                default:
                    break;
            }
        }

        void MainUpdate()
        {
            Vector2 mousePos = Screen.ScreenToWorldSpaceMouse();

            bool lmb = InputSystem.GetLMB();
            bool rmbDown = InputSystem.GetRMBDown();
            if (lmb || rmbDown)
            {
                if (!isDraggingNode)
                {
                    bool foundNode = false;

                    foreach (var spawner in level.Spawners)
                    {
                        RectangleF col = new RectangleF(spawner.Transform.Position.X - 16f, spawner.Transform.Position.Y - 16f, 32f, 32f);
                        if (col.Contains(Screen.ScreenToWorldSpaceMouse()))
                        {
                            if (lmb)
                                isDraggingNode = true;
                            selectedNode = spawner;
                            foundNode = true;
                            break;
                        }
                    }

                    if (!foundNode)
                    {
                        foreach (var eventNode in level.Events)
                        {
                            RectangleF col = new RectangleF(eventNode.Transform.Position.X - 16f, eventNode.Transform.Position.Y - 16f, 32f, 32f);
                            if (col.Contains(Screen.ScreenToWorldSpaceMouse()))
                            {
                                if (lmb)
                                    isDraggingNode = true;
                                selectedNode = eventNode;
                                foundNode = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (drawGrid)
                    {
                        mousePos.X = (int)Math.Floor(mousePos.X / gridSize) * gridSize;
                        mousePos.Y = (int)Math.Floor(mousePos.Y / gridSize) * gridSize;
                    }

                    selectedNode.Transform.Position = mousePos;
                    //UpdateUI();
                }
            }
            else if (isDraggingNode)
            {
                isDraggingNode = false;
            }

            if (InputSystem.GetKeyDown(Keys.Delete) && selectedNode != null)
            {
                if (selectedNode is EnemySpawner spawner)
                    level.RemoveSpawner(spawner);
                else if (selectedNode is EventNode eventN)
                    level.RemoveEvent(eventN);

                selectedNode = null;
                isDraggingNode = false;
            }

            if (InputSystem.GetKeyDown(Keys.R))
                Screen.ResetCamera();

            if (InputSystem.GetKeyDown(Keys.C))
                level.CreateSpawner(mousePos);

            if (InputSystem.GetKeyDown(Keys.V))
                level.CreateEvent(mousePos);
        }

        void MainDraw()
        {
            foreach (var spawner in level.Spawners)
            {
                if (spawner == selectedNode)
                    Screen.spriteBatch.DrawCircle(spawner.Transform.Position, 16f, 16, Color.Cyan, 16f);
                else
                    Screen.spriteBatch.DrawCircle(spawner.Transform.Position, 16f, 16, Color.Orange, 16f);
            }

            foreach (var spawner in level.Events)
            {
                if (spawner == selectedNode)
                    Screen.spriteBatch.DrawCircle(spawner.Transform.Position, 16f, 16, Color.Cyan, 16f);
                else
                    Screen.spriteBatch.DrawCircle(spawner.Transform.Position, 16f, 16, Color.Gray, 16f);
            }
        }

        public override void DrawImGui(GameTime gameTime)
        {
            switch (mode)
            {
                case EditorMode.Main:
                    NodeSettings();
                    break;
                case EditorMode.Path:
                    curveEditor.DrawImGui(gameTime);
                    break;
                default:
                    break;
            }

            BottomBar();
        }

        void NodeSettings()
        {
            ImGui.Begin("Node settings");
            if (selectedNode != null)
            {
                float pos = selectedNode.Transform.Position.X;
                if (ImGui.InputFloat("X", ref pos))
                {
                    selectedNode.Transform.Position = new Vector2(pos, selectedNode.Transform.Position.Y);
                }

                pos = selectedNode.Transform.Position.Y;
                if (ImGui.InputFloat("Y", ref pos))
                {
                    selectedNode.Transform.Position = new Vector2(selectedNode.Transform.Position.X, pos);
                }

                if (selectedNode is EnemySpawner spawner)
                {
                    string enemyId = spawner.EnemyId;
                    if(ImGui.InputText("Enemy ID", ref enemyId, 100))
                    {
                        spawner.EnemyId = enemyId;
                    }

                    bool followsCamera = spawner.FollowsCamera;
                    if(ImGui.Checkbox("Follows Camera", ref followsCamera))
                    {
                        spawner.FollowsCamera = followsCamera;
                    }

                    int enemyCount = spawner.EnemyCount;
                    if (ImGui.InputInt("Enemy Count", ref enemyCount))
                    {
                        spawner.EnemyCount = enemyCount;
                    }

                    float enemyDelay = spawner.DelayBetweenEnemies;
                    if (ImGui.InputFloat("Enemy Delay", ref enemyDelay))
                    {
                        spawner.DelayBetweenEnemies = enemyDelay;
                    }

                    bool hasPath = spawner.HasPath;
                    if (ImGui.Checkbox("Has Path", ref hasPath))
                    {
                        spawner.HasPath = hasPath;

                        if (spawner.HasPath)
                            spawner.Curve = new Curves.BezierCurve(new List<Vector2>() { spawner.Transform.Position, spawner.Transform.Position, spawner.Transform.Position, spawner.Transform.Position });
                        else
                            spawner.Curve = null;
                    }

                    if (spawner.HasPath)
                    {
                        if (ImGui.Button("Edit path"))
                        {
                            if (spawner.FollowsCamera)
                            {
                                mode = EditorMode.Path;
                                savedCameraPos = Screen.GetCameraPosition();
                                savedCameraZoom = Screen.GetCameraZoom();
                                savedCamera = true;
                                Screen.ResetCamera();
                                curveEditor.SetPath(spawner.Curve);
                            }
                            else
                            {
                                mode = EditorMode.Path;
                                curveEditor.SetPath(spawner.Curve);
                            }
                        }
                    }
                }
                else if (selectedNode is EventNode eventN)
                {
                    string eventId = eventN.EventId;
                    if (ImGui.InputText("Enemy ID", ref eventId, 100))
                    {
                        eventN.EventId = eventId;
                    }
                }
            }
            else
            {
                ImGui.Text("No node selected");
            }
            ImGui.End();
        }

        void BottomBar()
        {
            int bottomBarHeight = 35;
            int bottomBarWidth = 350;
            int bottomBarSpacing = 5;

            ImGui.SetNextWindowSize(new Numeric.Vector2(bottomBarWidth, bottomBarHeight));
            ImGui.SetNextWindowPos(new Numeric.Vector2(10, Screen.ScreenHeight - bottomBarHeight - bottomBarSpacing));
            ImGui.Begin("##Bottom Bar", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar);
            ImGui.Checkbox("Grid", ref drawGrid);
            ImGui.SameLine();
            if (ImGui.Button("Playtest"))
            {
                LevelManager.Playtest();
            }
            ImGui.SameLine();
            if(ImGui.Button("Create Spawner"))
            {
                level.CreateSpawner(Screen.GetCameraPosition());
            }
            ImGui.SameLine();
            if (ImGui.Button("Create Event"))
            {
                level.CreateEvent(Screen.GetCameraPosition());
            }
            ImGui.End();
        }

        internal void ReturnFromEditor()
        {
            mode = EditorMode.Main;
            if (savedCamera)
            {
                savedCamera = false;
                Screen.SetCameraZoom(savedCameraZoom);
                Screen.SetCameraPosition(savedCameraPos);
            }
        }
    }
}
