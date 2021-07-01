using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using XNATools.WndCore;
using XNATools.WndCore.Kinect;

namespace KinectLibraryTest
{
    public class LightsaberWnd : KinectWndHandle
    {
        public LightsaberWnd(Rectangle displayRect, WndGroup parent)
            : base(WndFactory.getCode(WndFactory.WndCodes.LightsaberWnd), displayRect, parent)
        {
            Lightsaber ls = new Lightsaber(new Rectangle(300, 300, 50, 455), this);
            addComponent(ls);
        }
    }
}
