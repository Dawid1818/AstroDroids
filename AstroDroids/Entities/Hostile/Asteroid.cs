using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.IO;
using Numeric = System.Numerics;

namespace AstroDroids.Entities.Hostile
{
    public class AsteroidSpawnData : IEnemySpawnData
    {
        public Vector2 InitialVelocity { get; set; }

        public void DrawEditor()
        {
            Numeric.Vector2 velocity = new Numeric.Vector2(InitialVelocity.X, InitialVelocity.Y);
            if (ImGui.InputFloat2("Initial Velocity", ref velocity))
            {
                InitialVelocity = new Vector2(velocity.X, velocity.Y);
            }
        }

        public void Load(BinaryReader reader, int version)
        {
            InitialVelocity = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(InitialVelocity.X);
            writer.Write(InitialVelocity.Y);
        }
    }
    public class Asteroid : Enemy
    {
        public float t = 0f;

        Texture2D texture;

        float angle = 3.14f;
        Vector2 velocity = Vector2.Zero;

        bool becameActive = false;

        bool wouldFollowPath = true;

        public Asteroid() : base(Vector2.Zero, 20)
        {
            IsNeutral = true;

            texture = TextureManager.Get("Asteroids/Asteroid 01 - Base");

            AddCircleCollider(Vector2.Zero, 16f);
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
            if (!Intersects(Scene.World.Bounds) && becameActive)
            {
                Despawn();
            }

            if (PathManager != null && wouldFollowPath)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
                angle = GameHelper.AngleFromDir(PathManager.Direction) + 1.571f;

                if (!PathManager.Active)
                {
                    Despawn();
                }
            }
            else
            {
                if (!FollowsCamera)
                {
                    Transform.Position += (velocity);
                }

                if (Transform.Position.Y > Scene.World.Bounds.Bottom + texture.Height)
                {
                    Despawn();
                }
            }

            foreach (var item in Scene.World.GetPlayers())
            {
                if (item.Intersects(this))
                {
                    item.Damage(50, false);
                    Damage(50, false);

                    return;
                }
            }

            foreach (var item in Scene.World.Enemies)
            {
                if (item.Intersects(this))
                {
                    item.Damage(50, false);
                    Damage(50, false);

                    return;
                }
            }

            angle += velocity.X * gameTime.GetElapsedSeconds();
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, position: new Vector2(Transform.Position.X, Transform.Position.Y), null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
        }

        public override void Push(Vector2 direction)
        {
            velocity += direction;

            wouldFollowPath = false;
        }

        public override void ApplySpawnData(IEnemySpawnData spawnData)
        {
            velocity = ((AsteroidSpawnData)spawnData).InitialVelocity;
        }
    }
}
