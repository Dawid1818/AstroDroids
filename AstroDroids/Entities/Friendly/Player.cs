using Microsoft.Xna.Framework;
using MonoGame.Extended;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using AstroDroids.Input;
using AstroDroids.Gameplay;

namespace AstroDroids.Entities.Friendly
{
    public class Player : AliveEntity
    {
        int playerIndex;
        float speed = 10f;

        public Player(int playerIndex, Vector2 position) : base(new RectangleF(position.X, position.Y, 32, 32), 1) 
        {
            this.playerIndex = playerIndex;
        }

        public override void Update(GameTime gameTime)
        {
            //Firing
            GameState.CurrentWeapon.Update(this, gameTime);

            //Player movement
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

            if (Collider.X < Scene.World.Bounds.Left)
            {
                Collider.X = Scene.World.Bounds.Left;
            }
            else if (Collider.Right > Scene.World.Bounds.Right)
            {
                Collider.X = Scene.World.Bounds.Right - Collider.Width;
            }

            if (Collider.Y < Scene.World.Bounds.Top)
            {
                Collider.Y = Scene.World.Bounds.Top;
            }
            else if (Collider.Bottom > Scene.World.Bounds.Bottom)
            {
                Collider.Y = Scene.World.Bounds.Bottom - Collider.Height;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), Collider.ToRectangle(), Color.White);

            GameState.CurrentWeapon.DrawEffects(this, gameTime);
        }

        public Vector2 GetPosition()
        {
            return Collider.Position;
        }
    }
}
