using AstroDroids.Entities;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstroDroids.Projectiles.Hostile
{
    public class LaserProjectile : Projectile
    {
        float t = 0f;
        float angle;
        Vector2 movementDirection;
        float speed = 10f;

        public LaserProjectile(Transform collider, float width, float height, float angle) : base(collider, width, height)
        {
            this.angle = angle;

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
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle((int)Transform.Position.X, (int)Transform.Position.Y, 32, 16), null, Color.White, angle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
        }
    }
}
