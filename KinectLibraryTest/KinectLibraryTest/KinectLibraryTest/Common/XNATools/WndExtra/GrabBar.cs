using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNATools.WndCore;

namespace XNATools.WndExtra
{
    public class GrabBar : WndComponent
    {
        protected WndHandle parentWindow;
        protected bool moveDialog;
        protected Vector2 relativeOffset;

        public GrabBar(Rectangle dest, WndHandle parentWindow)
            : base(dest)
        {
            this.parentWindow = parentWindow;
            moveDialog = false;
        }

        public override void mousePressedRight(Point p)
        {
            if (dest.Contains(p))
            {
                moveDialog = true;
                relativeOffset = new Vector2(p.X-dest.X, p.Y-dest.Y);
            }
        }

        public override void mouseClickedRight(Point p)
        {
            moveDialog = false;
        }

        public override void mouseMoved(Point oldP, Point newP)
        {
            if (moveDialog)
                parentWindow.moveToAndChildren(new Vector2(newP.X, newP.Y) - relativeOffset);
        }
    }
}
