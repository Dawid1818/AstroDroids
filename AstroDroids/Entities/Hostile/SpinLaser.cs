using AstroDroids.Entities.Friendly;
using AstroDroids.Entities.Warnings;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Paths;
using AstroDroids.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Data;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using System;
using System.Collections.Generic;

namespace AstroDroids.Entities.Hostile
{
    public class SpinLaser : Enemy
    {
        enum SpinLaserState
        {
            Idle,
            IdleMoved,
            Spinning,
            StoppingSpin,
            Moving
        }

        public float attackTimer = 0f;
        Vector2 destination;

        Texture2D texture;

        float angle = 0f;
        float waitTimer = 0f;
        float targetAngle = 0f;
        BeamWarning warning;

        SpinLaserState state = SpinLaserState.Idle;

        int maxMoveDistance = 300;
        ParticleEffect chargeEffect;

        PathManager TravelManager;
        BezierPath bezier;

        public SpinLaser() : base(new Transform(0, 0), 1, 32f, 32f)
        {
            TravelManager = new PathManager();
            texture = TextureManager.Get("Ships/SpinLaser/SpinLaser");

            chargeEffect = new ParticleEffect("ChargeBeam")
            {
                Position = Transform.Position,
                AutoTrigger = false,
                AutoTriggerFrequency = 0.1f
            };

            ParticleEmitter emitter = new ParticleEmitter(2000)
            {
                Name = "Charge Emitter",
                LifeSpan = 0.4f,
                TextureRegion = new Texture2DRegion(TextureManager.GetPixelTexture()),

                Profile = Profile.Ring(50, CircleRadiation.In),

                Parameters = new ParticleReleaseParameters
                {
                    Quantity = new ParticleInt32Parameter(10, 20),

                    Speed = new ParticleFloatParameter(100f, 150f),

                    Color = new ParticleColorParameter(new Vector3(0.0f, 1.0f, 0.6f)),
                    Scale = new ParticleVector2Parameter(new Vector2(4.0f, 4.0f))
                }
            };

            emitter.Modifiers.Add(new AgeModifier
            {
                Interpolators =
                {
                    new OpacityInterpolator
                    {
                        StartValue = 0.0f,
                        EndValue = 1f
                    }
                }
            });

            emitter.Modifiers.Add(new DragModifier
            {
                Density = 0.5f,
                DragCoefficient = 0.3f
            });

            chargeEffect.Emitters.Add(emitter);
        }

        public override void Spawned()
        {
            Scene.World.AddEffect(chargeEffect);
        }

        public override void Destroyed()
        {
            Scene.World.RemoveEffect(chargeEffect);
            if(warning != null)
                Scene.World.RemoveWarning(warning);

            base.Destroyed();
        }

        public override void Update(GameTime gameTime)
        {
            if (PathManager != null)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
            }

            if (state == SpinLaserState.Idle)
            {
                destination = Transform.LocalPosition + new Vector2(Scene.World.rnd.Next(-maxMoveDistance, maxMoveDistance), Scene.World.rnd.Next(-maxMoveDistance, maxMoveDistance));
                destination = ClampPosition(destination);

                bezier = GameHelper.CreateBezier(Transform.LocalPosition, destination);
                TravelManager.SetPath(bezier);

                state = SpinLaserState.Moving;
            }
            else if (state == SpinLaserState.Moving)
            {
                if (PathManager != null)
                {
                    state = SpinLaserState.IdleMoved;
                }
                else
                {
                    TravelManager.Update(gameTime);
                    Transform.LocalPosition = TravelManager.Position;

                    if(!TravelManager.Active)
                    {
                        state = SpinLaserState.IdleMoved;
                    }
                }
            }
            else if (state == SpinLaserState.IdleMoved)
            {
                waitTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (waitTimer >= 0.6f)
                {
                    Player target = Scene.World.GetRandomPlayer();
                    if (target != null)
                    {
                        Vector2 p1 = Transform.LocalPosition;
                        Vector2 p2 = target.LocalCenter;
                        targetAngle = (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);

                        warning = new BeamWarning(new Transform(Transform.LocalPosition.X, Transform.LocalPosition.Y), (float)targetAngle, 900);
                        Scene.World.AddWarning(warning, true);


                        state = SpinLaserState.Spinning;
                    }
                    else
                    {
                        state = SpinLaserState.Idle;
                    }

                    waitTimer = 0f;
                }
            }
            else if (state == SpinLaserState.Spinning)
            {
                chargeEffect.AutoTrigger = true;
                angle += 0.5f * waitTimer;
                waitTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (waitTimer >= 1f)
                {
                    Scene.World.AddProjectile(new SpinLaserBeam(new Transform(Transform.LocalPosition.X, Transform.LocalPosition.Y), 0, 0, (float)targetAngle));

                    if (warning != null)
                    {
                        Scene.World.RemoveWarning(warning);
                        warning = null;
                    }

                    waitTimer = 1f;
                    state = SpinLaserState.StoppingSpin;
                    chargeEffect.AutoTrigger = false;
                }
            }
            else if (state == SpinLaserState.StoppingSpin)
            {
                angle += 0.5f * waitTimer;
                waitTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (waitTimer <= 0)
                {
                    state = SpinLaserState.Idle;
                }
            }

            chargeEffect.Position = Transform.LocalPosition;
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, ToRectangle(), null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0f);

            //debugging the generated path
            //if(bezier != null)
            //{
            //    foreach (var item in bezier.KeyPoints)
            //    {
            //        Screen.spriteBatch.DrawCircle(new Vector2(item.X, item.Y) + Scene.World.camEntity.Transform.Position, 10f, 10, Color.Red, 1f);
            //    }
            //}
        }
    }
}
