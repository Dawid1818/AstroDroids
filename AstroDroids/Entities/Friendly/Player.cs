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

        public Player(int playerIndex, Vector2 position) : base(new Transform(position.X, position.Y), 1, 110, 98)
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

            Transform.LocalPosition += movement;

            if (Transform.LocalPosition.X < Scene.World.Bounds.Left)
            {
                Transform.LocalPosition = new Vector2(Scene.World.Bounds.Left, Transform.LocalPosition.Y);
            }
            else if (LocalRight > Scene.World.Bounds.Right)
            {
                Transform.LocalPosition = new Vector2(Scene.World.Bounds.Right - Width, Transform.LocalPosition.X);
            }

            if (Transform.LocalPosition.Y < Scene.World.Bounds.Top)
            {
                Transform.LocalPosition = new Vector2(Transform.LocalPosition.X, Scene.World.Bounds.Top);
            }
            else if (LocalBottom > Scene.World.Bounds.Bottom)
            {
                Transform.LocalPosition = new Vector2(Transform.LocalPosition.X, Scene.World.Bounds.Bottom - Height);
            }

            foreach (var enemy in Scene.World.Enemies)
            {
                if (enemy.Intersects(this))
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
            return Transform.Position;
        }

        public Vector2 GetLocalPosition()
        {
            return Transform.LocalPosition;
        }

        public override void Destroyed()
        {
            Scene.World.RemovePlayer(this);
            GameState.RemoveLife();
        }
    }
}
