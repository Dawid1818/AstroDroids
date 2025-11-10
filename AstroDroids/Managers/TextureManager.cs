using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AstroDroids.Managers
{
    public class TextureManager
    {
        static bool initialized = false;
        static Texture2D pixelTexture;

        static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        public static void Initialize(AstroDroidsGame game)
        {
            if(initialized) return;

            pixelTexture = new Texture2D(game.GraphicsDevice, 1, 1);

            pixelTexture.SetData(new Color[] { Color.White });

            LoadAllTextures(game.Content);

            initialized = true;
        }

        public static Texture2D Get(string textureName)
        {
            if (textures.TryGetValue(textureName, out Texture2D texture))
            {
                return texture;
            }
            else
            {
                return null;
            }
        }

        static void LoadAllTextures(ContentManager content)
        {
            Directory.GetFiles("Content/Textures", "*.xnb", SearchOption.AllDirectories).ToList().ForEach(filePath =>
            {
                string relativePath = filePath.Substring(8).Replace(".xnb", "").Replace("\\", "/");
                string textureName = Path.GetFileNameWithoutExtension(filePath);
                if (!textures.ContainsKey(textureName))
                {
                    Texture2D texture = content.Load<Texture2D>(relativePath);
                    textures.Add(relativePath.Substring(9), texture);
                }
            });
        }

        public static Texture2D GetPixelTexture() 
        {
            return pixelTexture; 
        }

        public static Texture2D GetStarfield()
        {
            return Get("Starfields/BlueStarfield");
        }
    }
}
