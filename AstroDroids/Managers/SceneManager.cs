using AstroDroids.Scenes;
using Microsoft.Xna.Framework;

namespace AstroDroids.Managers
{
    public class SceneManager
    {
        private static Scene scene;

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
            scene?.Update(gameTime);
        }

        public static void Draw(GameTime gameTime)
        {
            scene?.Draw(gameTime);
            scene?.DrawImGui(gameTime);
        }
    }
}
