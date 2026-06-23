using AstroDroids.Coroutines;
using AstroDroids.Entities.Friendly;
using AstroDroids.Extensions;
using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Input;
using AstroDroids.Interfaces;
using AstroDroids.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AstroDroids.Entities.Hostile.Bosses
{
    enum DronePositionStyle
    {
        Orbit,
        HorizontalWall,
        VerticalWall,
        Random,
        OrbitPlayer
    }

    public class DroneEntry
    {
        public int ID { get; set; }
        public ProjectileDrone Drone { get; set; }

        public Vector2 RandomDestination { get; set; }

        public bool Reached { get; set; }
        public bool Acted { get; set; }

        public DroneEntry(int id, ProjectileDrone drone)
        {
            ID = id;
            Drone = drone;
        }
    }

    public class DroneBoss : Enemy, IDroneController
    {
        public float t = 0f;

        Texture2D texture;

        float angle = MathHelper.ToRadians(90);

        DronePositionStyle style = DronePositionStyle.Orbit;

        List<DroneEntry> drones = new List<DroneEntry>();

        float orbitAngle = 0f;

        float orbitDistanceX = 100f;
        float orbitDistanceY = 50f;

        float deltaTime;

        float orbitRotation = 0f;

        int maxDroneAmount = 12;

        int angleOverrideMode = 0;

        bool increaseOrbitAngle = true;
        float orbitAngleMultiplier = 1f;

        readonly List<DronePositionStyle> phase1Attacks =
        [
            DronePositionStyle.Orbit,
            DronePositionStyle.HorizontalWall,
            DronePositionStyle.VerticalWall,
            DronePositionStyle.Random,
            DronePositionStyle.OrbitPlayer
        ];

        readonly List<DronePositionStyle> phase2Attacks =
        [
            DronePositionStyle.Orbit,
            DronePositionStyle.VerticalWall,
            DronePositionStyle.Random,
            DronePositionStyle.OrbitPlayer
        ];

        List<DronePositionStyle> Attacks = new List<DronePositionStyle>();

        public DroneBoss() : base(Vector2.Zero, 1000)
        {
            texture = TextureManager.Get("Ships/DroneBoss/base");

            AddCircleCollider(Vector2.Zero, 48f);
        }

        public override void Spawned()
        {
            for (int i = 0; i < maxDroneAmount; i++)
            {
                ProjectileDrone drone = createDrone();
                Scene.World.AddEnemy(drone, true);
                drones.Add(new DroneEntry(i, drone));
            }

            Scene.World.StartCoroutine(BossBehavior());
        }

        ProjectileDrone createDrone()
        {
            ProjectileDrone drone = new ProjectileDrone(this);
            drone.Transform.Position = Transform.Position;
            return drone;
        }

        public override void Destroyed()
        {
            base.Destroyed();

            foreach (var item in drones)
            {
                item.Drone.Damage(item.Drone.GetHealth(), false);
            }
        }

        void SetStyle(DronePositionStyle newStyle)
        {
            bool wasOrbitPlayer = style == DronePositionStyle.OrbitPlayer;

            DronePositionStyle oldStyle = style;
            style = newStyle;

            increaseOrbitAngle = true;
            orbitAngleMultiplier = 1f;

            angleOverrideMode = 0;

            foreach (var drone in drones)
            {
                drone.Drone.SetCollidable(!wasOrbitPlayer);
                drone.Reached = false;
                drone.Acted = false;
                drone.Drone.angleOverride = false;
            }

            switch (style)
            {
                case DronePositionStyle.Orbit:
                    orbitDistanceX = 100f;
                    orbitDistanceY = 50f;
                    break;
                case DronePositionStyle.Random:
                    Rectangle newBounds = Scene.World.Bounds;
                    newBounds.Inflate(-32f, -32f);

                    foreach (var drone in drones)
                    {
                        drone.RandomDestination = GameHelper.RandomPosition(newBounds);
                    }
                    break;
                case DronePositionStyle.VerticalWall:

                    if (oldStyle != DronePositionStyle.VerticalWall)
                    {
                        foreach (var drone in drones)
                        {
                            drone.Drone.SetCollidable(false);
                        }
                    }

                    float droneHeight = 10.5f;

                    float totalDroneHeight = droneHeight * drones.Count;
                    float gap = (Scene.World.Bounds.Height - totalDroneHeight) / (drones.Count + 1);

                    float y = gap;

                    bool direct = Random.Next(2) == 1;

                    int total = drones.Count;
                    int leftCount = total / 2;
                    int rightCount = total - leftCount;

                    List<int> dirs = new List<int>();

                    for (int i = 0; i < leftCount; i++)
                        dirs.Add(0);

                    for (int i = 0; i < rightCount; i++)
                        dirs.Add(1);

                    dirs.Shuffle(Random);

                    for (int i = 0; i < drones.Count; i++)
                    {
                        DroneEntry droneEntry = drones[i];
                        int dir = dirs[i];

                        Vector2 desiredPos = new Vector2(dir == 0 ? 15 : Scene.World.Bounds.Width - 15, y + droneHeight * 0.5f);

                        droneEntry.RandomDestination = desiredPos;

                        if (direct)
                        {
                            droneEntry.Drone.angleOverride = false;
                        }
                        else
                        {
                            droneEntry.Drone.angleOverride = true;
                            droneEntry.Drone.overridedAngle = MathHelper.ToRadians(dir == 0 ? 0 : 180);
                        }

                        y += droneHeight + gap;
                    }
                    break;
                case DronePositionStyle.OrbitPlayer:
                    orbitDistanceX = 300f;
                    orbitDistanceY = 300f;

                    foreach (var drone in drones)
                    {
                        drone.Drone.SetCollidable(false);
                    }
                    break;
                default:
                    break;
            }
        }

        void RespawnDrone(int index)
        {
            if (destroyed)
                return;

            DroneEntry droneEntry = drones[index];

            droneEntry.Drone.SetHealth(20);
            droneEntry.Drone.Transform.Position = Transform.Position;
            Scene.World.AddEnemy(droneEntry.Drone, true);

            if (style == DronePositionStyle.VerticalWall)
            {
                int dir = Random.Next(2);

                Vector2 desiredPos = new Vector2(dir == 0 ? 15 : Scene.World.Bounds.Width - 15, Random.Next(Scene.World.Bounds.Height));

                droneEntry.RandomDestination = desiredPos;
            }
            else if (style == DronePositionStyle.Random)
            {
                Rectangle newBounds = Scene.World.Bounds;
                newBounds.Inflate(-32f, -32f);

                droneEntry.RandomDestination = GameHelper.RandomPosition(newBounds);
            }
            else if (style == DronePositionStyle.OrbitPlayer)
            {
                droneEntry.Drone.SetCollidable(false);
            }
        }

        bool started = false;

        bool phase2Intro = false;
        bool phase2 = false;

        IEnumerator BossBehavior()
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);

                if (started)
                {
                    if (!phase2 || phase2 && phase2Intro)
                    {
                        if (Attacks.Count == 0)
                        {
                            List<DronePositionStyle> toUse;

                            if (phase2)
                                toUse = phase2Attacks;
                            else
                                toUse = phase1Attacks;

                            Attacks.AddRange(toUse);
                            Attacks.Shuffle(Random);

                            for (int i = 0; i < Attacks.Count - 1; i++)
                            {
                                DronePositionStyle current = Attacks[i];
                                DronePositionStyle ahead = Attacks[i+1];

                                if(current == DronePositionStyle.OrbitPlayer && ahead == DronePositionStyle.Random)
                                {
                                    Attacks[i] = DronePositionStyle.Random;
                                    Attacks[i + 1] = DronePositionStyle.OrbitPlayer;
                                    break;
                                }
                            }
                        }

                        SetStyle(Attacks[0]);
                        Attacks.RemoveAt(0);
                    }
                }

                started = true;

                if (phase2 && !phase2Intro)
                {
                    Attacks.Clear();

                    yield return Phase2IntroAttack();

                    phase2Intro = true;
                }
                else
                {

                    switch (style)
                    {
                        case DronePositionStyle.Orbit:
                            yield return OrbitAttack();
                            break;
                        case DronePositionStyle.HorizontalWall:
                            yield return HorizontalWallAttack();
                            break;
                        case DronePositionStyle.Random:
                            yield return RandomAttack();
                            break;
                        case DronePositionStyle.OrbitPlayer:
                            yield return OrbitPlayerAttack();
                            break;
                        case DronePositionStyle.VerticalWall:
                            yield return VerticalWallAttack();
                            break;
                        default:
                            break;
                    }
                }


                for (int i = 0; i < drones.Count; i++)
                {
                    DroneEntry item = drones[i];
                    if (item.Drone.destroyed)
                    {
                        RespawnDrone(i);
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Player player = Scene.World.GetRandomPlayer();

            if (player != null)
                angle = MathHelperEx.LerpAngle(angle, GameHelper.AngleBetween(Transform.Position, player.GetPosition()), 5f * gameTime.GetElapsedSeconds());

            if (InputSystem.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.I))
            {
                SetStyle(style.Next());
            }

            float dist;

            if (GetHealth() < 500)
                phase2 = true;

            if (PathManager != null)
            {
                PathManager.Update(gameTime);
                Transform.Position = PathManager.Position;
                angle = GameHelper.AngleFromDir(PathManager.Direction) + 1.571f;

                if (!PathManager.Active)
                {
                    Despawn();
                }
            }

            if (phase2)
                MoveTowards(this, Scene.World.Bounds.Center.ToVector2(), true, 0.6f);

            if (drones.Count > 0)
            {
                switch (style)
                {
                    case DronePositionStyle.HorizontalWall:

                        float droneWidth = drones[0].Drone.Width;

                        float totalDroneWidth = droneWidth * drones.Count;
                        float gap = (Scene.World.Bounds.Width - totalDroneWidth) / (drones.Count + 1);

                        float x = gap;

                        foreach (DroneEntry droneEntry in drones)
                        {
                            Vector2 desiredPos = new Vector2(x + droneWidth * 0.5f, 150);

                            if (MoveTowards(droneEntry.Drone, desiredPos, false, 1f))
                            {
                                droneEntry.Reached = true;
                                droneEntry.Drone.SetCollidable(true);
                            }

                            x += droneWidth + gap;
                        }

                        break;
                    case DronePositionStyle.Orbit:
                        {
                            dist = MathHelper.ToRadians(360 / drones.Count);

                            float ellipseRotation = orbitRotation;

                            orbitRotation += deltaTime * 0.5f;

                            if (orbitRotation > MathHelper.TwoPi)
                            {
                                orbitRotation -= MathHelper.TwoPi;
                            }

                            foreach (DroneEntry droneEntry in drones)
                            {
                                float currentAngle = orbitAngle + (dist * droneEntry.ID);

                                Vector2 desiredPos = GameHelper.OrbitEllipsePos(Transform.LocalPosition, currentAngle, orbitDistanceX, orbitDistanceY, ellipseRotation);

                                if (MoveTowards(droneEntry.Drone, desiredPos, false, 1f))
                                {
                                    droneEntry.Reached = true;
                                    droneEntry.Drone.SetCollidable(true);
                                }

                                if (angleOverrideMode != 0)
                                {
                                    droneEntry.Drone.angleOverride = true;

                                    if (angleOverrideMode == 1)
                                        droneEntry.Drone.overridedAngle = GameHelper.AngleBetween(Transform.Position, droneEntry.Drone.Transform.Position);
                                    else if (angleOverrideMode == 2 && player != null)
                                        droneEntry.Drone.overridedAngle = GameHelper.AngleBetween(Transform.Position, player.Transform.Position);
                                }
                                else
                                {
                                    droneEntry.Drone.angleOverride = false;
                                }
                            }

                            IncreaseOrbit();
                        }

                        break;
                    case DronePositionStyle.OrbitPlayer:
                        {
                            if (player != null)
                            {
                                dist = MathHelper.ToRadians(360 / drones.Count);

                                float ellipseRotation = MathHelper.ToRadians(0);

                                foreach (DroneEntry droneEntry in drones)
                                {
                                    float currentAngle = orbitAngle + (dist * droneEntry.ID);

                                    Vector2 desiredPos = GameHelper.OrbitEllipsePos(player.Transform.LocalPosition, currentAngle, orbitDistanceX, orbitDistanceY, ellipseRotation);

                                    if (MoveTowards(droneEntry.Drone, desiredPos, false, 1f, 100f))
                                    {
                                        droneEntry.Drone.SetCollidable(false);
                                        droneEntry.Reached = true;
                                    }
                                }
                            }

                            IncreaseOrbit();
                        }

                        break;
                    case DronePositionStyle.VerticalWall:
                    case DronePositionStyle.Random:
                        foreach (DroneEntry droneEntry in drones)
                        {
                            if (MoveTowards(droneEntry.Drone, droneEntry.RandomDestination, true, style == DronePositionStyle.Random ? 1f : 2f))
                            {
                                droneEntry.Reached = true;
                                droneEntry.Drone.SetCollidable(true);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        void IncreaseOrbit()
        {
            if (!increaseOrbitAngle)
                return;

            orbitAngle += deltaTime * 0.5f * orbitAngleMultiplier;

            if (orbitAngle > MathHelper.TwoPi)
            {
                orbitAngle -= MathHelper.TwoPi;
            }
        }


        bool MoveTowards(Entity drone, Vector2 position, bool constantSpeed, float speedMultiplier, float expectedDistance = 60f)
        {
            const float speed = 200f;

            if (constantSpeed)
            {
                Vector2 current = drone.Transform.LocalPosition;
                Vector2 direction = position - current;

                float distance = direction.Length();

                if (distance <= speed * speedMultiplier * deltaTime)
                {
                    drone.Transform.LocalPosition = position;
                    return true;
                }
                else
                {
                    drone.Transform.LocalPosition = current + direction / distance * speed * speedMultiplier * deltaTime;
                    return false;
                }
            }
            else
            {
                drone.Transform.LocalPosition = Vector2.Lerp(drone.Transform.LocalPosition, position, 1.5f * speedMultiplier * deltaTime);
                return Vector2.Distance(drone.Transform.LocalPosition, position) < expectedDistance;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.spriteBatch.Draw(texture, new Vector2(Transform.Position.X, Transform.Position.Y), null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);

            if (AstroDroidsGame.Debug)
            {
                Scene.World.DrawDebugText($"Drone Position Style: {style.ToString()}");
                Scene.World.DrawDebugText($"Angle Override Mode: {angleOverrideMode.ToString()}");
            }
        }

        public void DroneDestroyed(ProjectileDrone drone)
        {

        }

        void ForEachDrone(Action<DroneEntry> action)
        {
            foreach (var drone in drones)
            {
                if (!drone.Drone.destroyed)
                    action(drone);
            }
        }

        bool AllDronesReached() => drones.All(x => x.Reached || x.Drone.destroyed);

        bool AllDronesFinished() => drones.All(x => (x.Reached && x.Acted) || x.Drone.destroyed);

        DroneEntry GetRandomAliveDrone()
        {
            var aliveDrones = drones.Where(x => !x.Drone.destroyed).ToList();

            if (aliveDrones.Count > 0)
            {
                return aliveDrones[Random.Next(aliveDrones.Count)];
            }
            else
            {
                return null;
            }
        }

        #region Attacks
        IEnumerator Phase2IntroAttack()
        {
            ForEachDrone((d) => { d.Drone.SetCollidable(false); });

            SetStyle(DronePositionStyle.Orbit);

            angleOverrideMode = 1;

            orbitDistanceX = 100f;
            orbitDistanceY = 100f;

            orbitAngleMultiplier = 0.1f;

            yield return new WaitUntil(AllDronesReached);

            yield return new WaitForSeconds(0.5f);

            int n = drones.Count;

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int index = (int)((j + 0.5f) * n / 4f);

                    if (!drones[index].Drone.destroyed)
                        drones[index].Drone.Shoot();
                }

                yield return new WaitForSeconds(0.1f);
            }

            yield return null;
        }

        IEnumerator OrbitAttack()
        {
            int times = phase2 ? 3 : 1;

            for (int k = 0; k < times; k++)
            {
                angleOverrideMode = 0;

                int chance = Random.Next(phase2 ? 6 : 3);

                switch (chance)
                {
                    case 0:
                        yield return new WaitUntil(AllDronesReached);
                        for (int j = 0; j < 2; j++)
                        {
                            foreach (var item in drones)
                            {
                                if (item.Drone.destroyed)
                                    continue;

                                item.Drone.ShootLaser();
                                yield return new WaitForSeconds(0.7f);
                            }

                        }
                        break;
                    case 1:
                        yield return new WaitUntil(AllDronesReached);

                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                ForEachDrone(d => d.Drone.Shoot());

                                yield return new WaitForSeconds(0.2f);
                            }

                            yield return new WaitForSeconds(0.7f);
                        }
                        break;
                    case 2:
                        yield return new WaitUntil(AllDronesReached);

                        angleOverrideMode = 2;

                        if (k == 0)
                            yield return new WaitForSeconds(seconds: 0.5f);

                        foreach (var item in drones)
                        {
                            if (item.Drone.destroyed)
                                continue;

                            item.Drone.Shoot();
                            yield return new WaitForSeconds(0.1f);
                        }
                        break;
                    case 3:
                        yield return new WaitUntil(AllDronesReached);

                        angleOverrideMode = 1;

                        if (k == 0)
                            yield return new WaitForSeconds(seconds: 1f);

                        foreach (var item in drones)
                        {
                            if (item.Drone.destroyed)
                                continue;

                            item.Drone.Shoot();
                            yield return new WaitForSeconds(0.1f);
                        }
                        break;
                    case 4:
                        yield return new WaitUntil(AllDronesReached);

                        angleOverrideMode = 1;

                        if (k == 0)
                            yield return new WaitForSeconds(seconds: 1f);

                        foreach (var item in drones)
                        {
                            if (item.Drone.destroyed)
                                continue;

                            if (item.ID % 2 == 0)
                                item.Drone.Shoot();
                            yield return new WaitForSeconds(0.05f);
                        }

                        foreach (var item in drones)
                        {
                            if (item.Drone.destroyed)
                                continue;

                            if (item.ID % 2 != 0)
                                item.Drone.Shoot();
                            yield return new WaitForSeconds(0.05f);
                        }
                        break;
                    case 5:
                        yield return new WaitUntil(AllDronesReached);

                        angleOverrideMode = 1;

                        if (k == 0)
                            yield return new WaitForSeconds(seconds: 1f);

                        foreach (var item in drones)
                        {
                            if (item.Drone.destroyed)
                                continue;

                            if (item.ID % 2 == 0)
                                item.Drone.ShootLaser();
                            yield return new WaitForSeconds(0.05f);
                        }

                        foreach (var item in drones)
                        {
                            if (item.Drone.destroyed)
                                continue;

                            if (item.ID % 2 != 0)
                                item.Drone.ShootLaser();
                            yield return new WaitForSeconds(0.2f);
                        }
                        break;
                    default:
                        break;
                }

                if(times > 1)
                    yield return new WaitForSeconds(0.5f);
            }

        }

        IEnumerator HorizontalWallAttack()
        {
            int chance = Random.Next(5);

            if(chance == 2)
                ForEachDrone((d) => { d.Drone.angleOverride = true; d.Drone.overridedAngle = MathHelper.ToRadians(90); });

            yield return new WaitUntil(AllDronesReached);

            switch (chance)
            {
                case 0:
                    for (int i = 0; i < 3; i++)
                    {
                        ForEachDrone(d => d.Drone.Shoot());

                        yield return new WaitForSeconds(0.2f);
                    }
                    break;
                case 1:
                    foreach (var item in drones)
                    {
                        if (item.Drone.destroyed)
                            continue;

                        item.Drone.ShootLaser();

                        yield return new WaitForSeconds(0.4f);
                    }

                    yield return new WaitForSeconds(1f);

                    for (int i = drones.Count - 1; i >= 0; i--)
                    {
                        DroneEntry item = drones[i];
                        if (item.Drone.destroyed)
                            continue;

                        item.Drone.ShootLaser();

                        yield return new WaitForSeconds(0.4f);
                    }

                    yield return new WaitForSeconds(1f);
                    break;
                case 2:
                    for (int i = 0; i < 30; i++)
                    {
                        DroneEntry selected = GetRandomAliveDrone();
                        if (selected != null)
                        {
                            selected.Drone.Shoot();

                            yield return new WaitForSeconds(0.2f);
                        }
                    }
                    break;
                case 3:
                    {
                        int n = drones.Count;

                        for (int i = 0; i < n / 2; i++)
                        {
                            int left = i;
                            int right = (n - 1) - i;

                            if (!drones[left].Drone.destroyed)
                                drones[left].Drone.Shoot();

                            if (!drones[right].Drone.destroyed)
                                drones[right].Drone.Shoot();

                            yield return new WaitForSeconds(0.5f);
                        }

                        if (n % 2 == 1)
                        {
                            int middle = n / 2;

                            if (!drones[middle].Drone.destroyed)
                                drones[middle].Drone.Shoot();
                        }

                    }
                    break;
                case 4:
                    {
                        int n = drones.Count;

                        int leftStart = (n - 1) / 2;
                        int rightStart = n / 2;

                        int steps = n / 2;

                        for (int i = 0; i <= steps; i++)
                        {
                            int left = leftStart - i;
                            int right = rightStart + i;

                            if (left >= 0)
                            {
                                if (!drones[left].Drone.destroyed)
                                    drones[left].Drone.Shoot();
                            }

                            if (right < n && right != left)
                            {
                                if (!drones[right].Drone.destroyed)
                                    drones[right].Drone.Shoot();
                            }

                            yield return new WaitForSeconds(0.8f);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        IEnumerator VerticalWallAttack()
        {
            for (int i = 0; i < 3; i++)
            {
                do
                {
                    yield return null;

                    ForEachDrone(d =>
                    {
                        if (d.Reached && !d.Acted)
                        {
                            if (i % 2 != 0)
                                d.Drone.Shoot();
                            d.Acted = true;
                        }
                    });

                } while (!AllDronesFinished());

                yield return new WaitForSeconds(0.5f);

                SetStyle(DronePositionStyle.VerticalWall);
            }

            yield return new WaitForSeconds(0.2f);
        }

        IEnumerator RandomAttack()
        {
            int chance = Random.Next(2);
            do
            {
                yield return null;

                ForEachDrone(d =>
                {
                    if (d.Reached && !d.Acted)
                    {
                        if (chance == 0)
                            d.Drone.Shoot();
                        else
                            d.Drone.ShootLaser();
                        d.Acted = true;
                    }
                });
            } while (!AllDronesFinished());
        }

        IEnumerator OrbitPlayerAttack()
        {
            int times = phase2 ? 3 : 1;

            for (int k = 0; k < times; k++)
            {
                yield return new WaitUntil(AllDronesReached);

                yield return new WaitForSeconds(0.5f);

                int chance = Random.Next(phase2 ? 3 : 2);

                switch (chance)
                {
                    case 0:
                        foreach (var item in drones)
                        {
                            if (item.Drone.destroyed)
                                continue;

                            item.Drone.Shoot();
                            yield return new WaitForSeconds(0.3f);
                        }
                        break;
                    case 1:
                        for (int j = 0; j < 2; j++)
                        {
                            for (int i = drones.Count - 1; i >= 0; i--)
                            {
                                DroneEntry item = drones[i];
                                if (item.Drone.destroyed)
                                    continue;

                                item.Drone.Shoot();
                                yield return new WaitForSeconds(0.4f);
                            }
                        }
                        break;
                    case 2:
                        increaseOrbitAngle = false;

                        for (int j = 0; j < 2; j++)
                        {
                            for (int i = drones.Count - 1; i >= 0; i--)
                            {
                                DroneEntry item = drones[i];
                                if (item.Drone.destroyed)
                                    continue;

                                item.Drone.ShootLaser();
                                yield return new WaitForSeconds(1f);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion  
    }
}
