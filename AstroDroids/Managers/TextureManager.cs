using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroDroids.Managers
{
    public class TextureManager
    {
        static bool initialized = false;
        static Texture2D pixelTexture;

        public static void Initialize(AstroDroidsGame game)
        {
            if(initialized) return;

            pixelTexture = new Texture2D(game.GraphicsDevice, 1, 1);

            pixelTexture.SetData(new Color[] { Color.White });

            initialized = true;
        }

        public static Texture2D GetPixelTexture() 
        {
            return pixelTexture; 
        }
    }
}
