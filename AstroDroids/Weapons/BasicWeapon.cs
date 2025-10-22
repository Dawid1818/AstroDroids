﻿using AstroDroids.Entities.Friendly;
using AstroDroids.Input;
using AstroDroids.Projectiles;
using Microsoft.Xna.Framework;

namespace AstroDroids.Weapons
{
    public class BasicWeapon : Weapon
    {
        float currentCooldown = 0f;

        public override void Update(Player player, GameTime gameTime)
        {
            if (InputSystem.IsActionHeld(GameAction.Fire))
            {
                if (currentCooldown <= 0f)
                {
                    BasicProjectile projectile = new BasicProjectile(player.GetPosition());
                    
                    Scene.World.AddProjectile(projectile);

                    currentCooldown = 0.5f;
                }
            }

            if(currentCooldown > 0)
                currentCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
