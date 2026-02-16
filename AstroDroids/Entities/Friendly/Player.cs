using AstroDroids.Drawables;
using AstroDroids.Gameplay;
using AstroDroids.Input;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstroDroids.Entities.Friendly
{
    public class Player : AliveEntity
    {
        int playerIndex;
        float speed = 10f;

        Texture2D exhaustTexture;

        CompositeShip ship;

        float thrusterPower = 1f;

        public float Angle { get; private set; } = 0f;

        public Player(int playerIndex, Vector2 position) : base(new Transform(position.X, position.Y), 1, 110, 98)
        {
            this.playerIndex = playerIndex;
            exhaustTexture = TextureManager.Get("Ships/Player/Exhaust");

            ship = new CompositeShip();

            AddCircleCollider(Vector2.Zero, 55f);
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

            if (Transform.LocalPosition.X - Width / 2f < Scene.World.Bounds.Left)
            {
                Transform.LocalPosition = new Vector2(Scene.World.Bounds.Left + Width / 2f, Transform.LocalPosition.Y);
            }
            else if (Transform.LocalPosition.X + Width / 2f > Scene.World.Bounds.Right)
            {
                Transform.LocalPosition = new Vector2(Scene.World.Bounds.Right - Width / 2f, Transform.LocalPosition.Y);
            }

            if (Transform.LocalPosition.Y - Height / 2f < Scene.World.Bounds.Top)
            {
                Transform.LocalPosition = new Vector2(Transform.LocalPosition.X, Scene.World.Bounds.Top + Height / 2f);
            }
            else if (Transform.LocalPosition.Y + Height / 2f > Scene.World.Bounds.Bottom)
            {
                Transform.LocalPosition = new Vector2(Transform.LocalPosition.X, Scene.World.Bounds.Bottom - Height / 2f);
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
            ship.Draw(GetPosition(), Angle);

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
