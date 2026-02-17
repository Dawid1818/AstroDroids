using AstroDroids.Entities;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace AstroDroids.Projectiles.Hostile
{
    public class CircleProjectile : Projectile
    {
        float t = 0f;
        Vector2 movementDirection;
        float speed = 10f;

        public CircleProjectile(Transform collider, float width, float height, float angle) : base(collider, width, height)
        {
            movementDirection = GameHelper.DirFromAngle(angle);

            AddCircleCollider(Vector2.Zero, 16);
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
            Screen.spriteBatch.DrawCircle(Transform.Position, 16, 16, Color.OrangeRed, 16);
            Screen.spriteBatch.DrawCircle(Transform.Position, 12, 12, Color.DarkOrange, 12);
        }
    }
}
