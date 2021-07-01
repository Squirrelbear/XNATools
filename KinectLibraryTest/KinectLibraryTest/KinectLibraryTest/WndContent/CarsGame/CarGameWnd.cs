using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNATools.WndCore;
using Microsoft.Xna.Framework;
using XNATools;

namespace KinectLibraryTest
{
    public class CarGameWnd : WndHandle
    {
        private CarControlPanel cp;
        private CarRoadPanel rp;
        private CarScorePanel sp;
        private TabbedPanel mp;

        public CarGameWnd(Rectangle rect, WndGroup parent)
            : base(8888, rect, parent)
        {
            ImageTools imgTools = ImageTools.getSingleton(parent.getAppRef());

            Rectangle mainPanelRect = new Rectangle(rect.Left, rect.Top, (int)(rect.Width * 0.7), rect.Height);
            Rectangle controlPanelRect = new Rectangle(mainPanelRect.Right, rect.Top, rect.Width - mainPanelRect.Width, rect.Height);

            mp = new TabbedPanel(mainPanelRect);
            cp = new CarControlPanel(controlPanelRect, this);
            rp = new CarRoadPanel(mainPanelRect, this);
            sp = new CarScorePanel(mainPanelRect, this);
            mp.addPanel(rp);
            mp.addPanel(sp);
            addComponent(cp);
            addComponent(mp);
        }

        public void showHighScores()
        {
            mp.setPanel(1);
        }

        public void newGame()
        {
            mp.setPanel(0);
            rp.newGame(true);
        }

        public void togglePause()
        {
            rp.Paused = !rp.Paused;
        }

        public void updateScore(int score, int dodged)
        {
            cp.updateScore(score, dodged);
        }

        public void updateCrashes(int crashes)
        {
            cp.updateCrashes(crashes);
        }

        public float getCarYPosV(int i)
        {
            return rp.getCarYPosV(i);
        }

        public float getCarYPos()
        {
            return rp.getCarYPos();
        }
    }
}
