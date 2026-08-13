using AstroDroids.Graphics;
using Microsoft.Xna.Framework;

namespace AstroDroids.Entities.Warnings
{
    public class ShielderConnection : Entity
    {
        public Vector2 target { get; set; }

        public override void Draw(GameTime gameTime)
        {
            Screen.shapeBatch.DrawLine(Transform.Position, target, 12, new Color(Color.Blue.R, Color.Blue.G, Color.Blue.B, (byte)127), Color.Cyan);
        }
    }
}
