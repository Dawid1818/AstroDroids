using AstroDroids.Entities;
using AstroDroids.Extensions;
using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Levels;
using AstroDroids.Managers;
using AstroDroids.Paths;
using AstroDroids.Scenes;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using Numeric = System.Numerics;

namespace AstroDroids.Editors
{
    public class AttackWaveEditor
    {
        Level level { get { return LevelManager.CurrentLevel; } set { LevelManager.CurrentLevel = value; } }
        public List<Entity> AllNodes { get; private set; } = new List<Entity>();

        public AttackWave wave { get; private set; }
        LevelEditorScene scene;

        bool isDraggingNode;
        bool isDraggingSpawnPosition;
        PathPoint selectedSpawnPoint;
        List<Entity> selectedNodes = new List<Entity>();

        Vector2 prevMousePos = Vector2.Zero;
        Vector2 selRectStart = Vector2.Zero;
        bool isDraggingSelRect = false;

        int selectedEnemyType = 0;
        int selectedEnemy = -1;
        public AttackWaveEditor(LevelEditorScene scene)
        {
            this.scene = scene;
        }

        public void SetWave(AttackWave wave)
        {
            this.wave = wave;
        }

        public void Reset()
        {
            isDraggingNode = false;
            isDraggingSelRect = false;
            isDraggingSpawnPosition = false;
            selectedSpawnPoint = null;
            selectedNodes.Clear();
            AllNodes.Clear();
            wave = null;
        }

        void LoadAllNodes()
        {
            foreach (var item in wave.Spawners)
            {
                AllNodes.Add(item);
            }

            foreach (var item in wave.Events)
            {
                AllNodes.Add(item);
            }

            foreach (var item in wave.LaserBarriers)
            {
                AllNodes.Add(item);
            }
        }

        public void Update()
        {
            if (wave == null)
                return;

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
                    Entity foundNode = null;

                    foreach (var node in AllNodes)
                    {
                        RectangleF col;

                        if (node is EnemySpawner spawner)
                        {

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
                                    foundNode = node;
                                    selectedSpawnPoint = spawner.SpawnPosition;
                                    break;
                                }
                            }
                        }

                        col = new RectangleF(node.Transform.Position.X - 16f, node.Transform.Position.Y - 16f, 32f, 32f);
                        if (col.Contains(Screen.ScreenToWorldSpaceMouse()))
                        {
                            if (lmb)
                            {
                                isDraggingNode = true;
                                isDraggingSpawnPosition = false;
                            }
                            foundNode = node;
                            selectedSpawnPoint = null;
                            break;
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
                    if (scene.DrawGrid)
                    {
                        mousePos.X = (int)Math.Floor(mousePos.X / scene.gridSize) * scene.gridSize;
                        mousePos.Y = (int)Math.Floor(mousePos.Y / scene.gridSize) * scene.gridSize;

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

                            if (selectedNode is LaserBarrierGroupNode barrierGroup && !InputSystem.GetKey(Keys.LeftShift))
                            {
                                barrierGroup.Translate(deltaMousePos);
                            }

                            selectedNode.Transform.Position += deltaMousePos;
                        }
                    }
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

