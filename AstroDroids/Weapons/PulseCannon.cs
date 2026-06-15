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
            Vector2 rightWeaponPod = new Vector2((player.Width / 2f) + 4, -player.Height);
            Vector2 leftWeaponPod = new Vector2(-((player.Width / 2f) + 2), -player.Height);
            Vector2 middleWeaponPod = new Vector2(1, -player.Height);

            Vector2 rearRightWeaponPod = new Vector2((player.Width / 2f) + 12, player.Height - 20);
            Vector2 rearLeftWeaponPod = new Vector2(-((player.Width / 2f) + 10), player.Height - 20);

            if (InputSystem.IsActionHeld(GameAction.Fire))
            {
                if (currentCooldown <= 0f)
                {
                    switch (GameState.Firepower)
                    {
                        default:
                        case 1:
                            if(!otherShot)
                                SpawnProjectile(player, leftWeaponPod, BasicProjectileType.WeakCyan, (1 * phase));
                            else
                                SpawnProjectile(player, rightWeaponPod, BasicProjectileType.WeakCyan, -(1 * phase));
                            break;
                        case 2:
                            SpawnProjectile(player, leftWeaponPod, BasicProjectileType.WeakCyan, (1 * phase));
                            SpawnProjectile(player, rightWeaponPod, BasicProjectileType.WeakCyan, -(1 * phase));
                            break;
                        case 3:
                            SpawnProjectile(player, leftWeaponPod, BasicProjectileType.WeakCyan, (1 * phase));
                            SpawnProjectile(player, rightWeaponPod, BasicProjectileType.WeakCyan, -(1 * phase));
                            SpawnProjectile(player, middleWeaponPod, BasicProjectileType.WeakOrange, 0);
                            break;
                        case 4:
                            SpawnProjectile(player, leftWeaponPod, BasicProjectileType.WeakCyan, (1 * phase));
                            SpawnProjectile(player, rightWeaponPod, BasicProjectileType.WeakCyan, -(1 * phase));
                            SpawnProjectile(player, middleWeaponPod, BasicProjectileType.WeakOrange, 0);

                            if (!otherShot)
                                SpawnProjectile(player, rearLeftWeaponPod, BasicProjectileType.WeakCyan, -(1 * phase));
                            else
                                SpawnProjectile(player, rearRightWeaponPod, BasicProjectileType.WeakCyan, (1 * phase));
                            break;
                        case 5:
                            SpawnProjectile(player, leftWeaponPod, BasicProjectileType.WeakOrange, (1 * phase));
                            SpawnProjectile(player, rightWeaponPod, BasicProjectileType.WeakOrange, -(1 * phase));
                            SpawnProjectile(player, middleWeaponPod, BasicProjectileType.WeakRed, 0);

                            SpawnProjectile(player, rearLeftWeaponPod, BasicProjectileType.WeakCyan, -(1 * phase));
                            SpawnProjectile(player, rearRightWeaponPod, BasicProjectileType.WeakCyan, (1 * phase));
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

        void SpawnProjectile(Player player, Vector2 relative, BasicProjectileType type, float angle)
        {
            BasicProjectile projectile = new BasicProjectile(player.GetPosition() + relative, type, MathHelper.ToRadians(-90 + angle));
            Scene.World.AddProjectile(projectile, true);
        }
    }
}
