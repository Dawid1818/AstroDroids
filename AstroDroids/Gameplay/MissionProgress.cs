using AstroDroids.Interfaces;
using System.IO;

namespace AstroDroids.Gameplay
{
    public class MissionProgress : ISaveable
    {
        public int Lives { get; set; } = 3;
        public int ExtraLivesObtained { get; set; } = 0;
        public int Score { get; set; } = 0;
        public int Firepower { get; set; } = 1;
        public const int MaxFirepower = 5;

        public int CurrentWeapon { get; set; } = 0;

        public int LevelIndex { get; set; } = 0;
        public MissionType Type { get; set; } = MissionType.Editor;

        public bool Victory { get; set; } = false;

        public MissionProgress()
        {

        }

        public void Load(BinaryReader reader, int version)
        {
            Type = (MissionType)reader.ReadInt32();
            LevelIndex = reader.ReadInt32();
            Lives = reader.ReadInt32();
            ExtraLivesObtained = reader.ReadInt32();
            Score = reader.ReadInt32();
            Firepower = reader.ReadInt32();
            CurrentWeapon = reader.ReadInt32();
            Victory = false;
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write((int)Type);
            writer.Write(LevelIndex);
            writer.Write(Lives);
            writer.Write(ExtraLivesObtained);
            writer.Write(Score);
            writer.Write(Firepower);
            writer.Write(CurrentWeapon);
            writer.Write(Victory);
        }
    }
}
