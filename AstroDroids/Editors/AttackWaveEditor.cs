using AstroDroids.Entities;
using AstroDroids.Extensions;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Input;
using AstroDroids.Interfaces;
using AstroDroids.Levels;
using AstroDroids.Managers;
using AstroDroids.Paths;
using AstroDroids.Scenes;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.IO;
using Numeric = System.Numerics;

namespace AstroDroids.Editors
{
    public class AttackWaveEditor
    {
        Level level { get { return LevelManager.CurrentLevel; } set { LevelManager.CurrentLevel = value; } }
        public List<Entity> AllNodes { get; private set; } = new List<Entity>();

        public AttackWave wave { get; private set; }
        LevelEditorScene scene;

        Entity draggedNode;
        bool isDraggingNode;
        bool isDraggingSpawnPosition;
        PathPoint selectedSpawnPoint;
        List<Entity> selectedNodes = new List<Entity>();

        Vector2 prevMousePos = Vector2.Zero;
        Vector2 selRectStart = Vector2.Zero;
        bool isDraggingSelRect = false;

        int selectedEnemyType = 0;
        int selectedEnemy = -1;

        MemoryStream copyBuffer;

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

            foreach (var item in wave.BackgroundObjects)
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
                                draggedNode = node;
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
                else if(draggedNode != null)
                {
                    Vector2 startPos = draggedNode.Transform.Position;

                    if (scene.DrawGrid)
                    {
                        mousePos.X = (int)Math.Floor(mousePos.X / scene.gridSize) * scene.gridSize;
                        mousePos.Y = (int)Math.Floor(mousePos.Y / scene.gridSize) * scene.gridSize;

                        deltaMousePos = mousePos - prevMousePos;
                    }

                    Vector2 delta = mousePos - startPos;

                    if (isDraggingSpawnPosition)
                    {
                        selectedSpawnPoint.X = mousePos.X;
                        selectedSpawnPoint.Y = mousePos.Y;
                    }
                    else if (isDraggingNode)
                    {
                        foreach (var selectedNode in selectedNodes)
                        {
                            if (selectedNode is MovableNode movable && !movable.FollowsCamera && !InputSystem.GetKey(Keys.LeftShift))
                            {
                                if (movable.HasPath)
                                {
                                    movable.Path.Translate(delta);
                                }
                                else
                                {
                                    if(movable is EnemySpawner spawner)
                                        spawner.SpawnPosition += delta;
                                }
                            }

                            if (selectedNode is LaserBarrierGroupNode barrierGroup && !InputSystem.GetKey(Keys.LeftShift))
                            {
                                barrierGroup.Translate(delta);
                            }

                            selectedNode.Transform.Position += delta;
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
                    else if (selectedNode is BackgroundObjectNode bgObjN)
                        wave.RemoveBackgroundObject(bgObjN);

                    AllNodes.Remove(selectedNode);
                }

                selectedNodes.Clear();
                isDraggingNode = false;
            }

            if (InputSystem.GetKeyDown(Keys.C))
            {
                if (InputSystem.GetKey(Keys.LeftControl))
                {
                    CopyNodes();
                }
                else
                {
                    EnemySpawner spawner = wave.CreateSpawner(mousePos);
                    selectedNodes.Clear();
                    isDraggingNode = false;
                    selectedNodes.Add(spawner);
                    AllNodes.Add(spawner);
                }
            }

            if (InputSystem.GetKeyDown(Keys.V))
            {
                if (InputSystem.GetKey(Keys.LeftControl))
                {
                    PasteNodes();
                }
                else
                {
                    EventNode eventN = wave.CreateEvent(mousePos);
                    selectedNodes.Clear();
                    isDraggingNode = false;
                    selectedNodes.Add(eventN);
                    AllNodes.Add(eventN);
                }
            }

            if (InputSystem.GetKeyDown(Keys.B))
            {
                LaserBarrierGroupNode laserBarrierN = wave.CreateLaserBarrier(mousePos);
                selectedNodes.Clear();
                isDraggingNode = false;
                selectedNodes.Add(laserBarrierN);
                AllNodes.Add(laserBarrierN);
            }

            if (InputSystem.GetKeyDown(Keys.N))
            {
                BackgroundObjectNode bgObjN = wave.CreateBackgroundObject(mousePos);
                selectedNodes.Clear();
                isDraggingNode = false;
                selectedNodes.Add(bgObjN);
                AllNodes.Add(bgObjN);
            }

            if (InputSystem.GetKeyDown(Keys.T) && wave != null)
                LevelManager.Playtest(level.AttackWaves.IndexOf(wave));

            prevMousePos = mousePos;
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

            writer.Write(selectedNodes[0].Transform.Position.X);
            writer.Write(selectedNodes[0].Transform.Position.Y);

            writer.Write(selectedNodes.Count);
            foreach (var item in selectedNodes)
            {
                ISaveable saveable = item as ISaveable;

                switch (saveable)
                {
                    case EnemySpawner spawner:
                        writer.Write(0);
                        break;
                    case EventNode eventN:
                        writer.Write(1);
                        break;
                    case LaserBarrierGroupNode barrierGroupN:
                        writer.Write(2);
                        break;
                    case BackgroundObjectNode bgObjN:
                        writer.Write(3);
                        break;
                    default:
                        return;
                }

                saveable.Save(writer);
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
                int val = reader.ReadInt32();

                ISaveable node;

                switch (val)
                {
                    case 0:
                        node = new EnemySpawner();
                        node.Load(reader, 0);
                        EnemySpawner spawner = node as EnemySpawner;
                        wave.Spawners.Add(spawner);

                        if (!spawner.HasPath)
                        {
                            spawner.SpawnPosition += delta;
                        }
                        else
                        {
                            spawner.Path.Translate(delta);
                        }

                        break;
                    case 1:
                        node = new EventNode();
                        node.Load(reader, 0);
                        wave.Events.Add(node as EventNode);
                        break;
                    case 2:
                        node = new LaserBarrierGroupNode();
                        node.Load(reader, 0);
                        LaserBarrierGroupNode laserGroupN = node as LaserBarrierGroupNode;
                        wave.LaserBarriers.Add(laserGroupN);

                        laserGroupN.Translate(delta);
                        break;
                    case 3:
                        node = new BackgroundObjectNode();
                        node.Load(reader, 0);
                        BackgroundObjectNode bgObjN = node as BackgroundObjectNode;
                        wave.BackgroundObjects.Add(bgObjN);
                        break;
                    default:
                        reader.Dispose();
                        return;
                }

                Entity entNode = (node as Entity);

                AllNodes.Add(entNode);

                entNode.Transform.Position += delta;
            }

            reader.Dispose();
        }

