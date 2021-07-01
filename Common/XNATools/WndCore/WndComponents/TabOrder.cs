using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XNATools.WndCore
{
    public class TabOrder
    {
        protected List<WndComponent> components;
        protected int curItem;
        protected InputManager inputManager;
        protected Point lastPoint;

        public TabOrder(InputManager inputManager)
        {
            components = new List<WndComponent>();
            this.inputManager = inputManager;
            curItem = 0;

            lastPoint = inputManager.getCursor();
        }

        public void update(GameTime gameTime)
        {
            Point curPoint = inputManager.getCursor();
            if (curPoint.X != lastPoint.X || curPoint.Y != lastPoint.Y)
            {
                foreach (WndComponent c in components)
                {
                    c.mouseMoved(lastPoint, curPoint);
                    if (c.getFocusRect().Contains(curPoint))
                        setFocus(c);
                }
            }

            if (inputManager.isKeyPressed(Keys.Down) || inputManager.isBtnPressed(Buttons.DPadDown, 1))
            {
                previous();
            }
            else if (inputManager.isKeyPressed(Keys.Up) || inputManager.isBtnPressed(Buttons.DPadUp, 1))
            {
                next();
            }
            else if (inputManager.isKeyPressed(Keys.Left) || inputManager.isBtnPressed(Buttons.DPadLeft, 1))
            {
                components[curItem].previous();
            }
            else if (inputManager.isKeyPressed(Keys.Right) || inputManager.isBtnPressed(Buttons.DPadRight, 1))
            {
                components[curItem].next();
            }
        }

        public virtual void previous()
        {
            components[curItem].setFocus(false);

            curItem++;
            if (curItem >= components.Count)
                curItem = 0;

            components[curItem].setFocus(true);
        }

        public virtual void next()
        {
            components[curItem].setFocus(false);

            curItem--;
            if (curItem < 0)
                curItem = components.Count - 1;

            components[curItem].setFocus(true);
        }

        public void addComponent(WndComponent component)
        {
            components.Add(component);
        }

        public void setFocus(int targetID)
        {
            if (targetID < 0 || targetID >= components.Count)
                return;

            components[curItem].setFocus(false);

            curItem = targetID;
            components[curItem].setFocus(true);
        }

        public void setFocus(WndComponent component)
        {
            int index = -1;
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] == component)
                    index = i;
            }

            setFocus(index);
        }

        public WndComponent getCurComponent()
        {
            return components[curItem];
        }

        public int getTabOrderID()
        {
            return curItem;
        }
    }
}
