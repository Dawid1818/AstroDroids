using AstroDroids.Paths;
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

        //BezierCurve curve;

        EntityCell cell;

        Texture2D texture;

        float angle = 0f;

        public BasicEnemy() : base(new Transform(0, 0), 1, 32f, 32f)
        {
            texture = TextureManager.Get("Ships/Basic/Basic");
        }

        public BasicEnemy(Vector2 position, EntityCell cell) : base(new Transform(position.X, position.Y), 1, 32f, 32f)
        {
            //this.cell = cell;
            //curve = new BezierCurve(new List<Vector2>() { new Vector2(30, -32), new Vector2(30, 300), new Vector2(600, 300), cell.Position });
            texture = TextureManager.Get("Ships/Basic/Basic");
        }

        public override void Update(GameTime gameTime)
        {
            if(PathManager != null)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
            }

            //if (Path != null)
            //{
            //    if (t < 1f)
            //    {
            //        t += 0.01f;
            //        //Path.SetPointAtIndex(3, cell.Position);
            //        Transform.Position = PathManager.GetPoint(t);

            //        //Vector2 dir = Path.GetDirection(t);

            //        //angle = (float)Math.Atan2(dir.Y, dir.X) + 1.571f;
            //    }
            //    else
            //    {
            //        //Transform.Position = cell.Position;
            //        //angle = 3.142f;
            //    }
            //}
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, ToRectangle(), null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0f);
        }
    }
}
