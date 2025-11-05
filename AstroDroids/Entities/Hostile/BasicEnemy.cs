using AstroDroids.Curves;
using AstroDroids.Entities.Neutral;
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

        EntityCell cell;

        public float t = 0f;

        public BasicEnemy(Vector2 position, EntityCell cell) : base(new RectangleF(position.X, position.Y, 32f, 32f), 1)
        {
            this.cell = cell;
            curve = new BezierCurve(new List<Vector2>() { new Vector2(30, -32), new Vector2(30, 300), new Vector2(600, 300), cell.Position });
        }

        public override void Update(GameTime gameTime)
        {
            if(t < 1f)
            {
                t += 0.01f;
                curve.SetPointAtIndex(3, cell.Position);
                Collider.Position = curve.GetPoint(t);

            }
            else
            {
                Collider.Position = cell.Position;
            }        
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), Collider.ToRectangle(), Color.Red);
        }
    }
}
