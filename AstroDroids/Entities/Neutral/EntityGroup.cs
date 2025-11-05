using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AstroDroids.Entities.Neutral
{
    public class EntityCell
    {
        public Vector2 Position
        {
            get
            {
                return offset + owner.Position;
            }
        }

        Vector2 offset;

        EntityGroup owner;

        public EntityCell(EntityGroup owner, Vector2 offset)
        {
            this.offset = offset;
            this.owner = owner;
        }
    }

    public class EntityGroup : Entity
    {
        public Vector2 Position { get; private set; }

        List<List<EntityCell>> cells = new List<List<EntityCell>>();

        float cellWidth;
        float cellHeight;
        float spacing;

        bool movingRight = true;

        public EntityGroup(Vector2 position, int rows, int cols, float cellWidth, float cellHeight, float spacing)
        {
            Position = position;
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;
            this.spacing = spacing;

            for (int i = 0; i < rows; i++)
            {
                float xPos = 0f;

                List<EntityCell> cellsinRow = new List<EntityCell>();
                for (int j = 0; j < cols; j++)
                {
                    cellsinRow.Add(new EntityCell(this, new Vector2(xPos, i * (cellHeight + spacing))));

                    xPos += cellWidth + spacing;
                }
                cells.Add(cellsinRow);
            }
        }

        public EntityCell GetCell(int row, int col)
        {
            if (row < 0 || row >= cells.Count)
            {
                return null;
            }

            if (col < 0 || col >= cells[row].Count)
            {
                return null;
            }

            return cells[row][col];
        }

        public override void Update(GameTime gameTime)
        {
            if (movingRight)
            {
                if (Position.X + (cells[0].Count * (cellWidth + spacing)) >= Scene.World.Bounds.Width)
                {
                    movingRight = false;
                }

                Position += new Vector2(1f, 0f);
            }
            else
            {
                if (Position.X <= spacing)
                {
                    movingRight = true;
                }

                Position += new Vector2(-1f, 0f);
            }
        }
    }
}
