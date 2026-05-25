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
        public float Speed { get { return TravelManager.Speed; } set {  TravelManager.Speed = value; }  }

        Vector2 destination;
        PathManager TravelManager;

        const float margin = 100f;

        protected Scene Scene { get { return SceneManager.GetScene(); } }

        public RandomMoveManager(Vector2 startPos)
        {
            Position = startPos;
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
            TravelManager.Update(gameTime);
            Position = TravelManager.Position;

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
    }
}
