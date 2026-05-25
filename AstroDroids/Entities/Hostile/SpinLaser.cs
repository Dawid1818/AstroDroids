using AstroDroids.Entities.Effects;
using AstroDroids.Entities.Friendly;
using AstroDroids.Entities.Warnings;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Paths;
using AstroDroids.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Data;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using System;

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

        Texture2D texture;

        float angle = 0f;
        float waitTimer = 0f;
        float targetAngle = 0f;
        float attackTimer = 0f;
        BeamWarning warning;

        float turnSpeed = 7f;

        SpinLaserState state = SpinLaserState.Idle;

        ParticleEffectEntity chargeEffectEntity;

        RandomMoveManager RMM;

        public SpinLaser() : base(new Transform(0, 0), 5)
        {
            texture = TextureManager.Get("Ships/SpinLaser/tinyShip2");

            AddCircleCollider(Vector2.Zero, 16f);

            ParticleEffect chargeEffect = new ParticleEffect("ChargeBeam")
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

            chargeEffectEntity = new ParticleEffectEntity(chargeEffect);
        }

        public override void Spawned()
        {
            RMM = new RandomMoveManager(Transform.LocalPosition);
            Scene.World.AddEffect(chargeEffectEntity);
        }

        public override void Destroyed()
        {
            Scene.World.RemoveEffect(chargeEffectEntity);
            if (warning != null)
                Scene.World.RemoveWarning(warning);

            base.Destroyed();
        }

        public override void Update(GameTime gameTime)
        {
            Player target = Scene.World.GetRandomPlayer();

            if (PathManager != null && state == SpinLaserState.Moving)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
            }

            switch (state)
            {
                case SpinLaserState.Idle:
                    RMM.SetNewPath();
                    state = SpinLaserState.Moving;
                    break;
                case SpinLaserState.Moving:
                    Vector2 posBeforeMove = Transform.LocalPosition;

                    if (PathManager != null)
                    {
                        attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (!PathManager.Active || attackTimer >= 1f)
                        {
                            attackTimer = 0f;
                            state = SpinLaserState.IdleMoved;
                        }
                    }
                    else
                    {
                        RMM.Update(gameTime);
                        Transform.LocalPosition = RMM.Position;

                        if (!RMM.Active)
                        {
                            state = SpinLaserState.IdleMoved;
                        }
                    }

                    if(target != null)
                        targetAngle = GameHelper.AngleBetween(Transform.LocalPosition, target.GetLocalPosition());
                    Turn(gameTime);
                    break;
                case SpinLaserState.IdleMoved:
                    waitTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (target != null)
                        targetAngle = GameHelper.AngleBetween(Transform.LocalPosition, target.GetLocalPosition());
                    Turn(gameTime);


                    if (waitTimer >= 0.6f)
                    {
                        if (target != null)
                        {
                            Vector2 forwardPoint = GameHelper.RotateAroundPoint(Transform.Position + new Vector2(texture.Width / 2f, 0), Transform.Position, angle);

                            warning = new BeamWarning(new Transform(forwardPoint.X, forwardPoint.Y), (float)angle, 900);
                            Scene.World.AddWarning(warning, true);


                            state = SpinLaserState.Spinning;
                        }
                        else
                        {
                            state = SpinLaserState.Idle;
                        }

                        waitTimer = 0f;
                    }
                    break;
                case SpinLaserState.Spinning:
                    chargeEffectEntity.effect.AutoTrigger = true;
                    waitTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (waitTimer >= 1f)
                    {
                        Vector2 forwardPoint = GameHelper.RotateAroundPoint(Transform.Position + new Vector2(texture.Width / 2f, 0), Transform.Position, angle);

                        Scene.World.AddProjectile(new SpinLaserBeam(new Transform(forwardPoint.X, forwardPoint.Y), (float)angle), true);

                        if (warning != null)
                        {
                            Scene.World.RemoveWarning(warning);
                            warning = null;
                        }

                        waitTimer = 1f;
                        state = SpinLaserState.StoppingSpin;
                        chargeEffectEntity.effect.AutoTrigger = false;
                    }
                    break;
                case SpinLaserState.StoppingSpin:
                    waitTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (waitTimer <= 0)
                    {
                        state = SpinLaserState.Idle;
                    }
                    break;
                default:
                    break;
            }

            chargeEffectEntity.effect.Position = GameHelper.RotateAroundPoint(Transform.Position + new Vector2(texture.Width/2f, 0), Transform.Position, angle);
        }

        void Turn(GameTime gameTime)
        {
            float delta = MathHelper.WrapAngle(targetAngle - angle);

            angle += delta * MathF.Min(turnSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 1f);
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, new Rectangle((int)Transform.Position.X, (int)Transform.Position.Y, texture.Width, texture.Height), null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0f);

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
