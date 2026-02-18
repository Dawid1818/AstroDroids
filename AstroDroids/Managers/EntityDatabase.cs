using System;
using System.Collections.Generic;
using System.Linq;

namespace AstroDroids.Managers
{
    public class EntityDatabase
    {
        static Dictionary<int, Type> entityTypes = new Dictionary<int, Type>();

        public static void Initialize()
        {
            RegisterEnemy(0, typeof(Entities.Hostile.BasicEnemy));
            RegisterEnemy(1, typeof(Entities.Hostile.SpinLaser));
            RegisterEnemy(2, typeof(Entities.Hostile.DroneController));
            RegisterEnemy(3, typeof(Entities.Hostile.ProximityMine));
            RegisterEnemy(4, typeof(Entities.Hostile.TriGunTurret));
        }

        static void RegisterEnemy(int id, Type entity)
        {
            entityTypes[id] = entity;
        }

        public static Type GetEnemyType(int id)
        {
            if (entityTypes.TryGetValue(id, out Type type))
            {
                return type;
            }
            else
            {
                throw new Exception($"Enemy with ID {id} not found in EntityDatabase.");
            }
        }

        public static List<Type> GetAllEnemyTypes()
        {
            return entityTypes.Values.ToList();
        }
    }
}
