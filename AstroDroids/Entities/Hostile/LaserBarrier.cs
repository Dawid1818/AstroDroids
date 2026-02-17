using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AstroDroids.Entities.Hostile
{
    public class LaserBarrier : Enemy
    {
        List<LaserBarrier> connections = new List<LaserBarrier>();
        Dictionary<LaserBarrier, LaserBarrierBeam> beams = new Dictionary<LaserBarrier, LaserBarrierBeam>();
        Texture2D texture;
        public int Id { get; private set; }

        public LaserBarrier() : base(new Transform(0, 0), 1, 32f, 32f)
        {
            //placeholder texture
            texture = TextureManager.Get("Ships/Basic/Basic");

            AddCircleCollider(Vector2.Zero, 16f);
        }

        public LaserBarrier(Vector2 position, int id, int health) : base(new Transform(position.X, position.Y), 1, 32f, 32f)
        {
            Id = id;
            texture = TextureManager.Get("Ships/Basic/Basic");

            if (health >= 0)
                SetHealth(health);
            else
                CanBeDamaged = false;

            AddCircleCollider(Vector2.Zero, 16f);
        }

        public override void Spawned()
        {
            foreach (var item in connections)
            {
                LaserBarrierBeam beam = new LaserBarrierBeam(new Transform(Transform.LocalPosition.X, Transform.LocalPosition.Y), 0, 0, (float)GameHelper.AngleBetween(Transform.LocalPosition, item.Transform.LocalPosition), Vector2.Distance(Transform.LocalPosition, item.Transform.LocalPosition), !CanBeDamaged);
                beams.Add(item, beam);
                Scene.World.AddProjectile(beam, false);
            }
        }

        public override void Destroyed()
        {
            if(!destroyed)
            {
                foreach (var item in beams)
                {
                    Scene.World.RemoveProjectile(item.Value);
                }

                beams.Clear();

                connections.Clear();
            }

            base.Destroyed();
        }

        public void SetConnections(List<LaserBarrier> connections)
        {
            this.connections = connections;
        }

        public override void Update(GameTime gameTime)
        {
            if (PathManager != null)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
            }

            for (int i = connections.Count - 1; i >= 0; i--)
            {
                if (connections[i] == null || connections[i].destroyed)
                {
                    Scene.World.RemoveProjectile(beams[connections[i]]);

                    beams.Remove(connections[i]);

                    connections.RemoveAt(i);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, ToRectangle(), null, CanBeDamaged ? Color.Blue : Color.Red, 0f, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0f);
        }
    }
}
