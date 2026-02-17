using AstroDroids.Entities.Hostile;
using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Levels;
using AstroDroids.Scenes;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Xml.Linq;

namespace AstroDroids.Editors
{
    public class LaserBarrierEditor
    {
        LaserBarrierGroupNode BarrierGroup;

        LevelEditorScene scene;

        bool isDraggingPoint = false;

        LaserBarrierNode selectedNode = null;
        int selectedConnection = -1;

        bool connectionMode = false;

        public LaserBarrierEditor(LevelEditorScene scene)
        {
            this.scene = scene;
        }

        public void SetBarrier(LaserBarrierGroupNode node)
        {
            this.BarrierGroup = node;
        }

        public void Update(GameTime gameTime)
        {
            Vector2 mousePos = Screen.ScreenToWorldSpaceMouse();

            if (BarrierGroup != null)
            {
                if (connectionMode)
                {
                    if (InputSystem.GetKeyDown(Keys.Escape))
                    {
                        connectionMode = false;
                        return;
                    }

                    if (InputSystem.GetLMBDown())
                    {
                        bool found = false;

                        foreach (var node in BarrierGroup.Nodes.Values)
                        {
                            RectangleF col = new RectangleF(node.Position.X - 16f, node.Position.Y - 16f, 32f, 32f);
                            if (col.Contains(Screen.ScreenToWorldSpaceMouse()))
                            {
                                found = true;

                                if (node != selectedNode)
                                {
                                    if (!selectedNode.Connections.Contains(node.Id))
                                        selectedNode.Connections.Add(node.Id);
                                }

                                break;
                            }

                            if (found)
                                break;
                        }
                    }
                    return;
                }

                if (InputSystem.GetKeyDown(Keys.Delete) && selectedNode != null)
                {
                    BarrierGroup.Nodes.Remove(selectedNode.Id);
                    selectedNode = null;
                    isDraggingPoint = false;
                }

                if (InputSystem.GetKeyDown(Keys.C))
                {
                    BarrierGroup.Nodes.Add(BarrierGroup.AvailableId, new LaserBarrierNode() { Id = BarrierGroup.AvailableId, Position = mousePos });
                    BarrierGroup.AvailableId = BarrierGroup.AvailableId + 1;
                }

                if (InputSystem.GetLMB())
                {
                    if (!isDraggingPoint)
                    {
                        foreach (var node in BarrierGroup.Nodes.Values)
                        {
                            RectangleF col = new RectangleF(node.Position.X - 16f, node.Position.Y - 16f, 32f, 32f);
                            if (col.Contains(Screen.ScreenToWorldSpaceMouse()))
                            {
                                isDraggingPoint = true;
                                selectedConnection = -1;
                                selectedNode = node;
                                break;
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

                        selectedNode.Position = mousePos;
                    }
                }
                else if (isDraggingPoint)
                {
                    isDraggingPoint = false;
                }
            }
        }

        public void DrawBarriers(LaserBarrierGroupNode group)
        {
            //Draw connections
            foreach (var item in group.Nodes.Values)
            {
                foreach (var id in item.Connections)
                {
                    group.Nodes.TryGetValue(id, out LaserBarrierNode value);

                    if (value != null)
                        Screen.spriteBatch.DrawLine(item.Position, value.Position, item.Health >= 0 ? Color.Blue : Color.Red, 5f);

                }
            }

            //Draw nodes themselves
            foreach (var node in group.Nodes.Values)
            {
                scene.DrawNode($"{node.Id}", node.Position, node.Health >= 0 ? Color.Blue : Color.Red, Color.DarkSlateGray);
            }
        }

        public void Draw(GameTime gameTime)
        {
            scene.DrawNode("B", BarrierGroup.Transform.Position, Color.DarkViolet, Color.DarkSlateGray);

            DrawBarriers(BarrierGroup);
        }

        public void DrawImGui(GameTime gameTime)
        {
            ImGui.Begin("Barrier Editor");

            ImGui.SetNextItemWidth(-1);

            if (ImGui.BeginListBox("##Barriers"))
            {
                foreach (var barrier in BarrierGroup.Nodes.Values)
                {
                    if (ImGui.Selectable($"Barrier {barrier.Id}##BarrierSelectable{barrier.Id}", selectedNode == barrier))
                    {
                        selectedNode = barrier;
                        selectedConnection = -1;
                    }
                }

                ImGui.EndListBox();
            }

            if (ImGui.Button("Add##AddBarrier"))
            {
                BarrierGroup.Nodes.Add(BarrierGroup.AvailableId, new LaserBarrierNode() { Id = BarrierGroup.AvailableId, Position = BarrierGroup.Transform.Position });
                BarrierGroup.AvailableId = BarrierGroup.AvailableId + 1;
            }

            ImGui.SameLine();

            ImGui.BeginDisabled(selectedNode == null);
            if (ImGui.Button("Remove##RemoveBarrier") && selectedNode != null)
            {
                BarrierGroup.Nodes.Remove(selectedNode.Id);
                selectedNode = null;
            }
            ImGui.EndDisabled();

            if (ImGui.Button("Return"))
            {
                BarrierGroup = null;
                selectedNode = null;
                selectedConnection = -1;
                scene.ReturnFromEditor();
            }

            if (selectedNode != null)
            {
                ImGui.SeparatorText("Barrier settings");

                ImGui.Text($"Id: {selectedNode.Id}");

                float posCord = selectedNode.Position.X;
                if (ImGui.InputFloat("X", ref posCord))
                {
                    selectedNode.Position = new Vector2(posCord, selectedNode.Position.Y);
                }

                posCord = selectedNode.Position.Y;
                if (ImGui.InputFloat("Y", ref posCord))
                {
                    selectedNode.Position = new Vector2(selectedNode.Position.X, posCord);
                }

                int hp = selectedNode.Health;
                if (ImGui.InputInt("Health", ref hp))
                {
                    selectedNode.Health = hp;
                }

                ImGui.SetNextItemWidth(-1);
                if (ImGui.BeginListBox("##Connections"))
                {
                    for (int i = 0; i < selectedNode.Connections.Count; i++)
                    {
                        BarrierGroup.Nodes.TryGetValue(selectedNode.Connections[i], out LaserBarrierNode barrier);

                        if (barrier == null)
                        {
                            ImGui.Text($"Missing barrier with id {selectedNode.Connections[i]}");
                            continue;
                        }

                        if (ImGui.Selectable($"Barrier {barrier.Id}##BarrierConnection{i}", selectedConnection == i))
                        {
                            selectedConnection = i;
                        }
                    }

                    ImGui.EndListBox();
                }

                if (connectionMode)
                {
                    if (ImGui.Button("Stop connecting"))
                    {
                        connectionMode = false;
                    }
                }
                else
                {
                    if (ImGui.Button("Start connecting"))
                    {
                        connectionMode = true;
                        isDraggingPoint = false;
                    }
                }

                ImGui.SameLine();

                ImGui.BeginDisabled(selectedConnection == -1);
                if (ImGui.Button("Remove Connection"))
                {
                    selectedNode.Connections.Remove(selectedNode.Connections[selectedConnection]);
                    selectedConnection = -1;
                }
                ImGui.EndDisabled();
            }

            ImGui.End();
        }
    }
}
