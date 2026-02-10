using AstroDroids.Paths;
using AstroDroids.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace AstroDroids.Editors
{
    public class PathVisualizer
    {
        public static void DrawPath(CompositePath Path, PathPoint referencePoint = null, IPath selectedPath = null)
        {
            float t = 0f;
            PathPoint lastPos = Path.GetPoint(t);
            while (t < 1f)
            {
                t += 0.01f;
                PathPoint nextPos = Path.GetPoint(t);
                Screen.spriteBatch.DrawLine(lastPos, nextPos, Color.Green, 4f);
                lastPos = nextPos;
            }

            if(referencePoint != null)
                Screen.spriteBatch.DrawCircle(referencePoint, 16f, 16, Color.Yellow, 16f);

            var Paths = new List<IPath>(Path.Decompose());
            //Paths.Reverse();
            bool first = true;
            foreach (var path in Paths)
            {
                var keyPoints = path.KeyPoints;
                for (int i = 0; i < keyPoints.Length; i++)
                {
                    if (i == 0 && !first)
                        continue;

                    PathPoint point = keyPoints[i];

                    Screen.spriteBatch.DrawCircle(point, 16f, 16, selectedPath == path ? Color.Cyan : Color.Red, 16f);
                }

                first = false;
            }
        }
    }
}
