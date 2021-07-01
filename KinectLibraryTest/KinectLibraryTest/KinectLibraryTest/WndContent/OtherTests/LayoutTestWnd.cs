using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNATools.WndCore;
using Microsoft.Xna.Framework;
using XNATools;
using Microsoft.Xna.Framework.Graphics;
using XNATools.WndCore.Layout;

namespace KinectLibraryTest
{
    public class LayoutTestWnd : WndHandle
    {
        public LayoutTestWnd(Rectangle displayRect, WndGroup parent)
            : base(64896, displayRect, parent)
        {
            ImageTools imgTools = ImageTools.getSingleton(parent.getAppRef());
            Texture2D red = imgTools.createColorTexture(Color.Red);
            Texture2D blue = imgTools.createColorTexture(Color.Blue);
            Texture2D green = imgTools.createColorTexture(Color.Green);
            Texture2D black = imgTools.createColorTexture(Color.Black);

            WndComponent one = new WndComponent(new Rectangle(0, 0, 50, 50), red);
            WndComponent two = new WndComponent(new Rectangle(0, 0, 60, 80), blue);
            WndComponent three = new WndComponent(new Rectangle(0, 0, 300, 20), green);
            WndComponent four = new WndComponent(new Rectangle(0, 0, 200, 100), red);
            WndComponent five = new WndComponent(new Rectangle(0, 0, 200, 100), red);

            Panel p = new Panel(new Rectangle(200, 200, 250, 300), black);
            p.addComponent(one);
            p.addComponent(two);
            p.addComponent(three);
            p.addComponent(four);
            p.addComponent(five);
            FlowLayout layout = new FlowLayout();
            layout.setPanel(p);
            layout.pack();
            addComponent(p);
        }

    }
}
