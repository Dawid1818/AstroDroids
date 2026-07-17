using AstroDroids.Drawables;
using AstroDroids.Entities;
using AstroDroids.Entities.Effects;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace AstroDroids.Projectiles
{
    public class LaserCannonProjectile : Projectile
    {
        float angle;

        float t = 0f;
        int state = 0;

        bool damaged = false;

        float distance = 1000;

        int damage = 5;

        float charge = 0f;

        AnimatedSprite sprite;

        float beamScale = 1f;
        int powerLevel = 1;

        public LaserCannonProjectile(Vector2 position, float angle, float charge, int powerLevel) : base(position)
        {
            Friendly = true;

            this.charge = charge;
            this.powerLevel = int.Clamp(powerLevel, 1, 5);

            switch (this.powerLevel)
            {
                default:
                case 1:
                    damage = (int)MathF.Ceiling(2 * charge);
                    beamScale = charge/3f;
                    break;
                case 2:
                    damage = (int)MathF.Ceiling(3 * charge);
                    beamScale = charge/2.5f;
                    break;
                case 3:
                    damage = (int)MathF.Ceiling(4 * charge);
                    beamScale = charge/2f;
                    break;
                case 4:
                    damage = (int)MathF.Ceiling(5 * charge);
                    beamScale = charge/1.5f;
                    break;
                case 5:
                    damage = (int)MathF.Ceiling(6 * charge);
                    beamScale = charge;
                    break;
            }

            sprite = new AnimatedSprite(TextureManager.Get("Projectiles/LaserCannon/Laser.1"), 9, 105, 256, 0, 32, 50f);

            AddCapsuleCollider(Vector2.Zero, GameHelper.OrbitPos(Vector2.Zero, angle, 1000), (16 * beamScale) / 2f);

            this.angle = angle;
        }

        public override void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);

            if (t >= 1f && state == 0)
            {
                state = 1;
                t = 1f;
            }
            else if (t <= 0 && state == 1)
                Despawn();

            if (!damaged)
            {
                int remainingDamage = (int)MathF.Ceiling(damage * charge);
                float laserLength = 1000;

                var hits = new List<AliveEntity>();

                foreach (var enemy in Scene.World.Enemies)
                {
                    if (enemy.Intersects(this))
                        hits.Add(enemy);
                }

                hits.Sort((a, b) =>
                {
                    float da = Vector2.DistanceSquared(Transform.Position, a.Transform.Position);

                    float db = Vector2.DistanceSquared(Transform.Position, b.Transform.Position);

                    return da.CompareTo(db);
                });

                foreach (var enemy in hits)
                {
                    if (remainingDamage <= 0)
                        break;

                    int hp = enemy.GetHealth();

                    int damageToDeal = Math.Min(remainingDamage, hp);

                    enemy.Damage(damageToDeal, false);

                    laserLength = Vector2.Distance(Transform.Position, enemy.Transform.Position);

                    Vector2 hitPoint = GameHelper.OrbitPos(Transform.Position, angle, laserLength);
                    SimpleHitEffect hitEffect = new SimpleHitEffect(new Transform(hitPoint.X, hitPoint.Y), Color.Orange);
                    Scene.World.AddEffect(hitEffect);

                    remainingDamage -= damageToDeal;
                }

                if (remainingDamage == 0)
                    distance = laserLength;
                damaged = true;
            }

            if (state == 0)
                t += (float)gameTime.ElapsedGameTime.TotalSeconds * 5f;
            else
                t -= (float)gameTime.ElapsedGameTime.TotalSeconds * 5f;
        }

        public override void Draw(GameTime gameTime)
        {
            Color color = new Color(255, 255, 255, state == 0 ? 255 : (int)(255f * t));

            float segmentLength = 256f;

            float stepLength = segmentLength;

            Vector2 direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));

            float remaining = distance;
            float offset = 0f;

            while (remaining >= segmentLength)
            {
                Vector2 segmentPosition = Transform.Position + direction * offset;

                sprite.Draw(segmentPosition, angle + MathHelper.ToRadians(90f), new Vector2(beamScale / 2f, 1f), new Vector2(105 / 2f, 256), color);

                remaining -= stepLength;
                offset += stepLength;
            }

            if (remaining > 0f)
            {
                Vector2 segmentPosition = Transform.Position + direction * offset;

                float partialLength = remaining + 0.5f;
                if (partialLength > segmentLength) partialLength = segmentLength;

                sprite.DrawPartial(segmentPosition, angle + MathHelper.ToRadians(90f), new Vector2(beamScale / 2f, 1f), new Vector2(105 / 2f, partialLength), color, partialLength);
            }
        }
    }
}
