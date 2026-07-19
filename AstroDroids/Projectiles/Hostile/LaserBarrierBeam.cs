using AstroDroids.Collisions;
using AstroDroids.Entities;
using AstroDroids.Entities.Hostile;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Levels;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace AstroDroids.Projectiles.Hostile
{
    public class LaserBarrierBeam : Projectile
    {
        float angle;
        float length = 0;
        bool red = false;
        public LaserBarrier Owner { get; private set; }
        public LaserBarrier Target { get; private set; }
        CapsuleCollider col;
        LaserBarrierType type;

        Texture2D texture;
        float beamSpeed = 200f;
        float textureOffset = 0f;

        public LaserBarrierBeam(LaserBarrierType type, LaserBarrier owner, LaserBarrier target, Vector2 position, bool red, bool blocksPlayerProjectiles) : base(position)
        {
            Friendly = false;

            this.type = type;
            this.red = red;
            BlocksPlayerProjectiles = blocksPlayerProjectiles;
            Owner = owner;
            Target = target;

            texture = TextureManager.Get("Laser Barriers/Beam2");

            if (owner != null && target != null)
            {
                angle = GameHelper.AngleBetween(Owner.Transform.LocalPosition, Target.Transform.LocalPosition);
                length = Vector2.Distance(Owner.Transform.LocalPosition, Target.Transform.LocalPosition);

                col = AddCapsuleCollider(Vector2.Zero, GameHelper.OrbitPos(Vector2.Zero, angle, length), 16f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Owner == null)
                DefaultMove();
            else
                Transform.Position = Owner.Transform.Position;

            if (CheckIfValid())
            {
                angle = GameHelper.AngleBetween(Owner.Transform.LocalPosition, Target.Transform.LocalPosition);
                length = Vector2.Distance(Owner.Transform.LocalPosition, Target.Transform.LocalPosition);

                if (col != null)
                    col.PointB = GameHelper.OrbitPos(Vector2.Zero, angle, length);
            }
            else
            {
                Despawn();
            }

            foreach (var item in Scene.World.GetPlayers())
            {
                if (item.Intersects(this))
                {
                    item.Damage(100, false);
                }
            }

            textureOffset -= beamSpeed * gameTime.GetElapsedSeconds();
        }

        bool CheckIfValid()
        {
            if (Owner == null || Target == null)
                return false;

            if (Owner.destroyed || Target.destroyed)
                return false;

            if (type == LaserBarrierType.Normal)
                return true;

            return Owner.IsPowered();
        }

        public override void Draw(GameTime gameTime)
        {
            bool red = false;

            if ((Target != null && Target.CanBeDamaged) || Owner != null && Owner.CanBeDamaged)
                red = false;
            else
                red = this.red;

            Color beamColor;

            if (BlocksPlayerProjectiles)
                beamColor = new Color(Color.Orange.R, Color.Orange.G, Color.Orange.B, (byte)255);
            else
                beamColor = red ? new Color(Color.Red.R, Color.Red.G, Color.Red.B, (byte)255) : new Color(Color.Cyan.R, Color.Cyan.G, Color.Cyan.B, (byte)255);

            Rectangle sourceRectangle = new Rectangle((int)textureOffset, 0, (int)length, texture.Height);

            Vector2 origin = new Vector2(0f, texture.Height / 2f);

            Screen.spriteBatch.Draw(texture, Transform.Position, sourceRectangle, new Color(beamColor.R, beamColor.G, beamColor.B, (byte)127), angle, origin, new Vector2(1f, 1.4f), SpriteEffects.None, 0f);
            Screen.spriteBatch.Draw(texture, Transform.Position, sourceRectangle, beamColor, angle, origin, 1.0f, SpriteEffects.None, 0f);
            //Screen.spriteBatch.Draw(TextureManager.GetPixelTexture(), new Rectangle((int)Transform.Position.X, (int)Transform.Position.Y, (int)length, 32), null, red ? new Color(Color.Red.R, Color.Red.G, Color.Red.B, (byte)127) : new Color(Color.Blue.R, Color.Blue.G, Color.Blue.B, (byte)127), angle, new Vector2(0f, 0.5f), SpriteEffects.None, 0f);
        }
    }
}
