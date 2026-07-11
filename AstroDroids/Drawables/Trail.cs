using AstroDroids.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AstroDroids.Drawables
{
    public class Trail
    {
        Texture2D texture;

        List<Vector2> trailPositions = new List<Vector2>();
        int maxTrailPoints = 10;

        public Trail(Texture2D texture)
        {
            this.texture = texture;
        }

        public void Update(Vector2 position)
        {
            trailPositions.Insert(0, position);
            if (trailPositions.Count > maxTrailPoints)
            {
                trailPositions.RemoveAt(trailPositions.Count - 1);
            }
        }

        public void Draw(float angle)
        {
            for (int i = 0; i < trailPositions.Count; i++)
            {
                float alpha = 1f - (float)(i) / maxTrailPoints;
                Screen.spriteBatch.Draw(texture, trailPositions[i], null, Color.White * alpha, angle, new Vector2(texture.Width / 2, texture.Height / 2), 0.5f, SpriteEffects.None, 0f);
            }
        }
    }
}
