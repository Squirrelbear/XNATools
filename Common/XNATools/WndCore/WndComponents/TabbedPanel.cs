using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNATools.WndCore
{
    public class TabbedPanel : Panel
    {
        protected List<Panel> panels;
        protected int curPanel;

        public TabbedPanel(Rectangle dest)
            : this(dest, null)
        {
        }

        public TabbedPanel(Rectangle dest, Texture2D background)
            : base(dest, background)
        {
            panels = new List<Panel>();
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if(curPanel < panels.Count && curPanel >= 0)
                panels[curPanel].update(gameTime);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);

            if (curPanel < panels.Count && curPanel >= 0)
                panels[curPanel].draw(spriteBatch);
        }

        public override void moveByAndChildren(Vector2 translation)
        {
            foreach (Panel p in panels)
                p.moveByAndChildren(translation);

            base.moveByAndChildren(translation);
        }

        public void addPanel(Panel panel)
        {
            panels.Add(panel);
        }

        public void removePanel(int id)
        {
            panels.RemoveAt(id);
        }

        public List<Panel> getPanels()
        {
            return panels;
        }

        public void setPanel(int panelID)
        {
            this.curPanel = panelID;
        }
    }
}
