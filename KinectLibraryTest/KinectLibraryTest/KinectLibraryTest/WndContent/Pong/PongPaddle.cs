using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using XNATools.WndCore;

namespace KinectLibraryTest
{
    public class PongPaddle : DragPanel
    {
        public PongPaddle(Rectangle dest, WndHandle parent)
            : base(dest, parent)
        {
            Texture2D blank = new Texture2D(parent.getParent().getAppRef().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.White });
            WndComponent graphic = new WndComponent(dest, blank);
            addComponent(graphic);
            setEnforceBoundsRect(new Rectangle(dest.X, 0, 0, parent.getRect().Height));
            setRequireTargetToStart(false);
        }
    }
}
