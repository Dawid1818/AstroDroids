using AstroDroids.Entities.Friendly;
using AstroDroids.Gameplay;
using AstroDroids.Input;
using AstroDroids.Projectiles;
using Microsoft.Xna.Framework;
using System;

namespace AstroDroids.Weapons
{
    public class PlasmaMortar : Weapon
    {
        float currentCooldown = 0f;

        bool otherShot = false;

        Vector2 playerVelocity;
        Vector2 playerPreviousPosition;
        Vector2 playerPreviousVelocityPosition;
        float playerExtraAngle = 0;
        float playerExtraAngleMax = 45;

        public override void Update(Player player, GameTime gameTime)
        {
            playerVelocity = (player.GetPosition() - playerPreviousPosition);

            bool directionChanged = Math.Sign(playerPreviousVelocityPosition.X) != Math.Sign(playerVelocity.X);

            if (directionChanged)
            {
                playerExtraAngle = 0f;
            }

            if (playerVelocity != Vector2.Zero)
            {
                playerVelocity.Normalize();
            }
            else
            {
                playerExtraAngle = 0f;
            }

            playerExtraAngle += (float)gameTime.ElapsedGameTime.TotalSeconds * playerVelocity.X * 300f;
            playerExtraAngle = float.Clamp(playerExtraAngle, -playerExtraAngleMax, playerExtraAngleMax);

            if (InputSystem.IsActionHeld(GameAction.Fire))
            {
                if (currentCooldown <= 0f)
                {
                    switch (GameState.Firepower)
                    {
                        default:
                        case 1:
                            if (!otherShot)
                                SpawnProjectile(player, player.LeftWeaponPod, gameTime, 0, 1f, false);
                            else
                                SpawnProjectile(player, player.RightWeaponPod, gameTime, 0, 1f, false);

                            currentCooldown = 0.5f;
                            break;
                        case 2:
                            SpawnProjectile(player, player.LeftWeaponPod, gameTime, 0, 1f, false);
                            SpawnProjectile(player, player.RightWeaponPod, gameTime, 0, 1f, false);

                            currentCooldown = 0.5f;
                            break;
                        case 3:
                            SpawnProjectile(player, player.LeftWeaponPod, gameTime, 0, 1f, false);
                            SpawnProjectile(player, player.RightWeaponPod, gameTime, 0, 1f, false);


                            SpawnProjectile(player, player.RearLeftWeaponPod, gameTime, -20, 3f, true);
                            SpawnProjectile(player, player.RearRightWeaponPod, gameTime, 20, 3f, true);

                            currentCooldown = 1f;
                            break;
                        case 4:
                            SpawnProjectile(player, player.LeftWeaponPod, gameTime, 0, 1f, false);
                            SpawnProjectile(player, player.RightWeaponPod, gameTime, 0, 1f, false);

                            SpawnProjectile(player, player.RearLeftWeaponPod, gameTime, -20, 3f, true);
                            SpawnProjectile(player, player.RearRightWeaponPod, gameTime, 20, 3f, true);

                            SpawnProjectile(player, player.RearLeftWeaponPod, gameTime, -30, 3f, true);
                            SpawnProjectile(player, player.RearRightWeaponPod, gameTime, 30, 3f, true);

                            currentCooldown = 1f;
                            break;
                        case 5:
                            SpawnProjectile(player, player.LeftWeaponPod, gameTime, 0, 1f, false);
                            SpawnProjectile(player, player.RightWeaponPod, gameTime, 0, 1f, false);

                            SpawnProjectile(player, player.RearLeftWeaponPod, gameTime, -20, 3f, true);
                            SpawnProjectile(player, player.RearRightWeaponPod, gameTime, 20, 3f, true);

                            SpawnProjectile(player, player.RearLeftWeaponPod, gameTime, -30, 3f, true);
                            SpawnProjectile(player, player.RearRightWeaponPod, gameTime, 30, 3f, true);

                            currentCooldown = 1f;
                            break;
                    }
                    otherShot = !otherShot;
                }
            }

            if (currentCooldown > 0)
                currentCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            playerPreviousPosition = player.Transform.Position;
            playerPreviousVelocityPosition = playerVelocity;
        }

        void SpawnProjectile(Player player, Vector2 relative, GameTime gameTime, float angle, float launchForce, bool isCluster)
        {
            float extraPower = float.Clamp(-playerVelocity.Y, 0, 0.7f);
            PlasmaMortarProjectile projectile = new PlasmaMortarProjectile(player.GetPosition() + relative, MathHelper.ToRadians(-90 + angle + playerExtraAngle), isCluster, GameState.Firepower, launchForce + extraPower);
            Scene.World.AddProjectile(projectile, true);
        }
    }
}
