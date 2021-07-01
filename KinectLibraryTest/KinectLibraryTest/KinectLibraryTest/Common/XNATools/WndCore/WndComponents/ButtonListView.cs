using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNATools.WndCore
{
    public class ButtonListView : ButtonCollection
    {
        protected int maxInViewCount;
        protected Vector2 shiftAmount;
        protected Vector2 listTop;
        protected int top;

        public ButtonListView(int maxInViewCount, Vector2 listTop, Vector2 shiftAmount)
            : base(new Rectangle(0,0,0,0))
        {
            this.maxInViewCount = maxInViewCount;
            this.shiftAmount = shiftAmount;
            this.listTop = listTop;
            top = 0;
        }

        public override void add(Button button)
        {
            if (buttonList.Count >= maxInViewCount) button.setVisible(false);
            base.add(button);
        }

        public override void next()
        {
            base.next();

            if (selectedIndex > top + maxInViewCount - 1)
            {
                top++;
            }
            else if (selectedIndex == 0)
            {
                top = 0;
            }

            for (int i = 0; i < buttonList.Count; i++)
            {
                int mod = i - top;
                buttonList[i].setLocation(listTop + shiftAmount * mod);

                if (i >= top && i < top + maxInViewCount)
                {
                    buttonList[i].setVisible(true);
                }
                else
                {
                    buttonList[i].setVisible(false);
                }
            }
        }

        public override void previous()
        {
            base.previous();

            if (selectedIndex < top && top > 0)
            {
                top--;
            }
            else if (top == 0)
            {
                top = buttonList.Count - maxInViewCount;

                if (top < 0) top = 0;
            }

            for (int i = 0; i < buttonList.Count; i++)
            {
                int mod = i - top;
                buttonList[i].setLocation(listTop + shiftAmount * mod);

                if (i >= top && i < top + maxInViewCount)
                {
                    buttonList[i].setVisible(true);
                }
                else
                {
                    buttonList[i].setVisible(false);
                }
            }
        }
    }
}
