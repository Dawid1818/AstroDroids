using AstroDroids.Collisions;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace AstroDroids.Projectiles.Hostile
{
    internal class ChallengerBeam : Projectile
    {
        float timer = 0;

        public bool Locked { get; set; } = false;

        float length;
        public float Angle { get { return _angle; } set { _angle = value; col.PointB = GameHelper.OrbitPos(Vector2.Zero, _angle, length); } }
        float _angle;

        CapsuleCollider col;
        Texture2D texture;

        float beamSpeed = 200f;
        float textureOffset = 0f;

        public ChallengerBeam(Vector2 position, float angle, float length) : base(position)
        {
            Friendly = false;

            texture = TextureManager.Get("Projectiles/ChallengerBeam/ChallengerBeam");

            _angle = angle;
            this.length = length;

            col = AddCapsuleCollider(Vector2.Zero, GameHelper.OrbitPos(Vector2.Zero, angle, length), 15f);
        }

        public override void Update(GameTime gameTime)
        {
            timer += 1f * gameTime.GetElapsedSeconds() * 10f;

            if (Locked && timer >= 5)
                timer = 5;

            if (timer >= 10)
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

            textureOffset -= beamSpeed * gameTime.GetElapsedSeconds();
        }

        public override void Draw(GameTime gameTime)
        {
            Color beamColor = Color.Red;

            Rectangle sourceRectangle = new Rectangle((int)textureOffset, 0, (int)length, texture.Height);

            Vector2 origin = new Vector2(0f, texture.Height / 2f);

            float scale;
            if(timer <= 5)
            {
                scale = timer / 5f;
            }
            else
            {
                scale = -(timer - 10f) / 5f;
            }

            Screen.spriteBatch.Draw(texture, Transform.Position, sourceRectangle, new Color(beamColor.R, beamColor.G, beamColor.B, (byte)127), Angle, origin, new Vector2(1f, 1.4f * scale), SpriteEffects.None, 0f);
            Screen.spriteBatch.Draw(texture, Transform.Position, sourceRectangle, beamColor, Angle, origin, new Vector2(1f, scale), SpriteEffects.None, 0f);
        }
    }
}
