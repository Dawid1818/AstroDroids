using AstroDroids.Data;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

namespace AstroDroids.Components.Custom
{
    partial class LeaderboardItem
    {
        public void Setup(int place, ScoreEntry entry)
        {
            PlaceLabel.SetTextNoTranslate(place.ToString());
            NameLabel.SetTextNoTranslate(entry.Name);
            ScoreLabel.SetTextNoTranslate(entry.Score.ToString());
        }

        partial void CustomInitialize()
        {

        }
    }
}
