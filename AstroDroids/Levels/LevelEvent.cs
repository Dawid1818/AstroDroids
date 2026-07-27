using System;

namespace AstroDroids.Levels
{
    public class LevelEvent
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Action Callback { get; set; }
    }
}
