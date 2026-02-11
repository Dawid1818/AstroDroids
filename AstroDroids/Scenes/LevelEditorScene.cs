using AstroDroids.Drawables;
using AstroDroids.Editors;
using AstroDroids.Entities;
using AstroDroids.Gameplay;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Input;
using AstroDroids.Levels;
using AstroDroids.Managers;
using AstroDroids.Paths;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.IO;
using Numeric = System.Numerics;

namespace AstroDroids.Scenes
{
    enum EditorMode
    {
        Main,
        Path
    }

    enum BackgroundViewMode
    {
        Show,
        Darken,
        Hide
    }

    public class LevelEditorScene : Scene
    {
        EditorMode mode = EditorMode.Main;
        BackgroundViewMode bgViewMode = BackgroundViewMode.Show;
        string levelFileName = string.Empty;
        Level level { get { return LevelManager.CurrentLevel; } set { LevelManager.CurrentLevel = value; } }

        float cameraMoveSpeed = 5f;
        float cameraZoomSpeed = 0.2f;

        bool drawGrid = false;
        public bool DrawGrid { get { return drawGrid; } private set { drawGrid = value; } }

        public int gridSize = 32;

        LevelSettingsEditor levelSettingsEditor;
        PathEditor curveEditor;
        LevelBrowser levelBrowser;
        bool showLBModal = false;
        bool showSaveModal = false;
        bool showLevelSettings = false;

        bool isDraggingNode;
        bool isDraggingSpawnPosition;
        PathPoint selectedSpawnPoint;
        List<Entity> selectedNodes = new List<Entity>();

        Vector2 prevMousePos = Vector2.Zero;
        Vector2 selRectStart = Vector2.Zero;
        bool isDraggingSelRect = false;

        bool savedCamera = false;
        Vector2 savedCameraPos;
        float savedCameraZoom;

        int selectedEnemyType = 0;
        int selectedEnemy = -1;

        public LevelEditorScene()
        {
            World = new GameWorld();

            levelSettingsEditor = new LevelSettingsEditor(this);
            curveEditor = new PathEditor(this);
            levelBrowser = new LevelBrowser();
            levelBrowser.LevelSelected += (levelName) =>
            {
                levelFileName = levelName;
                level = new Level();
                FileSaver.RestoreObject(level, Path.Combine("Content/Levels/", levelName + ".adlvl"));

                isDraggingNode = false;
                isDraggingSelRect = false;
                isDraggingSpawnPosition = false;
                selectedSpawnPoint = null;
                selectedNodes.Clear();

                Screen.ResetCamera();
            };

            CreateNewLevel();
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

            if (!ImGui.GetIO().WantCaptureMouse)
            {
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

                if (scrollDelta > 0)
                {
                    Screen.ZoomCamera(cameraZoomSpeed);
                }
                else if (scrollDelta < 0)
                {
                    Screen.ZoomCamera(-cameraZoomSpeed);
                }
            }

            World.Starfield.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 cameraPos = Screen.GetCameraPosition();

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Screen.ScreenWidth, Screen.ScreenHeight, 0, 0, 1);
            Matrix uv_transform = Screen.GetUVTransform(TextureManager.GetStarfield(), new Vector2(0, 0), 1f, Screen.Viewport);

            Screen.Infinite.Parameters["view_projection"].SetValue(projection);
            Screen.Infinite.Parameters["uv_transform"].SetValue(Matrix.Invert(uv_transform));

            if (bgViewMode != BackgroundViewMode.Hide)
                World.Starfield.Draw();

            if (bgViewMode == BackgroundViewMode.Darken)
            {
                Screen.spriteBatch.Begin();
                Screen.spriteBatch.FillRectangle(new RectangleF(0, 0, Screen.ActualScreenWidth, Screen.ActualScreenHeight), new Color(0, 0, 0, 191));
                Screen.spriteBatch.End();
            }

            Screen.spriteBatch.Begin(transformMatrix: Screen.GetCameraMatrix());
            Matrix cam = Screen.GetCameraMatrix();
            Matrix invCam = Matrix.Invert(cam);

            Vector2 topLeft = Vector2.Transform(Vector2.Zero, invCam);
            Vector2 bottomRight = Vector2.Transform(new Vector2(Screen.ActualScreenWidth, Screen.ActualScreenHeight), invCam);

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