                    foreach (var node in AllNodes)
                    {
                        RectangleF col = new RectangleF(node.Transform.Position.X - 16f, node.Transform.Position.Y - 16f, 32f, 32f);
                        if (selectionRect.Intersects(col))
                        {
                            selectedNodes.Add(node);
                        }
                    }
                }
            }

            if (InputSystem.GetKeyDown(Keys.Delete) && selectedNodes.Count > 0)
            {
                foreach (var selectedNode in selectedNodes)
                {
                    if (selectedNode is EnemySpawner spawner)
                        wave.RemoveSpawner(spawner);
                    else if (selectedNode is EventNode eventN)
                        wave.RemoveEvent(eventN);
                    else if (selectedNode is LaserBarrierGroupNode laserBarrierN)
                        wave.RemoveLaserBarrier(laserBarrierN);

                    AllNodes.Remove(selectedNode);
                }

                selectedNodes.Clear();
                isDraggingNode = false;
            }

            if (InputSystem.GetKeyDown(Keys.C))
                AllNodes.Add(wave.CreateSpawner(mousePos));

            if (InputSystem.GetKeyDown(Keys.V))
                AllNodes.Add(wave.CreateEvent(mousePos));

            if (InputSystem.GetKeyDown(Keys.B))
                AllNodes.Add(wave.CreateLaserBarrier(mousePos));

            if (InputSystem.GetKeyDown(Keys.T) && wave != null)
                LevelManager.Playtest(level.AttackWaves.IndexOf(wave));

            prevMousePos = mousePos;
        }

        public void Draw()
        {
            if (wave == null)
                return;

            Vector2 mousePos = Screen.ScreenToWorldSpaceMouse();

            foreach (var node in AllNodes)
            {
                if (node is EnemySpawner spawner)
                {
                    if (!spawner.HasPath)
                        Screen.spriteBatch.DrawLine(spawner.Transform.Position, spawner.SpawnPosition, Color.Green, 4f);
                    else
                    {
                        PathVisualizer.DrawPath(spawner.Path, scene, highlightAll: selectedNodes.Contains(spawner));
                    }

                    scene.DrawNode("S", spawner.Transform.Position, selectedNodes.Contains(spawner) ? Color.Cyan : Color.Orange, Color.Green);

                    if (!spawner.HasPath)
                    {
                        scene.DrawNode(string.Empty, spawner.SpawnPosition, Color.Red, Color.Green);
                    }
                }
                else if (node is EventNode eventN)
                {
                    scene.DrawNode("E", eventN.Transform.Position, selectedNodes.Contains(eventN) ? Color.Cyan : Color.Gray, Color.White);
                }
                else if (node is LaserBarrierGroupNode laserBarrierN)
                {
                    scene.DrawNode("B", laserBarrierN.Transform.Position, selectedNodes.Contains(laserBarrierN) ? Color.Cyan : Color.DarkViolet, Color.DarkSlateGray);

                    scene.barrierEditor.DrawBarriers(laserBarrierN);
                }
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

        public void DrawImGui()
        {
            if (ImGui.Begin("Attack Waves"))
            {
                List<Type> enemyList = EntityDatabase.GetAllEnemyTypes();
                if (ImGui.BeginListBox("##WaveList", new Numeric.Vector2(-1, 0)))
                {
                    for (int i = 0; i < level.AttackWaves.Count; i++)
                    {
                        AttackWave thisWave = level.AttackWaves[i];

                        if (ImGui.Selectable($"{thisWave.Name} - Delay: {thisWave.Delay}{(thisWave.WaitForPreviousWave ? ", waits" : "")}##Wave{i}", thisWave == wave))
                        {
                            if (wave != thisWave)
                            {
                                wave = thisWave;

                                LoadAllNodes();

                                isDraggingNode = false;
                                isDraggingSelRect = false;
                                isDraggingSpawnPosition = false;
                                selectedSpawnPoint = null;
                                selectedNodes.Clear();
                            }
                        }
                    }

                    ImGui.EndListBox();
                }

                if (ImGui.Button("Add"))
                {
                    level.CreateAttackWave();
                }

                ImGui.SameLine();

                ImGui.BeginDisabled(wave == null);
                if (ImGui.Button("Remove") && wave != null)
                {
                    level.RemoveAttackWave(wave);
                    wave = null;
                }
                ImGui.SameLine();
                if (ImGui.Button("Up") && wave != null)
                {
                    level.AttackWaves.MoveItemUp(wave);
                }
                ImGui.SameLine();
                if (ImGui.Button("Down") && wave != null)
                {
                    level.AttackWaves.MoveItemDown(wave);
                }

                if (ImGui.Button("Playtest from this wave"))
                {
                    LevelManager.Playtest(level.AttackWaves.IndexOf(wave));
                }

                ImGui.EndDisabled();

                ImGui.SeparatorText("Wave settings");

                if (wave != null)
                {
                    string name = wave.Name;
                    if (ImGui.InputText("Name", ref name, 1024))
                    {
                        wave.Name = name;
                    }

                    double delay = wave.Delay;
                    if (ImGui.InputDouble("Delay", ref delay))
                    {
                        wave.Delay = delay;
                    }

                    bool waitForPreviousWave = wave.WaitForPreviousWave;
                    if (ImGui.Checkbox("Wait for Previous Wave", ref waitForPreviousWave))
                    {
                        wave.WaitForPreviousWave = waitForPreviousWave;
                    }
                }
                else
                {
                    ImGui.Text("No wave selected");
                }

                ImGui.End();
            }

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
                    SpawnerProperties(spawner);
                }
                else if (selectedNode is EventNode eventN)
                {
                    EventProperties(eventN);
                }
                else if (selectedNode is LaserBarrierGroupNode laserBarrierN)
                {
                    LaserBarrierProperties(laserBarrierN);
                }
            }
            else
            {
                ImGui.Text("No node selected");
            }
            ImGui.End();
        }

        void SpawnerProperties(EnemySpawner spawner)
        {
            ImGui.SeparatorText("Spawner settings");

            double initialDelay = spawner.InitialDelay;
            if (ImGui.InputDouble("Initial delay", ref initialDelay))
            {
                spawner.InitialDelay = initialDelay;
            }

            ImGui.Text("Enemies");
            List<Type> enemyList = EntityDatabase.GetAllEnemyTypes();
            if (ImGui.BeginListBox("##EnemyList", new Numeric.Vector2(-1, 0)))
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

            ImGui.SameLine();

            if (ImGui.Button("Remove"))
            {
                if (selectedEnemy >= 0 && selectedEnemy < spawner.EnemyIDs.Count)
                {
                    spawner.EnemyIDs.RemoveAt(selectedEnemy);
                    selectedEnemy = -1;
                }
            }

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

            bool followsCamera = spawner.FollowsCamera;
            if (ImGui.Checkbox("Follows Camera", ref followsCamera))
            {
                spawner.FollowsCamera = followsCamera;
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
                        scene.mode = EditorMode.Path;
                        Screen.ResetCamera();
                        scene.curveEditor.SetPath(spawner.Path);
                        scene.curveEditor.SetSpawner(spawner);
                    }
                    else
                    {
                        scene.mode = EditorMode.Path;
                        scene.curveEditor.SetPath(spawner.Path);
                        scene.curveEditor.SetSpawner(spawner);
                    }
                }
            }
        }
        void EventProperties(EventNode eventN)
        {
            ImGui.SeparatorText("Event settings");

            string eventId = eventN.EventId;
            if (ImGui.InputText("Event ID", ref eventId, 100))
            {
                eventN.EventId = eventId;
            }

            double initialDelay = eventN.InitialDelay;
            if (ImGui.InputDouble("Initial delay", ref initialDelay))
            {
                eventN.InitialDelay = initialDelay;
            }
        }

        void LaserBarrierProperties(LaserBarrierGroupNode laserBarrierN)
        {
            ImGui.SeparatorText("Laser Barrier settings");

            double initialDelay = laserBarrierN.InitialDelay;
            if (ImGui.InputDouble("Initial delay", ref initialDelay))
            {
                laserBarrierN.InitialDelay = initialDelay;
            }

            if (ImGui.Button("Edit barrier"))
            {
                scene.mode = EditorMode.Barrier;
                scene.barrierEditor.SetBarrier(laserBarrierN);
            }
        }
    }
}
