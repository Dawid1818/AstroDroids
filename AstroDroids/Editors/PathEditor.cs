using AstroDroids.Entities;
using AstroDroids.Extensions;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Input;
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
        Entity spawner;
        CompositePath Path;

        LevelEditorScene scene;

        PathPoint draggedPoint = null;
        bool isDraggingPoint = false;

        IPath selectedPath = null;
        PathPoint selectedPoint = null;

        int PathSelection = 0;

        float shiftX = 0;
        float shiftY = 0;

        public PathEditor(LevelEditorScene scene)
        {
            this.scene = scene;
        }

        public void SetPath(CompositePath path)
        {
            this.Path = path;
            selectedPath = null;
            selectedPoint = null;
        }

        public void SetSpawner(Entity spawner)
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
                    if (scene.DrawGrid)
                    {
                        mousePos.X = (int)Math.Floor(mousePos.X / scene.gridSize) * scene.gridSize;
                        mousePos.Y = (int)Math.Floor(mousePos.Y / scene.gridSize) * scene.gridSize;
                    }

                    if (InputSystem.GetKey(Keys.LeftShift))
                        AddNewSubpath(mousePos);
                    else if (selectedPath != null)
                    {
                        PathPoint newPoint = new PathPoint(mousePos.X, mousePos.Y);
                        selectedPath.KeyPoints.Add(newPoint);
                        selectedPath.RecalculateLength();
                        Path.RecalculateLength();
                        SyncPaths();

                        selectedPoint = newPoint;
                    }

                }

                if (InputSystem.GetKeyDown(Keys.Delete))
                {
                    if (InputSystem.GetKey(Keys.LeftShift))
                    {
                        if (selectedPath != null)
                            DeleteSelectedPath();
                    }
                    else if (selectedPoint != null)
                    {
                        if (selectedPath.KeyPoints.Count > selectedPath.MinimumPoints)
                        {
                            selectedPath.KeyPoints.Remove(selectedPoint);
                            selectedPoint = null;
                            selectedPath.RecalculateLength();
                            SyncPaths();
                        }
                    }

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
                            for (int i = 0; i < keyPoints.Count; i++)
                            {
                                if (i == 0 && !first)
                                    continue;

                                PathPoint point = keyPoints[i];
                                RectangleF col = new RectangleF(point.X - 16f, point.Y - 16f, 32f, 32f);
                                if (col.Contains(Screen.ScreenToWorldSpaceMouse()))
                                {
                                    isDraggingPoint = true;
                                    draggedPoint = point;

                                    selectedPoint = point;
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

                        Path.RecalculateLength();
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
            if (Path != null)
            {
                if (spawner != null)
                    PathVisualizer.DrawPath(Path, spawner.Transform.Position, selectedPath);
                else
                    PathVisualizer.DrawPath(Path, selectedPath: selectedPath);
            }
        }

        public void DrawImGui(GameTime gameTime)
        {
            if (Path == null)
                return;

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
                        if (selectedPath != path)
                            selectedPoint = null;

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
                if (ImGui.Selectable("Catmull-Rom Path", PathSelection == 2))
                    PathSelection = 2;
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
                selectedPath = null;
                selectedPoint = null;
                scene.ReturnFromEditor();
            }

            ImGui.End();

            if (ImGui.Begin("Path settings"))
            {
                if (selectedPath != null)
                {
                    ImGui.SeparatorText("Path points");

                    ImGui.SetNextItemWidth(-1);

                    if (ImGui.BeginListBox("##Points"))
                    {
                        for (int i = 0; i < selectedPath.KeyPoints.Count; i++)
                        {
                            PathPoint point = selectedPath.KeyPoints[i];
                            if (ImGui.Selectable($"Point {i}##PointSelectable{i}", selectedPoint == point))
                            {
                                selectedPoint = point;
                            }
                        }

                        ImGui.EndListBox();
                    }

                    if (ImGui.Button("Add"))
                    {
                        PathPoint newPoint = PathPoint.Zero;
                        selectedPath.KeyPoints.Add(newPoint);
                        selectedPath.RecalculateLength();
                        Path.RecalculateLength();
                        SyncPaths();

                        selectedPoint = newPoint;
                    }

                    ImGui.SameLine();

                    ImGui.BeginDisabled(selectedPoint == null);

                    if (ImGui.Button("Remove") && selectedPoint != null)
                    {
                        if (selectedPath.KeyPoints.Count > selectedPath.MinimumPoints)
                        {
                            selectedPath.KeyPoints.Remove(selectedPoint);
                            selectedPoint = null;
                            selectedPath.RecalculateLength();
                            SyncPaths();
                        }
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Up") && selectedPoint != null)
                    {
                        selectedPath.KeyPoints.MoveItemUp(selectedPoint);
                        selectedPath.RecalculateLength();
                        SyncPaths();
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Down") && selectedPoint != null)
                    {
                        selectedPath.KeyPoints.MoveItemDown(selectedPoint);
                        selectedPath.RecalculateLength();
                        SyncPaths();
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Duplicate") && selectedPoint != null)
                    {
                        PathPoint newPoint = new PathPoint();
                        FileSaver.CloneObject(selectedPoint, newPoint);
                        int index = selectedPath.KeyPoints.IndexOf(selectedPoint);
                        selectedPath.KeyPoints.Insert(index + 1, newPoint);
                        selectedPath.RecalculateLength();
                        SyncPaths();
                    }

                    ImGui.EndDisabled();

                    ImGui.InputFloat("Shift X", ref shiftX);
                    ImGui.InputFloat("Shift Y", ref shiftY);
                    if (ImGui.Button("Shift path"))
                    {
                        selectedPath.Translate(new PathPoint(shiftX, shiftY));
                        selectedPath.RecalculateLength();
                        Path.RecalculateLength();
                    }

                    ImGui.SeparatorText("Point properties");

                    if (selectedPoint != null)
                    {
                        float pos = selectedPoint.X;

                        if (ImGui.InputFloat("X", ref pos))
                        {
                            selectedPoint.X = pos;
                            selectedPath.RecalculateLength();
                            Path.RecalculateLength();
                        }

                        pos = selectedPoint.Y;
                        if (ImGui.InputFloat("Y", ref pos))
                        {
                            selectedPoint.Y = pos;
                            selectedPath.RecalculateLength();
                            Path.RecalculateLength();
                        }
                    }
                    else
                    {
                        ImGui.Text("No point selected");
                    }
                }
                else
                {
                    ImGui.Text("No path selected");
                }

                ImGui.End();
            }
        }

        void DeleteSelectedPath()
        {
            Path.Remove(selectedPath);
            selectedPath = null;
            selectedPoint = null;

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

            Path.RecalculateLength();
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
                case 2:
                    path = GameHelper.CreateCatmull(start, end);
                    path.EndPoint.X = end.X;
                    path.EndPoint.Y = end.Y;
                    break;
            }

            Path.Add(path);

            Path.RecalculateLength();
        }

        void SyncPaths()
        {
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

            Path.RecalculateLength();
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
                case 2:
                    return "Catmull-Rom Path";
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
            else if (path is CatmullRomPath)
                return "Catmull-Rom Path";
            else
                return "Unknown path";
        }
    }
}
