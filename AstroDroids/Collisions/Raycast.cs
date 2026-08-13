using AstroDroids.Entities;
using AstroDroids.Managers;
using AstroDroids.Scenes;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Collections.Generic;

namespace AstroDroids.Collisions
{
    public static class Raycast
    {
        static Scene scene { get { return SceneManager.GetScene(); } }

        public static List<CollidableEntity> Fire(Vector2 origin, Vector2 direction, CollidableEntity toIgnore = null)
        {
            List<CollidableEntity> entities = new List<CollidableEntity>();

            if (scene.World == null)
                return entities;

            Ray2D ray = new Ray2D(origin, direction);

            foreach (var item in scene.World.AllCollidables)
            {
                if (item == toIgnore)
                    continue;

                if (item.Intersects(ray))
                {
                    entities.Add(item);
                }
            }

            entities.Sort((a, b) =>
            {
                float da = Vector2.DistanceSquared(origin, a.Transform.Position);

                float db = Vector2.DistanceSquared(origin, b.Transform.Position);

                return da.CompareTo(db);
            });

            return entities;
        }

        public static List<CollidableEntity> FireCapsule(Vector2 origin, Vector2 destination, float radius, CollidableEntity toIgnore = null)
        {
            List<CollidableEntity> entities = new List<CollidableEntity>();
            if (scene.World == null)
                return entities;

            var boundingCapsule = new BoundingCapsule2D(origin, destination, radius);

            foreach (var item in scene.World.AllCollidables)
            {
                if (item == toIgnore)
                    continue;

                if (item.Intersects(boundingCapsule))
                {
                    entities.Add(item);
                }
            }

            entities.Sort((a, b) =>
            {
                float da = Vector2.DistanceSquared(origin, a.Transform.Position);

                float db = Vector2.DistanceSquared(origin, b.Transform.Position);

                return da.CompareTo(db);
            });

            return entities;
        }
    }
}
