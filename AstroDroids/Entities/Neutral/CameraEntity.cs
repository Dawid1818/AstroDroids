using AstroDroids.Graphics;
using Microsoft.Xna.Framework;

namespace AstroDroids.Entities.Neutral
{
    public class CameraEntity : Entity
    {
        public override void Update(GameTime gameTime)
        {
            Transform.Position = new Vector2(Screen.GetCameraPosition().X - Screen.ScreenWidth / 2, Screen.GetCameraPosition().Y - Screen.ScreenHeight / 2);
        }
    }
}
