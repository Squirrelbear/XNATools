using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNATools.WndCore.Layout
{
    public abstract class LayoutMode
    {
        protected Panel refPanel;

        public LayoutMode()
        {
        }

        public void setPanel(Panel panel)
        {
            this.refPanel = panel;
        }

        public virtual void pack()
        {

        }
    }
}
