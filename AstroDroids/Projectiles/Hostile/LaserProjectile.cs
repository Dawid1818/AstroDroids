using AstroDroids.Entities;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace AstroDroids.Projectiles.Hostile
{
    public class LaserProjectile : Projectile
    {
        float t = 0f;
        float angle;
        Vector2 movementDirection;
        float speed = 200f;

        bool becameActive = false;

        Texture2D texture;

        public LaserProjectile(Vector2 position, float angle) : base(position)
        {
            this.angle = angle;

            texture = TextureManager.GetProjectile("BasicWeapon/02");

            movementDirection = GameHelper.DirFromAngle(angle);

            AddCircleCollider(Vector2.Zero, 16);

            if (Intersects(Scene.World.Bounds))
            {
                becameActive = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if(!becameActive)
            {
                if(Intersects(Scene.World.Bounds))
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

            Transform.LocalPosition += movementDirection * speed * gameTime.GetElapsedSeconds();

            foreach (var item in Scene.World.GetPlayers())
            {
                if (item.Intersects(this))
                {
                    item.Damage(1, false);
                    Despawn();

                    return;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, Transform.Position, new Rectangle(0, (int)0, texture.Width, texture.Height), Color.White, angle, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, 0f);
            //Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle((int)Transform.Position.X, (int)Transform.Position.Y, 32, 16), null, Color.White, angle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
        }
    }
}
