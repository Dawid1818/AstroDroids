using AstroDroids.Drawables;
using AstroDroids.Editors;
using AstroDroids.Gameplay;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Input;
using AstroDroids.Levels;
using AstroDroids.Managers;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.IO;
using Numeric = System.Numerics;

namespace AstroDroids.Scenes
{
    public enum EditorMode
    {
        Main,
        Path,
        Barrier
    }

    enum BackgroundViewMode
    {
        Show,
        Darken,
        Hide
    }

    public class LevelEditorScene : Scene
    {
        public EditorMode mode { get; set; } = EditorMode.Main;
        BackgroundViewMode bgViewMode = BackgroundViewMode.Show;
        string levelFileName = string.Empty;
        Level level { get { return LevelManager.CurrentLevel; } set { LevelManager.CurrentLevel = value; } }

        float cameraMoveSpeed = 5f;
        float cameraZoomSpeed = 0.2f;

        bool drawGrid = false;
        public bool DrawGrid { get { return drawGrid; } private set { drawGrid = value; } }

        public int gridSize = 32;

        LevelSettingsEditor levelSettingsEditor;
        public PathEditor curveEditor { get; private set; }
        AttackWaveEditor waveEditor;
        PathBrowser pathBrowser;
        public LaserBarrierEditor barrierEditor { get; private set; }
        LevelBrowser levelBrowser;
        bool showLBModal = false;
        bool showSaveModal = false;
        bool showLevelSettings = false;
        bool showPathBrowser = false;

        bool savedCamera = false;
        Vector2 savedCameraPos;
        float savedCameraZoom;

        float savedTimer = 0f;

        public LevelEditorScene()
        {
            EntityDatabase.InitializePreviews();

            World = new GameWorld();

            levelSettingsEditor = new LevelSettingsEditor(this);
            curveEditor = new PathEditor(this);
            barrierEditor = new LaserBarrierEditor(this);
            waveEditor = new AttackWaveEditor(this);
            levelBrowser = new LevelBrowser();
            levelBrowser.LevelSelected += LoadLevel;

            pathBrowser = new PathBrowser(this);

            CreateNewLevel();

            //temporary
            LoadLevel("SnakeBoss");
        }

        public void LoadLevel(string name)
        {
            levelFileName = name;
            level = new Level();
            FileSaver.RestoreObject(level, Path.Combine("Content/Levels/", name + ".adlvl"));

            waveEditor.Reset();
            pathBrowser.Reset();

            RestoreStarfield();

            Screen.ResetCamera();
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
                case EditorMode.Barrier:
                    barrierEditor.Update(gameTime);
                    break;
                default:
                    break;
            }

