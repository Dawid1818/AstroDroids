using AstroDroids.Entities;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace AstroDroids.Projectiles.Hostile
{
    public class SpinLaserBeam : Projectile
    {
        float t = 0f;
        float angle;
        int state = 0;

        public SpinLaserBeam(Transform collider, float width, float height, float angle) : base(collider, width, height)
        {
            this.angle = angle;
            AddCapsuleCollider(Vector2.Zero, GameHelper.OrbitPos(Vector2.Zero, angle, 1000), 16f);
        }

        public override void Update(GameTime gameTime)
        {
            if(t >= 1f && state == 0)
            {
                state = 1;
                t = 1f;
            }else if(t <= 0 && state == 1)
                Despawn();

            foreach (var item in Scene.World.GetPlayers())
            {
                if (item.Intersects(this))
                {
                    item.Damage(100, false);
                }
            } 

            if (state == 0)
                t += (float)gameTime.ElapsedGameTime.TotalSeconds * 5f;
            else
                t -= (float)gameTime.ElapsedGameTime.TotalSeconds * 5f;
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle((int)Transform.Position.X, (int)Transform.Position.Y, 1000, (int)(state == 0 ? t * 32f : 32f)), null, new Color(255, 255, 255, state == 0 ? 255 : (int)(255f * t)), angle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
        }
    }
}