            Screen.spriteBatch.End();
        }

        void MainUpdate()
        {
            Vector2 mousePos = Screen.ScreenToWorldSpaceMouse();
            Vector2 deltaMousePos = mousePos - prevMousePos;

            if (ImGui.GetIO().WantCaptureMouse)
                return;

            bool lmb = InputSystem.GetLMB();
            bool rmbDown = InputSystem.GetRMBDown();
            if (lmb || rmbDown)
            {
                if (!isDraggingNode && !isDraggingSpawnPosition && !isDraggingSelRect)
                {
                    //bool foundNode = false;
                    Entity foundNode = null;

                    foreach (var spawner in level.Spawners)
                    {
                        RectangleF col;
                        if (!spawner.HasPath)
                        {
                            col = new RectangleF(spawner.SpawnPosition.X - 16f, spawner.SpawnPosition.Y - 16f, 32f, 32f);
                            if (col.Contains(Screen.ScreenToWorldSpaceMouse()))
                            {
                                if (lmb)
                                {
                                    isDraggingNode = false;
                                    isDraggingSpawnPosition = true;
                                }
                                foundNode = spawner;
                                selectedSpawnPoint = spawner.SpawnPosition;
                                break;
                            }
                        }

                        col = new RectangleF(spawner.Transform.Position.X - 16f, spawner.Transform.Position.Y - 16f, 32f, 32f);
                        if (col.Contains(Screen.ScreenToWorldSpaceMouse()))
                        {
                            if (lmb)
                            {
                                isDraggingNode = true;
                                isDraggingSpawnPosition = false;
                            }
                            foundNode = spawner;
                            selectedSpawnPoint = null;
                            break;
                        }
                    }

                    if (foundNode == null)
                    {
                        foreach (var eventNode in level.Events)
                        {
                            RectangleF col = new RectangleF(eventNode.Transform.Position.X - 16f, eventNode.Transform.Position.Y - 16f, 32f, 32f);
                            if (col.Contains(Screen.ScreenToWorldSpaceMouse()))
                            {
                                if (lmb)
                                {
                                    isDraggingNode = true;
                                    isDraggingSpawnPosition = false;
                                }

                                foundNode = eventNode;
                                selectedSpawnPoint = null;
                                break;
                            }
                        }
                    }

                    if (foundNode == null)
                    {
                        selectedNodes.Clear();
                        selectedSpawnPoint = null;

                        if (lmb && !isDraggingSelRect)
                        {
                            isDraggingSelRect = true;
                            selRectStart = mousePos;
                        }
                    }
                    else
                    {
                        selRectStart = mousePos;

                        if (!selectedNodes.Contains(foundNode))
                        {
                            selectedNodes.Clear();
                            selectedNodes.Add(foundNode);
                        }
                    }
                }
                else
                {
                    if (drawGrid)
                    {
                        mousePos.X = (int)Math.Floor(mousePos.X / gridSize) * gridSize;
                        mousePos.Y = (int)Math.Floor(mousePos.Y / gridSize) * gridSize;

                        deltaMousePos = mousePos - prevMousePos;
                    }

                    if (isDraggingSpawnPosition)
                    {
                        selectedSpawnPoint.X = mousePos.X;
                        selectedSpawnPoint.Y = mousePos.Y;
                    }
                    else if (isDraggingNode)
                    {
                        foreach (var selectedNode in selectedNodes)
                        {
                            if (selectedNode is EnemySpawner spawner && !spawner.FollowsCamera && !InputSystem.GetKey(Keys.LeftShift))
                            {
                                if (spawner.HasPath)
                                {
                                    spawner.Path.Translate(deltaMousePos);
                                }
                                else
                                {
                                    spawner.SpawnPosition += deltaMousePos;
                                }
                            }

                            selectedNode.Transform.Position += deltaMousePos;
                        }
                    }
                    //UpdateUI();
                }
            }
            else if (isDraggingNode || isDraggingSpawnPosition || isDraggingSelRect)
            {
                isDraggingNode = false;
                isDraggingSpawnPosition = false;

                if (isDraggingSelRect)
                {
                    isDraggingSelRect = false;
                    RectangleF selectionRect = new RectangleF(
                        Math.Min(selRectStart.X, mousePos.X),
                        Math.Min(selRectStart.Y, mousePos.Y),
                        Math.Abs(mousePos.X - selRectStart.X),
                        Math.Abs(mousePos.Y - selRectStart.Y)
                    );
                    selectedNodes.Clear();
                    foreach (var spawner in level.Spawners)
                    {
                        RectangleF col = new RectangleF(spawner.Transform.Position.X - 16f, spawner.Transform.Position.Y - 16f, 32f, 32f);
                        if (selectionRect.Intersects(col))
                        {
                            selectedNodes.Add(spawner);
                        }
                    }
                    foreach (var eventNode in level.Events)
                    {
                        RectangleF col = new RectangleF(eventNode.Transform.Position.X - 16f, eventNode.Transform.Position.Y - 16f, 32f, 32f);
                        if (selectionRect.Intersects(col))
                        {
                            selectedNodes.Add(eventNode);
                        }
                    }
                }
            }

            if (InputSystem.GetKeyDown(Keys.Delete) && selectedNodes.Count > 0)
            {
                foreach (var selectedNode in selectedNodes)
                {
                    if (selectedNode is EnemySpawner spawner)
                        level.RemoveSpawner(spawner);
                    else if (selectedNode is EventNode eventN)
                        level.RemoveEvent(eventN);
                }

                selectedNodes.Clear();
                isDraggingNode = false;
            }

            if (InputSystem.GetKeyDown(Keys.R))
                Screen.ResetCamera();

            if (InputSystem.GetKeyDown(Keys.C))
                level.CreateSpawner(mousePos);

            if (InputSystem.GetKeyDown(Keys.V))
                level.CreateEvent(mousePos);

            if (InputSystem.GetKeyDown(Keys.T))
                LevelManager.Playtest(mousePos.Y);

            prevMousePos = mousePos;
        }

