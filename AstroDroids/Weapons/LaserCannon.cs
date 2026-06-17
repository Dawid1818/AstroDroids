using AstroDroids.Entities.Effects;
using AstroDroids.Entities.Friendly;
using AstroDroids.Gameplay;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Input;
using AstroDroids.Managers;
using AstroDroids.Projectiles;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Data;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;

namespace AstroDroids.Weapons
{
    public class LaserCannon : Weapon
    {
        float currentCooldown = 0f;

        bool otherShot = false;
        float charge = 0.1f;
        bool charging = false;

        ParticleEffectEntity chargeEffectEntity;
        ParticleEmitter chargeEmitter;
        float triggerTimer = 0f;

        int rightStartAngle = 10;
        int leftStartAngle = -10;

        int leftTargetAngle = 2;
        int rightTargetAngle = -2;

        public LaserCannon()
        {
            ParticleEffect chargeEffect = new ParticleEffect("LaserCannonCharge")
            {
                AutoTrigger = false            
            };

            chargeEmitter = new ParticleEmitter(2000)
            {
                Name = "Charge Emitter",
                LifeSpan = 1f,
                TextureRegion = new Texture2DRegion(TextureManager.GetPixelTexture()),

                Profile = Profile.Ring(0, CircleRadiation.Out),

                Parameters = new ParticleReleaseParameters
                {
                    Quantity = new ParticleInt32Parameter(2, 5),

                    Speed = new ParticleFloatParameter(10, 20),

                    Color = new ParticleColorParameter(new Vector3(30f, 1.0f, 0.5f)),
                    Scale = new ParticleVector2Parameter(new Vector2(4.0f, 4.0f))
                }
            };

            chargeEmitter.Modifiers.Add(new AgeModifier
            {
                Interpolators =
                {
                    new OpacityInterpolator
                    {
                        StartValue = 0f,
                        EndValue = 1f
                    }
                }
            });

            chargeEmitter.Modifiers.Add(new DragModifier
            {
                Density = 0.5f,
                DragCoefficient = 0.3f
            });

            chargeEmitter.Modifiers.Add(new CircleContainerModifier
            {
                Radius = 5f,
                RestitutionCoefficient = 0.2f
            });

            chargeEffect.Emitters.Add(chargeEmitter);

            chargeEffectEntity = new ParticleEffectEntity(chargeEffect);

            Scene.World.AddEffect(chargeEffectEntity);
        }

