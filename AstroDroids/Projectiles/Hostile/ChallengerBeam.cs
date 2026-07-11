using AstroDroids.Collisions;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstroDroids.Projectiles.Hostile
{
    internal class ChallengerBeam : Projectile
    {
        int timer = 0;

        public bool Locked { get; set; } = false;

        float length;
        public float Angle { get { return _angle; } set { _angle = value; col.PointB = GameHelper.OrbitPos(Vector2.Zero, _angle, length); } }
        float _angle;

        CapsuleCollider col;

        public ChallengerBeam(Vector2 position, float angle, float length) : base(position)
        {
            _angle = angle;
            this.length = length;

            col = AddCapsuleCollider(Vector2.Zero, GameHelper.OrbitPos(Vector2.Zero, angle, length), 15f);
        }

        public override void Update(GameTime gameTime)
        {
            timer += 1;

            if (Locked && timer >= 5)
                timer = 5;

            if (timer >= 21)
            {
                Despawn();
            }

            foreach (var item in Scene.World.GetPlayers())
            {
                if (item.Intersects(this))
                {
                    item.Damage(1, false);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            float halfThickness = 15f;

            Vector2 dir = GameHelper.DirFromAngle(Angle);
            Vector2 perp = new Vector2(-dir.Y, dir.X);
            Vector2 basePos = Transform.Position;
            Vector2 upperPos = basePos + perp * halfThickness;
            Vector2 lowerPos = basePos - perp * halfThickness;


            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle((int)basePos.X, (int)basePos.Y, (int)length, 32), null, new Color(255, 0, 0, 255), Angle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle((int)upperPos.X, (int)upperPos.Y, (int)length, 4), null, Color.Red, Angle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
            Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle((int)lowerPos.X, (int)lowerPos.Y, (int)length, 4), null, Color.Red, Angle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
        }
    }
}