        void MainDraw()
        {
            Vector2 mousePos = Screen.ScreenToWorldSpaceMouse();

            foreach (var spawner in level.Spawners)
            {
                //if (spawner == selectedNode)
                //{
                //    if (!spawner.HasPath)
                //        Screen.spriteBatch.DrawLine(spawner.Transform.Position, spawner.SpawnPosition, Color.Green, 4f);

                //    Screen.spriteBatch.DrawCircle(spawner.Transform.Position, 16f, 16, Color.Cyan, 16f);

                //    if (!spawner.HasPath)
                //    {
                //        Screen.spriteBatch.DrawCircle(spawner.SpawnPosition, 16f, 16, Color.Red, 16f);
                //    }
                //}
                //else
                //{
                if (!spawner.HasPath)
                    Screen.spriteBatch.DrawLine(spawner.Transform.Position, spawner.SpawnPosition, Color.Green, 4f);
                else
                {
                    PathVisualizer.DrawPath(spawner.Path);
                }

                Screen.spriteBatch.DrawCircle(spawner.Transform.Position, 16f, 16, selectedNodes.Contains(spawner) ? Color.Cyan : Color.Orange, 16f);

                if (!spawner.HasPath)
                {
                    Screen.spriteBatch.DrawCircle(spawner.SpawnPosition, 16f, 16, Color.Red, 16f);
                }
                //}
            }

            foreach (var spawner in level.Events)
            {
                if (selectedNodes.Contains(spawner))
                    Screen.spriteBatch.DrawCircle(spawner.Transform.Position, 16f, 16, Color.Cyan, 16f);
                else
                    Screen.spriteBatch.DrawCircle(spawner.Transform.Position, 16f, 16, Color.Gray, 16f);
            }

            if (isDraggingSelRect)
            {
                RectangleF selectionRect = new RectangleF(
                    Math.Min(selRectStart.X, mousePos.X),
                    Math.Min(selRectStart.Y, mousePos.Y),
                    Math.Abs(mousePos.X - selRectStart.X),
                    Math.Abs(mousePos.Y - selRectStart.Y)
                );

                Screen.spriteBatch.DrawRectangle(selectionRect, Color.Cyan, 2f);
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

            MenuBar();
            BottomBar();

            if (showLevelSettings)
                levelSettingsEditor.DrawImGui(ref showLevelSettings);

            if (showLBModal)
            {
                levelBrowser.ShowModal();
                showLBModal = false;
            }
            levelBrowser.DrawImGui();

            if (showSaveModal)
            {
                ImGui.OpenPopup("Save Level");
                showSaveModal = false;
            }
            if (ImGui.BeginPopupModal("Save Level", ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.InputText("File name", ref levelFileName, 100);
                if (ImGui.Button("Save"))
                {
                    if (!string.IsNullOrWhiteSpace(levelFileName))
                    {
                        levelFileName = levelFileName.Trim();
                        SaveLevel(levelFileName);
                        ImGui.CloseCurrentPopup();
                    }
                }

                ImGui.SameLine();

                if (ImGui.Button("Cancel"))
                {
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }
        }

        void NodeSettings()
        {
            ImGui.Begin("Node settings");
            if (selectedNodes.Count > 1)
            {
                ImGui.Text("Node multi-editing is not supported");
            }
            else if (selectedNodes.Count == 1)
            {
                Entity selectedNode = selectedNodes[0];

                float pos = selectedNode.Transform.Position.X;

                ImGui.SeparatorText("Transform settings");

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
                    ImGui.SeparatorText("Spawner settings");

                    ImGui.Text("Enemies");
                    List<Type> enemyList = EntityDatabase.GetAllEnemyTypes();
                    if (ImGui.BeginListBox("##EnemyList", new Numeric.Vector2(-1,0)))
                    {
                        for (int i = 0; i < spawner.EnemyIDs.Count; i++)
                        {
                            int enemyId = spawner.EnemyIDs[i];
                            if (ImGui.Selectable($"{enemyList[enemyId].Name}##EnemyID{i}", i == selectedEnemy))
                            {
                                selectedEnemy = i;
                            }
                        }

                        ImGui.EndListBox();
                    }

                    if (ImGui.BeginCombo("##EnemyCombo", enemyList[selectedEnemyType].Name))
                    {
                        for (int i = 0; i < enemyList.Count; i++)
                        {
                            if (ImGui.Selectable(enemyList[i].Name, i == selectedEnemyType))
                            {
                                selectedEnemyType = i;
                            }
                        }

                        ImGui.EndCombo();
                    }

                    ImGui.SameLine();

                    if (ImGui.Button("Add"))
                    {
                        spawner.EnemyIDs.Add(selectedEnemyType);
                    }

                    //string enemyId = spawner.EnemyId;
                    //if (ImGui.InputText("Enemy ID", ref enemyId, 100))
                    //{
                    //    spawner.EnemyId = enemyId;
                    //}

                    bool followsCamera = spawner.FollowsCamera;
                    if (ImGui.Checkbox("Follows Camera", ref followsCamera))
                    {
                        spawner.FollowsCamera = followsCamera;
                    }

                    //int enemyCount = spawner.EnemyCount;
                    //if (ImGui.InputInt("Enemy Count", ref enemyCount))
                    //{
                    //    spawner.EnemyCount = enemyCount;
                    //}

                    float enemyDelay = spawner.DelayBetweenEnemies;
                    if (ImGui.InputFloat("Enemy Delay", ref enemyDelay))
                    {
                        spawner.DelayBetweenEnemies = enemyDelay;
                    }

                    ImGui.SeparatorText("Path settings");

                    bool hasPath = spawner.HasPath;
                    if (ImGui.Checkbox("Has Path", ref hasPath))
                    {
                        spawner.HasPath = hasPath;

                        if (spawner.HasPath)
                        {
                            CompositePath path = new CompositePath();
                            spawner.Path = path;
                            spawner.SpawnPosition = null;
                            //path.Add(new LinePath(spawner.Transform.Position, spawner.Transform.Position + new Vector2(100, 0)));
                            path.Add(new BezierPath(new List<PathPoint>() { PathPoint.Zero, PathPoint.Zero, PathPoint.Zero, PathPoint.Zero }));
                        }
                        else
                        {
                            spawner.Path = null;
                            spawner.SpawnPosition = spawner.Transform.Position;
                        }
                    }

                    if (spawner.HasPath)
                    {
                        float speed = spawner.PathSpeed;
                        if (ImGui.InputFloat("Speed", ref speed))
                        {
                            spawner.PathSpeed = speed;
                        }

                        LoopingMode loopMode = spawner.PathLoop;
                        if (ImGui.BeginCombo("Looping Mode", loopMode.ToString()))
                        {
                            foreach (var mode in Enum.GetValues<LoopingMode>())
                            {
                                bool isSelected = mode == spawner.PathLoop;
                                if (ImGui.Selectable(mode.ToString(), isSelected))
                                {
                                    spawner.PathLoop = mode;
                                }
                                if (isSelected)
                                    ImGui.SetItemDefaultFocus();
                            }
                            ImGui.EndCombo();
                        }

                        int minPath = spawner.MinPath;
                        if (ImGui.InputInt("Min Path", ref minPath))
                        {
                            spawner.MinPath = Math.Clamp(minPath, -1, spawner.Path.Decompose().Count);
                        }

                        if (ImGui.Button("Edit path"))
                        {
                            if (spawner.FollowsCamera)
                            {
                                mode = EditorMode.Path;
                                savedCameraPos = Screen.GetCameraPosition();
                                savedCameraZoom = Screen.GetCameraZoom();
                                savedCamera = true;
                                Screen.ResetCamera();
                                curveEditor.SetPath(spawner.Path);
                                curveEditor.SetSpawner(spawner);
                            }
                            else
                            {
                                mode = EditorMode.Path;
                                curveEditor.SetPath(spawner.Path);
                                curveEditor.SetSpawner(spawner);
                            }
                        }
                    }
                }
                else if (selectedNode is EventNode eventN)
                {
                    ImGui.SeparatorText("Event settings");

                    string eventId = eventN.EventId;
                    if (ImGui.InputText("Event ID", ref eventId, 100))
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

        void MenuBar()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("New"))
                    {
                        CreateNewLevel();
                    }

                    if (ImGui.MenuItem("Open"))
                    {
                        showLBModal = true;
                    }

                    if (ImGui.MenuItem("Save"))
                    {
                        if (string.IsNullOrWhiteSpace(levelFileName))
                        {
                            showSaveModal = true;
                        }
                        else
                        {
                            SaveLevel(levelFileName);
                        }
                    }

                    if (ImGui.MenuItem("Save as"))
                    {
                        showSaveModal = true;
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Level"))
                {
                    if (ImGui.MenuItem("Settings"))
                    {
                        showLevelSettings = true;
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("View"))
                {
                    if (ImGui.BeginMenu("Background"))
                    {
                        if (ImGui.MenuItem("Show", bgViewMode == BackgroundViewMode.Show))
                        {
                            bgViewMode = BackgroundViewMode.Show;
                        }

                        if (ImGui.MenuItem("Darken", bgViewMode == BackgroundViewMode.Darken))
                        {
                            bgViewMode = BackgroundViewMode.Darken;
                        }

                        if (ImGui.MenuItem("Hide", bgViewMode == BackgroundViewMode.Hide))
                        {
                            bgViewMode = BackgroundViewMode.Hide;
                        }

                        ImGui.EndMenu();
                    }

                    ImGui.EndMenu();
                }


                ImGui.EndMainMenuBar();
            }
        }
        void BottomBar()
        {
            int bottomBarHeight = 55;
            int bottomBarWidth = 355;
            int bottomBarSpacing = 5;

            ImGui.SetNextWindowSize(new Numeric.Vector2(bottomBarWidth, bottomBarHeight));
            ImGui.SetNextWindowPos(new Numeric.Vector2(10, Screen.ActualScreenHeight - bottomBarHeight - bottomBarSpacing));
            ImGui.Begin("##Bottom Bar", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar);
            Vector2 mousePos = Screen.ScreenToWorldSpaceMouse();
            ImGui.Text($"Mouse X:{mousePos.X.ToString("0.00")} Y:{mousePos.Y.ToString("0.00")}");
            ImGui.Checkbox("Grid", ref drawGrid);
            ImGui.SameLine();
            if (ImGui.Button("Playtest"))
            {
                LevelManager.Playtest(0f);
            }
            ImGui.SameLine();
            if (ImGui.Button("Create Spawner"))
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

        void CreateNewLevel()
        {
            isDraggingNode = false;
            isDraggingSelRect = false;
            isDraggingSpawnPosition = false;
            selectedSpawnPoint = null;
            selectedNodes.Clear();

            levelFileName = string.Empty;
            level = new Level();

            World.Starfield = new SimulationStarfield();

            Screen.ResetCamera();
        }

        void SaveLevel(string path)
        {
            FileSaver.SaveObject(level, Path.Combine("Content/Levels/", path + ".adlvl"));
        }
    }
}
