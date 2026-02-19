using AstroDroids.Entities.Friendly;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstroDroids.Entities.Hostile
{
    public class TriGunTurret : Enemy
    {
        public float t = 0f;

        Texture2D baseTexture;
        Texture2D topTexture;
        Texture2D cannonTexture;

        float attackTimer;
        float angle = 0f;

        Vector2 cannon1Offset = new Vector2(16, -14);
        Vector2 cannon2Offset = new Vector2(22, 0);
        Vector2 cannon3Offset = new Vector2(16, 14);

        Vector2 cannon1Pos;
        Vector2 cannon2Pos;
        Vector2 cannon3Pos;

        public TriGunTurret() : base(new Transform(0, 0), 1, 61f, 62f)
        {
            baseTexture = TextureManager.Get("Turrets/Base/TurretBase");
            topTexture = TextureManager.Get("Turrets/TriGun/TriGunTop");
            cannonTexture = TextureManager.Get("Turrets/TriGun/TriGunCannon");

            AddCircleCollider(Vector2.Zero, 32f);
        }

        public override void Update(GameTime gameTime)
        {
            Player player = Scene.World.GetRandomPlayer();

            DefaultMove();

            if (player != null)
            {
                angle = GameHelper.AngleBetween(Transform.Position, player.GetPosition());
            }

            cannon1Pos = GameHelper.RotateAroundPoint(Transform.Position + cannon1Offset, Transform.Position, angle);
            cannon2Pos = GameHelper.RotateAroundPoint(Transform.Position + cannon2Offset, Transform.Position, angle);
            cannon3Pos = GameHelper.RotateAroundPoint(Transform.Position + cannon3Offset, Transform.Position, angle);

            if (PathManager != null)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
            }

            attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (attackTimer >= 1f)
            {
                attackTimer = 0f;

                Shoot();
            }
        }

        void Shoot()
        {
            Scene.World.AddProjectile(new CircleProjectile(new Transform(cannon1Pos.X, cannon1Pos.Y), 32, 16, angle), true);
            Scene.World.AddProjectile(new CircleProjectile(new Transform(cannon2Pos.X, cannon2Pos.Y), 32, 16, angle), true);
            Scene.World.AddProjectile(new CircleProjectile(new Transform(cannon3Pos.X, cannon3Pos.Y), 32, 16, angle), true);
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(baseTexture, Transform.Position, null, Color.White, 0f, new Vector2(baseTexture.Width / 2, baseTexture.Height / 2), 1f, SpriteEffects.None, 0f);

            Screen.spriteBatch.Draw(cannonTexture, cannon1Pos, null, Color.White, angle, new Vector2(cannonTexture.Width / 2, cannonTexture.Height / 2), 1f, SpriteEffects.None, 0f);
            Screen.spriteBatch.Draw(cannonTexture, cannon2Pos, null, Color.White, angle, new Vector2(cannonTexture.Width / 2, cannonTexture.Height / 2), 1f, SpriteEffects.None, 0f);
            Screen.spriteBatch.Draw(cannonTexture, cannon3Pos, null, Color.White, angle, new Vector2(cannonTexture.Width / 2, cannonTexture.Height / 2), 1f, SpriteEffects.None, 0f);

            Screen.spriteBatch.Draw(topTexture, new Rectangle((int)Transform.Position.X, (int)Transform.Position.Y, 42, 42), null, Color.White, angle, new Vector2(topTexture.Width / 2, topTexture.Height / 2), SpriteEffects.None, 0f);
        }
    }
}
