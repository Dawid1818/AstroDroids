using AstroDroids.Extensions;
using AstroDroids.Gameplay;
using AstroDroids.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace AstroDroids.Data
{
    public class SaveData : ISaveable
    {
        public const string Magic = "adsave";
        public const int FileVersion = 4;

        public string PlayerName { get; set; } = "Player";
        public int ReachedLevel = 0;
        public bool FinishedStory = false;
        public ShipCustomization Ship { get; set; } = new ShipCustomization();
        public MissionProgress MissionProgress { get; set; }

        public List<ScoreEntry> Scores { get; set; } = new List<ScoreEntry>();

        public void Load(BinaryReader reader, int version)
        {
            if (reader.ReadFixedString(Magic.Length) != Magic)
            {
                throw new InvalidDataException("Invalid save data file, Magic string doesn't match.");
            }

            int actualVersion = reader.ReadInt32();

            if(actualVersion >= 2)
            {
                PlayerName = reader.ReadString();
            }
            else
            {
                PlayerName = "Player";
            }

            if(actualVersion >= 4)
            {
                ReachedLevel = reader.ReadInt32();
                FinishedStory = reader.ReadBoolean();
            }
            else
            {
                ReachedLevel = 0;
                FinishedStory = false;
            }

            Ship = new ShipCustomization();
            Ship.Load(reader, version);

            if(actualVersion >= 3)
            {
                int scoreCount = reader.ReadInt32();
                Scores = new List<ScoreEntry>();
                for(int i = 0; i < scoreCount; i++)
                {
                    ScoreEntry entry = new ScoreEntry();
                    entry.Load(reader, actualVersion);
                    Scores.Add(entry);
                }
            }

            if(actualVersion >= 1)
            {
                bool hasMission = reader.ReadBoolean();
                if (hasMission)
                {
                    MissionProgress = new MissionProgress();
                    MissionProgress.Load(reader, actualVersion);
                }
                else
                {
                    MissionProgress = null;
                }
            }
            else
            {
                MissionProgress = null;
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.WriteFixedString(Magic);

            writer.Write(FileVersion);

            writer.Write(PlayerName);

            writer.Write(ReachedLevel);
            writer.Write(FinishedStory);

            Ship.Save(writer);

            writer.Write(Scores.Count);
            foreach (var item in Scores)
            {
                item.Save(writer);
            }

            if (MissionProgress != null)
            {
                writer.Write(true);
                MissionProgress.Save(writer);
            }
            else
            {
                writer.Write(false);
            }
        }
    }
}
