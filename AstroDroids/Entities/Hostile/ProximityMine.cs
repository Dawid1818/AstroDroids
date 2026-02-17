using AstroDroids.Entities.Neutral;
using AstroDroids.Graphics;
using AstroDroids.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace AstroDroids.Entities.Hostile
{
    public class ProximityMine : Enemy
    {
        enum ProximityMineState
        {
            Idle,
            Detonating
        }

        ProximityMineState state = ProximityMineState.Idle;
        public float t = 0f;

        public ProximityMine() : base(new Transform(0, 0), 1, 32f, 32f)
        {
            AddCircleCollider(Vector2.Zero, 32f);
        }

        public ProximityMine(Vector2 position, EntityCell cell) : base(new Transform(position.X, position.Y), 1, 32f, 32f)
        {
            AddCircleCollider(Vector2.Zero, 32f);
        }

        public override void Destroyed()
        {
            if (!destroyed)
            {
                for (int i = 0; i < 360; i += 45)
                {
                    SpawnProjectile(i);
                }
            }

            base.Destroyed();
        }

        void SpawnProjectile(float angle)
        {
            Scene.World.AddProjectile(new CircleProjectile(new Transform(Transform.LocalPosition.X, Transform.LocalPosition.Y), 16, 16, MathHelper.ToRadians(angle)), true);
        }

        public override void Update(GameTime gameTime)
        {
            if (PathManager != null)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
            }

            switch (state)
            {
                case ProximityMineState.Idle:

                    foreach (var item in Scene.World.GetPlayers())
                    {
                        if (Vector2.Distance(Transform.Position, item.Transform.Position) <= 128f)
                        {
                            state = ProximityMineState.Detonating;
                        }
                    }

                    break;
                case ProximityMineState.Detonating:

                    t += (float)gameTime.ElapsedGameTime.TotalSeconds * 2f;

                    if(t >= 1f)
                    {
                        Damage(100, false);
                    }

                    break;
                default:
                    break;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.DrawCircle(Transform.Position, 32, 16, Color.Lerp(Color.OrangeRed, Color.White, t), 32);
        }
    }
}
