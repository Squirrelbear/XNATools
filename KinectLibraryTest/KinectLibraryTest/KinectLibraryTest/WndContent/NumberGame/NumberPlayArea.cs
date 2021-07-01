using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XNATools.WndCore;

namespace KinectLibraryTest
{
    public class NumberPlayArea : Panel
    {
        private NumberGameWnd wnd;
        private NumberPanel[,] panels;
        private Random gen;
        private bool twoHandMode;
        private int curLeftX, curLeftY, curRightX, curRightY;
        private int curSelX, curSelY;
        private int cellWidth, cellHeight;

        public NumberPlayArea(Rectangle dest, NumberGameWnd wnd)
            : base(dest)
        {
            this.wnd = wnd;
            this.panels = new NumberPanel[12, 9];
            SpriteFont font = wnd.loadFont("hugeFont");
            LayoutManger lm = new LayoutManger(dest, panels.GetLength(0), panels.GetLength(1));
            lm.setPadding(new Rectangle(3, 3, 3, 3));
            for (int y = 0; y < panels.GetLength(0); y++)
            {
                for (int x = 0; x < panels.GetLength(1); x++)
                {
                    panels[y, x] = new NumberPanel(x, y, lm.nextRect(), font, this);
                    addComponent(panels[y, x]);
                }
            }
            cellWidth = dest.Width / panels.GetLength(1);
            cellHeight = dest.Height / panels.GetLength(0);
            gen = new Random();
            twoHandMode = false;
            restartGame();
        }

        public void setTwoHandMode(bool twoHandMode)
        {
            this.twoHandMode = twoHandMode;

            restartGame();
        }

        public bool getTwoHandMode()
        {
            return twoHandMode;
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (!twoHandMode) return;

            bool leftValid = false, rightValid = false;
            Point oldPointLeft = wnd.getKinectInputManager().kinectVectorToPoint(wnd.getKinectInputManager().getOldGripPos(true), wnd.getRect());
            Point newPointLeft = wnd.getKinectInputManager().kinectVectorToPoint(wnd.getKinectInputManager().getGripPos(true), wnd.getRect());
            int oldLeftX = oldPointLeft.X / cellWidth;
            int oldLeftY = oldPointLeft.Y / cellHeight;
            if (oldLeftX >= 0 && oldLeftX < panels.GetLength(1) && oldLeftY >= 0 && oldLeftY < panels.GetLength(0))
            {
                panels[oldLeftY, oldLeftX].updateMouseEnterExit(oldPointLeft, newPointLeft);
            }
            curLeftX = newPointLeft.X / cellWidth;
            curLeftY = newPointLeft.Y / cellHeight; 
            if (curLeftX >= 0 && curLeftX < panels.GetLength(1) && curLeftY >= 0 && curLeftY < panels.GetLength(0))
            {
                panels[curLeftY, curLeftX].updateMouseEnterExit(oldPointLeft, newPointLeft);
                leftValid = true;
            }

            Point oldPointRight = wnd.getKinectInputManager().kinectVectorToPoint(wnd.getKinectInputManager().getOldGripPos(false), wnd.getRect());
            Point newPointRight = wnd.getKinectInputManager().kinectVectorToPoint(wnd.getKinectInputManager().getGripPos(false), wnd.getRect());
            int oldRightX = oldPointRight.X / cellWidth;
            int oldRightY = oldPointRight.Y / cellHeight;
            if (oldRightX >= 0 && oldRightX < panels.GetLength(1) && oldRightY >= 0 && oldRightY < panels.GetLength(0))
            {
                panels[oldRightY, oldRightX].updateMouseEnterExit(oldPointRight, newPointRight);
            }
            curRightX = newPointRight.X / cellWidth; 
            curRightY = newPointRight.Y / cellHeight;
            if (curRightX >= 0 && curRightX < panels.GetLength(1) && curRightY >= 0 && curRightY < panels.GetLength(0))
            {
                panels[curRightY, curRightX].updateMouseEnterExit(oldPointRight, newPointRight);
                rightValid = true;
            }

            if (leftValid && rightValid)
            {
                testTwoHandSelection();
            }
        }

