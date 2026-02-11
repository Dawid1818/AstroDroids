using AstroDroids.Drawables;
using AstroDroids.Levels;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

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
            if(ImGui.Begin("Level Settings", ref show))
            {
                string levelName = level.Name;
                if(ImGui.InputText("Level Name", ref levelName, 255))
                {
                    level.Name = levelName;
                }

                List<Texture2D> list = TextureManager.GetStarfields();

                if (ImGui.BeginCombo("Background", level.BackgroundId == 0 ? "Simulation" : list[level.BackgroundId - 1].Name))
                {
                    if(ImGui.Selectable("Simulation", level.BackgroundId == 0))
                    {
                        level.BackgroundId = 0;
                        scene.World.Starfield = new SimulationStarfield();
                    }

                    for (int i = 0; i < list.Count; i++)
                    {
                        if(ImGui.Selectable(list[i].Name, level.BackgroundId == i + 1))
                        {
                            level.BackgroundId = i + 1;
                            scene.World.Starfield = new ImageStarfield(list[i]);
                        }
                    }

                    ImGui.EndCombo();
                }

                ImGui.End();
            }
        }
    }
}
