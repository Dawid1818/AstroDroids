using AstroDroids.Graphics;
using AstroDroids.Paths;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace AstroDroids.Entities.Neutral
{
    public class CameraEntity : Entity
    {
        public PathManager PathManager { get; set; }

        public override void Update(GameTime gameTime)
        {
            if (PathManager != null)
            {
                PathManager.Update(gameTime);
                Transform.Position = new Vector2(-(PathManager.Position.X) - Scene.World.Bounds.Width / 2f, -(PathManager.Position.Y) - Scene.World.Bounds.Height / 2f);
            }
            else
            {
                Transform.Position = new Vector2(Screen.GetCameraPosition().X - Screen.ScreenWidth / 2, Screen.GetCameraPosition().Y - Screen.ScreenHeight / 2);
            }
            //Transform.Translate(new Vector2(0, 5f * gameTime.GetElapsedSeconds()));
        }
    }
}