        public bool testTwoHandSelection()
        {
            curSelX = curLeftX;
            curSelY = curLeftY;
            int x = curRightX;
            int y = curRightY;
            if ((curLeftX == -1 || curRightX == -1) || (curSelX != x && curSelY != y) || (curSelX == x && curSelY == y)
                || panels[curSelY, curSelX].getNumber() == 0 || panels[curSelY, curSelX].getNumber() == 0
                || (panels[curSelY, curSelX].getNumber() != panels[y, x].getNumber()
                        && panels[curSelY, curSelX].getNumber() + panels[y, x].getNumber() != 10))
            {
                return false;
            }
            else
            {
                Vector2 dirMod = new Vector2((x == curSelX) ? 0 : (x - curSelX) / Math.Abs((x - curSelX)),
                                        (y == curSelY) ? 0 : (y - curSelY) / Math.Abs((y - curSelY)));
                Vector2 checkPoint = new Vector2(curSelX, curSelY) + dirMod;
                Vector2 end = new Vector2(x, y);
                for (; !checkPoint.Equals(end); checkPoint += dirMod)
                {
                    if (panels[(int)checkPoint.Y, (int)checkPoint.X].getNumber() != 0)
                        return false;
                }
                int bonus = (Math.Abs((x - curSelX) + (y - curSelY)) - 1) * 2;
                panels[curSelY, curSelX].setNumber(0);
                panels[y, x].setNumber(0);
                wnd.addScore(10 + bonus);
                return true;
            }
        }

        // This method returns true only if the cell at x,y should be green
        public bool testSelection(int x, int y)
        {
            if (curSelX == -1)
            { // if there is no selection
                curSelX = x;
                curSelY = y;
                return true;
            }
            else if ((curSelX != x && curSelY != y) || (curSelX == x && curSelY == y)
                || (panels[curSelY, curSelX].getNumber() != panels[y, x].getNumber()
                 && panels[curSelY, curSelX].getNumber() + panels[y, x].getNumber() != 10))
            { // if not same row/col or not same and not add to 10
                panels[curSelY,curSelX].clearSelection();
                curSelY = curSelX = -1;
                return false;
            }
            else
            {
                Vector2 dirMod = new Vector2((x == curSelX) ? 0 : (x - curSelX) / Math.Abs((x - curSelX)),
                                        (y == curSelY) ? 0 : (y - curSelY) / Math.Abs((y - curSelY)));
                Vector2 checkPoint = new Vector2(curSelX, curSelY) + dirMod;
                Vector2 end = new Vector2(x, y);
                for (; !checkPoint.Equals(end); checkPoint += dirMod)
                {
                    if (panels[(int)checkPoint.Y, (int)checkPoint.X].getNumber() != 0)
                    {
                        panels[curSelY, curSelX].clearSelection();
                        curSelY = curSelX = -1;
                        return false;
                    }
                }
                int bonus = (Math.Abs((x - curSelX) + (y - curSelY)) - 1) * 2;
                panels[curSelY, curSelX].setNumber(0);
                panels[y, x].setNumber(0);
                wnd.addScore(10 + bonus);
                curSelY = curSelX = -1;
                return false;
            }
        }

        public void repopulateGame(bool clearAll)
        {
            for (int y = 0; y < panels.GetLength(0); y++)
            {
                for (int x = 0; x < panels.GetLength(1); x++)
                {
                    if (clearAll || panels[y,x].getNumber() == 0)
                        panels[y,x].setNumber(gen.Next(9) + 1);
                }
            }
            curSelX = curSelY = -1;
            curLeftX = curLeftY = curRightX = curRightY = -1;
        }

        public void restartGame()
        {
            repopulateGame(true);
        }

        public NumberGameWnd getParentWnd()
        {
            return wnd;
        }
    }
}
