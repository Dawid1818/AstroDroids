using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Input;
using AstroDroids.Levels;
using AstroDroids.Scenes;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Linq;

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
            BarrierGroup = node;
        }

        public void Update(GameTime gameTime)
        {
            Vector2 mousePos = Screen.ScreenToWorldSpaceMouse();

            bool lmb = InputSystem.GetLMB();
            bool rmbDown = InputSystem.GetRMBDown();

            if (BarrierGroup != null)
            {
                if (connectionMode || InputSystem.GetKey(Keys.LeftControl) || InputSystem.GetKey(Keys.LeftShift))
                {
                    if (InputSystem.GetKeyDown(Keys.Escape))
                    {
                        connectionMode = false;
                        return;
                    }

                    if (lmb || rmbDown)
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
                                    if (InputSystem.GetKey(Keys.LeftShift))
                                        RemoveConnection(selectedNode, node);
                                    else
                                        AddConnection(selectedNode, node);
                                    //if (!selectedNode.Connections.Contains(node.Id))
                                    //    selectedNode.Connections.Add(node.Id);
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
                    RemoveAllConnectionsFor(selectedNode);
                    selectedNode = null;
                    isDraggingPoint = false;
                }

                if (InputSystem.GetKeyDown(Keys.C))
                {
                    BarrierGroup.Nodes.Add(BarrierGroup.AvailableId, new LaserBarrierNode() { Id = BarrierGroup.AvailableId, Position = mousePos });
                    BarrierGroup.AvailableId = BarrierGroup.AvailableId + 1;
                }
                var io = ImGui.GetIO();
                if (io.WantCaptureMouse || io.WantTextInput || io.WantCaptureKeyboard)
                    return;

                if (lmb || rmbDown)
                {
                    if (!isDraggingPoint)
                    {
                        foreach (var node in BarrierGroup.Nodes.Values)
                        {
                            RectangleF col = new RectangleF(node.Position.X - 16f, node.Position.Y - 16f, 32f, 32f);
                            if (col.Contains(Screen.ScreenToWorldSpaceMouse()))
                            {
                                if (lmb)
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

        public void DrawBarriers(LaserBarrierGroupNode group, LaserBarrierNode selected = null)
        {
            //Draw connections
            foreach (var item in group.Connections)
            {
                group.Nodes.TryGetValue(item.FirstBarrierID, out LaserBarrierNode from);
                group.Nodes.TryGetValue(item.SecondBarrierID, out LaserBarrierNode to);

                Color color = Color.Red;

                if (from.Health >= 0 || to.Health >= 0)
                    color = Color.Blue;

                if (to != null)
                    Screen.spriteBatch.DrawLine(from.Position, to.Position, color, 5f);
            }

            //Draw nodes themselves
            foreach (var node in group.Nodes.Values)
            {
                GameHelper.DrawNode($"{node.Id}", node.Position, node.Health >= 0 ? selected == node ? Color.Cyan : Color.Blue : selected == node ? Color.Orange : Color.Red, Color.DarkSlateGray);
            }
        }

        public void Draw(GameTime gameTime)
        {
            GameHelper.DrawNode("BA", BarrierGroup.Transform.Position, Color.DarkViolet, Color.DarkSlateGray);

            DrawBarriers(BarrierGroup, selectedNode);
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

                if (ImGui.BeginCombo("Type", selectedNode.Type.ToString()))
                {
                    if (ImGui.Selectable("Normal", selectedNode.Type == LaserBarrierType.Normal))
                        selectedNode.Type = LaserBarrierType.Normal;

                    if (ImGui.Selectable("Relay", selectedNode.Type == LaserBarrierType.Relay))
                        selectedNode.Type = LaserBarrierType.Relay;

                    ImGui.EndCombo();
                }

                var connections = BarrierGroup.Connections.Where(x => x.FirstBarrierID == selectedNode.Id || x.SecondBarrierID == selectedNode.Id).ToList();

                ImGui.SetNextItemWidth(-1);
                if (ImGui.BeginListBox("##Connections"))
                {
                    for (int i = 0; i < connections.Count; i++)
                    {
                        var connection = connections[i];
                        BarrierGroup.Nodes.TryGetValue(connection.FirstBarrierID == selectedNode.Id ? connection.SecondBarrierID : connection.FirstBarrierID, out LaserBarrierNode barrier);

                        if (barrier == null)
                        {
                            ImGui.Text($"Missing barrier with id {((connection.FirstBarrierID == selectedNode.Id ? connection.SecondBarrierID : connection.FirstBarrierID))}");
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
                    RemoveConnection(BarrierGroup.Nodes.TryGetValue(connections[selectedConnection].FirstBarrierID, out LaserBarrierNode from) ? from : null, BarrierGroup.Nodes.TryGetValue(connections[selectedConnection].SecondBarrierID, out LaserBarrierNode to) ? to : null);
                    //RemoveConnection(selectedNode, BarrierGroup.Nodes[selectedNode.Connections[selectedConnection]]);)
                    //selectedNode.Connections.Remove(selectedNode.Connections[selectedConnection]);
                    selectedConnection = -1;
                }
                ImGui.EndDisabled();

                if(selectedConnection != -1)
                {
                    ImGui.SeparatorText("Connection settings");
                    ImGui.Checkbox("Blocks Player Projectiles", ref connections[selectedConnection].BlocksPlayerProjectiles);
                }
            }

            ImGui.End();
        }

        void AddConnection(LaserBarrierNode from, LaserBarrierNode to)
        {
            if (BarrierGroup == null || from == null || to == null)
                return;

            var link = BarrierGroup.Connections.FirstOrDefault(x => (x.FirstBarrierID == from.Id && x.SecondBarrierID == to.Id) || (x.FirstBarrierID == to.Id && x.SecondBarrierID == from.Id));

            if (link == default)
            {
                BarrierGroup.Connections.Add(new LaserBarrierConnection { FirstBarrierID = from.Id, SecondBarrierID = to.Id });
            }
        }

        void RemoveAllConnectionsFor(LaserBarrierNode node)
        {
            if (BarrierGroup == null || node == null)
                return;
            BarrierGroup.Connections.RemoveAll(x => x.FirstBarrierID == node.Id || x.SecondBarrierID == node.Id);
        }

        void RemoveConnection(LaserBarrierNode from, LaserBarrierNode to)
        {
            if (BarrierGroup == null)
                return;
            var link = BarrierGroup.Connections.FirstOrDefault(x => (x.FirstBarrierID == from.Id && x.SecondBarrierID == to.Id) || (x.FirstBarrierID == to.Id && x.SecondBarrierID == from.Id));
            if (link != default)
            {
                BarrierGroup.Connections.Remove(link);
            }
        }
    }
}
