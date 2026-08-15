using AstroDroids.Coroutines;
using AstroDroids.Drawables;
using AstroDroids.Entities.Friendly;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Projectiles.Hostile;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AstroDroids.Entities.Hostile
{
    public enum EnemyLookStyle
    {
        Ahead,
        AtPlayer,
        Custom
    }

    public class GunnerSpawnData : IEnemySpawnData
    {
        public float LookAngle { get; set; }
        public bool DespawnAtPathEnd { get; set; } = true;
        public EnemyLookStyle LookStyle { get; set; } = EnemyLookStyle.Ahead;
        public float MoveSpeed { get; set; } = 2f;

        public void DrawEditor()
        {
            float lookangle = LookAngle;
            if (ImGui.InputFloat("Look Angle", ref lookangle))
            {
                LookAngle = lookangle;
            }

            float moveSpeed = MoveSpeed;
            if(ImGui.InputFloat("Move Speed", ref moveSpeed))
            {
                MoveSpeed = moveSpeed;
            }

            if (ImGui.BeginCombo("Look Style", LookStyle.ToString()))
            {
                foreach (var style in Enum.GetValues(typeof(EnemyLookStyle)))
                {
                    bool isSelected = (EnemyLookStyle)style == LookStyle;
                    if (ImGui.Selectable(style.ToString(), isSelected))
                    {
                        LookStyle = (EnemyLookStyle)style;
                    }
                }
                ImGui.EndCombo();
            }

            bool despawn = DespawnAtPathEnd;
            if (ImGui.Checkbox("Despawn at path end", ref despawn))
            {
                DespawnAtPathEnd = despawn;
            }

            //bool facePlayer = FacePlayerDuringPath;
            //if(ImGui.Checkbox("Face player during path", ref facePlayer))
            //{
            //    FacePlayerDuringPath = facePlayer;
            //}
        }

        public void Load(BinaryReader reader, int version)
        {
            if (version >= 6)
            {
                LookStyle = (EnemyLookStyle)reader.ReadInt32();
                LookAngle = reader.ReadSingle();
                DespawnAtPathEnd = reader.ReadBoolean();

                if (version >= 9)
                {
                    MoveSpeed = reader.ReadSingle();
                }
                else
                {
                    MoveSpeed = 2f;
                }
            }
            else
            {
                bool FacePlayerDuringPath = reader.ReadBoolean();

                if (FacePlayerDuringPath)
                    LookStyle = EnemyLookStyle.AtPlayer;
                else
                    LookStyle = EnemyLookStyle.Ahead;

                LookAngle = 0f;
                DespawnAtPathEnd = true;

                MoveSpeed = 2f;
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write((int)LookStyle);
            writer.Write(LookAngle);
            writer.Write(DespawnAtPathEnd);
            writer.Write(MoveSpeed);
        }
    }
    public class Gunner : Enemy
    {
        public Texture2D texture;

        AnimatedSprite sprite;

        float angle = 3.14f;

        int shotsToFire = 3;

        float timer = 0;

        bool firing = false;
        CoroutineInstance fireLoop;

        //bool facePlayer = false;
        EnemyLookStyle lookStyle = EnemyLookStyle.Ahead;
        float lookAngle = 0;
        bool despawnAtPathEnd = true;

        float moveSpeed = 2f;

        public Gunner() : base(Vector2.Zero, 10)
        {
            CanBeShielded = true;
            texture = TextureManager.Get("Ships/Gunner/tinyShip20");
            AddCircleCollider(Vector2.Zero, 22f);

            sprite = new AnimatedSprite(texture, 5, 44, 44, 1, 5, 10f);
        }

        public override void Destroyed()
        {
            base.Destroyed();

            if (fireLoop != null)
            {
                Scene.World.StopCoroutine(fireLoop);
                fireLoop = null;
            }
        }

        public override void ApplySpawnData(IEnemySpawnData spawnData)
        {
            GunnerSpawnData data = (GunnerSpawnData)spawnData;
            lookStyle = data.LookStyle;
            lookAngle = MathHelper.ToRadians(data.LookAngle);
            despawnAtPathEnd = data.DespawnAtPathEnd;
            moveSpeed = data.MoveSpeed;
        }

        public override void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);

            Player player = Scene.World.GetRandomPlayer();

            if (player != null && lookStyle == EnemyLookStyle.AtPlayer)
            {
                angle = GameHelper.AngleBetween(Transform.Position, player.GetPosition()) + 1.571f;
            }

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= 2f && !firing)
            {
                timer = 0f;
                firing = true;
                fireLoop = Scene.World.StartCoroutine(FireSequence());
            }

            if (PathManager != null)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;

                switch (lookStyle)
                {
                    default:
                    case EnemyLookStyle.Ahead:
                        angle = GameHelper.AngleFromDir(PathManager.Direction) + 1.571f;
                        break;
                    case EnemyLookStyle.AtPlayer:
                        break;
                    case EnemyLookStyle.Custom:
                        angle = lookAngle;
                        break;
                }


                if (despawnAtPathEnd && !PathManager.Active)
                {
                    Despawn();
                }
            }
            else
            {
                if (!FollowsCamera)
                {
                    Transform.Position = new Vector2(Transform.Position.X, Transform.Position.Y + moveSpeed);
                    //DefaultMove();
                }

                if (Transform.Position.Y > Scene.World.Bounds.Bottom + texture.Height)
                {
                    Despawn();
                }
            }
        }

        IEnumerator FireSequence()
        {
            List<float> angles = null;
            int pattern = Random.Next(3);
            switch (pattern)
            {
                case 0:
                    for (int i = 0; i < shotsToFire; i++)
                    {
                        if (i != 0)
                            yield return new WaitForSeconds(0.1f);
                        Shoot(angle);
                    }
                    break;
                case 1:
                    angles = GameHelper.SpreadAngle(angle, shotsToFire, 25);
                    foreach (var item in angles)
                    {
                        Shoot(item);
                    }
                    break;
                case 2:
                    for (int i = 0; i < 3; i++)
                    {
                        if (i != 0)
                            yield return new WaitForSeconds(0.1f);
                        Shoot(angle);
                    }

                    yield return new WaitForSeconds(0.2f);

                    angles = GameHelper.SpreadAngle(angle, shotsToFire - 1, 25);
                    foreach (var item in angles)
                    {
                        Shoot(item);
                    }

                    break;
            }

            firing = false;
        }


        void Shoot(float angle)
        {
            Scene.World.AddProjectile(new CircleProjectile(GameHelper.OrbitPos(Transform.Position, angle - 1.571f, 20), angle - 1.571f, 5f, 12f), true);
        }

        public override void Draw(GameTime gameTime)
        {
            sprite.Draw(new Vector2(Transform.Position.X, Transform.Position.Y), angle, 1f);
        }
    }
}
