using AstroDroids.Entities;
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

        Vector2 destination;
        PathManager TravelManager;

        protected Scene Scene { get { return SceneManager.GetScene(); } }

        public RandomMoveManager(Vector2 startPos)
        {
            Position = startPos;
            TravelManager = new PathManager();
            TravelManager.Position = startPos;
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
            destination = Position + new Vector2(AstroDroidsGame.rnd.Next(-maxMoveDistance, maxMoveDistance), AstroDroidsGame.rnd.Next(-maxMoveDistance, maxMoveDistance));
            destination = CollidableEntity.ClampPosition(destination, Scene.World);

            if (useBezier)
            {
                TravelManager.SetPath(GameHelper.CreateBezier(Position, destination), 1f);
            }
            else
                TravelManager.SetPath(new LinePath(Position, destination), 1f);
        }
    }
}
