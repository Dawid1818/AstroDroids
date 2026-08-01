using AstroDroids.Scenes;
using AstroDroids.Screens;

namespace AstroDroids.Interfaces
{
    public interface IMenuPage
    {
        public void Initialize(MainMenuScene scene, HintedScreenGum hinted);
        public void Uninitialize();
        public void TransitionOut();
        public bool TransitionFinished();
        public void BackPressed();
    }
}
