using AstroDroids.Extensions;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Levels;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Numeric = System.Numerics;

namespace AstroDroids.Editors
{
    public class PathBrowser
    {
        Level level { get { return LevelManager.CurrentLevel; } set { LevelManager.CurrentLevel = value; } }
        NamedPath selectedPath = null;

        LevelEditorScene scene;

        RenderTarget2D pathPreview;
        ImTextureRef pathRef;
        NamedPath pathHovered;
        public PathBrowser(LevelEditorScene scene)
        {
            this.scene = scene;

            Rectangle source = scene.World.Bounds;
            Rectangle bounds = new Rectangle(source.X, source.Y, source.Width, source.Height);
            bounds.Inflate(80, 80);

            pathPreview = new RenderTarget2D(Screen.GetGraphicsManager().GraphicsDevice, bounds.Width, bounds.Height);

            pathRef = Screen.GetImGuiRenderer().BindTexture(pathPreview);
        }

        public void DrawImGui(GameTime gameTime, ref bool show)
        {
            if (ImGui.Begin("Path Browser", ref show))
            {
                Vector2 windowPos = ImGui.GetWindowPos();

                Vector2 availableSpace = ImGui.GetContentRegionAvail();

                if (ImGui.BeginListBox("##PathList", new Numeric.Vector2(-1, availableSpace.Y - 120)))
                {
                    for (int i = 0; i < level.Paths.Count; i++)
                    {
                        NamedPath thisPath = level.Paths[i];

                        string label = $"{i} - {thisPath.Name}";

                        if (ImGui.Selectable($"{label}##Path{i}", thisPath == selectedPath))
                        {
                            if (selectedPath != thisPath)
                            {
                                selectedPath = thisPath;
                            }
                        }

                        if (ImGui.IsItemHovered())
                        {
                            Vector2 windowSize = ImGui.GetWindowSize();

                            ImGui.SetNextWindowPos(new Numeric.Vector2(
                                windowPos.X + windowSize.X + 30,
                                windowPos.Y));

                            ImGui.BeginTooltip();

                            ImGui.Text("Path");
                            ImGui.Text(label);
                            pathHovered = thisPath;

                            ImGui.Image(pathRef, new Numeric.Vector2(440, 340));

                            ImGui.EndTooltip();
                        }
                    }

                    ImGui.EndListBox();
                }

                if (ImGui.Button("Add"))
                {
                    level.CreatePath();
                }

                ImGui.SameLine();

                ImGui.BeginDisabled(selectedPath == null);
                if (ImGui.Button("Edit") && selectedPath != null)
                {
                    scene.mode = EditorMode.Path;
                    scene.curveEditor.SetPath(selectedPath.Path);
                }

                ImGui.SameLine();

                if (ImGui.Button("Remove") && selectedPath != null)
                {
                    level.RemovePath(selectedPath);
                    selectedPath = null;
                }
                ImGui.SameLine();
                if (ImGui.Button("Up") && selectedPath != null)
                {
                    level.Paths.MoveItemUp(selectedPath);
                }
                ImGui.SameLine();
                if (ImGui.Button("Down") && selectedPath != null)
                {
                    level.Paths.MoveItemDown(selectedPath);
                }
                ImGui.SameLine();
                if (ImGui.Button("Duplicate") && selectedPath != null)
                {
                    NamedPath newPath = new NamedPath();
                    FileSaver.CloneObject(selectedPath, newPath);
                    int index = level.Paths.IndexOf(selectedPath);
                    level.Paths.Insert(index + 1, newPath);
                }

                ImGui.EndDisabled();

                ImGui.SeparatorText("Path settings");

                if (selectedPath != null)
                {
                    string name = selectedPath.Name;
                    if (ImGui.InputText("Name", ref name, 1024))
                    {
                        selectedPath.Name = name;
                    }
                }
                else
                {
                    ImGui.Text("No path selected");
                }


                ImGui.End();
            }
        }

        public void DrawPreview(NamedPath path)
        {
            GraphicsDeviceManager manager = Screen.GetGraphicsManager();
            manager.GraphicsDevice.SetRenderTarget(pathPreview);

            Vector2 screenCenter = new Vector2(manager.GraphicsDevice.Viewport.Width / 2f, manager.GraphicsDevice.Viewport.Height / 2f);

            Matrix m = Matrix.CreateTranslation(-(Screen.ScreenWidth / 2f), -(Screen.ScreenHeight / 2f), 0)
             * Matrix.CreateScale(1f)
             * Matrix.CreateTranslation(screenCenter.X, screenCenter.Y, 0);

            Screen.spriteBatch.Begin(transformMatrix: m);
            Screen.spriteBatch.DrawRectangle(0, 0, 800, 600, Color.White, 5);
            PathVisualizer.DrawPath(path.Path);
            Screen.spriteBatch.End();

            manager.GraphicsDevice.SetRenderTarget(null);
        }

        public void DrawPreviews()
        {
            if (pathHovered != null)
            {
                DrawPreview(pathHovered);
            }
        }

        public void Reset()
        {
            selectedPath = null;
        }
    }
}
