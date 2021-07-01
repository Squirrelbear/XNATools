using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNATools.WndCore
{
    public class Panel : WndComponent
    {
        protected List<WndComponent> components;

        public Panel(Rectangle dest)
            : this(dest, null)
        {
        }

        public Panel(Rectangle dest, Texture2D background)
            : base(dest, background)
        {
            components = new List<WndComponent>();
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            foreach (WndComponent c in components)
            {                
                c.update(gameTime);
            }
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);

            foreach(WndComponent c in components)
            {
                if(c.isVisible())
                    c.draw(spriteBatch);
            }
        }

        public override void mouseMoved(Point oldP, Point newP)
        {
            base.mouseMoved(oldP, newP);
            foreach (WndComponent c in components)
            {
                if (c.isVisible())
                    c.mouseMoved(oldP, newP);
            }
        }

        public override void mouseClickedLeft(Point p)
        {
            base.mouseClickedLeft(p);

            foreach (WndComponent c in components)
            {
                if (c.isVisible())
                    c.mouseClickedLeft(p);
            }
        }

        public override void mouseClickedRight(Point p)
        {
            base.mouseClickedRight(p);

            foreach (WndComponent c in components)
            {
                if (c.isVisible())
                    c.mouseClickedRight(p);
            }
        }

        public override void mousePressedLeft(Point p)
        {
            base.mousePressedLeft(p);

            foreach (WndComponent c in components)
            {
                if (c.isVisible())
                    c.mousePressedLeft(p);
            }
        }

        public override void mousePressedRight(Point p)
        {
            base.mousePressedRight(p);

            foreach (WndComponent c in components)
            {
                if (c.isVisible())
                    c.mousePressedRight(p);
            }
        }

        public void addComponent(WndComponent component)
        {
            components.Add(component);
        }

        public List<WndComponent> getComponents()
        {
            return components;
        }

        public void removeComponent(WndComponent component)
        {
            components.Remove(component);
        }

        public override void moveByAndChildren(Vector2 translation)
        {
            foreach (WndComponent c in components)
                c.moveByAndChildren(translation);

            base.moveByAndChildren(translation);
        }
    }
}
