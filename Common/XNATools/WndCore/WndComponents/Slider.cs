using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNATools.WndCore
{
    public class Slider : Panel
    {
        protected DragPanel slideControl;
        protected int min, max, curValue, sliderMin, sliderMax, lastValue;
        protected bool vertical, hasChanged;

        public Slider(Rectangle dest, int min, int max, int init, WndHandle wnd, bool vertical = false)
            : base(dest)
        {
            this.vertical = vertical;
            this.min = min;
            this.max = max;
            lastValue = curValue = init;
            Texture2D slideColour = new Texture2D(wnd.getParent().getAppRef().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            slideColour.SetData(new[] { Color.DarkGray });
            Texture2D bgColour = new Texture2D(wnd.getParent().getAppRef().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            bgColour.SetData(new[] { Color.Black });
            Rectangle slideRect;
            if (vertical)
            {
                sliderMin = dest.Y;
                int sliderHeight = 50;
                sliderMax = dest.Bottom - sliderHeight;

                slideRect = new Rectangle(dest.X, calculatePos(init), dest.Width, sliderHeight);
                Rectangle bgRect = new Rectangle(dest.Center.X - 5, dest.Y, 10, dest.Height);
                addComponent(new WndComponent(bgRect, bgColour));
            }
            else
            {
                sliderMin = dest.X;
                int sliderHeight = 50;
                sliderMax = dest.Right - sliderHeight;

                slideRect = new Rectangle(calculatePos(init), dest.Y, sliderHeight, dest.Height);
                Rectangle bgRect = new Rectangle(dest.X, dest.Center.Y - 5, dest.Width, 10);
                addComponent(new WndComponent(bgRect, bgColour));
            }
            slideControl = new DragPanel(slideRect, slideColour, null, false);
            slideControl.setEnforceBoundsRect(dest);

            addComponent(slideControl);
            hasChanged = false;
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            lastValue = curValue;
            curValue = calculateValue();
            hasChanged = lastValue != curValue;
        }

        private int calculatePos(int curValue)
        {
            return (sliderMax - sliderMin) * (curValue - min) / (max - min) + sliderMin;
        }

        private int calculateValue()
        {
            if (vertical)
                return (max - min) * (slideControl.getRect().Y - sliderMin) / (sliderMax - sliderMin) + min;
            else
                return (max - min) * (slideControl.getRect().X - sliderMin) / (sliderMax - sliderMin) + min;
        }

        public int getValue()
        {
            return curValue;
        }

        public bool getHasChanged()
        {
            return hasChanged;
        }
    }
}
