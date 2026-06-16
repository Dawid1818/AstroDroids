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
                                SpawnProjectile(player, leftWeaponPod, 2);
                            else
                                SpawnProjectile(player, rightWeaponPod, -2);
                            break;
                        case 2:
                            SpawnProjectile(player, leftWeaponPod, 2);
                            SpawnProjectile(player, rightWeaponPod, -2);
                            break;
                        case 3:
                            SpawnProjectile(player, leftWeaponPod, 2);
                            SpawnProjectile(player, rightWeaponPod, -2);
                            break;
                        case 4:
                            SpawnProjectile(player, leftWeaponPod, 2);
                            SpawnProjectile(player, rightWeaponPod, -2);
                            break;
                        case 5:
                            SpawnProjectile(player, leftWeaponPod, 2);
                            SpawnProjectile(player, rightWeaponPod, -2);

                            SpawnProjectile(player, rearLeftWeaponPod, 2);
                            SpawnProjectile(player, rearRightWeaponPod, -2);
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
            Vector2 rightWeaponPod = new Vector2((player.Width / 2f) + 4, -player.Height);
            Vector2 leftWeaponPod = new Vector2(-((player.Width / 2f) + 2), -player.Height);
            Vector2 middleWeaponPod = new Vector2(1, -player.Height);

            Vector2 rearRightWeaponPod = new Vector2((player.Width / 2f) + 12, player.Height - 20);
            Vector2 rearLeftWeaponPod = new Vector2(-((player.Width / 2f) + 10), player.Height - 20);

            float chargem = 12 * charge;

            switch (GameState.Firepower)
            {
                default:
                case 1:
                    if (!otherShot)
                        DrawChargeIndicator(player, leftWeaponPod);
                    else
                        DrawChargeIndicator(player, rightWeaponPod);
                    break;
                case 2:
                    DrawChargeIndicator(player, leftWeaponPod);
                    DrawChargeIndicator(player, rightWeaponPod);
                    break;
                case 3:
                    DrawChargeIndicator(player, leftWeaponPod);
                    DrawChargeIndicator(player, rightWeaponPod);
                    break;
                case 4:
                    DrawChargeIndicator(player, leftWeaponPod);
                    DrawChargeIndicator(player, rightWeaponPod);
                    break;
                case 5:
                    DrawChargeIndicator(player, leftWeaponPod);
                    DrawChargeIndicator(player, rightWeaponPod);

                    DrawChargeIndicator(player, rearLeftWeaponPod);
                    DrawChargeIndicator(player, rearRightWeaponPod);
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
