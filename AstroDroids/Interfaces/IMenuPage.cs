using AstroDroids.Scenes;
using AstroDroids.Screens;

namespace AstroDroids.Interfaces
{
    public interface IMenuPage
    {
        public void Initialize(MainMenuScene scene, HintedScreenGum hinted);
        public void BackPressed();
    }
}
