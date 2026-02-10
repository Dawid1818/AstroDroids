using AstroDroids.Paths;
using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Levels;
using AstroDroids.Scenes;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using AstroDroids.Exensions;
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

                        if(draggedPoint == selectedPath.EndPoint)
                        {
                            int ind = paths.IndexOf(selectedPath);
                            ind++;
                            if(ind < paths.Count)
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
            PathVisualizer.DrawPath(Path, spawner.Transform.Position, selectedPath);
            //float t = 0f;
            //PathPoint lastPos = Path.GetPoint(t);
            //while (t < 1f)
            //{
            //    t += 0.01f;
            //    PathPoint nextPos = Path.GetPoint(t);
            //    Screen.spriteBatch.DrawLine(lastPos, nextPos, Color.Green, 4f);
            //    lastPos = nextPos;
            //}

            //if(spawner != null)
            //    Screen.spriteBatch.DrawCircle(spawner.Transform.Position, 16f, 16, Color.Yellow, 16f);

            //foreach (var path in Path.Decompose())
            //{
            //    var keyPoints = path.KeyPoints;
            //    for (int i = 0; i < keyPoints.Length; i++)
            //    {
            //        PathPoint point = keyPoints[i];

            //        Screen.spriteBatch.DrawCircle(point, 16f, 16, Color.Red, 16f);
            //    }
            //}
        }

        public void DrawImGui(GameTime gameTime)
        {
            List<IPath> paths = Path.Decompose();

            ImGui.Begin("Path Editor");

            ImGui.SetNextItemWidth(-1);

            if(ImGui.BeginListBox("##Paths"))
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

            if(ImGui.Button("Add"))
            {
                IPath path = null;
                switch (PathSelection)
                {
                    case 0:
                    default:
                        path = new LinePath();
                        break;
                    case 1:
                        path = new BezierPath();
                        break;
                }

                if(paths.LastOrDefault() is IPath lastPath)
                {
                    path.StartPoint.X = lastPath.EndPoint.X;
                    path.StartPoint.Y = lastPath.EndPoint.Y;
                }

                Path.Add(path);
            }

            ImGui.SameLine();

            ImGui.BeginDisabled(selectedPath == null);
            if(ImGui.Button("Remove") && selectedPath != null)
            {
                Path.Remove(selectedPath);
                selectedPath = null;
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

            if(ImGui.Button("Return"))
            {
                Path = null;
                scene.ReturnFromEditor();
            }

            ImGui.End();
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