        public void Draw(GameTime gameTime)
        {
            if (wave == null)
                return;

            Vector2 mousePos = Screen.ScreenToWorldSpaceMouse();

            foreach (var node in AllNodes)
            {
                bool selected = selectedNodes.Contains(node);

                if (node is EnemySpawner spawner)
                {
                    if (!spawner.HasPath)
                        Screen.spriteBatch.DrawLine(spawner.Transform.Position, spawner.SpawnPosition, Color.Green, 4f);
                    else
                    {
                        PathVisualizer.DrawPath(spawner.Path, highlightAll: selected);
                    }

                    GameHelper.DrawNode("S", spawner.Transform.Position, selected ? Color.Cyan : Color.Orange, Color.Green);

                    if (!spawner.HasPath)
                    {
                        GameHelper.DrawNode(string.Empty, spawner.SpawnPosition, Color.Red, Color.Green);
                    }
                }
                else if (node is EventNode eventN)
                {
                    GameHelper.DrawNode("E", eventN.Transform.Position, selected ? Color.Cyan : Color.Gray, Color.White);
                }
                else if (node is LaserBarrierGroupNode laserBarrierN)
                {
                    GameHelper.DrawNode("BA", laserBarrierN.Transform.Position, selected ? Color.Cyan : Color.DarkViolet, Color.DarkSlateGray);

                    scene.barrierEditor.DrawBarriers(laserBarrierN);
                }
                else if (node is BackgroundObjectNode bgObjN)
                {
                    bgObjN.Draw(gameTime);

                    if (bgObjN.HasPath)
                    {
                        PathVisualizer.DrawPath(bgObjN.Path, highlightAll: selected);
                    }

                    GameHelper.DrawNode("BG", bgObjN.Transform.Position, selected ? Color.Cyan : Color.LightSkyBlue, Color.DarkSlateGray);
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

                                AllNodes.Clear();
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
                else if (selectedNode is BackgroundObjectNode bgObjN)
                {
                    BackgroundObjectProperties(bgObjN);
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
            
            PathSettings(spawner);
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

        void BackgroundObjectProperties(BackgroundObjectNode bgObjN)
        {
            ImGui.SeparatorText("Background object settings");

            double initialDelay = bgObjN.InitialDelay;
            if (ImGui.InputDouble("Initial delay", ref initialDelay))
            {
                bgObjN.InitialDelay = initialDelay;
            }

            float angle = bgObjN.Angle;
            if (ImGui.InputFloat("Rotation", ref angle))
            {
                bgObjN.Angle = angle;
            }

            bool flip = bgObjN.FlipH;
            if (ImGui.Checkbox("Flip Horizontally", ref flip))
            {
                bgObjN.FlipH = flip;
            }

            flip = bgObjN.FlipV;
            if (ImGui.Checkbox("Flip Vertically", ref flip))
            {
                bgObjN.FlipV = flip;
            }

            if (ImGui.BeginCombo("Texture", bgObjN.TextureName))
            {
                foreach (var item in TextureManager.GetBackgroundObjects())
                {
                    if (ImGui.Selectable(item.Name, bgObjN.TextureName == item.Name))
                    {
                        bgObjN.TextureName = item.Name;
                        bgObjN.UpdateTexture();
                    }
                }

                ImGui.EndCombo();
            }

            ImGui.SeparatorText("Path settings");

            bool hasPath = bgObjN.HasPath;
            if (ImGui.Checkbox("Has Path", ref hasPath))
            {
                bgObjN.HasPath = hasPath;

                if (bgObjN.HasPath)
                {
                    CompositePath path = new CompositePath();
                    bgObjN.Path = path;
                    //path.Add(new LinePath(spawner.Transform.Position, spawner.Transform.Position + new Vector2(100, 0)));
                    path.Add(new BezierPath(new List<PathPoint>() { PathPoint.Zero, PathPoint.Zero, PathPoint.Zero, PathPoint.Zero }));
                }
                else
                {
                    bgObjN.Path = null;
                }
            }

            PathSettings(bgObjN);
        }
        void PathSettings(MovableNode movable)
        {
            bool followsCamera = movable.FollowsCamera;
            if (ImGui.Checkbox("Follows Camera", ref followsCamera))
            {
                movable.FollowsCamera = followsCamera;
            }

            if (movable.HasPath)
            {
                float speed = movable.PathSpeed;
                if (ImGui.InputFloat("Speed", ref speed))
                {
                    movable.PathSpeed = speed;
                }

                if (movable.Path.Length > 0)
                {
                    float travelTime = (float)movable.Path.Length / movable.PathSpeed;

                    if (ImGui.InputFloat("Time", ref travelTime))
                    {
                        if (movable.Path.Length > 0)
                        {
                            movable.PathSpeed = (float)movable.Path.Length / travelTime;
                        }
                    }
                }
                else
                {
                    ImGui.BeginDisabled();
                    float travelTime = 0f;
                    ImGui.InputFloat("Time", ref travelTime);
                    ImGui.EndDisabled();
                }

                    LoopingMode loopMode = movable.PathLoop;
                if (ImGui.BeginCombo("Looping Mode", loopMode.ToString()))
                {
                    foreach (var mode in Enum.GetValues<LoopingMode>())
                    {
                        bool isSelected = mode == movable.PathLoop;
                        if (ImGui.Selectable(mode.ToString(), isSelected))
                        {
                            movable.PathLoop = mode;
                        }
                        if (isSelected)
                            ImGui.SetItemDefaultFocus();
                    }
                    ImGui.EndCombo();
                }

                int minPath = movable.MinPath;
                if (ImGui.InputInt("Min Path", ref minPath))
                {
                    movable.MinPath = Math.Clamp(minPath, -1, movable.Path.Decompose().Count);
                }

                if (ImGui.Button("Edit path"))
                {
                    scene.mode = EditorMode.Path;
                    scene.curveEditor.SetPath(movable.Path);
                    scene.curveEditor.SetSpawner(movable);
                }
            }
        }
    }
}
