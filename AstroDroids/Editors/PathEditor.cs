using AstroDroids.Curves;
using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Scenes;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace AstroDroids.Editors
{
    public class PathEditor
    {
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

        public void Update(GameTime gameTime)
        {
            Vector2 mousePos = Screen.ScreenToWorldSpaceMouse();

            if (Path != null)
            {
                if (InputSystem.GetLMB())
                {
                    if (!isDraggingPoint)
                    {
                        foreach (var path in Path.Decompose())
                        {
                            var keyPoints = path.KeyPoints;
                            for (int i = 0; i < keyPoints.Length; i++)
                            {
                                PathPoint point = keyPoints[i];
                                RectangleF col = new RectangleF(point.X - 16f, point.Y - 16f, 32f, 32f);
                                if (col.Contains(Screen.ScreenToWorldSpaceMouse()))
                                {
                                    isDraggingPoint = true;
                                    draggedPoint = point;
                                    break;
                                }
                            }

                            if (isDraggingPoint)
                                break;
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
            float t = 0f;
            PathPoint lastPos = Path.GetPoint(t);
            while (t < 1f)
            {
                t += 0.01f;
                PathPoint nextPos = Path.GetPoint(t);
                Screen.spriteBatch.DrawLine(lastPos, nextPos, Color.Green, 4f);
                lastPos = nextPos;
            }

            foreach (var path in Path.Decompose())
            {
                var keyPoints = path.KeyPoints;
                for (int i = 0; i < keyPoints.Length; i++)
                {
                    PathPoint point = keyPoints[i];

                    Screen.spriteBatch.DrawCircle(point, 16f, 16, Color.Red, 16f);
                }
            }
        }

        public void DrawImGui(GameTime gameTime)
        {
            ImGui.Begin("Path Editor");

            ImGui.SetNextItemWidth(-1);

            if(ImGui.BeginListBox("##Paths"))
            {
                List<IPath> list = Path.Decompose();
                for (int i = 0; i < list.Count; i++)
                {
                    IPath path = list[i];
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

                Path.Add(path);
            }

            ImGui.SameLine();

            ImGui.BeginDisabled(selectedPath == null);
            if(ImGui.Button("Remove") && selectedPath != null)
            {
                Path.Remove(selectedPath);
                selectedPath = null;
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
