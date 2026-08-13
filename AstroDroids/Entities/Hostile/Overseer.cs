using AstroDroids.Coroutines;
using AstroDroids.Entities.Friendly;
using AstroDroids.Entities.Warnings;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AstroDroids.Entities.Hostile
{
    public enum OverseerFlightMode
    {
        Freeroam,
        Locked
    }

    public class Overseer : Enemy
    {
        Texture2D texture;

        float angle;

        OverseerFlightMode Flight = OverseerFlightMode.Freeroam;

        CoroutineInstance attackLoop;

        Vector2 targetPos;

        public Vector2 Left1Cannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f) + 15, -Height + 24), Vector2.Zero, angle); } }
        public Vector2 Left2Cannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f), -Height + 5), Vector2.Zero, angle); } }

        public Vector2 Right1Cannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f) + 15, Height - 24), Vector2.Zero, angle); } }
        public Vector2 Right2Cannon { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f), Height - 5), Vector2.Zero, angle); } }

        public bool moving { get; set; } = true;
        public bool firing { get; set; } = false;
        List<ReflectBeamWarning> warnings = new List<ReflectBeamWarning>();
        List<ReflectBeam> beams = new List<ReflectBeam>();

        public Overseer() : base(Vector2.Zero, 200)
        {
            AddCircleCollider(Vector2.Zero, 45);
            texture = TextureManager.Get("Ships/Overseer/ship_017");
        }

        public override void Destroyed()
        {
            base.Destroyed();

            if (attackLoop != null)
            {
                Scene.World.StopCoroutine(attackLoop);
                attackLoop = null;
            }

            foreach (var item in warnings.ToList())
            {
                RemoveWarning(item);
            }

            foreach (var item in beams.ToList())
            {
                RemoveBeam(item);
            }
        }

        void AddWarning(ReflectBeamWarning warning)
        {
            warnings.Add(warning);
            Scene.World.AddWarning(warning, true);
        }

        void RemoveWarning(ReflectBeamWarning warning)
        {
            warnings.Remove(warning);
            Scene.World.RemoveWarning(warning);
        }

        void AddBeam(ReflectBeam beam)
        {
            beams.Add(beam);
            Scene.World.AddProjectile(beam, true);
        }

        void RemoveBeam(ReflectBeam beam)
        {
            beam.Locked = false;
            beams.Remove(beam);
        }

        public override void Spawned()
        {
            angle = MathHelper.ToRadians(90);

            targetPos = new Vector2(Random.NextSingle(60, Scene.World.Bounds.Width - 60), 92);

            attackLoop = Scene.World.StartCoroutine(Behavior());
        }

        private bool IsAnyOtherOverseerFiring()
        {
            var enemies = Scene.World.Enemies;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] is Overseer siege && siege != this && siege.firing)
                {
                    return true;
                }
            }
            return false;
        }

        IEnumerator Behavior()
        {
            while (true)
            {
                yield return new WaitUntil(() => !IsAnyOtherOverseerFiring());
                firing = true;
                yield return new WaitForSeconds(seconds: 1f);
                int choice = Random.Next(2);
                switch (choice)
                {
                    case 0:
                    default:
                        yield return Fire();
                        break;
                    case 1:
                        yield return RapidLasers();
                        break;
                }

                firing = false;

                yield return new WaitForSeconds(seconds: 2f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            float targetAngle = 0f;
            float dt = gameTime.GetElapsedSeconds();

            if (!moving)
            {
                Player player = Scene.World.GetRandomPlayer();
                if (player != null)
                {
                    targetAngle = GameHelper.AngleBetween(Transform.Position, player.Transform.Position);

                    //angle = MathHelper.Lerp(angle, targetAngle, 8f * dt);
                }

                return;
            }

            Vector2 direction = targetPos - Transform.Position;
            float distance = direction.Length();

            const float maxSpeed = 100f;
            const float slowRadius = 60f;

            if (distance < 1f)
            {
                Transform.Position = targetPos;

                if (Flight == OverseerFlightMode.Freeroam)
                    targetPos = new Vector2(Random.NextSingle(60, Scene.World.Bounds.Width - 60), 92);
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

                targetAngle = neutralAngle + MathF.Sign(direction.X) * maxBank * t + MathHelper.ToRadians(180);

                angle = MathHelper.Lerp(angle, targetAngle, 8f * dt);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, Transform.Position, null, Color.White, angle, new Vector2(texture.Width / 2f, texture.Height / 2f), 0.7f, SpriteEffects.None, 0f);

            if (AstroDroidsGame.Debug)
            {
                Screen.spriteBatch.DrawCircle(Left1Cannon + Transform.Position, 12, 12, Color.Green);
                Screen.spriteBatch.DrawCircle(Left2Cannon + Transform.Position, 12, 12, Color.Green);

                Screen.spriteBatch.DrawCircle(Right1Cannon + Transform.Position, 12, 12, Color.Green);
                Screen.spriteBatch.DrawCircle(Right2Cannon + Transform.Position, 12, 12, Color.Green);
            }
        }

        IEnumerator Fire()
        {
            moving = false;

            ReflectBeamWarning warning = new ReflectBeamWarning(new Transform(Left1Cannon.X + Transform.Position.X, Left1Cannon.Y + Transform.Position.Y), MathHelper.ToRadians(15), 10000);
            ReflectBeamWarning warning2 = new ReflectBeamWarning(new Transform(Right1Cannon.X + Transform.Position.X, Right1Cannon.Y + Transform.Position.Y), MathHelper.ToRadians(165), 10000);
            AddWarning(warning);
            AddWarning(warning2);

            for (int i = 0; i < 100; i++)
            {
                warning.Transform.Position = Left1Cannon + Transform.Position;
                warning2.Transform.Position = Right1Cannon + Transform.Position;
                yield return null;
            }

            RemoveWarning(warning);
            RemoveWarning(warning2);

            ReflectBeam beam1 = new ReflectBeam(Left1Cannon + Transform.Position, MathHelper.ToRadians(15), 10000);
            ReflectBeam beam2 = new ReflectBeam(Right1Cannon + Transform.Position, MathHelper.ToRadians(165), 10000);

            beam1.Locked = true;
            beam2.Locked = true;

            AddBeam(beam1);
            AddBeam(beam2);

            for (int i = 0; i < 500; i++)
            {
                if(i % 50 == 0)
                {
                    ChallengerHomingMissile missile = new ChallengerHomingMissile(Transform.Position, Scene.World.GetRandomPlayer(), MathHelper.ToRadians(90));
                    missile.SetHomingMaxTime(7f);
                    Scene.World.AddProjectile(missile, true);
                }

                beam1.Transform.Position = Left1Cannon + Transform.Position;
                beam2.Transform.Position = Right1Cannon + Transform.Position;
                yield return null;
            }

            RemoveBeam(beam1);
            RemoveBeam(beam2);

            yield return null;
            moving = true;
        }

        IEnumerator RapidLasers()
        {
            moving = false;

            for (int i = 0; i < 5; i++)
            {
                int random = Random.Next(5, 25);

                ReflectBeamWarning warning = new ReflectBeamWarning(new Transform(Left1Cannon.X + Transform.Position.X, Left1Cannon.Y + Transform.Position.Y), MathHelper.ToRadians(random), 10000);
                ReflectBeamWarning warning2 = new ReflectBeamWarning(new Transform(Right1Cannon.X + Transform.Position.X, Right1Cannon.Y + Transform.Position.Y), MathHelper.ToRadians(150 + random), 10000);
                AddWarning(warning);
                AddWarning(warning2);

                for (int j = 0; j < 70; j++)
                {
                    warning.Transform.Position = Left1Cannon + Transform.Position;
                    warning2.Transform.Position = Right1Cannon + Transform.Position;
                    yield return null;
                }

                RemoveWarning(warning);
                RemoveWarning(warning2);

                ReflectBeam beam1 = new ReflectBeam(Left1Cannon + Transform.Position, MathHelper.ToRadians(random), 10000);
                ReflectBeam beam2 = new ReflectBeam(Right1Cannon + Transform.Position, MathHelper.ToRadians(150 + random), 10000);

                beam1.Locked = true;
                beam2.Locked = true;

                AddBeam(beam1);
                AddBeam(beam2);

                for (int j = 0; j < 100; j++)
                {
                    beam1.Transform.Position = Left1Cannon + Transform.Position;
                    beam2.Transform.Position = Right1Cannon + Transform.Position;
                    yield return null;
                }

                RemoveBeam(beam1);
                RemoveBeam(beam2);
            }
            yield return null;
            moving = true;
        }
    }
}
