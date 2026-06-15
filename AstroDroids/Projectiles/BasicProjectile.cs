using AstroDroids.Entities;
using AstroDroids.Entities.Effects;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace AstroDroids.Projectiles
{
    public enum BasicProjectileType
    {
        WeakCyan,
        WeakOrange,
        WeakRed
    }
    public class BasicProjectile : Projectile
    {
        Texture2D texture;
        float speed = 25f;

        bool fade = false;
        float fadePercentage = 0;

        BasicProjectileType type;
        float angle;

        public BasicProjectile(Vector2 position, BasicProjectileType type, float angle) : base(position)
        {
            switch (type)
            {
                default:
                case BasicProjectileType.WeakCyan:
                    texture = TextureManager.GetProjectile("BasicWeapon/04");
                    break;
                case BasicProjectileType.WeakOrange:
                    texture = TextureManager.GetProjectile("BasicWeapon/06");
                    break;
                case BasicProjectileType.WeakRed:
                    texture = TextureManager.GetProjectile("BasicWeapon/05");
                    break;
            }
            AddCapsuleCollider(GameHelper.OrbitPos(Vector2.Zero, angle, 20f), GameHelper.OrbitPos(Vector2.Zero, angle, -10f), 10f);
            this.angle = angle;
            this.type = type;
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

            Transform.Translate(new Vector2(MathF.Cos(angle) * speed, MathF.Sin(angle) * speed));

            if (!Intersects(Scene.World.Bounds))
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
            Screen.spriteBatch.Draw(texture, Transform.Position, new Rectangle(0, (int)0, texture.Width, texture.Height), Color.White, angle, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, 0f);
        }
    }
}
