using Microsoft.Xna.Framework;
using MonoGame.Extended;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using AstroDroids.Input;

namespace AstroDroids.Entities
{
    public class Player : Entity
    {
        float speed = 10f;

        public Player(Vector2 position) : base(new RectangleF(position.X, position.Y, 32, 32), 1) 
        {

        }

        public override void Update(GameTime gameTime)
        {
            Vector2 movement = Vector2.Zero;

            if (InputSystem.IsActionHeld(GameAction.Up))
            {
                movement.Y -= speed;
            }

            if (InputSystem.IsActionHeld(GameAction.Down))
            {
                movement.Y += speed;
            }

            if (InputSystem.IsActionHeld(GameAction.Left))
            {
                movement.X -= speed;
            }

            if (InputSystem.IsActionHeld(GameAction.Right))
            {
                movement.X += speed;
            }

            Collider.Position += movement;
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), Collider.ToRectangle(), Color.White);
        }
    }
}
