using System.Collections.Generic;

namespace AstroDroids.Gameplay
{
    public enum MissionType
    {
        Tutorial,
        Story,
        BossRush,
        Editor
    }
    public class GameMission
    {
        public string Name { get; set; }
        public MissionType Type { get; set; } = MissionType.Editor;
        public List<string> LevelNames { get; set; } = new List<string>();
    }
}
