using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XNATools.WndCore;
using XNATools.WndCore.Kinect;

namespace KinectLibraryTest
{
    public class NumberGameWnd : KinectWndHandle
    {
        private NumberPlayArea npa;
        private NumberScorePanel sp;

        public NumberGameWnd(Rectangle displayRect, WndGroup parent)
            : base(WndFactory.getCode(WndFactory.WndCodes.NumberGameWnd), displayRect, parent)
        {
            kinectInteraction = KinectAutoInteraction.AsMouseOnly;
            leftAsMainHand = false;
            Rectangle gameRect = new Rectangle(displayRect.X, displayRect.Y, (int)(0.8 * displayRect.Width), displayRect.Height);
            npa = new NumberPlayArea(gameRect, this);
            addComponent(npa);
            Rectangle scoreRect = new Rectangle(gameRect.Right, displayRect.Y, displayRect.Width - gameRect.Width, displayRect.Height);
            sp = new NumberScorePanel(scoreRect, this);
            addComponent(sp);
            Cursor cursorleft = new Cursor(leftAsMainHand, loadTexture("hand"), this);
            addComponent(cursorleft);
            Cursor cursorright = new Cursor(!leftAsMainHand, loadTexture("hand"), this);
            addComponent(cursorright);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Black);
            base.draw(spriteBatch);
        }

        public void addScore(int score)
        {
            sp.addScore(score);
        }

        public void restartGame()
        {
            npa.restartGame();
            sp.resetScore();
        }

        public void repopulateGame()
        {
            npa.repopulateGame(false);
        }

        public bool getTwoHandMode()
        {
            return npa.getTwoHandMode();
        }

        public void setTwoHandMode(bool twoHandMode)
        {
            npa.setTwoHandMode(twoHandMode);
        }
    }
}
