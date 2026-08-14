using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Projectiles.Hostile;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.IO;

namespace AstroDroids.Entities.Hostile
{
    public class OrbTurretSpawnData : IEnemySpawnData
    {
        public float RotationSpeed { get; set; } = 1f;
        public float InitialRotation { get; set; } = 0f;
        public float FireFrequency { get; set; } = 0.5f;
        public float ProjectileSize { get; set; } = 8f;
        public float ProjectileSpeed { get; set; } = 5f;
        public float ProjectileDecay { get; set; } = 0f;
        public bool FireCannon1 { get; set; } = true;
        public bool FireCannon2 { get; set; } = true;
        public bool FireCannon3 { get; set; } = true;
        public bool FireCannon4 { get; set; } = true;

        public void DrawEditor()
        {
            float rotSpeed = RotationSpeed;
            if (ImGui.InputFloat("Rotation speed", ref rotSpeed))
            {
                RotationSpeed = rotSpeed;
            }

            float initRot = InitialRotation;
            if (ImGui.InputFloat("Initial rotation", ref initRot))
            {
                InitialRotation = initRot;
            }

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

            bool fireCannon1 = FireCannon1;
            if(ImGui.Checkbox("Fire cannon 1", ref fireCannon1))
            {
                FireCannon1 = fireCannon1;
            }

            bool fireCannon2 = FireCannon2;
            if (ImGui.Checkbox("Fire cannon 2", ref fireCannon2))
            {
                FireCannon2 = fireCannon2;
            }

            bool fireCannon3 = FireCannon3;
            if (ImGui.Checkbox("Fire cannon 3", ref fireCannon3))
            {
                FireCannon3 = fireCannon3;
            }

            bool fireCannon4 = FireCannon4;
            if (ImGui.Checkbox("Fire cannon 4", ref fireCannon4))
            {
                FireCannon4 = fireCannon4;
            }
        }

        public void Load(BinaryReader reader, int version)
        {
            RotationSpeed = reader.ReadSingle();
            InitialRotation = reader.ReadSingle();
            FireFrequency = reader.ReadSingle();
            ProjectileSize = reader.ReadSingle();
            ProjectileSpeed = reader.ReadSingle();
            ProjectileDecay = reader.ReadSingle();

            FireCannon1 = reader.ReadBoolean();
            FireCannon2 = reader.ReadBoolean();
            FireCannon3 = reader.ReadBoolean();
            FireCannon4 = reader.ReadBoolean();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(RotationSpeed);
            writer.Write(InitialRotation);
            writer.Write(FireFrequency);
            writer.Write(ProjectileSize);
            writer.Write(ProjectileSpeed);
            writer.Write(ProjectileDecay);

            writer.Write(FireCannon1);
            writer.Write(FireCannon2);
            writer.Write(FireCannon3);
            writer.Write(FireCannon4);
        }
    }
    public class OrbTurret : Enemy
    {
        bool becameActive = false;
        public float t = 0f;

        Texture2D baseTexture;
        Texture2D cannonTexture;

        float attackTimer;
        float angle = 0f;

        public Vector2 Cannon1 { get { return GameHelper.RotateAroundPoint(new Vector2((Width / 2f) + 5, 0), Vector2.Zero, angle); } }
        public Vector2 Cannon2 { get { return GameHelper.RotateAroundPoint(new Vector2(-(Width / 2f) - 5, 0), Vector2.Zero, angle); } }
        public Vector2 Cannon3 { get { return GameHelper.RotateAroundPoint(new Vector2(0, (Height / 2f) + 5), Vector2.Zero, angle); } }
        public Vector2 Cannon4 { get { return GameHelper.RotateAroundPoint(new Vector2(0, -(Height / 2f) - 5), Vector2.Zero, angle); } }

        float frequency = 0.5f;
        float projectileRadius = 8f;
        float rotationSpeed = 1f;
        float projectileSpeed = 5f;
        float projectileDecay = 0f;

        bool fireCannon1 = true;
        bool fireCannon2 = true;
        bool fireCannon3 = true;
        bool fireCannon4 = true;

        public OrbTurret() : base(Vector2.Zero, 1)
        {
            baseTexture = TextureManager.Get("Turrets/Base/TurretBasev2");
            cannonTexture = TextureManager.Get("Turrets/OrbTurret/OrbTurret");

            AddCircleCollider(Vector2.Zero, 24f);
        }

        public override void ApplySpawnData(IEnemySpawnData spawnData)
        {
            OrbTurretSpawnData data = (OrbTurretSpawnData)spawnData;

            frequency = data.FireFrequency;
            projectileRadius = data.ProjectileSize;
            rotationSpeed = data.RotationSpeed;
            angle = MathHelper.ToRadians(data.InitialRotation);
            projectileDecay = data.ProjectileDecay;
            projectileSpeed = data.ProjectileSpeed;

            fireCannon1 = data.FireCannon1;
            fireCannon2 = data.FireCannon2;
            fireCannon3 = data.FireCannon3;
            fireCannon4 = data.FireCannon4;
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
            if (!becameActive)
            {
                if (Intersects(Scene.World.Bounds))
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
            if (!Intersects(Scene.World.ExpandedBounds) && becameActive)
            {
                Despawn();
            }

            angle += gameTime.GetElapsedSeconds() * rotationSpeed;

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

                Shoot();
            }
        }

        void Shoot()
        {
            if(fireCannon1)
                Scene.World.AddProjectile(new CircleProjectile(Transform.Position + Cannon1, angle, projectileSpeed, projectileRadius, projectileDecay), true);
            if(fireCannon2)
                Scene.World.AddProjectile(new CircleProjectile(Transform.Position + Cannon2, angle + MathHelper.ToRadians(180), projectileSpeed, projectileRadius, projectileDecay), true);
            if(fireCannon3)
                Scene.World.AddProjectile(new CircleProjectile(Transform.Position + Cannon3, angle + MathHelper.ToRadians(90), projectileSpeed, projectileRadius, projectileDecay), true);
            if(fireCannon4)
                Scene.World.AddProjectile(new CircleProjectile(Transform.Position + Cannon4, angle + MathHelper.ToRadians(-90), projectileSpeed, projectileRadius, projectileDecay), true);
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(baseTexture, Transform.Position, null, Color.White, 0f, new Vector2(baseTexture.Width / 2, baseTexture.Height / 2), 1f, SpriteEffects.None, 0f);

            Screen.spriteBatch.Draw(cannonTexture, Transform.Position, null, Color.White, angle, new Vector2(cannonTexture.Width / 2, cannonTexture.Height / 2), 1f, SpriteEffects.None, 0f);

            if (AstroDroidsGame.Debug)
            {
                Screen.spriteBatch.DrawCircle(Cannon1 + Transform.Position, 12, 12, Color.Green);
                Screen.spriteBatch.DrawCircle(Cannon2 + Transform.Position, 12, 12, Color.Green);
                Screen.spriteBatch.DrawCircle(Cannon3 + Transform.Position, 12, 12, Color.Green);
                Screen.spriteBatch.DrawCircle(Cannon4 + Transform.Position, 12, 12, Color.Green);
            }
        }
    }
}
