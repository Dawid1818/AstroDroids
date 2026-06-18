using AstroDroids.Drawables;
using AstroDroids.Editors;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Input;
using AstroDroids.Managers;
using AstroDroids.Paths;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.IO;

namespace AstroDroids.Entities.Hostile.Bosses
{
    public class SnakeBossSegment : Enemy
    {
        SnakeBoss boss;
        SnakeBossSegment parentSegment;
        Texture2D texture;

        public float angle { get; private set; }

        int historyOffset;

        RandomMoveManager RMM;

        float speed = 2f;
        public SnakeBossSegment(SnakeBoss boss, SnakeBossSegment parentSegment, int historyOffset) : base(Vector2.Zero, 10)
        {
            this.boss = boss;
            this.parentSegment = parentSegment;
            if(parentSegment == null)
                texture = TextureManager.Get("Ships/SnakeBoss/SnakeBoss");
            else
                texture = TextureManager.Get("Ships/SnakeBoss/SnakeBossNocockpit");
            AddCircleCollider(Vector2.Zero, 42);
            this.historyOffset = historyOffset;
        }

        public override void Spawned()
        {
            RMM = new RandomMoveManager(Transform.LocalPosition);
            RMM.maxMoveDistance = 1000;
            RMM.SetNewPath(angle);
        }

        public override void Update(GameTime gameTime)
        {
            if (parentSegment != null)
            {
                if (boss.PositionHistory.Count > historyOffset + 1)
                {
                    Transform.Position = boss.PositionHistory[historyOffset];

                    Vector2 current = boss.PositionHistory[historyOffset];

                    Vector2 next = boss.PositionHistory[historyOffset + 1];

                    Vector2 dir = current - next;

                    if (dir.LengthSquared() > 0.001f)
                        angle = MathF.Atan2(dir.Y, dir.X);
                }
            }
            else
            {
                RMM.Update(gameTime);
                Transform.LocalPosition = RMM.Position;

                if (!RMM.Active)
                {
                    RMM.SetNewPath(angle);
                }

                angle = RMM.MovementAngle;

                boss.PositionHistory.Insert(0, Transform.Position);

                if (boss.PositionHistory.Count > 1000)
                    boss.PositionHistory.RemoveAt(boss.PositionHistory.Count - 1);

                //Transform.Translate(new Vector2(MathF.Cos(angle) * speed, MathF.Sin(angle) * speed));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, Transform.Position, null, CanBeDamaged ? Color.White : Color.Red, angle + MathHelper.ToRadians(90), new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, 0f);

            if(parentSegment == null)
            {
                IPath Path = RMM.GetPath();

                float t = 0f;
                PathPoint lastPos = Path.GetPoint(t);
                while (t < 1f)
                {
                    t += 0.001f;
                    PathPoint nextPos = Path.GetPoint(t);
                    Screen.spriteBatch.DrawLine(lastPos, nextPos, Color.Green, 4f);
                    lastPos = nextPos;
                }
            }
        }

        public override void Destroyed()
        {
            CanBeDamaged = false;
            SetHealth(0);

            //can change sprite to damaged state

            //original behavior
            //if (destroyed) return;

            //Scene.World.AddEffect(new StandardExplosion(new Transform(Transform.Position.X, Transform.Position.Y), 0.6f));

            //GameState.AddScore(Score);
            //Despawn();

            //destroyed = true;
        }
    }
}
