using AstroDroids.Drawables;
using AstroDroids.Entities;
using AstroDroids.Extensions;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstroDroids.Projectiles.Hostile
{
    public class ChallengerHomingMissile : Projectile
    {
        Texture2D sprite;

        private Vector2 velocity;
        private float angle;
        private float turnSpeed = 0.05f;
        private float speed = 6f;

        private Trail trail;

        private float homingTimer = 0;
        private float homingMaxTime = 20f;

        private Entity Target;

        public ChallengerHomingMissile(Vector2 position, Entity target, float initialAngle) : base(position)
        {
            Friendly = false;

            sprite = TextureManager.GetProjectile("BasicWeapon/02");

            Target = target;

            angle = initialAngle;
            velocity = GameHelper.DirFromAngle(angle) * speed;

            trail = new Trail(sprite);

            AddCircleCollider(Vector2.Zero, 10);
        }

        public override void Update(GameTime gameTime)
        {
            if (Target != null && !(homingTimer > homingMaxTime))
            {
                Vector2 targetPos = Target.Transform.Position;
                Vector2 toTarget = Vector2.Normalize(targetPos - Transform.Position);

                float targetAngle = GameHelper.AngleFromDir(toTarget);

                angle = MathHelperEx.LerpAngle(angle, targetAngle, turnSpeed);

                velocity = GameHelper.DirFromAngle(angle) * speed;
            }

            if (!(homingTimer > homingMaxTime))
            {
                homingTimer += 0.1f;
            }

            trail.Update(Transform.Position);

            if (homingTimer > homingMaxTime && !Colliders[0].Intersects(Scene.World.Bounds, Transform))
            {
                Despawn();
            }

            foreach (var item in Scene.World.GetPlayers())
            {
                if (item.Intersects(this))
                {
                    item.Damage(1, false);
                    Despawn();

                    return;
                }
            }

            Transform.Position += velocity;
        }

        public override void Draw(GameTime gameTime)
        {
            trail.Draw(angle);

            Screen.spriteBatch.Draw(sprite, Transform.Position, null, Color.White, angle, new Vector2(sprite.Width / 2, sprite.Height / 2), 1f, SpriteEffects.None, 0f);
        }
    }
}
