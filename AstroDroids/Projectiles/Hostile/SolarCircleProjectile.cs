using Apos.Shapes;
using AstroDroids.Collisions;
using AstroDroids.Entities;
using AstroDroids.Entities.Effects;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Linq;

namespace AstroDroids.Projectiles.Hostile
{
    public class SolarCircleProjectile : Projectile
    {
        float t = 0f;
        public float Angle { get; set; }
        Vector2 movementDirection { get { return GameHelper.DirFromAngle(Angle); } }
        public float Speed { get; set; } = 10f;
        public float Size { get; set; } = 0f;
        public float MaxSize { get; set; } = 16f;
        Vector2 actualPosition;
        CircleCollider col;
        float deltaTime = 0f;
        public bool FollowDestination { get; set; } = false;
        public Vector2 Destination { get; set; }

        float dashOffset = 0;
        float blastRadius = 64;

        public SolarCircleProjectile(Vector2 position, float angle, float speed, float size, float maxSize, float blastRadius) : base(position)
        {
            Friendly = false;

            Angle = angle;
            Speed = speed;
            Size = size;
            MaxSize = maxSize;
            this.blastRadius = blastRadius;

            col = AddCircleCollider(Vector2.Zero, size);
        }

        public override void Update(GameTime gameTime)
        {
            deltaTime = gameTime.GetElapsedSeconds();

            dashOffset += deltaTime;

            if (!Intersects(Scene.World.Bounds))
            {
                if (t >= 10f)
                    Despawn();
            }
            else if (Vector2.Distance(Transform.Position, Destination) <= 5f)
            {
                Explode();
                Despawn();
            }

            Vector2 direction = GameHelper.DirectionFromTo(Destination, Transform.Position);

            if (FollowDestination)
                actualPosition = (direction * Speed);
            else
                actualPosition = (movementDirection * Speed);
            Transform.LocalPosition += actualPosition;

            foreach (var item in Scene.World.Neutrals)
            {
                if (item.Intersects(this))
                {
                    item.Damage(1, false);
                    item.Push(GameHelper.DirectionFromTo(item.Transform.Position, Transform.Position));
                    Explode();
                    Despawn();

                    return;
                }
            }

            foreach (var item in Scene.World.GetPlayers())
            {
                if (item.Intersects(this))
                {
                    item.Damage(1, false);
                    Explode();
                    Despawn();

                    return;
                }
            }

            t += gameTime.GetElapsedSeconds();

            Size += gameTime.GetElapsedSeconds() * 30f;

            col.Radius = Size;

            if (Size >= MaxSize)
                Size = MaxSize;
        }

        void Explode()
        {
            Scene.World.AddEffect(new StandardExplosion(new Transform(Transform.Position.X, Transform.Position.Y), (Size / 35f) * 2.5f));

            CircleF blast = new CircleF(Transform.Position, Size + blastRadius);

            foreach (var neutral in Scene.World.Neutrals.ToList())
            {
                if (neutral.Intersects(blast))
                {
                    neutral.Damage(100, false);
                    neutral.Push(GameHelper.DirectionFromTo(neutral.Transform.Position, Transform.Position));
                }
            }

            foreach (var player in Scene.World.GetPlayers())
            {
                if (player.Intersects(blast))
                {
                    player.Damage(100, false);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //warning indicator
            if (Speed > 0 && FollowDestination)
            {
                Color clr = new Color(Color.Red.R, Color.Red.G, Color.Red.B, (byte)127);
                float radius = Size + blastRadius;
                Screen.shapeBatch.FillCircle(Destination, radius, new Gradient(Destination, clr, Destination + new Vector2(radius), new Color(Color.Red.R, Color.Red.G, Color.Red.B, (byte)0), Gradient.Shape.Radial));
                Screen.shapeBatch.BorderCircle(Destination, radius, new Gradient(Destination, clr, Destination + new Vector2(radius), new Color(Color.Red.R, Color.Red.G, Color.Red.B, (byte)0), Gradient.Shape.Radial), dash: new DashStyle(24f, 16f, dashOffset));
            }
            Screen.shapeBatch.BorderCircleBlurred(Transform.Position, Size * 1.2f, Color.Yellow, 50, Size + blastRadius);
            Screen.shapeBatch.DrawCircle(Transform.Position, Size - 3, new Apos.Shapes.Gradient(Transform.Position, Color.LightYellow, Transform.Position + new Vector2(Size, 0), Color.Yellow, Apos.Shapes.Gradient.Shape.Radial), Color.Yellow, 1);
        }
    }
}
