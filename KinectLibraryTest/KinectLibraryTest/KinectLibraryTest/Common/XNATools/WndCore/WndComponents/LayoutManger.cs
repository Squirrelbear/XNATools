using System;
using Microsoft.Xna.Framework;

namespace XNATools.WndCore
{
    public class LayoutManger
    {
        private Rectangle layoutRegion;
        private int rows, cols;
        private int width, height;

        private int curRow, curCol;

        // width = padding right, height = padding bottom
        private Rectangle padding;

        public LayoutManger(Rectangle layoutRegion, int rows, int cols)
        {
            this.layoutRegion = layoutRegion;
            this.rows = rows;
            this.cols = cols;

            width = layoutRegion.Width / cols;
            height = layoutRegion.Height / rows;

            curRow = curCol = 0;
            padding = new Rectangle(0, 0, 0, 0);
        }

        public void setPadding(Rectangle padding)
        {
            this.padding = padding;
        }

        public Rectangle nextRect()
        {
            Rectangle result = getRect(curRow, curCol);

            curCol++;
            if (curCol >= cols)
            {
                curCol = 0;
                curRow++;
            }

            return result;
        }

        public Rectangle getRect(int row, int col)
        {
            return new Rectangle(layoutRegion.X + width * col + padding.X, layoutRegion.Y + height * row + padding.Y,
                                  width - padding.X - padding.Width, height - padding.Y - padding.Height);
        }
    }
}
