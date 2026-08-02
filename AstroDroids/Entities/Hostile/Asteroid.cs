using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace AstroDroids.Entities.Hostile
{
    public class Asteroid : Enemy
    {
        public float t = 0f;

        Texture2D texture;

        float angle = 3.14f;
        Vector2 velocity = Vector2.Zero;

        bool becameActive = false;

        public Asteroid() : base(Vector2.Zero, 20)
        {
            IsNeutral = true;

            texture = TextureManager.Get("Asteroids/Asteroid 01 - Base");

            AddCircleCollider(Vector2.Zero, 16f);
        }

        public override void Update(GameTime gameTime)
        {
            if (!becameActive)
            {
                if (Intersects(Scene.World.Bounds))
                {
                    becameActive = true;
                }
                else
                {
                    if (t >= 10f)
                        Despawn();

                    t += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
            if (!Intersects(Scene.World.Bounds) && becameActive)
            {
                Despawn();
            }

            if (PathManager != null)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
                angle = GameHelper.AngleFromDir(PathManager.Direction) + 1.571f;

                if (!PathManager.Active)
                {
                    Despawn();
                }
            }
            else
            {
                if (!FollowsCamera)
                {
                    Transform.Position += (velocity);
                }

                if (Transform.Position.Y > Scene.World.Bounds.Bottom + texture.Height)
                {
                    Despawn();
                }
            }

            foreach (var item in Scene.World.GetPlayers())
            {
                if (item.Intersects(this))
                {
                    item.Damage(50, false);
                    Damage(50, false);

                    return;
                }
            }

            foreach (var item in Scene.World.Enemies)
            {
                if (item.Intersects(this))
                {
                    item.Damage(50, false);
                    Damage(50, false);

                    return;
                }
            }

            angle += velocity.X * gameTime.GetElapsedSeconds();
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, position: new Vector2(Transform.Position.X, Transform.Position.Y), null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
        }

        public override void Push(Vector2 direction)
        {
            velocity += direction;
        }
    }
}
