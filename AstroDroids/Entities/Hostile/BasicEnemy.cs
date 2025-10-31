using AstroDroids.Curves;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Collections.Generic;

namespace AstroDroids.Entities.Hostile
{
    public class BasicEnemy : Enemy
    {
        BezierCurve curve;

        public float t = 0f;

        public BasicEnemy(Vector2 position) : base(new RectangleF(position.X, position.Y, 32f, 32f), 1)
        {
            curve = new BezierCurve(new List<Vector2>() { new Vector2(30, -32), new Vector2(30, 300), new Vector2(600, 300), new Vector2(600, 20) });
        }

        public override void Update(GameTime gameTime)
        {
            t += 0.01f;
            if (t > 1f)
            {
                t = 1f;
            }
            Collider.Position = curve.GetPoint(t);
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), Collider.ToRectangle(), Color.Red);
        }
    }
}
