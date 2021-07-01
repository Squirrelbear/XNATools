using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XNATools.WndCore;

namespace KinectLibraryTest
{
    public class NumberPanel : Panel
    {
        private NumberPlayArea gp;
        private int number, _x, _y;
        private Label label;

        public NumberPanel(int x, int y, Rectangle dest, SpriteFont font, NumberPlayArea gp)
            : base(dest)
        {
            this._x = x;
            this._y = y;
            this.gp = gp;
            
            Texture2D blank = new Texture2D(gp.getParentWnd().getParent().getAppRef().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.Red });
            setTexture(blank);
            label = new Label(dest, "0", font);
            label.centreInRect();
            addComponent(label);
        }

        public void setNumber(int number) 
        {
            this.number = number;
            setBackground((number==0) ? Color.Orange : Color.Red);
            label.setText((number==0) ? "" : number.ToString());
        }

        public void setBackground(Color c)
        {
            texture.SetData(new[] { c });
        }

        public Color getBackground()
        {
            Color[] c = new Color[] { Color.Red };
            texture.GetData<Color>(c);
            return c[0];
        }
    
        public int getNumber() 
        {
            return number;
        }
    
        public void clearSelection() {
            setNumber(getNumber());
        }
    
        public override void mouseClickedLeft(Point p)
        {
            if (gp.getTwoHandMode())
                return;

 	        base.mouseClickedLeft(p);
            if(number != 0 && dest.Contains(p) && gp.testSelection(_x, _y))
                setBackground(Color.Green);    
        }

        public void updateMouseEnterExit(Point oldP, Point newP)
        {
            bool inOld = dest.Contains(oldP);
            bool inNew = dest.Contains(newP);
            if (!inOld && inNew) // mouse entered
            {
                if (getBackground().Equals(Color.Red))
                    setBackground(Color.Yellow);
            }
            else if (inOld && !inNew) // mouse exited
            {
                if (getBackground().Equals(Color.Yellow))
                    setNumber(getNumber());
            }
        }

        public override void mouseMoved(Point oldP, Point newP)
        {
            if (gp.getTwoHandMode())
                return;

 	        base.mouseMoved(oldP, newP);
            updateMouseEnterExit(oldP, newP);
        }
    }
}
