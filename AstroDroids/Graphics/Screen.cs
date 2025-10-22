using Gum.DataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;

namespace AstroDroids.Graphics
{
    public static class Screen
    {
        public static SpriteBatch spriteBatch { get; private set; }
        public static GumService GumUI => GumService.Default;
        static GumProjectSave gumProject;

        public static void Initialize(AstroDroidsGame game)
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);

            gumProject = GumUI.Initialize(game, "GumProject/AstroDroidsGum.gumx");
        }

        public static void Update(GameTime gameTime)
        {
            GumUI.Update(gameTime);
        }

        public static void Draw(GameTime gameTime)
        {
            GumUI.Draw();
        }
    }
}
