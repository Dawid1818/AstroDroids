using AstroDroids.Entities.Friendly;
using AstroDroids.Gameplay;
using AstroDroids.Input;
using AstroDroids.Managers;
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

        public PulseCannon()
        {
            WeaponIcon = TextureManager.Get("UI/Weapons/PulseCannon");
        }

        public override void Update(Player player, GameTime gameTime)
        {
            if (InputSystem.IsActionHeld(GameAction.Fire) || InputSystem.GetLMB())
            {
                if (currentCooldown <= 0f)
                {
                    switch (GameStateManager.GetFirepower())
                    {
                        default:
                        case 1:
                            if(!otherShot)
                                SpawnProjectile(player, player.LeftWeaponPod, PulseCannonProjectileType.WeakCyan, (1 * phase), 1.5f);
                            else
                                SpawnProjectile(player, player.RightWeaponPod, PulseCannonProjectileType.WeakCyan, -(1 * phase), 1.5f);
                            break;
                        case 2:
                            SpawnProjectile(player, player.LeftWeaponPod, PulseCannonProjectileType.WeakCyan, (1 * phase), 0.9f);
                            SpawnProjectile(player, player.RightWeaponPod, PulseCannonProjectileType.WeakCyan, -(1 * phase), 0.9f);
                            break;
                        case 3:
                            SpawnProjectile(player, player.LeftWeaponPod, PulseCannonProjectileType.WeakCyan, (1 * phase), 0.7f);
                            SpawnProjectile(player, player.RightWeaponPod, PulseCannonProjectileType.WeakCyan, -(1 * phase), 0.7f);
                            SpawnProjectile(player, player.MiddleWeaponPod, PulseCannonProjectileType.WeakOrange, 0, 0.8f);
                            break;
                        case 4:
                            SpawnProjectile(player, player.LeftWeaponPod, PulseCannonProjectileType.WeakCyan, (1 * phase), 0.7f);
                            SpawnProjectile(player, player.RightWeaponPod, PulseCannonProjectileType.WeakCyan, -(1 * phase), 0.7f);
                            SpawnProjectile(player, player.MiddleWeaponPod, PulseCannonProjectileType.WeakOrange, 0, 0.8f);

                            if (!otherShot)
                                SpawnProjectile(player, player.RearLeftWeaponPod, PulseCannonProjectileType.WeakCyan, -(1 * phase), 0.4f);
                            else
                                SpawnProjectile(player, player.RearRightWeaponPod, PulseCannonProjectileType.WeakCyan, (1 * phase), 0.4f);
                            break;
                        case 5:
                            SpawnProjectile(player, player.LeftWeaponPod, PulseCannonProjectileType.WeakOrange, (1 * phase), 0.7f);
                            SpawnProjectile(player, player.RightWeaponPod, PulseCannonProjectileType.WeakOrange, -(1 * phase), 0.7f);
                            SpawnProjectile(player, player.MiddleWeaponPod, PulseCannonProjectileType.WeakRed, 0, 1f);

                            SpawnProjectile(player, player.RearLeftWeaponPod, PulseCannonProjectileType.WeakCyan, -(1 * phase), 0.4f);
                            SpawnProjectile(player, player.RearRightWeaponPod, PulseCannonProjectileType.WeakCyan, (1 * phase), 0.4f);
                            break;
                    }

                    SoundManager.PlaySound("laser1", 1f * (GameStateManager.GetFirepower() / (float)GameStateManager.MaxFirepower));

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

        void SpawnProjectile(Player player, Vector2 relative, PulseCannonProjectileType type, float angle, float damage)
        {
            PulseCannonProjectile projectile = new PulseCannonProjectile(player.GetPosition() + relative, type, MathHelper.ToRadians(-90 + angle) + player.Angle, damage);
            Scene.World.AddProjectile(projectile, true);
        }
    }
}
