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
    public class GunnerSpawnData : IEnemySpawnData
    {
        public bool FacePlayerDuringPath = false;

        public void DrawEditor()
        {
            ImGui.Checkbox("Face player during path", ref FacePlayerDuringPath);
        }

        public void Load(BinaryReader reader, int version)
        {
            FacePlayerDuringPath = reader.ReadBoolean();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(FacePlayerDuringPath);   
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

        bool facePlayer = false;

        public Gunner() : base(Vector2.Zero, 10)
        {
            texture = TextureManager.Get("Ships/Gunner/tinyShip20");
            AddCircleCollider(Vector2.Zero, 22f);

            sprite = new AnimatedSprite(texture, 5, 44, 44, 1, 5, 10f);
        }

        public override void ApplySpawnData(IEnemySpawnData spawnData)
        {
            facePlayer = ((GunnerSpawnData)spawnData).FacePlayerDuringPath;
        }

        public override void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);

            Player player = Scene.World.GetRandomPlayer();

            if (player != null)
            {
                angle = GameHelper.AngleBetween(Transform.Position, player.GetPosition()) + 1.571f;
            }

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(timer >= 2f && !firing)
            {
                timer = 0f;
                firing = true;
                Scene.World.StartCoroutine(FireSequence());
            }

            if (PathManager != null)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;

                if(!facePlayer)
                    angle = GameHelper.AngleFromDir(PathManager.Direction) + 1.571f;


                //if (!PathManager.Active)
                //{
                //    Despawn();
                //} 
            }
            else
            {
                if (!FollowsCamera)
                    DefaultMove();

                if (Transform.Position.Y > Scene.World.Bounds.Bottom + texture.Height)
                {
                    Despawn();
                }
            }
        }

        IEnumerator FireSequence()
        {
            List<float> angles = null;
            int pattern = AstroDroidsGame.rnd.Next(3);  
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
