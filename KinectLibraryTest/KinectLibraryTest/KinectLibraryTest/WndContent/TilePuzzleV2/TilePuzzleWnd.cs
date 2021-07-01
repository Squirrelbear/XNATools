using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect;

using XNATools.WndCore;
using XNATools.WndCore.Kinect;

namespace KinectLibraryTest
{
    public class TilePuzzleWnd : KinectWndHandle
    {
        private Color c;
        private Tile[] tiles;
        private BackTile[] shadowCells;
        private Tile curTarget;

        private bool rotate;
        private int initialRot;

        public TilePuzzleWnd(Rectangle displayRect, WndGroup parent)
            : base(WndFactory.getCode(WndFactory.WndCodes.GripWndSample), displayRect, parent) 
        {
            kinectInteraction = KinectAutoInteraction.AsKinectOnly;
            Cursor cursor = new Cursor(false, loadTexture("hand"), this);
            c = Color.Black;

            int dimension = (displayRect.Width > displayRect.Height) ? displayRect.Height : displayRect.Width;
            dimension = (int)(dimension * 0.8);
            rotate = false;

            Random gen = new Random();
            List<int> unassignedVal = new List<int>();
            for (int i = 1; i <= 9; i++)
            {
                int nextSol = -1;
                while (true)
                {
                    nextSol = gen.Next(1, 10);
                    if (!unassignedVal.Contains(nextSol))
                        break;
                }
                unassignedVal.Add(nextSol);
            }

            LayoutManger lm = new LayoutManger(new Rectangle(displayRect.Center.X - dimension/2,
                                                            displayRect.Center.Y - dimension/2,
                                                            dimension, dimension), 3, 3);
            lm.setPadding(new Rectangle(5, 5, 5, 5));
            tiles = new Tile[9];
            shadowCells = new BackTile[9];
            for (int i = 1; i <= 9; i++)
            {
                tiles[i - 1] = new Tile(lm.nextRect(), loadTexture("s" + unassignedVal[i - 1]), unassignedVal[i - 1], gen.Next(0, 4));
                shadowCells[i - 1] = new BackTile(tiles[i - 1].getRect(), loadTexture("DialogBackground"), i);
                tiles[i - 1].updateIsSolution(shadowCells[i-1]);
            }

            for (int i = 0; i < 9; i++)
            {
                addComponent(shadowCells[i]);
            }
            for (int i = 0; i < 9; i++)
            {
                addComponent(tiles[i]);
            }
            addComponent(cursor);
        }

        public override void handGripBegun(Point p, bool isLeft, int playerID = 0)
        {
            if (curTarget == null && !isLeft)
            {
                // find a tile to pick up
                foreach (Tile tile in tiles)
                {
                    if (tile.getRect().Contains(new Point(p.X, p.Y)))
                    {
                        curTarget = tile;
                        break;
                    }
                }
            }
            else if (curTarget != null && isLeft)
            {
                // begin rotation
                rotate = true;
                initialRot = curTarget.getRotation();
            }
        }

        public override void handGripEnded(Point p, bool isLeft, int playerID = 0)
        {
            if (curTarget != null && !isLeft)
            {
                // place down tile
                BackTile targetCell = null;
                foreach (BackTile s in shadowCells)
                {
                    if (s.getRect().Contains(new Point(p.X, p.Y)))
                    {
                        targetCell = s;
                        break;
                    }
                }

                if (targetCell != null)
                {
                    curTarget.setLocation(targetCell.getLocation());
                    curTarget.updateIsSolution(targetCell);

                    Tile newCell = null;
                    foreach (Tile t in tiles)
                    {
                        if (t != curTarget && targetCell.getRect().Contains(t.getRect().Center))
                        {
                            newCell = t;
                            break;
                        }
                    }
                    curTarget = newCell;

                    if (curTarget == null && isGameOver())
                        c = Color.Orange;
                    rotate = false;
                }
            }
            else if (isLeft)
            {
                // release left hand
                rotate = false;
            }
        }

        public override void handMoved(Point oldP, Point newP, bool isLeft, int playerID = 0)
        {
            if (curTarget != null && !isLeft)
            {
                curTarget.setLocation(new Vector2(newP.X, newP.Y) - (curTarget.getSize() / 2));
            }
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (curTarget != null && rotate)
            {
                int newRotation = initialRot - (int)(kinectInputManager.getJointRotationOnZAxis(JointType.HandLeft)*1.5);
                curTarget.setRotation(newRotation);
            }
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            parent.getAppRef().GraphicsDevice.Clear(c);

            base.draw(spriteBatch);

            if (curTarget != null)
                curTarget.draw(spriteBatch);
        }

        public bool isGameOver()
        {
            foreach (Tile t in tiles)
            {
                if (!t.getIsSolution())
                    return false;
            }
            return true;
        }
    }
}
