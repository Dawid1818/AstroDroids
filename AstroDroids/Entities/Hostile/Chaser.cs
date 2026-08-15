using AstroDroids.Drawables;
using AstroDroids.Entities.Friendly;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Paths;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstroDroids.Entities.Hostile
{
    public class Chaser : Enemy
    {
        public float t = 0f;

        Texture2D texture;
        AnimatedSprite sprite;

        float angle = 3.14f;

        Vector2 velocity;

        public Chaser() : base(Vector2.Zero, 3)
        {
            CanBeShielded = true;
            texture = TextureManager.Get("Ships/Chaser/tinyShip17");
            Score = 100;
            sprite = new AnimatedSprite(texture, 5, 34, 25, 1, 6, 10f);

            AddCircleCollider(Vector2.Zero, 16f);
        }

        public override void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);

            if (PathManager != null && PathManager.Active)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
                angle = GameHelper.AngleFromDir(PathManager.Direction) + 1.571f;
            }
            else
            {
                Player player = Scene.World.GetRandomPlayer();

                if (player != null)
                {
                    var dir = GameHelper.DirectionFromTo(player.Transform.Position, Transform.Position);
                    velocity += dir;

                    Vector2 separationForce = Vector2.Zero;
                    float separationRadius = 40f;

                    foreach (var otherEnemy in Scene.World.Enemies)
                    {
                        if (otherEnemy != this)
                        {
                            float dist = Vector2.Distance(Transform.Position, otherEnemy.Transform.Position);

                            if (dist > 0 && dist < separationRadius)
                            {
                                Vector2 pushAway = Transform.Position - otherEnemy.Transform.Position;

                                separationForce += Vector2.Normalize(pushAway) * (separationRadius - dist);
                            }
                        }
                    }

                    velocity += separationForce * 0.2f;
                }
                else
                {
                    if (!Intersects(Scene.World.Bounds))
                    {
                        Despawn();
                    }
                }

                if (velocity.LengthSquared() > 25f)
                    velocity = Vector2.Normalize(velocity) * 5f;

                Transform.Position += velocity;

                if (velocity != Vector2.Zero)
                    angle = GameHelper.AngleFromDir(Vector2.Normalize(velocity)) + MathHelper.ToRadians(90);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            sprite.Draw(new Vector2(Transform.Position.X, Transform.Position.Y), angle, 1f);
        }
    }
}
