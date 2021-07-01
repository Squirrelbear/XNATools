using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using XNATools.WndCore;
using XNATools.WndCore.Kinect;

namespace KinectLibraryTest
{
    public class MenuWndSample : KinectWndHandle
    {
        public MenuWndSample(Rectangle displayRect, WndGroup parent)
            : base(WndFactory.getCode(WndFactory.WndCodes.MenuWndSample), displayRect, parent)
        {
            WndComponent background = new WndComponent(displayRect, loadTexture("dialogBackground"));
            Rectangle panelRect = new Rectangle(50, 50, 400, 200);
            LayoutManger lm = new LayoutManger(panelRect, 1, 2);
            DragPanel dragPanel = new DragPanel(panelRect, this);
            dragPanel.setEnforceBoundsRect(new Rectangle(0, panelRect.Y, displayRect.Width, 0));
            //ButtonCollection btnCollection = new ButtonCollection(panelRect);
            TextButton btn1 = new TextButton(lm.nextRect(), "Button 1", loadFont("hugeFont"),
                                                loadTexture("s1"), loadTexture("s2"));
            btn1.setActionID(1);
            btn1.setEnableMouseOverSwap(true);
            TextButton btn2 = new TextButton(lm.nextRect(), "Button 2", loadFont("hugeFont"),
                                                loadTexture("s3"), loadTexture("s4"));
            btn2.setActionID(2);
            btn2.setEnableMouseOverSwap(true);
            dragPanel.addComponent(btn1);
            dragPanel.addComponent(btn2);
            //btnCollection.add(btn1);
            //btnCollection.add(btn2);
            //dragPanel.addComponent(btnCollection);
            Cursor cursor = new Cursor(leftAsMainHand, loadTexture("hand"), this);

            addComponent(background);
            addComponent(dragPanel);
            addComponent(cursor);
        }
    }
}