            if (!ImGui.GetIO().WantCaptureMouse)
            {
                Vector2 cameraTranslation = Vector2.Zero;

                if (!InputSystem.GetKey(Keys.LeftControl))
                {
                    if (InputSystem.GetKey(Keys.W))
                        cameraTranslation.Y -= cameraMoveSpeed;

                    if (InputSystem.GetKey(Keys.S))
                        cameraTranslation.Y += cameraMoveSpeed;

                    if (InputSystem.GetKey(Keys.A))
                        cameraTranslation.X -= cameraMoveSpeed;

                    if (InputSystem.GetKey(Keys.D))
                        cameraTranslation.X += cameraMoveSpeed;

                    if (InputSystem.GetKeyDown(Keys.G))
                        DrawGrid = !DrawGrid;
                }

                if(InputSystem.GetKey(Keys.LeftControl))
                {
                    if (InputSystem.GetKeyDown(Keys.S))
                    {
                        if (string.IsNullOrWhiteSpace(levelFileName))
                        {
                            showSaveModal = true;
                        }
                        else
                        {
                            SaveLevel(levelFileName);
                        }
                    }else if(InputSystem.GetKeyDown(Keys.O))
                    {
                        showLBModal = true;
                    }
                }

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

            if(savedTimer > 0)
                savedTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(GameTime gameTime)
        {
            if (mode == EditorMode.Main)
            {
                waveEditor.DrawPreviews();
                pathBrowser.DrawPreviews();
            }

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

            Screen.spriteBatch.Begin(transformMatrix: Screen.GetCameraMatrix(), blendState: BlendState.AlphaBlend);
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

            Screen.spriteBatch.DrawCircle(new Vector2(400, 300), 2, 12, Color.White, 2);

            Screen.spriteBatch.DrawLine(new Vector2(0, topLeft.Y), new Vector2(0, bottomRight.Y), Color.White, 5f);
            Screen.spriteBatch.DrawLine(new Vector2(800, topLeft.Y), new Vector2(800, bottomRight.Y), Color.White, 5f);

            switch (mode)
            {
                case EditorMode.Main:
                    MainDraw(gameTime);
                    break;
                case EditorMode.Path:
                    curveEditor.Draw(gameTime);
                    break;
                case EditorMode.Barrier:
                    barrierEditor.Draw(gameTime);
                    break;
                default:
                    break;
            }

            Screen.spriteBatch.End();
        }

        void MainUpdate()
        {
            waveEditor.Update();

            if (InputSystem.GetKeyDown(Keys.R))
                Screen.ResetCamera();
        }

        void MainDraw(GameTime gameTime)
        {
            waveEditor.Draw(gameTime);
        }

        public override void DrawImGui(GameTime gameTime)
        {
            switch (mode)
            {
                case EditorMode.Main:
                    NodeSettings();

                    if(showPathBrowser)
                        pathBrowser.DrawImGui(gameTime, ref showPathBrowser);
                    break;
                case EditorMode.Path:
                    curveEditor.DrawImGui(gameTime);
                    break;
                case EditorMode.Barrier:
                    barrierEditor.DrawImGui(gameTime);
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

            ImGuiIOPtr io = ImGui.GetIO();

            if (ImGui.IsPopupOpen("Save Level"))
                ImGui.SetNextWindowPos(new Numeric.Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Always, new Numeric.Vector2(0.5f, 0.5f));
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

            if(savedTimer > 0)
            {
                ImGui.SetNextWindowPos(new Numeric.Vector2(10, 30));

                ImGui.PushStyleVar(ImGuiStyleVar.Alpha, savedTimer);
                ImGui.Begin("Level Saved", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize);
                ImGui.Text("Level saved!");
                ImGui.End();
                ImGui.PopStyleVar();
            }
        }

        void NodeSettings()
        {
            waveEditor.DrawImGui();
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
                        showLevelSettings = !showLevelSettings;
                    }

                    if (ImGui.MenuItem("Paths"))
                    {
                        showPathBrowser = !showPathBrowser;
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
            int bottomBarHeight = 70;
            int bottomBarWidth = 553;
            int bottomBarSpacing = 5;

            ImGui.SetNextWindowSize(new Numeric.Vector2(bottomBarWidth, bottomBarHeight));
            ImGui.SetNextWindowPos(new Numeric.Vector2(10, Screen.ActualScreenHeight - bottomBarHeight - bottomBarSpacing));
            ImGui.Begin("##Bottom Bar", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar);
            Vector2 mousePos = Screen.ScreenToWorldSpaceMouse();
            ImGui.Text($"Mouse X:{mousePos.X.ToString("0.00")} Y:{mousePos.Y.ToString("0.00")}");
            ImGui.Text($"Dist X:{800 - mousePos.X} Dist Y:{600 - mousePos.Y}");
            ImGui.Checkbox("Grid", ref drawGrid);
            ImGui.SameLine();
            if (ImGui.Button("Playtest"))
            {
                LevelManager.Playtest(0);
            }
            ImGui.SameLine();
            if (mode != EditorMode.Main || waveEditor.wave == null)
                ImGui.BeginDisabled();
            if (ImGui.Button("Create Spawner"))
            {
                waveEditor.AllNodes.Add(waveEditor.wave.CreateSpawner(Screen.GetCameraPosition()));
            }
            ImGui.SameLine();
            if (ImGui.Button("Create Event"))
            {
                waveEditor.AllNodes.Add(waveEditor.wave.CreateEvent(Screen.GetCameraPosition()));
            }
            ImGui.SameLine();
            if (ImGui.Button("Create LBarrier"))
            {
                waveEditor.AllNodes.Add(waveEditor.wave.CreateLaserBarrier(Screen.GetCameraPosition()));
            }
            ImGui.SameLine();
            if (ImGui.Button("Create BG"))
            {
                waveEditor.AllNodes.Add(waveEditor.wave.CreateBackgroundObject(Screen.GetCameraPosition()));
            }
            if (mode != EditorMode.Main || waveEditor.wave == null)
                ImGui.EndDisabled();
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
            waveEditor.Reset();
            pathBrowser.Reset();

            levelFileName = string.Empty;
            level = new Level();

            RestoreStarfield();

            Screen.ResetCamera();
        }

        void RestoreStarfield()
        {
            if (LevelManager.CurrentLevel.BackgroundId == 0)
            {
                World.Starfield = new SimulationStarfield();
            }
            else
            {
                List<Texture2D> starfields = TextureManager.GetStarfields();
                World.Starfield = new ImageStarfield(starfields[LevelManager.CurrentLevel.BackgroundId - 1]);
            }
        }

        void SaveLevel(string path)
        {
            FileSaver.SaveObject(level, Path.Combine("Content/Levels/", path + ".adlvl"));
            savedTimer = 3f;
        }
    }
}
