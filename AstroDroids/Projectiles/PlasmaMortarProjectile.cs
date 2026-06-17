using AstroDroids.Entities;
using AstroDroids.Entities.Effects;
using AstroDroids.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace AstroDroids.Projectiles
{
    public class PlasmaMortarProjectile : Projectile
    {
        float speed = 500f;

        float angle;

        int clusterAmount = 0;
        bool isCluster = false;
        int powerLevel;

        int damage = 1;
        float size;

        public PlasmaMortarProjectile(Vector2 position, float angle, bool isCluster, int powerLevel, float launchForce) : base(position)
        {
            speed = speed * launchForce;
            switch (powerLevel)
            {
                default:
                case 1:
                    damage = 2;
                    clusterAmount = 0;
                    size = 14;
                    break;
                case 2:
                    damage = 3;
                    clusterAmount = 0;
                    size = 16;
                    break;
                case 3:
                    damage = 4;
                    clusterAmount = 2;
                    size = 18;
                    break;
                case 4:
                    damage = 5;
                    clusterAmount = 2;
                    size = 20;
                    break;
                case 5:
                    damage = 6;
                    clusterAmount = 3;
                    size = 22;
                    break;
            }

            if (isCluster)
            {
                speed = speed / 3f;
                size = size / 1.5f;
                damage = damage / 2;
            }

            AddCircleCollider(Vector2.Zero, size);
            this.angle = angle;
            this.isCluster = isCluster;
            this.powerLevel = int.Clamp(powerLevel, 1, 5);
        }

        public override void Update(GameTime gameTime)
        {
            var velocity = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * speed;
            Transform.Translate(velocity * (float)gameTime.ElapsedGameTime.TotalSeconds);

            speed *= 1f - (2.5f * (float)gameTime.ElapsedGameTime.TotalSeconds);

            if (speed <= 5f)
            {
                Explode();
                Despawn();
            }
            else if (!Intersects(Scene.World.Bounds))
            {
                Despawn();
            }
            else
            {
                foreach (var enemy in Scene.World.Enemies)
                {
                    if (enemy.Intersects(this))
                    {
                        Explode();
                        Despawn();
                        break;
                    }
                }
            }
        }

        void Explode()
        {
            Scene.World.AddEffect(new StandardExplosion(new Transform(Transform.Position.X, Transform.Position.Y), (size / 35f) * 2.5f));

            CircleF blast = new CircleF(Transform.Position, size * 2.5f);

            foreach (var enemy in Scene.World.Enemies)
            {
                if (enemy.Intersects(blast))
                {
                    enemy.Damage(damage, false);
                }
            }

            if (!isCluster)
            {
                for (int i = 0; i < clusterAmount; i++)
                {
                    float angle = AstroDroidsGame.rnd.Next(0, 360);
                    PlasmaMortarProjectile proj = new PlasmaMortarProjectile(Transform.Position, MathHelper.ToRadians(angle), true, powerLevel, 1f);
                    Scene.World.AddProjectile(proj, true);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.DrawCircle(Transform.Position, size, (int)size / 2, Color.Blue, size);
        }
    }
}
