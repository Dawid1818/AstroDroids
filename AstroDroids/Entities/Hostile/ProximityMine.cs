using AstroDroids.Collisions;
using AstroDroids.Entities.Neutral;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Projectiles.Hostile;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.IO;

namespace AstroDroids.Entities.Hostile
{
    public class ProximityMineSpawnData : IEnemySpawnData
    {
        public ProximityMineType Type { get; set; } = ProximityMineType.Orbs;

        public float Speed { get; set; } = 3f;
        public float Angle { get; set; } = 90f;

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

            float speed = Speed;
            if(ImGui.InputFloat("Speed", ref speed))
            {
                Speed = speed;
            }

            float angle = Angle;
            if(ImGui.InputFloat("Angle", ref angle))
            {
                Angle = angle;
            }
        }

        public void Load(BinaryReader reader, int version)
        {
            Type = (ProximityMineType)reader.ReadInt32();
            Speed = reader.ReadSingle();
            Angle = reader.ReadSingle();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write((int)Type);
            writer.Write(Speed);
            writer.Write(Angle);
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

        public bool Move { get; set; } = true;

        enum ProximityMineState
        {
            Idle,
            Detonating
        }

        ProximityMineState state = ProximityMineState.Idle;
        public float t = 0f;
        public float expireTime = 0f;

        ProximityMineType type = ProximityMineType.Orbs;

        float angle;
        Vector2 movementDirection { get { return GameHelper.DirFromAngle(angle); } }
        float speed = 10f;

        bool becameActive = false;

        public float RevealProgress { get; private set; } = 0;

        CircleCollider col;

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
            ProximityMineSpawnData data = (ProximityMineSpawnData)spawnData;
            type = data.Type;

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

            speed = data.Speed;
            angle = MathHelper.ToRadians(data.Angle);
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
            if (!becameActive)
            {
                if (Intersects(Scene.World.Bounds))
                {
                    becameActive = true;
                }
                else
                {
                    if (expireTime >= 10f)
                        Despawn();

                    expireTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
            if (!Intersects(Scene.World.Bounds) && becameActive)
            {
                Despawn();
            }

            if (PathManager != null)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
            }
            else
            {
                if (Move)
                {
                    Transform.LocalPosition += movementDirection * speed;
                }
            }

            switch (state)
            {
                case ProximityMineState.Idle:

                    foreach (var item in Scene.World.GetPlayers())
                    {
                        if (RevealProgress == 0 && Vector2.Distance(Transform.Position, item.Transform.Position) <= 128f)
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

            if(RevealProgress > 0)
            {
                RevealProgress -= gameTime.GetElapsedSeconds() * 100;

                if (RevealProgress < 0)
                {
                    RevealProgress = 0;
                    AddCircleCollider(Vector2.Zero, 32f);
                }
            }
            else
            {
                RevealProgress = 0;
            }
        }

        public void RevealSlowly()
        {
            RevealProgress = texture.Height;
            ClearColliders();
        }

        public override void Draw(GameTime gameTime)
        {
            Rectangle source = new Rectangle(0, (int)RevealProgress, texture.Width, texture.Height - (int)RevealProgress);

            Screen.spriteBatch.Draw(texture, Transform.Position, source, Color.White, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 0.5f, SpriteEffects.None, 0f);
            if (t != 0)
                Screen.spriteBatch.Draw(overlay, Transform.Position, source, new Color(Color.White.R, Color.White.G, Color.White.B, (byte)(t * 255)), 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 0.5f, SpriteEffects.None, 0f);
        }
    }
}
