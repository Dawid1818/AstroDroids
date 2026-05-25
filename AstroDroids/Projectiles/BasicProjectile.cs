using AstroDroids.Entities;
using AstroDroids.Entities.Effects;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstroDroids.Projectiles
{
    public class BasicProjectile : Projectile
    {
        Texture2D texture;
        float speed = 25f;

        bool fade = false;
        float fadePercentage = 0;
        public BasicProjectile(Vector2 position) : base(new Transform(position.X, position.Y))
        {
            texture = TextureManager.GetProjectile("BasicWeapon/04");
            AddCapsuleCollider(new Vector2(0, -20f), new Vector2(0f, 15), 10f);
        }

        public override void Update(GameTime gameTime)
        {
            if(fade)
            {
                if(fadePercentage >= texture.Height)
                {
                    Despawn();
                }
                else
                {
                    fadePercentage += speed;
                }
                return;
            }

            Transform.Translate(new Vector2(0f, -speed));

            if (Transform.LocalPosition.Y + texture.Height < 0)
            {
                Despawn();
            }
            else
            {
                foreach (var enemy in Scene.World.Enemies)
                {
                    if (enemy.Intersects(this))
                    {
                        SimpleHitEffect hitEffect = new SimpleHitEffect(new Transform(Transform.Position.X, Transform.Position.Y));
                        Scene.World.AddEffect(hitEffect);

                        fade = true;
                        enemy.Damage(1, true);
                        break;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, Transform.Position, new Rectangle(0, (int)fadePercentage, texture.Width, texture.Height), Color.White, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, 0f);
        }
    }
}
