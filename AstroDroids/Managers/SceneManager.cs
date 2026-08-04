using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Scenes;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Numeric = System.Numerics;

namespace AstroDroids.Managers
{
    public class SceneManager
    {
        private static Scene scene;
        private static bool openSceneSelect = false;
        private static bool showSceneSelect = false;
        private static int selectedScene = -1;

        public static Scene GetScene()
        {
            return scene;
        }

        public static void SetScene(Scene newScene)
        {
            scene = newScene;
            scene.Set();
        }

        public static void Update(GameTime gameTime)
        {
            if (InputSystem.GetKeyDown(Keys.F1))
            {
                selectedScene = -1;
                openSceneSelect = true;
            }

            scene?.Update(gameTime);
        }

        public static void Draw(GameTime gameTime)
        {
            scene?.Draw(gameTime);
            if (Screen.GetImGuiRenderer().InputReady)
            {
                scene?.DrawImGui(gameTime);

                if (openSceneSelect)
                {
                    ImGui.OpenPopup("Scene Select##SceneManagerSceneSelect");
                    showSceneSelect = true;
                    openSceneSelect = false;
                }

                DrawSceneSelect();
            }
        }

        public static void DrawDebug(GameTime gameTime)
        {
            scene?.DrawDebug(gameTime);
        }

        static void DrawSceneSelect()
        {
            ImGuiIOPtr io = ImGui.GetIO();

            if (ImGui.IsPopupOpen("Scene Select##SceneManagerSceneSelect"))
            {
                ImGui.SetNextWindowPos(new Numeric.Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Always, new Numeric.Vector2(0.5f, 0.5f));
                //ImGui.SetNextWindowSize(new Numeric.Vector2(400, 300), ImGuiCond.Always);
            }
            if (ImGui.BeginPopupModal("Scene Select##SceneManagerSceneSelect", ref showSceneSelect))
            {
                ImGui.SetNextItemWidth(-1);
                if (ImGui.BeginListBox("##Scenes"))
                {
                    if (ImGui.Selectable("Main Menu", selectedScene == 0))
                    {
                        selectedScene = 0;
                    }

                    if (ImGui.Selectable("Game", selectedScene == 1))
                    {
                        selectedScene = 1;
                    }

                    if (ImGui.Selectable("Level Editor", selectedScene == 2))
                    {
                        selectedScene = 2;
                    }

                    ImGui.EndListBox();
                }

                if (ImGui.Button("Select"))
                {
                    switch (selectedScene)
                    {
                        case 0:
                            SetScene(new MainMenuScene());
                            break;
                        case 1:
                            SetScene(new GameScene());
                            break;
                        case 2:
                            SetScene(new LevelEditorScene());
                            break;
                        default:
                            break;
                    }
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
        }
    }
}
