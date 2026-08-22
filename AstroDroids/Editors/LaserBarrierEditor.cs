using AstroDroids.Entities;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Input;
using AstroDroids.Interfaces;
using AstroDroids.Levels;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Numeric = System.Numerics;

namespace AstroDroids.Editors
{
    public class LaserBarrierEditor
    {
        LaserBarrierGroupNode BarrierGroup;

        LevelEditorScene scene;

        bool isDraggingPoint = false;

        //LaserBarrierNode selectedNode = null;
        LaserBarrierNode draggedNode;
        int selectedConnection = -1;

        bool connectionMode = false;

        List<LaserBarrierNode> selectedNodes = new List<LaserBarrierNode>();

        Vector2 prevMousePos = Vector2.Zero;
        Vector2 selRectStart = Vector2.Zero;
        bool isDraggingSelRect = false;
        MemoryStream copyBuffer;

        public LaserBarrierEditor(LevelEditorScene scene)
        {
            this.scene = scene;
        }

        public void Reset()
        {
            isDraggingSelRect = false;
            selectedNodes.Clear();
            draggedNode = null;
            BarrierGroup = null;
        }

        public void SetBarrier(LaserBarrierGroupNode node)
        {
            BarrierGroup = node;
        }

        public void Update(GameTime gameTime)
        {
            Vector2 mousePos = Screen.ScreenToWorldSpaceMouse();
            Vector2 deltaMousePos = mousePos - prevMousePos;

            bool lmb = InputSystem.GetLMB();
            bool rmbDown = InputSystem.GetRMBDown();

            if (BarrierGroup != null)
            {
                if (InputSystem.GetKeyDown(Keys.C))
                {
                    if (InputSystem.GetKey(Keys.LeftControl))
                    {
                        CopyNodes();
                    }
                    else
                    {
                        BarrierGroup.Nodes.Add(BarrierGroup.AvailableId, new LaserBarrierNode() { Id = BarrierGroup.AvailableId, Position = mousePos });
                        BarrierGroup.AvailableId = BarrierGroup.AvailableId + 1;
                    }
                }

                if (InputSystem.GetKeyDown(Keys.V) && InputSystem.GetKey(Keys.LeftControl))
                {
                    PasteNodes();
                }

                if (selectedNodes.Count == 1 && (connectionMode || InputSystem.GetKey(Keys.LeftControl) || InputSystem.GetKey(Keys.LeftShift)))
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

                                if (node != selectedNodes[0])
                                {
                                    if (InputSystem.GetKey(Keys.LeftShift))
                                        RemoveConnection(selectedNodes[0], node);
                                    else
                                        AddConnection(selectedNodes[0], node);
                                }

                                break;
                            }

                            if (found)
                                break;
                        }
                    }
                    return;
                }

                if (InputSystem.GetKeyDown(Keys.Delete) && selectedNodes.Count > 0)
                {
                    foreach (var selectedNode in selectedNodes)
                    {
                        BarrierGroup.Nodes.Remove(selectedNode.Id);
                        RemoveAllConnectionsFor(selectedNode);
                        isDraggingPoint = false;
                    }

                    selectedNodes.Clear();
                }

                var io = ImGui.GetIO();
                if (io.WantCaptureMouse || io.WantTextInput || io.WantCaptureKeyboard)
                    return;

                if (lmb || rmbDown)
                {
                    LaserBarrierNode foundNode = null;

                    if (!isDraggingPoint && !isDraggingSelRect)
                    {
                        foreach (var node in BarrierGroup.Nodes.Values)
                        {
                            RectangleF col = new RectangleF(node.Position.X - 16f, node.Position.Y - 16f, 32f, 32f);
                            if (col.Contains(Screen.ScreenToWorldSpaceMouse()))
                            {
                                if (lmb)
                                    isDraggingPoint = true;
                                selectedConnection = -1;
                                //selectedNode = node;
                                //selectedNodes.Clear();
                                //selectedNodes.Add(node);
                                foundNode = node;
                                draggedNode = node;
                                break;
                            }

                            if (isDraggingPoint)
                                break;
                        }

                        if(foundNode == null)
                        {
                            selectedNodes.Clear();
                            selectedConnection = -1;

                            if(lmb && !isDraggingSelRect)
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
                    else if(draggedNode != null)
                    {
                        Vector2 startPos = draggedNode.Position;

                        if (scene.DrawGrid)
                        {
                            mousePos.X = (int)Math.Floor(mousePos.X / scene.gridSize) * scene.gridSize;
                            mousePos.Y = (int)Math.Floor(mousePos.Y / scene.gridSize) * scene.gridSize;
                        }

                        Vector2 delta = mousePos - startPos;

                        if (isDraggingPoint)
                        {
                            foreach (var selectedNode in selectedNodes)
                            {
                                selectedNode.Position += delta;
                            }
                        }

                        //selectedNode.Position = mousePos;
                    }
                }
                else if (isDraggingPoint || isDraggingSelRect)
                {
                    isDraggingPoint = false;

                    if(isDraggingSelRect)
                    {
                        isDraggingSelRect = false;
                        RectangleF selectionRect = new RectangleF(
                            Math.Min(selRectStart.X, mousePos.X),
                            Math.Min(selRectStart.Y, mousePos.Y),
                            Math.Abs(mousePos.X - selRectStart.X),
                            Math.Abs(mousePos.Y - selRectStart.Y)
                        );
                        selectedNodes.Clear();

                        foreach (var node in BarrierGroup.Nodes.Values)
                        {
                            RectangleF col = new RectangleF(node.Position.X - 16f, node.Position.Y - 16f, 32f, 32f);
                            if (selectionRect.Intersects(col))
                            {
                                selectedNodes.Add(node);
                            }
                        }
                    }
                }
            }
        }

        void CopyNodes()
        {
            if (selectedNodes.Count == 0)
                return;

            if (copyBuffer != null)
            {
                copyBuffer.Dispose();
                copyBuffer = null;
            }

            copyBuffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(copyBuffer, System.Text.Encoding.UTF8, true);

            writer.Write(selectedNodes[0].Position.X);
            writer.Write(selectedNodes[0].Position.Y);

            writer.Write(selectedNodes.Count);
            foreach (var item in selectedNodes)
            {
                item.Save(writer);
            }
            writer.Dispose();
        }

        void PasteNodes()
        {
            if (copyBuffer == null)
                return;

            copyBuffer.Position = 0;

            BinaryReader reader = new BinaryReader(copyBuffer, System.Text.Encoding.UTF8, true);

            Vector2 startPos = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            Vector2 mousePos = Screen.ScreenToWorldSpaceMouse();

            Vector2 delta = mousePos - startPos;

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                LaserBarrierNode node;

                node = new LaserBarrierNode();
                node.Load(reader, Level.FileVersion);

                node.Position += delta;

                BarrierGroup.Nodes.Add(BarrierGroup.AvailableId, node);
                node.Id = BarrierGroup.AvailableId;
                BarrierGroup.AvailableId += 1;
            }

            reader.Dispose();
        }

        public void DrawBarriers(LaserBarrierGroupNode group, List<LaserBarrierNode> selected)
        {
            //Draw connections
            foreach (var item in group.Connections)
            {
                group.Nodes.TryGetValue(item.FirstBarrierID, out LaserBarrierNode from);
                group.Nodes.TryGetValue(item.SecondBarrierID, out LaserBarrierNode to);

                if (from == null || to == null)
                    continue;

                Color color = Color.Red;

                if (from.Health >= 0 || to.Health >= 0)
                    color = Color.Blue;

                if (to != null)
                    Screen.spriteBatch.DrawLine(from.Position, to.Position, color, 5f);
            }

            //Draw nodes themselves
            foreach (var node in group.Nodes.Values)
            {
                bool sel = selected != null && selected.Contains(node);
                GameHelper.DrawNode($"{node.Id}", node.Position, node.Health >= 0 ? sel ? Color.Cyan : Color.Blue : sel ? Color.Orange : Color.Red, Color.DarkSlateGray);

                if (node.HasEnemy)
                {
                    Screen.spriteBatch.DrawCircle(node.Position, 20, 12, Color.Red, 2f, 0f);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            GameHelper.DrawNode("BA", BarrierGroup.Transform.Position, Color.DarkViolet, Color.DarkSlateGray);

            //DrawBarriers(BarrierGroup, selectedNode);
            DrawBarriers(BarrierGroup, selectedNodes);

            Vector2 mousePos = Screen.ScreenToWorldSpaceMouse();

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

        public void DrawImGui(GameTime gameTime)
        {
            ImGui.Begin("Barrier Editor");

            ImGui.SetNextItemWidth(-1);

            if (ImGui.BeginListBox("##Barriers"))
            {
                foreach (var barrier in BarrierGroup.Nodes.Values)
                {
                    if (ImGui.Selectable($"Barrier {barrier.Id}##BarrierSelectable{barrier.Id}", selectedNodes.Contains(barrier)))
                    {
                        selectedNodes.Clear();
                        selectedNodes.Add(barrier);
                        //selectedNode = barrier;
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

            ImGui.BeginDisabled(selectedNodes.Count == 0);
            if (ImGui.Button("Remove##RemoveBarrier"))
            {
                foreach (var selectedNode in selectedNodes)
                {
                    BarrierGroup.Nodes.Remove(selectedNode.Id);
                    RemoveAllConnectionsFor(selectedNode);
                    isDraggingPoint = false;
                }

                selectedNodes.Clear();
            }
            ImGui.EndDisabled();

            if (ImGui.Button("Return"))
            {
                BarrierGroup = null;
                selectedNodes.Clear();
                selectedConnection = -1;
                copyBuffer.Dispose();
                copyBuffer = null;
                scene.ReturnFromEditor();
            }

            if (selectedNodes.Count != 0)
            {
                ImGui.SeparatorText("Barrier settings");

                ImGui.Text($"Id: {selectedNodes[0].Id}");

                float posCord = selectedNodes[0].Position.X;
                if (ImGui.InputFloat("X", ref posCord))
                {
                    ForSelected((selectedNode) => { selectedNode.Position = new Vector2(posCord, selectedNode.Position.Y); });
                    //selectedNode.Position = new Vector2(posCord, selectedNode.Position.Y);
                }

                posCord = selectedNodes[0].Position.Y;
                if (ImGui.InputFloat("Y", ref posCord))
                {
                    //selectedNode.Position = new Vector2(selectedNode.Position.X, posCord);
                    ForSelected((selectedNode) => { selectedNode.Position = new Vector2(selectedNode.Position.X, posCord); });
                }

                int hp = selectedNodes[0].Health;
                if (ImGui.InputInt("Health", ref hp))
                {
                    ForSelected((selectedNode) => { selectedNode.Health = hp; });
                    //selectedNode.Health = hp;
                }

                if (ImGui.BeginCombo("Type", selectedNodes[0].Type.ToString()))
                {
                    if (ImGui.Selectable("Normal", selectedNodes[0].Type == LaserBarrierType.Normal))
                        ForSelected((selectedNode) => { selectedNode.Type = LaserBarrierType.Normal; });
                        //selectedNode.Type = LaserBarrierType.Normal;

                    if (ImGui.Selectable("Relay", selectedNodes[0].Type == LaserBarrierType.Relay))
                        ForSelected((selectedNode) => { selectedNode.Type = LaserBarrierType.Relay; });
                        //selectedNode.Type = LaserBarrierType.Relay;

                    ImGui.EndCombo();
                }

                List<Type> enemyList = GameDatabase.GetAllEnemyTypes();
                //
                var avaSpace = ImGui.GetContentRegionAvail();
                ImGui.PushItemWidth(avaSpace.X);
                if (ImGui.BeginCombo("##EnemyCombo", selectedNodes[0].Enemy != null ? enemyList[selectedNodes[0].Enemy.EnemyID].Name : "None", ImGuiComboFlags.HeightLarge))
                {
                    if (ImGui.Selectable("None", selectedNodes[0].Enemy == null))
                    {
                        ForSelected((selectedNode) => { selectedNode.HasEnemy = false; selectedNode.Enemy = null; });
                        //selectedNode.HasEnemy = false;
                        //selectedNode.Enemy = null;
                    }

                    for (int i = 0; i < enemyList.Count; i++)
                    {
                        selectableButton(enemyList[i].Name, selectedNodes[0].Enemy != null && i == selectedNodes[0].Enemy.EnemyID, 0, 72,
                        () =>
                        {
                            ForSelected((selectedNode) => { selectedNode.HasEnemy = true; selectedNode.Enemy = new EnemySpawnEntry { EnemyID = i, SpawnData = GameDatabase.CreateEnemySpawnData(i) }; });
                            //selectedNode.HasEnemy = true;
                            //selectedNode.Enemy = new EnemySpawnEntry { EnemyID = i, SpawnData = GameDatabase.CreateEnemySpawnData(i) };
                        },
                        () =>
                        {
                            float yPos = ImGui.GetCursorPosY();
                            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 4);
                            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 4);
                            ImGui.Image(GameDatabase.GetEntityPreview(i), new Numeric.Vector2(64, 64));
                            ImGui.SameLine();
                            var textSize = ImGui.CalcTextSize(enemyList[i].Name);
                            ImGui.SetCursorPosY(yPos + 72 / 2 - textSize.Y / 2);
                            ImGui.Text(enemyList[i].Name);
                        });
                    }

                    ImGui.EndCombo();
                }

                ImGui.PopItemWidth();
                //



                var connections = BarrierGroup.Connections.Where(x => x.FirstBarrierID == selectedNodes[0].Id || x.SecondBarrierID == selectedNodes[0].Id).ToList();

                ImGui.SetNextItemWidth(-1);
                if (ImGui.BeginListBox("##Connections"))
                {
                    for (int i = 0; i < connections.Count; i++)
                    {
                        var connection = connections[i];
                        BarrierGroup.Nodes.TryGetValue(connection.FirstBarrierID == selectedNodes[0].Id ? connection.SecondBarrierID : connection.FirstBarrierID, out LaserBarrierNode barrier);

                        if (barrier == null)
                        {
                            ImGui.Text($"Missing barrier with id {((connection.FirstBarrierID == selectedNodes[0].Id ? connection.SecondBarrierID : connection.FirstBarrierID))}");
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

                if (selectedConnection != -1)
                {
                    ImGui.SeparatorText("Connection settings");
                    ImGui.Checkbox("Blocks Player Projectiles", ref connections[selectedConnection].BlocksPlayerProjectiles);
                }

                ImGui.Begin("Enemy settings");

                if (selectedNodes[0].HasEnemy && selectedNodes[0].Enemy != null)
                {
                    if (selectedNodes.Count == 1)
                    {
                        int enemyId = selectedNodes[0].Enemy.EnemyID;
                        ImGui.SeparatorText($"{enemyList[enemyId].Name} settings");
                        selectedNodes[0].Enemy.SpawnData?.DrawEditor();
                    }
                    else
                    {
                        ImGui.SeparatorText("Multi-node enemy editing not supported");
                    }
                }
                else
                {
                    ImGui.SeparatorText("No enemy selected");
                }
                ImGui.End();
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

        void selectableButton(string label, bool selected, float width, float height, Action onSelect = null, Action content = null)
        {
            ImGui.BeginGroup();

            if (ImGui.Selectable($"##{label}", selected, ImGuiSelectableFlags.None, new Numeric.Vector2(width, height)))
            {
                onSelect?.Invoke();
            }

            var min = ImGui.GetItemRectMin();

            ImGui.SetCursorScreenPos(min);
            content?.Invoke();

            ImGui.EndGroup();
        }

        void ForSelected(Action<LaserBarrierNode> action)
        {
            foreach (var selectedNode in selectedNodes)
            {
                action(selectedNode);
            }
        }
    }
}
