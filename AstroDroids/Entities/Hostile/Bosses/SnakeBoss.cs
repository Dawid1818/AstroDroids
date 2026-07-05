using AstroDroids.Coroutines;
using AstroDroids.Entities.Effects;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Paths;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AstroDroids.Entities.Hostile.Bosses
{
    public enum SnakeBossMovementStyle
    {
        None,
        Path,
        TowardsPlayer
    }

    public enum SnakeBossAttackType
    {
        Wandering1,
        TargetingPlayer1,
        SnakePath,
        VerticalRightWall,
        VerticalLeftWall,
        SPath,
        SpiralPath
    }

    public class SnakeBoss : Enemy
    {
        public float t = 0f;

        List<SnakeBossAttackType> Attacks = new List<SnakeBossAttackType>();

        readonly List<SnakeBossAttackType> phase1Attacks =
        [
            SnakeBossAttackType.Wandering1,
            SnakeBossAttackType.TargetingPlayer1,
            SnakeBossAttackType.SnakePath,
            SnakeBossAttackType.VerticalRightWall,
            SnakeBossAttackType.VerticalLeftWall,
            SnakeBossAttackType.SPath,
            SnakeBossAttackType.SpiralPath
        ];

        List<SnakeBossSegment> segments = new List<SnakeBossSegment>();

        SnakeBossSegment head;

        CoroutineInstance bossBehavior;
        CoroutineInstance continousAttack;

        bool eliminated = false;

        public List<Vector2> PositionHistory { get; } = new();

        const int MaxHistory = 1000;

        Dictionary<string, IPath> paths = new Dictionary<string, IPath>();

        SnakeBossAttackType attackType = SnakeBossAttackType.Wandering1;

        bool defeated = false;

        bool continuosBreak = false;

        public SnakeBoss() : base(Vector2.Zero, 2)
        {

        }

        void LoadPath(string name)
        {
            paths.Add(name, GetPath(name).Path);
        }

        void StopContinousAttack()
        {
            if (continousAttack != null)
            {
                Scene.World.StopCoroutine(continousAttack);
                continousAttack = null;
            }
        }

        public override void Spawned()
        {
            LoadPath("SnakePath");
            LoadPath("SPath");
            LoadPath("SpiralPath");

            int segmentCount = 10;
            for (int i = 0; i < segmentCount; i++)
            {
                SnakeBossSegment segment;

                if (i == 0)
                {
                    segment = new SnakeBossSegment(this, null, i * 40);
                    head = segment;
                }
                else
                {
                    segment = new SnakeBossSegment(this, segments[i - 1], i * 40);
                }

                segment.Transform.Position = Transform.Position + new Vector2(-(i * 20), 0);
                segments.Add(segment);
            }

            for (int i = 0; i < MaxHistory; i++)
            {
                PositionHistory.Add(Transform.Position + new Vector2(-(i), 0));
            }

            for (int i = segmentCount - 1; i >= 0; i--)
            {
                Scene.World.AddEnemy(segments[i], true);
            }

            bossBehavior = Scene.World.StartCoroutine(BossBehavior());
        }

        bool started = false;

        IEnumerator BossBehavior()
        {
            continousAttack = Scene.World.StartCoroutine(SnakePathPreAttack());

            while (true)
            {
                if (started)
                {
                    if (Attacks.Count == 0)
                    {
                        List<SnakeBossAttackType> toUse;

                        toUse = phase1Attacks;

                        Attacks.AddRange(toUse);
                        Attacks.Shuffle(Random);
                    }

                    attackType = Attacks[0];
                    Attacks.RemoveAt(0);
                }
                else
                    started = true;

                if (continousAttack == null)
                {
                    continousAttack = Scene.World.StartCoroutine(SnakePathPreAttack());
                }

                switch (attackType)
                {
                    default:
                    case SnakeBossAttackType.Wandering1: //alright
                        StopContinousAttack();
                        yield return Wandering1Attack();
                        break;
                    case SnakeBossAttackType.TargetingPlayer1: //alright
                        StopContinousAttack();
                        yield return TargetingPlayer1Attack();
                        break;
                    case SnakeBossAttackType.SnakePath: //alright
                        yield return SnakePathAttack();
                        break;
                    case SnakeBossAttackType.VerticalRightWall:
                        yield return VerticalWallAttack(Scene.World.Bounds.Width - 64, false);
                        break;
                    case SnakeBossAttackType.VerticalLeftWall:
                        yield return VerticalWallAttack(64, true);
                        break;
                    case SnakeBossAttackType.SPath:
                        yield return SPathAttack();
                        break;
                    case SnakeBossAttackType.SpiralPath:
                        yield return SpiralPathAttack();
                        break;
                }

                continuosBreak = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!segments.Any(x => x.GetHealth() > 0) && !defeated)
            {
                defeated = true;
                if (bossBehavior != null)
                {
                    Scene.World.StopCoroutine(bossBehavior);
                }

                if (continousAttack != null)
                {
                    Scene.World.StopCoroutine(continousAttack);
                }

                Scene.World.StartCoroutine(DestroySequence());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.DrawText($"Boss Health: {segments.Sum(x => x.GetHealth())}/1000", new Vector2(20, 10), Color.White, 12f);
        }

        void ForEachSegment(Action<int, SnakeBossSegment> action)
        {
            for (int i = 0; i < segments.Count; i++)
            {
                SnakeBossSegment segment = segments[i];
                //if (segment.CanBeDamaged)
                action(i, segment);
            }
        }

        int pickRandomSegmentId()
        {
            return Random.Next(segments.Count);
        }

        IEnumerator DestroySequence()
        {
            foreach (var item in segments)
            {
                item.Despawn();
                Scene.World.AddEffect(new StandardExplosion(new Transform(item.Transform.Position.X, item.Transform.Position.Y), 1f));
                yield return new WaitForSeconds(0.3f);
            }

            Despawn();
        }

        #region Attacks
        IEnumerator TravelRandomly()
        {
            while (true)
            {
                head.GoRandom();
                yield return new WaitUntil(head.DestinationReached);
            }
        }

        IEnumerator TravelPath(IPath path, bool reverse)
        {
            head.GoTowards(path, reverse);

            yield return new WaitUntil(head.DestinationReached);

            head.GoFollowPath(path, reverse);

            yield return new WaitUntil(head.DestinationReached);
        }

        IEnumerator TravelToPath(IPath path, bool reverse)
        {
            head.GoTowards(path, reverse);

            yield return new WaitUntil(head.DestinationReached);
        }

        IEnumerator Wandering1Attack()
        {
            var traveler = Scene.World.StartCoroutine(TravelRandomly());

            head.AimAtPlayer();

            for (int i = 0; i < 2; i++)
            {
                head.Fire(3, 25);

                yield return new WaitForSeconds(1f);

                head.Fire(3, 25);

                yield return new WaitForSeconds(1f);

                head.Fire(5, 25);

                yield return new WaitForSeconds(1f);

                head.Fire(7, 25);

                yield return new WaitForSeconds(1f);

                head.Fire(7, 25);

                yield return new WaitForSeconds(1f);
            }

            Scene.World.StopCoroutine(traveler);
        }

        IEnumerator TargetingPlayer1Attack()
        {
            head.GoTargetPlayer();
            head.AimAtPlayer();

            for (int i = 0; i < 50; i++)
            {
                head.Fire(3, 15, 4f, 0, 0);
                yield return new WaitForSeconds(0.1f);
            }
        }

        IEnumerator SnakePathAttack()
        {
            int choice = Random.Next(2);

            yield return TravelToPath(paths["SnakePath"], choice == 0);

            head.GoFollowPath(paths["SnakePath"], choice == 0);

            yield return new WaitUntil(head.DestinationReached);
        }

        IEnumerator SnakePathPreAttack()
        {
            int attacksDone = 0;
            while (true)
            {
                if (continuosBreak)
                {
                    yield return new WaitForSeconds(3f);
                    continuosBreak = false;
                    attacksDone = 0;
                }

                int choice = Random.Next(7);

                switch (choice)
                {
                    default:
                    case 0: //all turrets at once, 1 projectile, direct
                        ForEachSegment((i, s) => s.AimAtPlayer());
                        yield return new WaitForSeconds(1);
                        for (int i = 0; i < 5; i++)
                        {
                            ForEachSegment((i, s) => s.Fire(1, 0));
                            yield return new WaitForSeconds(1);
                        }
                        break;
                    case 1: //one after another, direct
                        ForEachSegment((i, s) => s.AimAtPlayer());
                        yield return new WaitForSeconds(1);
                        switch (choice)
                        {
                            default:
                            case 0: //from first to last
                                for (int i = 0; i < segments.Count; i++)
                                {
                                    segments[i].Fire(1, 0);
                                    yield return new WaitForSeconds(0.2f);
                                }
                                break;
                            case 1: //from last to first
                                for (int i = segments.Count - 1; i >= 0; i--)
                                {
                                    segments[i].Fire(1, 0);
                                    yield return new WaitForSeconds(0.2f);
                                }
                                break;
                        }
                        break;
                    case 2: //fire to the sides
                        for (int i = 0; i < segments.Count; i++)
                        {
                            if (i == 0)
                                segments[i].AimRelatively(0);
                            else if (i == segments.Count - 1)
                                segments[i].AimRelatively(MathHelper.ToRadians(180));
                            else if (i % 2 == 0)
                                segments[i].AimRelatively(MathHelper.ToRadians(-90));
                            else
                                segments[i].AimRelatively(MathHelper.ToRadians(90));
                        }

                        yield return new WaitForSeconds(1);

                        for (int i = 0; i < 5; i++)
                        {
                            ForEachSegment((i, s) => s.Fire(2, 5));

                            yield return new WaitForSeconds(0.2);
                        }

                        break;
                    case 3: //fire at random angles
                        for (int i = 0; i < 3; i++)
                        {
                            ForEachSegment((i, s) => s.AimAtAngle(MathHelper.ToRadians(Random.Next(361))));

                            yield return new WaitForSeconds(1);

                            ForEachSegment((i, s) => s.Fire(2, 5));
                        }
                        break;
                    case 4:
                        {
                            int n = segments.Count;

                            int leftStart = (n - 1) / 2;
                            int rightStart = n / 2;

                            int steps = n / 2;

                            for (int i = 0; i <= steps; i++)
                            {
                                int left = leftStart - i;
                                int right = rightStart + i;

                                if (left >= 0)
                                {
                                    segments[left].AimAtPlayer(MathHelper.ToRadians(i * 5));
                                }

                                if (right < n && right != left)
                                {
                                    segments[right].AimAtPlayer(MathHelper.ToRadians(-(i * 5)));
                                }
                            }

                            yield return new WaitForSeconds(1);

                            for (int i = 0; i <= steps; i++)
                            {
                                int left = leftStart - i;
                                int right = rightStart + i;

                                if (left >= 0)
                                {
                                    segments[left].Fire(1, 0);
                                }

                                if (right < n && right != left)
                                {
                                    segments[right].Fire(1, 0);
                                }
                            }
                        }
                        break;
                    case 5:
                        {
                            float targetAngle = 0f;

                            for (; targetAngle <= 360; targetAngle += 25)
                            {
                                ForEachSegment((i, s) => s.AimAtAngle(MathHelper.ToRadians(targetAngle)));

                                yield return new WaitForSeconds(0.3f);

                                for (int i = 0; i < segments.Count; i++)
                                {
                                    if (i % 5 == 0)
                                        segments[i].Fire(2, 5, 5f);
                                }
                            }


                        }
                        break;
                    case 6:
                        for (int i = 0; i < segments.Count; i++)
                        {
                            if (i == 0)
                                segments[i].AimAtPlayer();
                            else if (i == segments.Count - 1)
                                segments[i].AimRelatively(MathHelper.ToRadians(180));
                            else if (i % 2 == 0)
                                segments[i].AimRelatively(MathHelper.ToRadians(-90));
                            else
                                segments[i].AimRelatively(MathHelper.ToRadians(90));
                        }

                        yield return new WaitForSeconds(1);

                        for (int i = 0; i < 5; i++)
                        {
                            ForEachSegment((i, s) =>
                            {
                                if (i == 0)
                                    s.Fire(shots: 3, 5, 5, 10, 10);
                                if (i == segments.Count - 1)
                                    s.Fire(shots: 3, 5, 5, 10, 10);
                                else
                                    s.Fire(shots: 1, 5, 5, 10, 10);
                            });

                            yield return new WaitForSeconds(0.6);
                        }
                        break;
                }

                attacksDone++;

                if (attacksDone == 5)
                    continuosBreak = true;
            }
        }

        IEnumerator SPathAttack()
        {
            int choice = Random.Next(2);

            yield return TravelToPath(paths["SPath"], choice == 0);

            head.GoFollowPath(paths["SPath"], choice == 0);

            yield return new WaitUntil(head.DestinationReached);
        }

        IEnumerator SpiralPathAttack()
        {
            int choice = Random.Next(2);

            yield return TravelToPath(paths["SpiralPath"], choice == 0);

            head.GoFollowPath(paths["SpiralPath"], choice == 0);

            yield return new WaitUntil(head.DestinationReached);
        }

        IEnumerator VerticalWallAttack(float xStart, bool leftFiringAngle)
        {
            head.GoTowards(new Vector2(xStart, 0), MathHelper.ToRadians(90));
            yield return new WaitUntil(head.DestinationReached);

            StopContinousAttack();
            ForEachSegment((i, s) => s.AimRelatively(MathHelper.ToRadians(leftFiringAngle ? -90 : 90)));

            head.GoTowards(new Vector2(xStart, Scene.World.Bounds.Height), MathHelper.ToRadians(90));
            yield return new WaitUntil(head.DestinationReached);

            for (int i = 0; i < 10; i++)
            {
                int chance = pickRandomSegmentId();

                var around = GameHelper.GetAroundMinMax(chance, 1, 0, segments.Count);

                for (int j = 0; j < segments.Count; j++)
                {
                    if (around.Contains(j))
                        continue;

                    segments[j].Fire(2, 5);
                }

                yield return new WaitForSeconds(1f);
            }
        }
        #endregion
    }
}
