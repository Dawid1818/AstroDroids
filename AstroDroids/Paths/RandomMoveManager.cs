using AstroDroids.Helpers;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;

namespace AstroDroids.Paths
{
    public class RandomMoveManager
    {
        public Vector2 Position { get; private set; }
        public bool Active { get; private set; }
        public int maxMoveDistance { get; set; } = 300;
        public float Speed { get { return TravelManager.Speed; } set { TravelManager.Speed = value; } }

        Vector2 destination;
        PathManager TravelManager;

        const float margin = 100f;

        protected Scene Scene { get { return SceneManager.GetScene(); } }

        public Vector2 MovementDirection { get; private set; } = Vector2.UnitX;
        public float MovementAngle => GameHelper.AngleFromDir(MovementDirection);

        private Vector2 previousPosition;

        public RandomMoveManager(Vector2 startPos)
        {
            Position = startPos;
            previousPosition = startPos;
            TravelManager = new PathManager();
            TravelManager.Position = startPos;
            TravelManager.Speed = 100f;
        }

        public void UpdatePosition(Vector2 pos)
        {
            Position = pos;
            TravelManager.Position = pos;
        }

        public void Update(GameTime gameTime)
        {
            previousPosition = Position;

            TravelManager.Update(gameTime);
            Position = TravelManager.Position;

            Vector2 movement = Position - previousPosition;

            if (movement.LengthSquared() > 0.0001f)
                MovementDirection = Vector2.Normalize(movement);

            if (!TravelManager.Active)
            {
                Active = false;
            }
        }

        public void SetNewPath(bool useBezier = true)
        {
            Active = true;

            float angle = (float)(AstroDroidsGame.rnd.NextDouble() * MathHelper.TwoPi);
            float distance = (float)(AstroDroidsGame.rnd.NextDouble() * maxMoveDistance);

            Vector2 offset = GameHelper.DirFromAngle(angle);

            destination = Position + offset * distance;

            destination.X = MathHelper.Clamp(destination.X, margin, Scene.World.Bounds.Width - margin);

            destination.Y = MathHelper.Clamp(destination.Y, margin, Scene.World.Bounds.Height - margin);

            if (useBezier)
            {
                TravelManager.SetPath(GameHelper.CreateBezier(Position, destination), TravelManager.Speed);
            }
            else
                TravelManager.SetPath(new LinePath(Position, destination), TravelManager.Speed);
        }

        public void SetNewPath(float currentAngle)
        {
            Active = true;

            float maxTurn = MathHelper.ToRadians(45);

            float turn = MathHelper.Lerp(-maxTurn, maxTurn, (float)AstroDroidsGame.rnd.NextDouble());

            float newAngle = currentAngle + turn;

            float distance = (float)(AstroDroidsGame.rnd.NextDouble() + 0.1d) * maxMoveDistance;

            Vector2 direction = GameHelper.DirFromAngle(newAngle);

            destination = Position + direction * distance;

            destination.X = MathHelper.Clamp(destination.X, margin, Scene.World.Bounds.Width - margin);

            destination.Y = MathHelper.Clamp(destination.Y, margin, Scene.World.Bounds.Height - margin);

            TravelManager.SetPath(GameHelper.CreateBezier(Position, destination, currentAngle), TravelManager.Speed);
        }

        public void SetNewPath2(float currentAngle)
        {
            float margin = 0;

            Active = true;

            float maxTurn = 360;

            float selectedTurn = MathHelper.Lerp(-maxTurn, maxTurn, (float)AstroDroidsGame.rnd.NextDouble());
            //selectedTurn = 180; //override

            float turn = MathHelper.ToRadians(selectedTurn);

            float newAngle = currentAngle + turn;

            float distance = (float)(AstroDroidsGame.rnd.NextDouble() + 1d) * maxMoveDistance;
            distance = 300; //override

            Vector2 direction = GameHelper.DirFromAngle(newAngle);

            destination = Position + direction * distance;  

            //destination.X = MathHelper.Clamp(destination.X, margin, Scene.World.Bounds.Width - margin);

            //destination.Y = MathHelper.Clamp(destination.Y, margin, Scene.World.Bounds.Height - margin);

            destination.X = GameHelper.WrapValue(destination.X, 0, Scene.World.Bounds.Width);

            destination.Y = GameHelper.WrapValue(destination.Y, 0, Scene.World.Bounds.Height);

            TravelManager.SetPath(GameHelper.CreateCatmull(Position, destination, currentAngle), TravelManager.Speed);
        }

        public void SetNewPath3(Vector2 position, float currentAngle)
        {
            Active = true;

            TravelManager.SetPath(GameHelper.CreateCatmull(Position, position, currentAngle), TravelManager.Speed);
        }

        public void SetPath(IPath path, bool reverse = false)
        {
            Active = true;

            TravelManager.SetPath(path, TravelManager.Speed, reverse);
        }

        public IPath GetPath()
        {
            return TravelManager.GetPath();
        }

        public float GetProgress()
        {
            return TravelManager.Time;
        }
    }
}
