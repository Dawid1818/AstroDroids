using AstroDroids.Entities.Friendly;
using AstroDroids.Gameplay;
using AstroDroids.Input;
using AstroDroids.Projectiles;
using Microsoft.Xna.Framework;

namespace AstroDroids.Weapons
{
    public class PulseCannon : Weapon
    {
        float currentCooldown = 0f;

        float phase = 0;
        int direction = 1;
        bool otherShot = false;

        public override void Update(Player player, GameTime gameTime)
        {
            if (InputSystem.IsActionHeld(GameAction.Fire))
            {
                if (currentCooldown <= 0f)
                {
                    switch (GameState.Firepower)
                    {
                        default:
                        case 1:
                            if(!otherShot)
                                SpawnProjectile(player, player.LeftWeaponPod, PulseCannonProjectileType.WeakCyan, (1 * phase));
                            else
                                SpawnProjectile(player, player.RightWeaponPod, PulseCannonProjectileType.WeakCyan, -(1 * phase));
                            break;
                        case 2:
                            SpawnProjectile(player, player.LeftWeaponPod, PulseCannonProjectileType.WeakCyan, (1 * phase));
                            SpawnProjectile(player, player.RightWeaponPod, PulseCannonProjectileType.WeakCyan, -(1 * phase));
                            break;
                        case 3:
                            SpawnProjectile(player, player.LeftWeaponPod, PulseCannonProjectileType.WeakCyan, (1 * phase));
                            SpawnProjectile(player, player.RightWeaponPod, PulseCannonProjectileType.WeakCyan, -(1 * phase));
                            SpawnProjectile(player, player.MiddleWeaponPod, PulseCannonProjectileType.WeakOrange, 0);
                            break;
                        case 4:
                            SpawnProjectile(player, player.LeftWeaponPod, PulseCannonProjectileType.WeakCyan, (1 * phase));
                            SpawnProjectile(player, player.RightWeaponPod, PulseCannonProjectileType.WeakCyan, -(1 * phase));
                            SpawnProjectile(player, player.MiddleWeaponPod, PulseCannonProjectileType.WeakOrange, 0);

                            if (!otherShot)
                                SpawnProjectile(player, player.RearLeftWeaponPod, PulseCannonProjectileType.WeakCyan, -(1 * phase));
                            else
                                SpawnProjectile(player, player.RearRightWeaponPod, PulseCannonProjectileType.WeakCyan, (1 * phase));
                            break;
                        case 5:
                            SpawnProjectile(player, player.LeftWeaponPod, PulseCannonProjectileType.WeakOrange, (1 * phase));
                            SpawnProjectile(player, player.RightWeaponPod, PulseCannonProjectileType.WeakOrange, -(1 * phase));
                            SpawnProjectile(player, player.MiddleWeaponPod, PulseCannonProjectileType.WeakRed, 0);

                            SpawnProjectile(player, player.RearLeftWeaponPod, PulseCannonProjectileType.WeakCyan, -(1 * phase));
                            SpawnProjectile(player, player.RearRightWeaponPod, PulseCannonProjectileType.WeakCyan, (1 * phase));
                            break;
                    }

                    phase += direction;

                    if (phase > 5)
                    {
                        direction = -1;
                    }else if(phase < 0)
                    {
                        direction = 1;
                    }

                    currentCooldown = 0.1f;

                    otherShot = !otherShot;
                }
            }

            if (currentCooldown > 0)
                currentCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        void SpawnProjectile(Player player, Vector2 relative, PulseCannonProjectileType type, float angle)
        {
            PulseCannonProjectile projectile = new PulseCannonProjectile(player.GetPosition() + relative, type, MathHelper.ToRadians(-90 + angle));
            Scene.World.AddProjectile(projectile, true);
        }
    }
}
