using AstroDroids.Entities.Friendly;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Projectiles.Hostile;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace AstroDroids.Entities.Hostile
{
    public class TriGunTurretSpawnData : IEnemySpawnData
    {
        public float FireFrequency { get; set; } = 1f;
        public float ProjectileSize { get; set; } = 8f;
        public float ProjectileSpeed { get; set; } = 5f;
        public float ProjectileDecay { get; set; } = 0f;

        public void DrawEditor()
        {
            float fireFreq = FireFrequency;
            if (ImGui.InputFloat("Fire Frequency", ref fireFreq))
            {
                FireFrequency = fireFreq;
            }

            float projSize = ProjectileSize;
            if (ImGui.InputFloat("Projectile size", ref projSize))
            {
                ProjectileSize = projSize;
            }

            float projSpeed = ProjectileSpeed;
            if (ImGui.InputFloat("Projectile speed", ref projSpeed))
            {
                ProjectileSpeed = projSpeed;
            }

            float projDecay = ProjectileDecay;
            if (ImGui.InputFloat("Projectile decay", ref projDecay))
            {
                ProjectileDecay = projDecay;
            }
        }

        public void Load(BinaryReader reader, int version)
        {
            if (version >= 14)
            {
                FireFrequency = reader.ReadSingle();
                ProjectileSize = reader.ReadSingle();
                ProjectileSpeed = reader.ReadSingle();
                ProjectileDecay = reader.ReadSingle();
            }
            else
            {
                FireFrequency = 1f;
                ProjectileDecay = 0f;
                ProjectileSpeed = 5f;
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(FireFrequency);
            writer.Write(ProjectileSize);
            writer.Write(ProjectileSpeed);
            writer.Write(ProjectileDecay);
        }
    }
    public class TriGunTurret : Enemy
    {
        bool becameActive = false;
        public float t = 0f;

        Texture2D baseTexture;
        //Texture2D topTexture;
        Texture2D cannonTexture;

        float attackTimer;
        float angle = 0f;

        Vector2 cannon1Offset = new Vector2(16, -14);
        Vector2 cannon2Offset = new Vector2(22, 0);
        Vector2 cannon3Offset = new Vector2(16, 14);

        Vector2 cannon1Pos;
        Vector2 cannon2Pos;
        Vector2 cannon3Pos;

        float frequency = 1f;
        float projectileSpeed = 5f;
        float projectileDecay = 0f;
        float projectileRadius = 8f;

        public TriGunTurret() : base(Vector2.Zero, 10)
        {
            CanBeShielded = true;

            //baseTexture = TextureManager.Get("Turrets/Base/TurretBase");
            baseTexture = TextureManager.Get("Turrets/Base/TurretBasev2");
            //topTexture = TextureManager.Get("Turrets/TriGun/TriGunTop");
            //cannonTexture = TextureManager.Get("Turrets/TriGun/TriGunCannon");
            cannonTexture = TextureManager.Get("Turrets/TriGun/DoubleGunCannon");

            AddCircleCollider(Vector2.Zero, 24f);
        }

        public override void ApplySpawnData(IEnemySpawnData spawnData)
        {
            TriGunTurretSpawnData data = (TriGunTurretSpawnData)spawnData;

            frequency = data.FireFrequency;
            projectileRadius = data.ProjectileSize;
            projectileDecay = data.ProjectileDecay;
            projectileSpeed = data.ProjectileSpeed;
        }

        public override void Spawned()
        {
            if (Intersects(Scene.World.Bounds))
            {
                becameActive = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            bool isInBounds = Intersects(Scene.World.Bounds);

            if (!becameActive && !DespawnOnCameraPathEnd)
            {
                if (isInBounds)
                {
                    becameActive = true;
                }
                else
                {
                    if (t >= 10f)
                        Despawn();

                    t += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
            if (!Intersects(Scene.World.ExpandedBounds) && becameActive && !DespawnOnCameraPathEnd)
            {
                Despawn();
            }

            if (DespawnOnCameraPathEnd)
            {
                if (Scene.World.camEntity.PathManager == null || !Scene.World.camEntity.PathManager.Active)
                {
                    Despawn();
                }
            }

            Player player = Scene.World.GetRandomPlayer();

            if (player != null)
            {
                angle = GameHelper.AngleBetween(Transform.Position, player.GetPosition());
            }

            cannon1Pos = GameHelper.RotateAroundPoint(Transform.Position + cannon1Offset, Transform.Position, angle);
            cannon2Pos = GameHelper.RotateAroundPoint(Transform.Position + cannon2Offset, Transform.Position, angle);
            cannon3Pos = GameHelper.RotateAroundPoint(Transform.Position + cannon3Offset, Transform.Position, angle);

            if (PathManager != null && PathManager.Active)
            {
                if (!FollowsCamera)
                    PathManager.Translate(new Vector2(0, (float)Scene.World.speed));
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
            }
            else
            {
                if (!FollowsCamera && Transform.GetParent() == null)
                    DefaultMove();
            }

            attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (attackTimer >= frequency)
            {
                attackTimer = 0f;

                if(isInBounds)
                    Shoot();
            }
        }

        void Shoot()
        {
            Scene.World.AddProjectile(new CircleProjectile(cannon1Pos, angle, projectileSpeed, projectileRadius, projectileDecay), true);
            //Scene.World.AddProjectile(new CircleProjectile(cannon2Pos, angle), true);
            Scene.World.AddProjectile(new CircleProjectile(cannon3Pos, angle, projectileSpeed, projectileRadius, projectileDecay), true);
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(baseTexture, Transform.Position, null, Color.White, 0f, new Vector2(baseTexture.Width / 2, baseTexture.Height / 2), 1f, SpriteEffects.None, 0f);

            Screen.spriteBatch.Draw(cannonTexture, Transform.Position, null, Color.White, angle, new Vector2(cannonTexture.Width / 2, cannonTexture.Height / 2), 1f, SpriteEffects.None, 0f);
            //Screen.spriteBatch.Draw(cannonTexture, cannon2Pos, null, Color.White, angle, new Vector2(cannonTexture.Width / 2, cannonTexture.Height / 2), 1f, SpriteEffects.None, 0f);
            //Screen.spriteBatch.Draw(cannonTexture, cannon3Pos, null, Color.White, angle, new Vector2(cannonTexture.Width / 2, cannonTexture.Height / 2), 1f, SpriteEffects.None, 0f);

            //Screen.spriteBatch.Draw(topTexture, new Rectangle((int)Transform.Position.X, (int)Transform.Position.Y, 42, 42), null, Color.White, angle, new Vector2(topTexture.Width / 2, topTexture.Height / 2), SpriteEffects.None, 0f);
        }
    }
}
