using AstroDroids.Entities;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstroDroids.Projectiles.Hostile
{
    public class LaserBarrierBeam : Projectile
    {
        float angle;
        float length = 0;
        bool red = false;

        public LaserBarrierBeam(Transform collider, float width, float height, float angle, float length, bool red) : base(collider, width, height)
        {
            this.angle = angle;
            this.length = length;
            this.red = red;
            AddCapsuleCollider(Vector2.Zero, GameHelper.OrbitPos(Vector2.Zero, angle, length), 16f);
        }

        public override void Update(GameTime gameTime)
        {
            DefaultMove();

            foreach (var item in Scene.World.GetPlayers())
            {
                if (item.Intersects(this))
                {
                    item.Damage(100, false);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle((int)Transform.Position.X, (int)Transform.Position.Y, (int)length, 32), null, red ? Color.Red : Color.Blue, angle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
        }
    }
}
