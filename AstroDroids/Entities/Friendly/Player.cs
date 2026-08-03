using AstroDroids.Data;
using AstroDroids.Drawables;
using AstroDroids.Entities.Effects;
using AstroDroids.Gameplay;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Input;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace AstroDroids.Entities.Friendly
{
    public class Player : AliveEntity
    {
        int playerIndex;
        float speed = 10f;

        Texture2D exhaustTexture;
        Texture2D prototypeTexture;

        CompositeShip ship;

        float thrusterPower = 1f;

        public float Angle { get; private set; } = 0f;

        public Vector2 RightWeaponPod { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f) + 4, -Height), Vector2.Zero, Angle); } }
        public Vector2 LeftWeaponPod { get { return GameHelper.RotateAroundPoint(new Vector2(-((Width / 2f) + 2), -Height), Vector2.Zero, Angle); } }
        public Vector2 MiddleWeaponPod { get { return GameHelper.RotateAroundPoint(new Vector2(1, -Height), Vector2.Zero, Angle); } }

        public Vector2 RearRightWeaponPod { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f) + 12, Height - 20), Vector2.Zero, Angle); } }
        public Vector2 RearLeftWeaponPod { get { return GameHelper.RotateAroundPoint(new Vector2(-((Width / 2f) + 10), Height - 20), Vector2.Zero, Angle); } }

        bool destroyed = false;

        public bool LockMovement { get; set; } = false;

        float InvTime = 0f;

        public Player(int playerIndex, Vector2 position) : base(new Transform(position), 1)
        {
            this.playerIndex = playerIndex;
            exhaustTexture = TextureManager.Get("Ships/Player/Exhaust");
            prototypeTexture = TextureManager.Get("Ships/Player/PlayerShipPrototype");

            ship = new CompositeShip();

            AddCircleCollider(Vector2.Zero, 25f);

            InvTime = 3f;
        }

        public override void Update(GameTime gameTime)
        {
            if (destroyed)
                return;

            if(InvTime > 0f)
            {
                CanBeDamaged = false;
                InvTime -= gameTime.GetElapsedSeconds();
            }
            else
            {
                CanBeDamaged = true;
            }

            //Firing
            GameStateManager.UpdateCurrentWeapon(this, gameTime);

            //Player movement
            Vector2 movement = Vector2.Zero;

            //Angle = MathHelper.ToRadians(180);
            //Angle += 0.01f;

            if (!LockMovement)
            {
                float actualSpeed = InputSystem.IsActionHeld(GameAction.Focus) ? speed / 2f : speed;

                if (InputSystem.IsActionHeld(GameAction.Up))
                {
                    //movement.Y -= actualSpeed;
                    movement.Y -= 1;
                }

                if (InputSystem.IsActionHeld(GameAction.Down))
                {
                    //movement.Y += actualSpeed;
                    movement.Y += 1;
                }

                if (InputSystem.IsActionHeld(GameAction.Left))
                {
                    //movement.X -= actualSpeed;
                    movement.X -= 1;
                }

                if (InputSystem.IsActionHeld(GameAction.Right))
                {
                    //movement.X += actualSpeed;
                    movement.X += 1;
                }

                Vector2 leftJoy = InputSystem.GetLeftJoystick();
                movement.X += leftJoy.X;
                movement.Y += -leftJoy.Y;

                Vector2 mouseDelta = InputSystem.GetMouseDelta();
                movement.X += mouseDelta.X;
                movement.Y += mouseDelta.Y;

                float length = movement.Length();

                if (length > 1f)
                    movement /= length;

                Transform.LocalPosition += movement * actualSpeed;

                if (Transform.LocalPosition.X - Width < Scene.World.Bounds.Left)
                {
                    Transform.LocalPosition = new Vector2(Scene.World.Bounds.Left + Width, Transform.LocalPosition.Y);
                }
                else if (Transform.LocalPosition.X + Width > Scene.World.Bounds.Right)
                {
                    Transform.LocalPosition = new Vector2(Scene.World.Bounds.Right - Width, Transform.LocalPosition.Y);
                }

                if (Transform.LocalPosition.Y - Height < Scene.World.Bounds.Top)
                {
                    Transform.LocalPosition = new Vector2(Transform.LocalPosition.X, Scene.World.Bounds.Top + Height);
                }
                else if (Transform.LocalPosition.Y + Height > Scene.World.Bounds.Bottom)
                {
                    Transform.LocalPosition = new Vector2(Transform.LocalPosition.X, Scene.World.Bounds.Bottom - Height);
                }
            }


            if(InvTime > 0) {
                return;
            }
            foreach (var enemy in Scene.World.Enemies)
            {
                if (enemy.Collidable && enemy.Intersects(this))
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
            //Screen.spriteBatch.Draw(prototypeTexture, new Vector2(Transform.Position.X, Transform.Position.Y), Color.White);
            //Screen.spriteBatch.Draw(prototypeTexture, new Vector2(Transform.Position.X, Transform.Position.Y), null, Color.White, Angle, new Vector2(prototypeTexture.Width / 2, prototypeTexture.Height / 2), 0.5f, SpriteEffects.None, 0f);

            ship.Draw(GetPosition(), Angle, 0.5f);

            GameStateManager.DrawCurrentWeapon(this, gameTime);

            if(InvTime > 0f)
            {
                Screen.spriteBatch.DrawCircle(GetPosition(), 42, 12, new Color(Color.Blue.R, Color.Blue.G, Color.Blue.B, (byte)127), 12);
            }
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
            if (!destroyed)
            {
                Scene.World.RemovePlayer(this);
                GameStateManager.RemoveLife();
                Scene.World.RequestPlayerRespawn(playerIndex);
                Scene.World.AddEffect(new StandardExplosion(new Transform(Transform.Position.X, Transform.Position.Y), 1f));
                destroyed = true;
            }
        }

        public void ApplyCustomization(ShipCustomization customization)
        {
            ship.ApplyCustomization(customization);
        }
    }
}
