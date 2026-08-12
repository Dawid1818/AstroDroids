using AstroDroids.Entities.Neutral;
using AstroDroids.Graphics;
using AstroDroids.Managers;
using AstroDroids.Projectiles.Hostile;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace AstroDroids.Entities.Hostile
{
    public class ProximityMineSpawnData : IEnemySpawnData
    {
        public ProximityMineType Type { get; set; } = ProximityMineType.Orbs;

        public void DrawEditor()
        {
            if (ImGui.BeginCombo("Mine Type", Type.ToString()))
            {
                foreach (var style in Enum.GetValues(typeof(ProximityMineType)))
                {
                    bool isSelected = (ProximityMineType)style == Type;
                    if (ImGui.Selectable(style.ToString(), isSelected))
                    {
                        Type = (ProximityMineType)style;
                    }
                }
                ImGui.EndCombo();
            }
        }

        public void Load(BinaryReader reader, int version)
        {
            Type = (ProximityMineType)reader.ReadInt32();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write((int)Type);
        }
    }

    public enum ProximityMineType
    {
        Orbs,
        Missiles
    }

    public class ProximityMine : Enemy
    {
        Texture2D texture;
        Texture2D overlay;

        enum ProximityMineState
        {
            Idle,
            Detonating
        }

        ProximityMineState state = ProximityMineState.Idle;
        public float t = 0f;

        ProximityMineType type = ProximityMineType.Orbs;

        public ProximityMine() : base(Vector2.Zero, 1)
        {
            texture = TextureManager.Get("Mines/ship_005Y");
            overlay = TextureManager.Get("Mines/MineOverlay");

            AddCircleCollider(Vector2.Zero, 32f);
        }

        public ProximityMine(Vector2 position, EntityCell cell) : base(position, 1)
        {
            AddCircleCollider(Vector2.Zero, 32f);
        }

        public override void ApplySpawnData(IEnemySpawnData spawnData)
        {
            type = ((ProximityMineSpawnData)spawnData).Type;

            switch (type)
            {
                case ProximityMineType.Orbs:
                    texture = TextureManager.Get("Mines/ship_005Y");
                    break;
                case ProximityMineType.Missiles:
                    texture = TextureManager.Get("Mines/ship_005R");
                    break;
                default:
                    break;
            }
        }

        public override void Destroyed()
        {
            if (!destroyed)
            {
                switch (type)
                {
                    case ProximityMineType.Orbs:
                        for (int i = 0; i < 360; i += 45)
                        {
                            SpawnProjectile(i);
                        }
                        break;
                    case ProximityMineType.Missiles:
                        for (int i = 0; i < 5; i++)
                        {
                            SpawnMissile();
                        }
                        break;
                    default:
                        break;
                }
            }

            base.Destroyed();
        }

        void SpawnProjectile(float angle)
        {
            Scene.World.AddProjectile(new CircleProjectile(Transform.LocalPosition, MathHelper.ToRadians(angle), 10f, 16f), true);
        }

        void SpawnMissile()
        {
            var missile = new ChallengerHomingMissile(Transform.Position, Scene.World.GetRandomPlayer(), MathHelper.ToRadians(Random.Next(0, 360)));
            missile.SetHomingMaxTime(7f);
            Scene.World.AddProjectile(missile, true);
        }

        public override void Update(GameTime gameTime)
        {
            if (PathManager != null)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
            }
            else
            {
                DefaultMove();

                if (Transform.Position.Y > Scene.World.Bounds.Bottom + texture.Height)
                {
                    Despawn();
                }
            }

            switch (state)
            {
                case ProximityMineState.Idle:

                    foreach (var item in Scene.World.GetPlayers())
                    {
                        if (Vector2.Distance(Transform.Position, item.Transform.Position) <= 128f)
                        {
                            state = ProximityMineState.Detonating;
                        }
                    }

                    break;
                case ProximityMineState.Detonating:

                    t += (float)gameTime.ElapsedGameTime.TotalSeconds * 2f;

                    if (t >= 1f)
                    {
                        Damage(100, false);
                    }

                    break;
                default:
                    break;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, Transform.Position, null, Color.White, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 0.5f, SpriteEffects.None, 0f);
            if (t != 0)
                Screen.spriteBatch.Draw(overlay, Transform.Position, null, new Color(Color.White.R, Color.White.G, Color.White.B, (byte)(t * 255)), 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 0.5f, SpriteEffects.None, 0f);
        }
    }
}
