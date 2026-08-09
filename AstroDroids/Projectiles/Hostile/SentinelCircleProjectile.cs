using AstroDroids.Collisions;
using AstroDroids.Entities;
using AstroDroids.Entities.Friendly;
using AstroDroids.Entities.Hostile.Bosses;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroDroids.Projectiles.Hostile
{
    public class SentinelCircleOrbit
    {
        public CircleCollider Collider { get; set; }
    }
    public class SentinelCircleProjectile : Projectile
    {
        float t = 0f;
        public float Angle { get; set; }
        Vector2 movementDirection { get { return GameHelper.DirFromAngle(Angle); } }
        public float Speed { get; set; } = 10f;
        public float Size { get; set; } = 0f;
        public float MaxSize { get; set; } = 16f;

        public bool CanDetonate { get; set; } = false;

        public bool FastGrow { get; set; } = false;

        Vector2 actualPosition;
        CircleCollider col;

        List<CircleCollider> orbits = new List<CircleCollider>();

        float orbitRotation = MathHelper.ToRadians(AstroDroidsGame.rnd.Next(0, 360));
        float deltaTime = 0f;

        public SentinelCircleProjectile(Vector2 position, float angle, float speed, float size, float maxSize) : base(position)
        {
            Friendly = false;

            Angle = angle;
            Speed = speed;
            Size = size;
            MaxSize = maxSize;

            col = AddCircleCollider(Vector2.Zero, size);

            //actualPosition = position;
        }

        public override void Update(GameTime gameTime)
        {
            deltaTime = gameTime.GetElapsedSeconds();

            if (!Intersects(Scene.World.Bounds))
            {
                if (t >= 10f)
                    Despawn();

                    t += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            actualPosition = (movementDirection * Speed);
            Transform.LocalPosition += actualPosition;

            foreach (var item in Scene.World.Neutrals)
            {
                if (item.Intersects(this))
                {
                    item.Damage(1, false);
                    item.Push(GameHelper.DirectionFromTo(item.Transform.Position, Transform.Position));
                    Despawn();

                    return;
                }
            }

            foreach (var item in Scene.World.GetPlayers())
            {
                if (item.Intersects(this))
                {
                    item.Damage(1, false);
                    Despawn();

                    return;
                }
            }

            t += gameTime.GetElapsedSeconds();

            if(!FastGrow)
                Size += gameTime.GetElapsedSeconds() * 30f;
            else
                Size += gameTime.GetElapsedSeconds() * 50f;

            col.Radius = Size;

            if (Size >= MaxSize)
                Size = MaxSize;

            if (orbits.Count > 0)
            {
                float dist = MathHelper.ToRadians(360 / orbits.Count);

                float ellipseRotation = orbitRotation;

                orbitRotation += gameTime.GetElapsedSeconds() * 1f;

                if (orbitRotation > MathHelper.TwoPi)
                {
                    orbitRotation -= MathHelper.TwoPi;
                }

                for (int i = 0; i < orbits.Count; i++)
                {
                    CircleCollider orbit = orbits[i];
                    float currentAngle = (dist * i);

                    Vector2 desiredPos = GameHelper.OrbitEllipsePos(Vector2.Zero, currentAngle, 72, 72, ellipseRotation);

                    MoveTowards(orbit, desiredPos, false, 1f);
                }
            }
        }

        void MoveTowards(CircleCollider collider, Vector2 position, bool constantSpeed, float speedMultiplier, float expectedDistance = 60f)
        {
            const float speed = 200f;

            if (constantSpeed)
            {
                Vector2 current = collider.LocalOffset;
                Vector2 direction = position - current;

                float distance = direction.Length();

                if (distance <= speed * speedMultiplier * deltaTime)
                {
                    collider.LocalOffset = position;
                }
                else
                {
                    collider.LocalOffset = current + direction / distance * speed * speedMultiplier * deltaTime; ;
                }
            }
            else
            {
                collider.LocalOffset = Vector2.Lerp(collider.LocalOffset, position, 1.5f * speedMultiplier * deltaTime);
            }
        }

        public void AddOrbit()
        {
            for (int i = 0; i < 3; i++)
            {
                orbits.Add(AddCircleCollider(Vector2.Zero, 16f));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < orbits.Count; i++)
            {
                Screen.shapeBatch.DrawCircle(Transform.Position + orbits[i].LocalOffset, orbits[i].Radius - 3, Color.DarkRed, Color.Red, 1);
                Screen.shapeBatch.BorderCircleBlurred(Transform.Position + orbits[i].LocalOffset, orbits[i].Radius, Color.Red, 3, 3);
            }

            Screen.shapeBatch.DrawCircle(Transform.Position, Size - 3, Color.DarkRed, Color.Red, 1);
            Screen.shapeBatch.BorderCircleBlurred(Transform.Position, Size, Color.Red, 3, 3);
        }

        void FireArc(Vector2 position, float startAngle, float endAngle, int bulletCount, float bulletSpeed)
        {
            if (bulletCount < 2) return;

            float angleStep = (endAngle - startAngle) / (bulletCount - 1);

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = MathHelper.ToRadians(startAngle + i * angleStep);

                Shoot(angle, bulletSpeed);
            }
        }

        void Shoot(float angle, float speed = 5f, float phaseSpeed = 0f, float phaseMax = 0f)
        {
            var projectile = new SentinelCircleProjectile(GameHelper.OrbitPos(Transform.Position, angle, 20), angle, speed, 0f, 16f);
            projectile.FastGrow = true;
            Scene.World.AddProjectile(projectile, true);
        }

        internal void Detonate()
        { 
            if(!CanDetonate)
                return;

            Despawn();

            FireArc(Transform.Position, 0, 360, 10, 3);
        }
    }
}
