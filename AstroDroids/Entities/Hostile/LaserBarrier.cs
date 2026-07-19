using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Levels;
using AstroDroids.Managers;
using AstroDroids.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;

namespace AstroDroids.Entities.Hostile
{
    public class LaserBarrier : Enemy
    {
        public List<LaserBarrier> Connections { get; private set; } = new List<LaserBarrier>();
        Dictionary<LaserBarrier, LaserBarrierBeam> beams = new Dictionary<LaserBarrier, LaserBarrierBeam>();
        Texture2D blueTexture;
        Texture2D redTexture;
        Vector2 moveDir = Vector2.Zero;
        public LaserBarrierType Type { get; private set; } = LaserBarrierType.Normal;
        public int Id { get; private set; }

        float t = 0f;
        //bool becameActive = false;

        public LaserBarrier() : base(Vector2.Zero, 1)
        {
            //placeholder texture
            blueTexture = TextureManager.Get("Laser Barriers/accesory_002b");
            redTexture = TextureManager.Get("Laser Barriers/accesory_002r");

            AddCircleCollider(Vector2.Zero, 16f);
        }

        public LaserBarrier(Vector2 position, int id, int health, Vector2 moveDir, LaserBarrierType type) : base(position, 1)
        {
            Type = type;
            Id = id;
            blueTexture = TextureManager.Get("Laser Barriers/accesory_002b");
            redTexture = TextureManager.Get("Laser Barriers/accesory_002r");
            this.moveDir = moveDir;

            if (health >= 0)
                SetHealth(health);
            else
                CanBeDamaged = false;

            AddCircleCollider(Vector2.Zero, 16f);

            //if (Intersects(Scene.World.Bounds))
            //{
            //    becameActive = true;
            //}
        }

        public override void Spawned()
        {
            //foreach (var item in connections)
            //{
            //    LaserBarrierBeam beam = new LaserBarrierBeam(type, this, item, Transform.LocalPosition, (float)GameHelper.AngleBetween(Transform.LocalPosition, item.Transform.LocalPosition), Vector2.Distance(Transform.LocalPosition, item.Transform.LocalPosition), !CanBeDamaged);
            //    beams.Add(item, beam);
            //    Scene.World.AddProjectile(beam, false);
            //}
        }

        public override void Destroyed()
        {
            //if(!destroyed)
            //{
            //    foreach (var item in beams)
            //    {
            //        Scene.World.RemoveProjectile(item.Value);
            //    }

            //    beams.Clear();

            //    connections.Clear();
            //}

            base.Destroyed();
        }

        public void SetConnections(List<LaserBarrier> connections)
        {
            Connections = connections;
        }

        public void AddConnection(LaserBarrier barrier)
        {
            Connections.Add(barrier);
        }

        public override void Update(GameTime gameTime)
        {
            //if (!becameActive)
            //{
            //    if (Intersects(Scene.World.Bounds))
            //    {
            //        becameActive = true;
            //    }
            //    else
            //    {
            //        if (t >= 10f)
            //            Despawn();

            //        t += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //    }
            //}
            //if (!Intersects(Scene.World.Bounds) && becameActive)
            //{
            //    Despawn();
            //}

            if(!Intersects(Scene.World.Bounds))
            {
                if (t >= 10f)
                    Despawn();

                t += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (PathManager != null)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
            }
            else
            {
                Transform.Position = new Vector2(Transform.Position.X + moveDir.X, Transform.Position.Y + moveDir.Y);
            }

            //for (int i = connections.Count - 1; i >= 0; i--)
            //{
            //    if (connections[i] == null || connections[i].destroyed)
            //    {
            //        Scene.World.RemoveProjectile(beams[connections[i]]);

            //        beams.Remove(connections[i]);

            //        connections.RemoveAt(i);
            //    }
            //}
        }

        public bool IsPowered()
        {
            return IsPowered(new HashSet<LaserBarrier>());
        }

        private bool IsPowered(HashSet<LaserBarrier> visited)
        {
            if (!visited.Add(this))
                return false;

            if (Type == LaserBarrierType.Normal)
                return !destroyed;

            foreach (var barrier in Connections)
            {
                if (barrier.destroyed)
                    continue;

                if (barrier.IsPowered(visited))
                    return true;
            }

            return false;
        }

        public override void Draw(GameTime gameTime)
        {
            Texture2D texture = CanBeDamaged ? blueTexture : redTexture;

            Screen.spriteBatch.Draw(texture, Transform.Position, null, CanBeDamaged ? Color.White : Color.Red, 0f, new Vector2(texture.Width / 2, texture.Height / 2), 1.2f, SpriteEffects.None, 0f);
            Screen.spriteBatch.DrawCircle(Transform.Position, 4, 8, CanBeDamaged ? Color.Cyan : Color.Red, 4);
        }
    }
}
