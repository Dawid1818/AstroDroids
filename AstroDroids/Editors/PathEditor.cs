using AstroDroids.Extensions;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Input;
using AstroDroids.Levels;
using AstroDroids.Paths;
using AstroDroids.Scenes;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AstroDroids.Editors
{
    public class PathEditor
    {
        EnemySpawner spawner;
        CompositePath Path;

        LevelEditorScene scene;

        PathPoint draggedPoint = null;
        bool isDraggingPoint = false;

        IPath selectedPath = null;

        int PathSelection = 0;

        public PathEditor(LevelEditorScene scene)
        {
            this.scene = scene;
        }

        public void SetPath(CompositePath path)
        {
            this.Path = path;
        }

        public void SetSpawner(EnemySpawner spawner)
        {
            this.spawner = spawner;
        }

        public void Update(GameTime gameTime)
        {
            Vector2 mousePos = Screen.ScreenToWorldSpaceMouse();
            List<IPath> paths = Path.Decompose();

            if (Path != null)
            {
                if (InputSystem.GetKeyDown(Keys.C))
                {
                    AddNewSubpath(mousePos);
                }

                if (InputSystem.GetKeyDown(Keys.Delete) && selectedPath != null)
                {
                    DeleteSelectedPath();
                }

                if (InputSystem.GetKeyDown(Keys.D1))
                {
                    PathSelection = 0;
                }
                else if (InputSystem.GetKeyDown(Keys.D2))
                {
                    PathSelection = 1;
                }

                if (InputSystem.GetLMB())
                {
                    if (!isDraggingPoint)
                    {
                        bool first = true;

                        foreach (var path in paths)
                        {
                            var keyPoints = path.KeyPoints;
                            for (int i = 0; i < keyPoints.Length; i++)
                            {
                                if (i == 0 && !first)
                                    continue;

                                PathPoint point = keyPoints[i];
                                RectangleF col = new RectangleF(point.X - 16f, point.Y - 16f, 32f, 32f);
                                if (col.Contains(Screen.ScreenToWorldSpaceMouse()))
                                {
                                    isDraggingPoint = true;
                                    draggedPoint = point;
                                    selectedPath = path;
                                    break;
                                }
                            }

                            if (isDraggingPoint)
                                break;

                            first = false;
                        }
                    }
                    else
                    {
                        if (scene.DrawGrid)
                        {
                            mousePos.X = (int)Math.Floor(mousePos.X / scene.gridSize) * scene.gridSize;
                            mousePos.Y = (int)Math.Floor(mousePos.Y / scene.gridSize) * scene.gridSize;
                        }

                        draggedPoint.X = mousePos.X;
                        draggedPoint.Y = mousePos.Y;

                        if (draggedPoint == selectedPath.EndPoint)
                        {
                            int ind = paths.IndexOf(selectedPath);
                            ind++;
                            if (ind < paths.Count)
                            {
                                paths[ind].StartPoint.X = mousePos.X;
                                paths[ind].StartPoint.Y = mousePos.Y;
                            }
                        }
                    }
                }
                else if (isDraggingPoint)
                {
                    isDraggingPoint = false;
                    draggedPoint = null;
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            PathVisualizer.DrawPath(Path, scene, spawner.Transform.Position, selectedPath);
        }

        public void DrawImGui(GameTime gameTime)
        {
            List<IPath> paths = Path.Decompose();

            ImGui.Begin("Path Editor");

            ImGui.SetNextItemWidth(-1);

            if (ImGui.BeginListBox("##Paths"))
            {
                for (int i = 0; i < paths.Count; i++)
                {
                    IPath path = paths[i];
                    if (ImGui.Selectable($"{GetPathTypeName(path)}##PathSelectable{i}", selectedPath == path))
                    {
                        selectedPath = path;
                    }
                }

                ImGui.EndListBox();
            }

            ImGui.SetNextItemWidth(-1);

            if (ImGui.BeginCombo("##PathCombo", GetPathTypeName(PathSelection)))
            {
                if (ImGui.Selectable("Line Path", PathSelection == 0))
                    PathSelection = 0;
                if (ImGui.Selectable("Bezier Path", PathSelection == 1))
                    PathSelection = 1;
                ImGui.EndCombo();
            }

            if (ImGui.Button("Add"))
            {
                AddNewSubpath(Vector2.Zero);
            }

            ImGui.SameLine();

            ImGui.BeginDisabled(selectedPath == null);
            if (ImGui.Button("Remove") && selectedPath != null)
            {
                DeleteSelectedPath();
            }
            ImGui.SameLine();
            if (ImGui.Button("Up") && selectedPath != null)
            {
                Path.Decompose().MoveItemUp(selectedPath);
            }
            ImGui.SameLine();
            if (ImGui.Button("Down") && selectedPath != null)
            {
                Path.Decompose().MoveItemDown(selectedPath);
            }
            ImGui.EndDisabled();

            if (ImGui.Button("Return"))
            {
                Path = null;
                scene.ReturnFromEditor();
            }

            ImGui.End();
        }

        void DeleteSelectedPath()
        {
            Path.Remove(selectedPath);
            selectedPath = null;

            List<IPath> paths = Path.Decompose();

            for (int i = 1; i < paths.Count; i++)
            {
                IPath path = paths[i];

                if (i != 0)
                {
                    if (paths[i - 1] is IPath prevPath)
                    {
                        path.StartPoint.X = prevPath.EndPoint.X;
                        path.StartPoint.Y = prevPath.EndPoint.Y;
                    }
                }
            }
        }

        void AddNewSubpath(Vector2 end)
        {
            IPath path = null;
            Vector2 start = Vector2.Zero;

            if (Path.Decompose().LastOrDefault() is IPath lastPath)
            {
                start.X = lastPath.EndPoint.X;
                start.Y = lastPath.EndPoint.Y;
            }

            switch (PathSelection)
            {
                case 0:
                default:
                    path = new LinePath(start, end);
                    break;
                case 1:
                    path = GameHelper.CreateBezier(start, end);
                    path.EndPoint.X = end.X;
                    path.EndPoint.Y = end.Y;
                    break;
            }

            Path.Add(path);
        }

        string GetPathTypeName(int id)
        {
            switch (id)
            {
                case 0:
                default:
                    return "Line Path";
                case 1:
                    return "Bezier Path";
            }
        }
        string GetPathTypeName(IPath path)
        {
            if (path is CompositePath)
                return "Composite Path";
            else if (path is LinePath)
                return "Line Path";
            else if (path is BezierPath)
                return "Bezier Path";
            else
                return "Unknown path";
        }
    }
}
