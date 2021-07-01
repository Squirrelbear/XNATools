using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNATools.WndCore
{
    public class DragPanel : Panel
    {
        protected WndHandle parentWindow;
        protected bool movePanel;
        protected Vector2 relativeOffset;
        protected Rectangle boundRect;
        protected bool enforceBounds, requireTargetToStart;
        protected bool mouseRight;

        public DragPanel(Rectangle dest, WndHandle parentWindow, bool mouseRight = true)
            : this(dest, null, parentWindow, mouseRight)
        {
        }

        public DragPanel(Rectangle dest, Texture2D background, WndHandle parentWindow, bool mouseRight = true)
            : base(dest, background)
        {
            this.parentWindow = parentWindow;
            this.mouseRight = mouseRight;
            movePanel = false;
            boundRect = new Rectangle(-1, -1, 0, 0);
            enforceBounds = false;
            requireTargetToStart = true;
        }

        public override void mousePressedRight(Point p)
        {
            if (!mouseRight)
                return;

            base.mousePressedRight(p);

            if (!requireTargetToStart || dest.Contains(p))
            {
                movePanel = true;
                relativeOffset = new Vector2(p.X-dest.X, p.Y-dest.Y);
            }
        }

        public override void mouseClickedRight(Point p)
        {
            if (!mouseRight)
                return;

            base.mouseClickedRight(p);

            movePanel = false;
        }

        public override void mousePressedLeft(Point p)
        {
            if (mouseRight)
                return;

            base.mousePressedLeft(p);

            if (!requireTargetToStart || dest.Contains(p))
            {
                movePanel = true;
                relativeOffset = new Vector2(p.X - dest.X, p.Y - dest.Y);
            }
        }

        public override void mouseClickedLeft(Point p)
        {
            if (mouseRight)
                return;

            base.mouseClickedLeft(p);

            movePanel = false;
        }

        public override void mouseMoved(Point oldP, Point newP)
        {
            base.mouseMoved(oldP, newP);

            if (movePanel)
            {
                Vector2 newPos = new Vector2(newP.X, newP.Y) - relativeOffset;
                if (enforceBounds)
                {
                    if (newPos.X + getRect().Width > boundRect.Right)
                        newPos.X = boundRect.Right - getRect().Width;
                    if (newPos.X < boundRect.Left)
                        newPos.X = boundRect.Left;

                    if (newPos.Y + getRect().Height > boundRect.Bottom)
                        newPos.Y = boundRect.Bottom - getRect().Height;
                    if (newPos.Y < boundRect.Top)
                        newPos.Y = boundRect.Top;
                }

                moveToAndChildren(newPos, getRect());
            }
        }

        public void setEnforceBoundsRect(Rectangle boundRect)
        {
            this.boundRect = boundRect;
            enforceBounds = true;
        }

        public Rectangle getEnforceBoundsRect()
        {
            return boundRect;
        }

        public bool getEnforceBounds()
        {
            return enforceBounds;
        }

        public void setEnforceBounds(bool enforceBounds)
        {
            this.enforceBounds = enforceBounds;
        }

        public void setRequireTargetToStart(bool requireTarget)
        {
            this.requireTargetToStart = requireTarget;
        }
    }
}
