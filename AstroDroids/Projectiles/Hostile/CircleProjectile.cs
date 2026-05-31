using AstroDroids.Entities;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace AstroDroids.Projectiles.Hostile
{
    public class CircleProjectile : Projectile
    {
        float t = 0f;
        Vector2 movementDirection;
        float speed = 10f;
        float size = 16f;

        public CircleProjectile(Vector2 position, float angle, float speed, float size) : base(position)
        {
            movementDirection = GameHelper.DirFromAngle(angle);
            this.speed = speed;
            this.size = size;

            AddCircleCollider(Vector2.Zero, size);
        }

        public override void Update(GameTime gameTime)
        {
            if (t >= 3)
                Despawn();

            Transform.LocalPosition += movementDirection * speed;

            foreach (var item in Scene.World.GetPlayers())
            {
                if (item.Intersects(this))
                {
                    item.Damage(1, false);
                    Despawn();

                    return;
                }
            }

            t += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.DrawCircle(Transform.Position, size, 16, Color.OrangeRed, size);
            Screen.spriteBatch.DrawCircle(Transform.Position, size - 4f, 16, Color.DarkOrange, size - 4);
        }
    }
}