        public override void Update(Player player, GameTime gameTime)
        {
            if (InputSystem.IsActionHeld(GameAction.Fire))
            {
                if (currentCooldown <= 0f)
                {
                    charging = true;

                    if (charge < 1f)
                    {
                        charge += (float)gameTime.ElapsedGameTime.TotalSeconds + (0.015f * ((float)GameState.Firepower / GameState.MaxFirepower));

                        if (charge > 1f)
                            charge = 1f;
                    }
                }
            }
            else if (charging)
            {
                switch (GameState.Firepower)
                {
                    default:
                    case 1:
                        if (!otherShot)
                            SpawnProjectile(player, player.LeftWeaponPod, leftTargetAngle, leftStartAngle);
                        else
                            SpawnProjectile(player, player.RightWeaponPod, rightTargetAngle, rightStartAngle);
                        break;
                    case 2:
                        SpawnProjectile(player, player.LeftWeaponPod, leftTargetAngle, leftStartAngle);
                        SpawnProjectile(player, player.RightWeaponPod, rightTargetAngle, rightStartAngle);
                        break;
                    case 3:
                        SpawnProjectile(player, player.LeftWeaponPod, leftTargetAngle, leftStartAngle);
                        SpawnProjectile(player, player.RightWeaponPod, rightTargetAngle, rightStartAngle);

                        if (!otherShot)
                            SpawnProjectile(player, player.RearLeftWeaponPod, leftTargetAngle, leftStartAngle);
                        else
                            SpawnProjectile(player, player.RearRightWeaponPod, rightTargetAngle, rightStartAngle);
                        break;
                    case 4:
                        SpawnProjectile(player, player.LeftWeaponPod, leftTargetAngle, leftStartAngle);
                        SpawnProjectile(player, player.RightWeaponPod, rightTargetAngle, rightStartAngle);

                        SpawnProjectile(player, player.RearLeftWeaponPod, leftTargetAngle, leftStartAngle);
                        SpawnProjectile(player, player.RearRightWeaponPod, rightTargetAngle, rightStartAngle);
                        break;
                    case 5:
                        SpawnProjectile(player, player.LeftWeaponPod, leftTargetAngle, leftStartAngle);
                        SpawnProjectile(player, player.RightWeaponPod, rightTargetAngle, rightStartAngle);

                        SpawnProjectile(player, player.RearLeftWeaponPod, leftTargetAngle, leftStartAngle);
                        SpawnProjectile(player, player.RearRightWeaponPod, rightTargetAngle, rightStartAngle);
                        break;
                }

                charging = false;
                charge = 0f;
                ClearParticles();

                otherShot = !otherShot;
            }

            triggerTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            chargeEmitter.Offset = player.Transform.Position;

            if (triggerTimer >= 0.1f)
            {
                chargeEmitter.Parameters.Quantity = new ParticleInt32Parameter((int)(12 * charge));

                switch (GameState.Firepower)
                {
                    default:
                    case 1:
                        if (!otherShot)
                            DrawChargeIndicator(player.LeftWeaponPod);
                        else
                            DrawChargeIndicator(player.RightWeaponPod);
                        break;
                    case 2:
                        DrawChargeIndicator(player.LeftWeaponPod);
                        DrawChargeIndicator(player.RightWeaponPod);
                        break;
                    case 3:
                        DrawChargeIndicator(player.LeftWeaponPod);
                        DrawChargeIndicator(player.RightWeaponPod);

                        if (!otherShot)
                            DrawChargeIndicator(player.RearLeftWeaponPod);
                        else
                            DrawChargeIndicator(player.RearRightWeaponPod);
                        break;
                    case 4:
                        DrawChargeIndicator(player.LeftWeaponPod);
                        DrawChargeIndicator(player.RightWeaponPod);

                        DrawChargeIndicator(player.RearLeftWeaponPod);
                        DrawChargeIndicator(player.RearRightWeaponPod);
                        break;
                    case 5:
                        DrawChargeIndicator(player.LeftWeaponPod);
                        DrawChargeIndicator(player.RightWeaponPod);

                        DrawChargeIndicator(player.RearLeftWeaponPod);
                        DrawChargeIndicator(player.RearRightWeaponPod);
                        break;
                }

                triggerTimer = 0f;
            }

            if (currentCooldown > 0)
                currentCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        //Old automatic laser cannon behavior
        //public override void Update(Player player, GameTime gameTime)
        //{
        //    if (InputSystem.IsActionHeld(GameAction.Fire))
        //    {
        //        if (currentCooldown <= 0f)
        //        {
        //            switch (GameState.Firepower)
        //            {
        //                default:
        //                case 1:
        //                    if(!otherShot)
        //                        SpawnProjectile(player, player.LeftWeaponPod, 2);
        //                    else
        //                        SpawnProjectile(player, player.RightWeaponPod, -2);
        //                    break;
        //                case 2:
        //                    SpawnProjectile(player, player.LeftWeaponPod, 2);
        //                    SpawnProjectile(player, player.RightWeaponPod, -2);
        //                    break;
        //                case 3:
        //                    SpawnProjectile(player, player.LeftWeaponPod, 2);
        //                    SpawnProjectile(player, player.RightWeaponPod, -2);
        //                    break;
        //                case 4:
        //                    SpawnProjectile(player, player.LeftWeaponPod, 2);
        //                    SpawnProjectile(player, player.RightWeaponPod, -2);
        //                    break;
        //                case 5:
        //                    SpawnProjectile(player, player.LeftWeaponPod, 2);
        //                    SpawnProjectile(player, player.RightWeaponPod, -2);

        //                    SpawnProjectile(player, player.RearLeftWeaponPod, 2);
        //                    SpawnProjectile(player, player.RearRightWeaponPod, -2);
        //                    break;
        //            }

        //            phase += direction;

        //            if (phase > 5)
        //            {
        //                direction = -1;
        //            }else if(phase < 0)
        //            {
        //                direction = 1;
        //            }

        //            currentCooldown = 0.5f;
        //            charge = 0.1f;

        //            otherShot = !otherShot;

        //            ClearParticles();
        //        }
        //    }

        //    triggerTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        //    chargeEmitter.Offset = player.Transform.Position;

        //    if (triggerTimer >= 0.1f)
        //    {
        //        chargeEmitter.Parameters.Quantity = new ParticleInt32Parameter((int)(12 * charge));

        //        switch (GameState.Firepower)
        //        {
        //            default:
        //            case 1:
        //                if (!otherShot)
        //                    DrawChargeIndicator(player.LeftWeaponPod);
        //                else
        //                    DrawChargeIndicator(player.RightWeaponPod);
        //                break;
        //            case 2:
        //                DrawChargeIndicator(player.LeftWeaponPod);
        //                DrawChargeIndicator(player.RightWeaponPod);
        //                break;
        //            case 3:
        //                DrawChargeIndicator(player.LeftWeaponPod);
        //                DrawChargeIndicator(player.RightWeaponPod);
        //                break;
        //            case 4:
        //                DrawChargeIndicator(player.LeftWeaponPod);
        //                DrawChargeIndicator(player.RightWeaponPod);
        //                break;
        //            case 5:
        //                DrawChargeIndicator(player.LeftWeaponPod);
        //                DrawChargeIndicator(player.RightWeaponPod);

        //                DrawChargeIndicator(player.RearLeftWeaponPod);
        //                DrawChargeIndicator(player.RearRightWeaponPod);
        //                break;
        //        }

        //        triggerTimer = 0f;
        //    }

        //    if (currentCooldown > 0)
        //        currentCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        //    else if(charge < 1f)
        //    {
        //        charge += (float)gameTime.ElapsedGameTime.TotalSeconds;

        //        if (charge > 1f)
        //            charge = 1f;
        //    }
        //}

        void ClearParticles()
        {
            chargeEmitter.Buffer.Reclaim(chargeEmitter.Buffer.Count);
        }

        public override void DrawEffects(Player player, GameTime gameTime)
        {
            if(charging)
            {
                switch (GameState.Firepower)
                {
                    default:
                    case 1:
                        if (!otherShot)
                        {
                            DrawAimIndicator(player, player.LeftWeaponPod, leftTargetAngle, leftStartAngle);
                        }
                        else
                        {
                            DrawAimIndicator(player, player.RightWeaponPod, rightTargetAngle, rightStartAngle);
                        }
                        break;
                    case 2:
                        DrawAimIndicator(player, player.LeftWeaponPod, leftTargetAngle, leftStartAngle);
                        DrawAimIndicator(player, player.RightWeaponPod, rightTargetAngle, rightStartAngle);
                        break;
                    case 3:
                        DrawAimIndicator(player, player.LeftWeaponPod, leftTargetAngle, leftStartAngle);
                        DrawAimIndicator(player, player.RightWeaponPod, rightTargetAngle, rightStartAngle);

                        if (!otherShot)
                            DrawAimIndicator(player, player.RearLeftWeaponPod, leftTargetAngle, leftStartAngle);
                        else
                            DrawAimIndicator(player, player.RearRightWeaponPod, rightTargetAngle, rightStartAngle);
                        break;
                    case 4:
                        DrawAimIndicator(player, player.LeftWeaponPod, leftTargetAngle, leftStartAngle);
                        DrawAimIndicator(player, player.RightWeaponPod, rightTargetAngle, rightStartAngle);

                        DrawAimIndicator(player, player.RearLeftWeaponPod, leftTargetAngle, leftStartAngle);
                        DrawAimIndicator(player, player.RearRightWeaponPod, rightTargetAngle, rightStartAngle);
                        break;
                    case 5:
                        DrawAimIndicator(player, player.LeftWeaponPod, leftTargetAngle, leftStartAngle);
                        DrawAimIndicator(player, player.RightWeaponPod, rightTargetAngle, rightStartAngle);

                        DrawAimIndicator(player, player.RearLeftWeaponPod, leftTargetAngle, leftStartAngle);
                        DrawAimIndicator(player, player.RearRightWeaponPod, rightTargetAngle, rightStartAngle);
                        break;
                }
            }
        }

        void DrawAimIndicator(Player player, Vector2 pos, float targetAngle, float startAngle)
        {
            float actualTargetAngle = -90 + targetAngle;
            float actualAngle = float.Lerp(-90 + startAngle, actualTargetAngle, charge);
            Vector2 targetPos = GameHelper.OrbitPos(player.GetPosition() + pos, MathHelper.ToRadians(actualAngle), 1000);

            Screen.spriteBatch.DrawLine(player.GetPosition() + pos, targetPos, new Color((byte)255, (byte)255, (byte)255, (byte)(charge * 127)), 2);
        }

        void DrawChargeIndicator(Vector2 relative)
        {
            chargeEffectEntity.effect.Trigger(relative + new Vector2(0, -5f));
        }

        void SpawnProjectile(Player player, Vector2 relative, float angle)
        {
            LaserCannonProjectile projectile = new LaserCannonProjectile(player.GetPosition() + relative, MathHelper.ToRadians(-90 + angle), charge, GameState.Firepower);
            Scene.World.AddProjectile(projectile, true);
        }

        void SpawnProjectile(Player player, Vector2 relative, float targetAngle, float startAngle)
        {
            float actualTargetAngle = -90 + targetAngle;
            float actualAngle = float.Lerp(-90 + startAngle, actualTargetAngle, charge);

            LaserCannonProjectile projectile = new LaserCannonProjectile(player.GetPosition() + relative, MathHelper.ToRadians(actualAngle), charge, GameState.Firepower);
            Scene.World.AddProjectile(projectile, true);
        }

        public override void ResetState()
        {
            otherShot = false;
            charge = 0;
        }
    }
}
