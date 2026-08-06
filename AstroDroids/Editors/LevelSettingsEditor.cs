using AstroDroids.Drawables;
using AstroDroids.Levels;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Numeric = System.Numerics;

namespace AstroDroids.Editors
{
    public class LevelSettingsEditor
    {
        Level level { get { return LevelManager.CurrentLevel; } }

        LevelEditorScene scene;
        public LevelSettingsEditor(LevelEditorScene scene)
        {
            this.scene = scene;
        }

        public void DrawImGui(ref bool show)
        {
            if (ImGui.Begin("Level Settings", ref show))
            {
                string levelName = level.Name;
                if (ImGui.InputText("Level Name", ref levelName, 255))
                {
                    level.Name = levelName;
                }

                List<Texture2D> list = TextureManager.GetStarfields();

                if (ImGui.BeginCombo("Background", level.BackgroundId == 0 ? "Simulation" : list[level.BackgroundId - 1].Name))
                {
                    if (ImGui.Selectable("Simulation", level.BackgroundId == 0))
                    {
                        level.BackgroundId = 0;
                        scene.World.Starfield = new SimulationStarfield();
                    }

                    for (int i = 0; i < list.Count; i++)
                    {
                        //if (ImGui.Selectable(list[i].Name, level.BackgroundId == i + 1))
                        //{
                        //    level.BackgroundId = i + 1;
                        //    scene.World.Starfield = new ImageStarfield(list[i]);
                        //}

                        selectableButton(list[i].Name, level.BackgroundId == i + 1, 0, 72,
                        () =>
                        {

                            level.BackgroundId = i + 1;
                            scene.World.Starfield = new ImageStarfield(list[i]);
                        }, () =>
                        {
                            float yPos = ImGui.GetCursorPosY();
                            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 4);
                            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 4);
                            ImGui.Image(GameDatabase.GetStarfieldPreview(i + 1), new Numeric.Vector2(64, 64));
                            ImGui.SameLine();
                            var textSize = ImGui.CalcTextSize(list[i].Name);
                            ImGui.SetCursorPosY(yPos + 72 / 2 - textSize.Y / 2);
                            ImGui.Text(list[i].Name);
                        });
                    }

                    ImGui.EndCombo();
                }

                var handlers = GameDatabase.GetAllEventHandlers();

                if (ImGui.BeginCombo("Event Handler", handlers.TryGetValue(level.EventHandlerId, out Type val) ? val.Name : $"Handler with ID {level.EventHandlerId} not found!"))
                {
                    foreach (var handler in handlers)
                    {
                        if (ImGui.Selectable(handler.Value.Name, level.EventHandlerId == handler.Key))
                        {
                            level.EventHandlerId = handler.Key;
                            level.ReloadEventHandler();
                            level.RegisterEvents();
                        }
                    }

                    ImGui.EndCombo();
                }

                var allMusic = GameDatabase.GetAllMusic();
                string musicName = GameDatabase.GetMusic(level.MusicId);

                if (ImGui.BeginCombo("Music", string.IsNullOrWhiteSpace(musicName) ? $"Music with ID {level.MusicId} not found!" : musicName))
                {
                    foreach (var music in allMusic)
                    {
                        if (ImGui.Selectable(music, level.MusicId == allMusic.IndexOf(music)))
                        {
                            level.MusicId = allMusic.IndexOf(music);
                        }
                    }
                    ImGui.EndCombo();
                }

                ImGui.End();
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
    }
}
