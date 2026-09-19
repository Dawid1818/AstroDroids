using AstroDroids.Scenes;
using AstroDroids.Screens;
using Microsoft.Xna.Framework;

namespace AstroDroids.Interfaces
{
    public interface IMenuPage
    {
        public bool UpdateWhenTransitioning { get; }
        public void Initialize(IPageHost scene, HintedScreenGum hinted);
        public void Uninitialize();
        public void TransitionOut();
        public void TransitionIn();
        public bool TransitionFinished();
        public void BackPressed();
        public void Update(GameTime gameTime);
    }
}
