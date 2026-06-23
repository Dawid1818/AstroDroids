using AstroDroids.Graphics;
using AstroDroids.Helpers;
using AstroDroids.Paths;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Collections.Generic;

namespace AstroDroids.Editors
{
    public class PathVisualizer
    {
        public static void DrawPath(CompositePath Path, PathPoint referencePoint = null, IPath selectedPath = null, bool highlightAll = false)
        {
            float t = 0f;
            PathPoint lastPos = Path.GetPoint(t);
            while (t < 1f)
            {
                t += 0.001f;
                PathPoint nextPos = Path.GetPoint(t);
                Screen.spriteBatch.DrawLine(lastPos, nextPos, Color.Green, 4f);
                lastPos = nextPos;
            }

            if (referencePoint != null)
            {
                GameHelper.DrawNode("R", referencePoint, Color.Orange, Color.Green);
            }

            var Paths = new List<IPath>(Path.Decompose());
            //Paths.Reverse();
            bool first = true;
            for (int i = 0; i < Paths.Count; i++)
            {
                IPath path = Paths[i];
                var keyPoints = path.KeyPoints;
                for (int j = 0; j < keyPoints.Length; j++)
                {
                    if (j == 0 && !first)
                        continue;

                    PathPoint point = keyPoints[j];

                    GameHelper.DrawNode($"{i}:{j}", point, selectedPath == path || highlightAll ? Color.Cyan : Color.Red, Color.Green, 14);
                }

                first = false;
            }
        }

        public static void DrawPath(IPath Path)
        {
            float t = 0f;
            PathPoint lastPos = Path.GetPoint(t);
            while (t < 1f)
            {
                t += 0.001f;
                PathPoint nextPos = Path.GetPoint(t);
                Screen.spriteBatch.DrawLine(lastPos, nextPos, Color.Green, 4f);
                lastPos = nextPos;
            }

            var keyPoints = Path.KeyPoints;
            for (int j = 0; j < keyPoints.Length; j++)
            {
                PathPoint point = keyPoints[j];

                GameHelper.DrawNode($"{j}", point, Color.Red, Color.Green, 14);
            }
        }
    }
}

