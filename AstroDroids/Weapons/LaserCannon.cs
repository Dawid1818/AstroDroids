using AstroDroids.Entities.Friendly;
using AstroDroids.Gameplay;
using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Projectiles;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace AstroDroids.Weapons
{
    public class LaserCannon : Weapon
    {
        float currentCooldown = 0f;

        float phase = 0;
        int direction = 1;
        bool otherShot = false;
        float charge = 0.1f;

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
                                SpawnProjectile(player, player.LeftWeaponPod, 2);
                            else
                                SpawnProjectile(player, player.RightWeaponPod, -2);
                            break;
                        case 2:
                            SpawnProjectile(player, player.LeftWeaponPod, 2);
                            SpawnProjectile(player, player.RightWeaponPod, -2);
                            break;
                        case 3:
                            SpawnProjectile(player, player.LeftWeaponPod, 2);
                            SpawnProjectile(player, player.RightWeaponPod, -2);
                            break;
                        case 4:
                            SpawnProjectile(player, player.LeftWeaponPod, 2);
                            SpawnProjectile(player, player.RightWeaponPod, -2);
                            break;
                        case 5:
                            SpawnProjectile(player, player.LeftWeaponPod, 2);
                            SpawnProjectile(player, player.RightWeaponPod, -2);

                            SpawnProjectile(player, player.RearLeftWeaponPod, 2);
                            SpawnProjectile(player, player.RearRightWeaponPod, -2);
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

                    currentCooldown = 0.5f;
                    charge = 0.1f;

                    otherShot = !otherShot;
                }
            }

            if (currentCooldown > 0)
                currentCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if(charge < 1f)
            {
                charge += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (charge > 1f)
                    charge = 1f;
            }
        }

        public override void DrawEffects(Player player, GameTime gameTime)
        {
            switch (GameState.Firepower)
            {
                default:
                case 1:
                    if (!otherShot)
                        DrawChargeIndicator(player, player.LeftWeaponPod);
                    else
                        DrawChargeIndicator(player, player.RightWeaponPod);
                    break;
                case 2:
                    DrawChargeIndicator(player, player.LeftWeaponPod);
                    DrawChargeIndicator(player, player.RightWeaponPod);
                    break;
                case 3:
                    DrawChargeIndicator(player, player.LeftWeaponPod);
                    DrawChargeIndicator(player, player.RightWeaponPod);
                    break;
                case 4:
                    DrawChargeIndicator(player, player.LeftWeaponPod);
                    DrawChargeIndicator(player, player.RightWeaponPod);
                    break;
                case 5:
                    DrawChargeIndicator(player, player.LeftWeaponPod);
                    DrawChargeIndicator(player, player.RightWeaponPod);

                    DrawChargeIndicator(player, player.RearLeftWeaponPod);
                    DrawChargeIndicator(player, player.RearRightWeaponPod);
                    break;
            }
        }

        void DrawChargeIndicator(Player player, Vector2 relative)
        {
            float chargem = 12 * charge;

            Screen.spriteBatch.DrawCircle(player.GetPosition() + relative, chargem, (int)chargem, Color.White, chargem, 0);
        }

        void SpawnProjectile(Player player, Vector2 relative, float angle)
        {
            LaserCannonProjectile projectile = new LaserCannonProjectile(player.GetPosition() + relative, MathHelper.ToRadians(-90 + angle), charge);
            Scene.World.AddProjectile(projectile, true);
        }

        public override void ResetState()
        {
            phase = 0;
            direction = 1;
            otherShot = false;
            charge = 0.1f;
        }
    }
}
