using AstroDroids.Managers;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using System;

namespace AstroDroids.Graphics
{
    public static class Screen
    {
        static SpriteBatch spriteBatch;
        static GumService GumUI => GumService.Default;
        static GumProjectSave gumProject;

        public static void Initialize(AstroDroidsGame game)
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);

            gumProject = GumUI.Initialize(game, "GumProject/AstroDroidsGum.gumx");
        }

        public static SpriteBatch GetSpriteBatch()
        {
            return spriteBatch;
        }

        public static GumService GetUIService()
        {
            return GumUI;
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
