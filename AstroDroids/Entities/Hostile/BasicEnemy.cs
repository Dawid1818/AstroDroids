using AstroDroids.Curves;
using AstroDroids.Entities.Neutral;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace AstroDroids.Entities.Hostile
{
    public class BasicEnemy : Enemy
    {
        public float t = 0f;

        BezierCurve curve;

        EntityCell cell;

        Texture2D texture;

        float angle = 0f;

        public BasicEnemy(Vector2 position, EntityCell cell) : base(new Transform(position.X, position.Y, 32f, 32f), 1)
        {
            this.cell = cell;
            curve = new BezierCurve(new List<Vector2>() { new Vector2(30, -32), new Vector2(30, 300), new Vector2(600, 300), cell.Position });
            texture = TextureManager.Get("Ships/Basic/Basic");
        }

        public override void Update(GameTime gameTime)
        {
            if(t < 1f)
            {
                t += 0.01f;
                curve.SetPointAtIndex(3, cell.Position);
                Transform.Position = curve.GetPoint(t);

                Vector2 dir = curve.GetDirection(t);

                angle = (float)Math.Atan2(dir.Y, dir.X) + 1.571f;
            }
            else
            {
                Transform.Position = cell.Position;
                angle = 3.142f;
            }        
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, Transform.ToRectangle(), null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0f);
        }
    }
}
