using AstroDroids.Editors;
using AstroDroids.Entities.Friendly;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Input;
using AstroDroids.Managers;
using AstroDroids.Paths;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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

        float speed = 100f;
        float turnSpeed = 3f;

        Vector2 targetPos;

        bool followPlayer = true;

        public SnakeBossSegment(SnakeBoss boss, SnakeBossSegment parentSegment, int historyOffset) : base(Vector2.Zero, 10)
        {
            this.boss = boss;
            this.parentSegment = parentSegment;
            if (parentSegment == null)
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
            RMM.SetNewPath2(angle);

            //targetPos = new Vector2(AstroDroidsGame.rnd.Next(Scene.World.Bounds.Width), AstroDroidsGame.rnd.Next(Scene.World.Bounds.Height));
            targetPos = new Vector2(Transform.Position.X - 10, Transform.Position.Y + 30);
        }

        public override void Update(GameTime gameTime)
        {
            if (InputSystem.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.I))
            {
                followPlayer = !followPlayer;

                if (!followPlayer)
                {
                    RMM.UpdatePosition(Transform.Position);
                    RMM.SetNewPath2(angle);
                }
            }

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
                if (!followPlayer)
                {
                    RMM.Update(gameTime);
                    Transform.LocalPosition = RMM.Position;

                    if (!RMM.Active)
                    {
                        //RMM.SetNewPath2(GameHelper.AngleFromDir(RMM.GetPath().GetDirection(1.0f)));
                        RMM.SetNewPath2(angle);

                        //RMM.SetNewPath3(player.Transform.Position, angle);
                    }

                    angle = RMM.MovementAngle;
                }
                else
                {
                    Player player = Scene.World.GetRandomPlayer();
                    if (player != null)
                    {
                        Vector2 desiredDirection = (player.Transform.Position - Transform.Position);
                        desiredDirection.Normalize();

                        Vector2 lerp = Vector2.Lerp(GameHelper.DirFromAngle(angle), desiredDirection, turnSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        lerp.Normalize();
                        angle = GameHelper.AngleFromDir(lerp);

                        Transform.Translate(new Vector2(MathF.Cos(angle) * speed * (float)gameTime.ElapsedGameTime.TotalSeconds, MathF.Sin(angle) * speed * (float)gameTime.ElapsedGameTime.TotalSeconds));
                    }
                }

                boss.PositionHistory.Insert(0, Transform.Position);

                if (boss.PositionHistory.Count > 1000)
                    boss.PositionHistory.RemoveAt(boss.PositionHistory.Count - 1);

                //Transform.Translate(new Vector2(MathF.Cos(angle) * speed, MathF.Sin(angle) * speed));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, Transform.Position, null, CanBeDamaged ? Color.White : Color.Red, angle + MathHelper.ToRadians(90), new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, 0f);

            //Screen.spriteBatch.DrawCircle(targetPos.X, targetPos.Y, 12, 12, Color.White);

            if (parentSegment == null)
            {
                if(!followPlayer)
                PathVisualizer.DrawPath(RMM.GetPath());

                //walker.Draw(Screen.spriteBatch);
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
