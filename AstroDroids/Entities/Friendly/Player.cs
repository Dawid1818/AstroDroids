using Microsoft.Xna.Framework;
using MonoGame.Extended;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using AstroDroids.Input;
using AstroDroids.Gameplay;
using Microsoft.Xna.Framework.Graphics;
using AstroDroids.Drawables;

namespace AstroDroids.Entities.Friendly
{
    public class Player : AliveEntity
    {
        int playerIndex;
        float speed = 10f;

        Texture2D shipTexture;
        Texture2D exhaustTexture;

        CompositeShip ship;

        float thrusterPower = 1f;

        public Player(int playerIndex, Vector2 position) : base(new RectangleF(position.X, position.Y, 110, 98), 1) 
        {
            this.playerIndex = playerIndex;
            shipTexture = TextureManager.Get("Ships/Player/PlayerShip");
            exhaustTexture = TextureManager.Get("Ships/Player/Exhaust");

            ship = new CompositeShip();
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

            foreach (var enemy in Scene.World.Enemies)
            {
                if(enemy.CollidesWith(Collider))
                {
                    enemy.Damage(5, true);
                    Damage(1, false);
                    break;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //Screen.spriteBatch.Draw(exhaustTexture, new Rectangle((int)GetPosition().X, (int)Collider.Bottom, 20, exhaustTexture.Height), Color.White);
            //Screen.spriteBatch.Draw(exhaustTexture, new Rectangle((int)Collider.Right - 20, (int)Collider.Bottom, 20, exhaustTexture.Height), Color.White);
            //Screen.spriteBatch.Draw(shipTexture, Collider.ToRectangle(), Color.White);
            ship.Draw(GetPosition());

            GameState.CurrentWeapon.DrawEffects(this, gameTime);
        }

        public Vector2 GetPosition()
        {
            return Collider.Position;
        }

        public override void Destroyed()
        {
            Scene.World.RemovePlayer(this);
            GameState.RemoveLife();
        }
    }
}
