using Hexa.NET.ImGui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numeric = System.Numerics;

namespace AstroDroids.Editors
{
    public class LevelBrowser
    {
        public Action<string> LevelSelected;

        int selectedIndex = -1;
        List<string> Levels = new List<string>();

        bool shown = false;

        public void ShowModal()
        {
            if(Directory.Exists("Content/Levels"))
                Levels = System.IO.Directory.GetFiles("Content/Levels", "*.adlvl").Select(f => System.IO.Path.GetFileNameWithoutExtension(f)).ToList();
            ImGui.OpenPopup("Level Browser##LevelBrowser");
            selectedIndex = -1;
            shown = true;
        }

        public void DrawImGui()
        {
            ImGuiIOPtr io = ImGui.GetIO();

            if (ImGui.IsPopupOpen("Level Browser##LevelBrowser"))
                ImGui.SetNextWindowPos(new Numeric.Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Always, new Numeric.Vector2(0.5f, 0.5f));
            if (ImGui.BeginPopupModal("Level Browser##LevelBrowser", ref shown, ImGuiWindowFlags.AlwaysAutoResize))
            {
                if(ImGui.BeginListBox("##Levels"))
                {
                    for (int i = 0; i < Levels.Count; i++)
                    {
                        string item = Levels[i];
                        if (ImGui.Selectable(item, selectedIndex == i))
                        {
                            selectedIndex = i;
                        }
                    }

                    ImGui.EndListBox();
                }

                if(ImGui.Button("Select"))
                {
                    if(selectedIndex > -1 && selectedIndex < Levels.Count)
                        LevelSelected?.Invoke(Levels[selectedIndex]);
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
        }
    }
}
