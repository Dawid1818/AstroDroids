using AstroDroids.Coroutines;
using AstroDroids.Entities.Friendly;
using AstroDroids.Entities.Hostile.Bosses;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Paths;
using AstroDroids.Projectiles.Hostile;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections;

namespace AstroDroids.Entities.Hostile
{
    public class MineDeployer : Enemy
    {
        public float t = 0f;

        Texture2D texture;

        float angle = MathHelper.ToRadians(-90);

        public Vector2 Left1Cannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f) + 20, -Height + 20), Vector2.Zero, angle); } }
        public Vector2 Right1Cannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f) + 20, Height - 20), Vector2.Zero, angle); } }
        public Vector2 MiddleCannon { get { return GameHelper.RotateAroundPoint(new Vector2(-(Width / 2f), 0), Vector2.Zero, angle); } }

        CoroutineInstance behavior;
        CoroutineInstance fireLoop;

        bool charging = false;

        Vector2 targetPos;

        ProximityMine projectile;

        public MineDeployer() : base(Vector2.Zero, 150)
        {
            texture = TextureManager.Get("Ships/MineDeployer/ship_014");
            Score = 100;

            AddCircleCollider(Vector2.Zero, 50f);
        }

        public override void Spawned()
        {
            targetPos = new Vector2(Random.NextSingle(60, Scene.World.Bounds.Width - 60), Random.NextSingle(92, 120));

            behavior = Scene.World.StartCoroutine(Behavior());
        }

        public override void Destroyed()
        {
            base.Destroyed();

            if(fireLoop != null)
            {
                Scene.World.StopCoroutine(fireLoop);
                fireLoop = null;
            }

            if (behavior != null)
            {
                Scene.World.StopCoroutine(behavior);
                behavior = null;
            }

            if(projectile != null)
            {
                float angle = MathHelper.ToRadians(90);
                Player player = Scene.World.GetRandomPlayer();
                if (player != null)
                    angle = GameHelper.AngleBetween(Transform.Position, player.Transform.Position);

                projectile = null;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (PathManager != null && PathManager.Active)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
            }
            else
            {
                Vector2 direction = targetPos - Transform.Position;
                float distance = direction.Length();

                const float maxSpeed = 100f;
                const float slowRadius = 60f;

                float dt = gameTime.GetElapsedSeconds();

                if (distance < 1f)
                {
                    Transform.Position = targetPos;

                    targetPos = new Vector2(Random.NextSingle(60, Scene.World.Bounds.Width - 60), Random.NextSingle(92, 120));
                }
                else
                {
                    direction /= distance;

                    float speed = maxSpeed;

                    if (distance < slowRadius)
                        speed *= distance / slowRadius;

                    Transform.Position += direction * speed * dt;

                    float t = MathF.Abs(direction.X) * (speed / maxSpeed);

                    float neutralAngle = -MathF.PI / 2f;
                    float maxBank = 0.18f;

                    float targetAngle = neutralAngle + MathF.Sign(direction.X) * maxBank * t;

                    angle = MathHelper.Lerp(angle, targetAngle, 8f * dt);
                }
            }

            if (projectile != null && charging)
            {
                projectile.Transform.Position = MiddleCannon + Transform.Position + new Vector2(0, 10);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, new Vector2(Transform.Position.X, Transform.Position.Y), null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), 0.8f, SpriteEffects.None, 0f);

            if (AstroDroidsGame.Debug)
            {
                Screen.spriteBatch.DrawCircle(Left1Cannon + Transform.Position, 12, 12, Color.Green);
                Screen.spriteBatch.DrawCircle(Right1Cannon + Transform.Position, 12, 12, Color.Green);
                Screen.spriteBatch.DrawCircle(MiddleCannon + Transform.Position, 12, 12, Color.Green);
            }
        }

        IEnumerator Behavior()
        {
            while(true)
            {
                int choice = Random.Next(2);
                switch (choice)
                {
                    case 0:
                    default:
                        for (int i = 0; i < 3; i++)
                        {
                            yield return Fire();
                        }
                        break;
                    case 1:
                        for (int i = 0; i < 3; i++)
                        {
                            yield return FireTriple();
                        }
                        break;
                }

                yield return new WaitForSeconds(2f);
            }
        }

        IEnumerator Fire()
        {
            charging = true;
            projectile = new ProximityMine() { Transform = new Transform(MiddleCannon + Transform.Position) };
            projectile.Move = false;
            projectile.RevealSlowly();
            Scene.World.AddEnemy(projectile, false, spawnData: new ProximityMineSpawnData() { Type = getRandomMineType(), Angle = 90, Speed = 3f }, addAtBeginning: true);
            yield return new WaitUntil(() => projectile.RevealProgress <= 0 || projectile.destroyed);
            projectile.Move = true;

            projectile = null;
            charging = false;
        }

        IEnumerator FireTriple()
        {
            charging = true;
            projectile = new ProximityMine() { Transform = new Transform(MiddleCannon + Transform.Position) };
            projectile.Move = false;
            projectile.RevealSlowly();
            Scene.World.AddEnemy(projectile, followsCamera: false, spawnData: new ProximityMineSpawnData() { Type = getRandomMineType(), Angle = 90, Speed = 3f }, addAtBeginning: true);
            yield return new WaitUntil(() => projectile.RevealProgress <= 0 || projectile.destroyed);
            projectile.Move = true;

            for (int i = 0; i < 2; i++)
            {
                var otherproj = new ProximityMine() { Transform = new Transform(MiddleCannon + Transform.Position) };
                otherproj.Move = true;
                Scene.World.AddEnemy(otherproj, followsCamera: false, spawnData: new ProximityMineSpawnData() { Type = getRandomMineType(), Angle = i == 0 ? 45 : 135, Speed = 3f }, addAtBeginning: true);
            }

            projectile = null;
            charging = false;
        }

        ProximityMineType getRandomMineType()
        {
            int choice = Random.Next(2);
            return (ProximityMineType)choice;
        }
    }
}
