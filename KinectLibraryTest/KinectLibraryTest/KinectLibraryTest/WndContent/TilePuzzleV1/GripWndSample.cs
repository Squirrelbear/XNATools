using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XNATools;
using XNATools.WndCore;
using XNATools.WndCore.Kinect;

namespace KinectLibraryTest
{
    public class GripWndSample : WndHandle
    {
        private KinectInputManager kinectInputManager;
        private Color c;
        private Label posLabel;
        private WndComponent handRight;//handLeft, handRight;
        private Tile[] tiles;
        private BackTile[] shadowCells;
        private Tile curTarget;

        private Vector2 rotHandLeft, rotHandRight;
        private float initialDistance;
        private bool rotate;
        private Timer pushCooldown;

        public GripWndSample(Rectangle displayRect, WndGroup parent)
            : base(WndFactory.getCode(WndFactory.WndCodes.GripWndSample), displayRect, parent) 
        {
            kinectInputManager = KinectInputManager.getSharedKinectInputManager(parent.getAppRef());
            posLabel = new Label(new Rectangle(10, 10, 50, 50), "Awaiting data...", loadFont("largeFont"));
            posLabel.setColor(Color.Blue);
            //handLeft = new WndComponent(new Rectangle(-50, -50, 50, 50), loadTexture("hand"));
            handRight = new WndComponent(new Rectangle(-50, -50, 50, 50), loadTexture("hand"));
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

            addComponent(posLabel);
            //addComponent(handLeft);
            addComponent(handRight);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            kinectInputManager.update(gameTime);

            if (c != Color.Orange)
            {
                if (kinectInputManager.getGripState(true) == KinectPlayer.GripState.GripReleased
                    && kinectInputManager.getGripState(false) == KinectPlayer.GripState.GripReleased)
                    c = Color.Black;
                else if (kinectInputManager.getGripState(true) == KinectPlayer.GripState.GripHeld
                    && kinectInputManager.getGripState(false) == KinectPlayer.GripState.GripReleased)
                    c = Color.LawnGreen;
                else if (kinectInputManager.getGripState(true) == KinectPlayer.GripState.GripReleased
                    && kinectInputManager.getGripState(false) == KinectPlayer.GripState.GripHeld)
                    c = Color.Red;
                else
                    c = Color.White;
            }

            Vector2 leftPos = kinectInputManager.getGripPos(true);
            Vector2 rightPos = kinectInputManager.getGripPos(false);
            String newPos = "Left: (" + leftPos.X + ", " + leftPos.Y + ")\n"
                            +"Right: (" + rightPos.X + ", " + rightPos.Y + ")"
                            + "\nPush: " + kinectInputManager.getPushValue(true, true);
            posLabel.setText(newPos);

            Vector2 scaledLeftPos = leftPos * new Vector2(displayRect.Width, displayRect.Height);
            Vector2 scaledRightPos = rightPos * new Vector2(displayRect.Width, displayRect.Height);

            if (curTarget != null)
            {
                curTarget.setLocation(scaledRightPos - (curTarget.getSize() / 2));
            }

            if (kinectInputManager.getGripBegun(false) && curTarget == null)
            {
                foreach (Tile tile in tiles)
                {
                    if (tile.getRect().Contains(new Point((int)scaledRightPos.X, (int)scaledRightPos.Y)))
                    {
                        curTarget = tile;
                        break;
                    }
                }
            }
            else if ( curTarget != null
                      && kinectInputManager.getGripState(false) == KinectPlayer.GripState.GripHeld
                     && kinectInputManager.getGripBegun(true))
            {
                rotate = true;
                rotHandLeft = leftPos;
                rotHandRight = rightPos;
                initialDistance = Vector2.Distance(rotHandLeft, rotHandRight);
            }
            else if (rotate && kinectInputManager.getGripEnded(true))
            {
                rotate = false;

                // fixes the issue where releasing both hands at same time keeps holding
                if (kinectInputManager.getGripEnded(false))
                {
                    curTarget = null;
                }
            }
            else if (rotate)
            {
                float curDistance = Vector2.Distance(leftPos, rightPos);
                int modDistance = (int)((curDistance) / 0.25f);
                curTarget.setRotation(modDistance);
            }
            /*else if (kinectInputManager.getGripEnded(false))
            {
                curTarget = null;
                rotate = false;
            }*/

            if (pushCooldown != null)
            {
                pushCooldown.update(gameTime);
                if (pushCooldown.wasTriggered())
                    pushCooldown = null;
            }

            if (curTarget != null && pushCooldown == null &&
                kinectInputManager.getPush(true, true) &&
                kinectInputManager.getPushValue(true, true) >= 1)
            {
                BackTile targetCell = null;
                foreach (BackTile s in shadowCells)
                {
                    if (s.getRect().Contains(new Point((int)scaledRightPos.X, (int)scaledRightPos.Y)))
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
                        if (t != curTarget && 
                            targetCell.getRect().Contains(t.getRect().Center))
                        {
                            newCell = t;
                            break;
                        }
                    }
                    curTarget = newCell;

                    if (curTarget == null && isGameOver())
                        c = Color.Orange;
                    rotate = false;

                    pushCooldown = new Timer(3000);
                }
            }

            //handLeft.setLocation(scaledLeftPos);
            handRight.setLocation(scaledRightPos);
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
