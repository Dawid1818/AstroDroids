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
    public enum PulseCannonProjectileType
    {
        WeakCyan,
        WeakOrange,
        WeakRed
    }
    public class PulseCannonProjectile : Projectile
    {
        Texture2D texture;
        float speed = 25f;

        bool fade = false;
        float fadePercentage = 0;

        PulseCannonProjectileType type;
        float angle;

        public PulseCannonProjectile(Vector2 position, PulseCannonProjectileType type, float angle) : base(position)
        {
            Friendly = true;

            switch (type)
            {
                default:
                case PulseCannonProjectileType.WeakCyan:
                    texture = TextureManager.GetProjectile("BasicWeapon/04");
                    break;
                case PulseCannonProjectileType.WeakOrange:
                    texture = TextureManager.GetProjectile("BasicWeapon/06");
                    break;
                case PulseCannonProjectileType.WeakRed:
                    texture = TextureManager.GetProjectile("BasicWeapon/05");
                    break;
            }
            AddCapsuleCollider(GameHelper.OrbitPos(Vector2.Zero, angle, 20f), GameHelper.OrbitPos(Vector2.Zero, angle, -10f), 10f);
            this.angle = angle;
            this.type = type;
        }

        Color GetHitColor()
        {
            switch (type)
            {
                default:
                case PulseCannonProjectileType.WeakCyan:
                    return Color.Cyan;
                    break;
                case PulseCannonProjectileType.WeakOrange:
                    return Color.Orange;
                    break;
                case PulseCannonProjectileType.WeakRed:
                    return Color.Red;
                    break;
            }
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
                        SimpleHitEffect hitEffect = new SimpleHitEffect(new Transform(Transform.Position.X, Transform.Position.Y), GetHitColor());
                        Scene.World.AddEffect(hitEffect);

                        fade = true;
                        enemy.Damage(1, true);
                        break;
                    }
                }

                foreach (var projectile in Scene.World.Projectiles)
                {
                    if (!projectile.Friendly && projectile.BlocksPlayerProjectiles && projectile.Intersects(this))
                    {
                        SimpleHitEffect hitEffect = new SimpleHitEffect(new Transform(Transform.Position.X, Transform.Position.Y), GetHitColor());
                        Scene.World.AddEffect(hitEffect);

                        fade = true;
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
