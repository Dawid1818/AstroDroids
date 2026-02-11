using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstroDroids.Drawables
{
    public class ImageStarfield : Starfield
    {
        Texture2D image;

        public ImageStarfield(Texture2D image)
        {
            this.image = image;
        }

        public override void Draw()
        {
            Screen.spriteBatch.Begin(effect: Screen.Infinite, transformMatrix: Screen.GetCameraMatrix(), samplerState: SamplerState.LinearWrap);
            Screen.spriteBatch.Draw(image, new Rectangle(0, 0, Screen.ScreenWidth, Screen.ScreenHeight), Color.White);
            Screen.spriteBatch.End();
        }
    }
}
