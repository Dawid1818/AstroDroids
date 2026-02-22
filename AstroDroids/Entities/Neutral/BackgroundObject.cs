using AstroDroids.Editors;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using AstroDroids.Paths;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstroDroids.Entities.Neutral
{
    public class BackgroundObject : Entity
    {
        float angle;
        Texture2D texture;
        SpriteEffects Effects;
        public PathManager PathManager { get; set; }
        public bool FollowsCamera { get; set; }

        public BackgroundObject(string textureName, float angle, bool flipH, bool flipV)
        {
            this.angle = angle;
            texture = TextureManager.GetBackgroundObject(textureName);

            if (flipH)
                Effects = Effects | SpriteEffects.FlipHorizontally;
            if (flipV)
                Effects = Effects | SpriteEffects.FlipVertically;
        }

        public override void Update(GameTime gameTime)
        {
            if (texture == null)
            {
                Despawn();
                return;
            }

            if (PathManager != null && PathManager.Active)
            {
                if(!FollowsCamera)
                    PathManager.Translate(new Vector2(0, (float)Scene.World.speed));
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
            }
            else
            {
                if (!FollowsCamera)
                    DefaultMove();
            }
        }
        public override void Draw(GameTime gameTime)
        {
            if(PathManager != null)
            {
                PathVisualizer.DrawPath((CompositePath)PathManager.GetPath());
            }

            if (texture != null)
                Screen.spriteBatch.Draw(texture, Transform.Position, null, Color.White, MathHelper.ToRadians(angle), new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, Effects, 0f);
        }

        protected void Despawn()
        {
            Scene.World.RemoveBackgroundObject(this);
        }
    }
}
