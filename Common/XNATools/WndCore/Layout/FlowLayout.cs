using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNATools.WndCore.Layout
{
    public class FlowLayout : LayoutMode
    {
        /*public int PaddingLeft { get; set; }
        public int PaddingRight { get; set; }
        public int PaddingTop { get; set; }
        public int PaddingBottom { get; set; }*/
        public int CellPadding { get; set; }
        
        public FlowLayout()
        {
            //PaddingBottom = PaddingTop = PaddingRight = PaddingLeft = 0;
            CellPadding = 5;
        }

        public override void pack()
        {
            base.pack();

            List<WndComponent> curRow = new List<WndComponent>();
            int elements = refPanel.getComponents().Count;
            int count = 0;
            int maxWidth = refPanel.getRect().Width - 2 * CellPadding;
            int maxElementHeight = 0;
            int yValue = refPanel.getRect().Top + CellPadding;
            for(int i = 0; i < elements; i++)
            {
                int elementWidth = refPanel.getComponents()[i].getRect().Width;
                // dump all existing elements as required into their places (if this element is going to not fit)
                if ((count + curRow.Count * CellPadding + elementWidth > maxWidth) && count > 0)
                {
                    int startX = refPanel.getRect().Center.X - (count + (curRow.Count - 1) * CellPadding) / 2;
                    foreach (WndComponent c in curRow)
                    {
                        c.setLocation(new Vector2(startX, yValue + maxElementHeight / 2 - c.getRect().Height / 2));
                        startX += c.getRect().Width + CellPadding;
                    }
                    curRow.Clear();
                    yValue += maxElementHeight + CellPadding;
                    count = 0;
                    maxElementHeight = 0;
                }
                
                // if the element is too wide or it's the last element
                if (elementWidth > maxWidth || i == elements - 1)
                {
                    maxElementHeight = Math.Max(maxElementHeight, refPanel.getComponents()[i].getRect().Height);
                    curRow.Add(refPanel.getComponents()[i]);
                    if (refPanel.getComponents()[i].getRect().Width > maxWidth)
                    {
                        refPanel.getComponents()[i].setSize(new Vector2(maxWidth, refPanel.getComponents()[i].getRect().Height));
                    }
                    count += refPanel.getComponents()[i].getRect().Width;

                    int startX = refPanel.getRect().Center.X - (count + (curRow.Count - 1) * CellPadding) / 2;
                    foreach (WndComponent c in curRow)
                    {
                        c.setLocation(new Vector2(startX, yValue + maxElementHeight / 2 - c.getRect().Height / 2));
                        startX += c.getRect().Width + CellPadding;
                    }
                    curRow.Clear();
                    yValue += maxElementHeight + CellPadding;
                    count = 0;
                }
                else
                {
                    maxElementHeight = Math.Max(maxElementHeight, refPanel.getComponents()[i].getRect().Height);
                    curRow.Add(refPanel.getComponents()[i]);
                    count += refPanel.getComponents()[i].getRect().Width;
                }
            }
        }
    }
}
