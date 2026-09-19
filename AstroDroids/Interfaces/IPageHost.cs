using AstroDroids.Scenes;
using Gum.Forms.Controls;

namespace AstroDroids.Interfaces
{
    public interface IPageHost
    {
        public void SetPage(FrameworkElement page, bool hideLogo);
        public void TransitionClose();
        public void TransitionToScene(Scene scene);
    }
}
