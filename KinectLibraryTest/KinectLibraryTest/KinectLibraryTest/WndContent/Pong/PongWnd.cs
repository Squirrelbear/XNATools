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
    public class PongWnd : KinectWndHandle
    {
        protected PongPaddle paddleOne, paddleTwo;
        protected int scoreOne, scoreTwo;
        protected Label labelOne, labelTwo;

        public PongWnd(Rectangle displayRect, WndGroup parent)
            : base(WndFactory.getCode(WndFactory.WndCodes.PongWnd), displayRect, parent)
        {
            kinectInteraction = KinectAutoInteraction.AsKinectOnly;

            int paddleHeight = displayRect.Height / 5, paddleWidth = 35;
            paddleOne = new PongPaddle(new Rectangle(10, displayRect.Height / 2 - paddleHeight / 2, paddleWidth, paddleHeight), this);
            paddleTwo = new PongPaddle(new Rectangle(displayRect.Width - 10 - paddleWidth, 
                                        displayRect.Height / 2 - paddleHeight / 2, paddleWidth, paddleHeight), this);
            PongBall ball = new PongBall(new Rectangle(displayRect.Width / 2 - 10, displayRect.Height / 2 - 10, 20, 20), this);

            scoreOne = scoreTwo = 0;
            SpriteFont font = loadFont("hugeFont");
            Vector2 fontDim = font.MeasureString("Score: 0000");
            labelOne = new Label(new Rectangle(10, 10, (int)fontDim.X, (int)fontDim.Y), "Score: 0", font);
            labelOne.setColor(Color.Red);
            labelTwo = new Label(new Rectangle(displayRect.Right - (int)fontDim.X - 10, 10, (int)fontDim.X, 
                                (int)fontDim.Y), "Score: 0", font);
            labelTwo.setColor(Color.Red);
            addComponent(paddleOne);
            addComponent(paddleTwo);
            addComponent(ball);
            addComponent(labelOne);
            addComponent(labelTwo);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Black);
            base.draw(spriteBatch);
        }

        public override void handMoved(Point oldP, Point newP, bool isLeft, int playerID = 0)
        {
            base.handMoved(oldP, newP, isLeft, playerID);

            if (!isLeft && playerID == 0) paddleOne.mouseMoved(oldP, newP);
            else if (!isLeft && playerID == 1) paddleTwo.mouseMoved(oldP, newP);
        }

        public override void handGripBegun(Point p, bool isLeft, int playerID = 0)
        {
            base.handGripBegun(p, isLeft, playerID);

            if (!isLeft && playerID == 0) paddleOne.mousePressedRight(p);
            else if (!isLeft && playerID == 1) paddleTwo.mousePressedRight(p);
        }

        public override void handGripEnded(Point p, bool isLeft, int playerID = 0)
        {
            base.handGripEnded(p, isLeft, playerID);

            if (!isLeft && playerID == 0) paddleOne.mouseClickedRight(p);
            else if (!isLeft && playerID == 1) paddleTwo.mouseClickedRight(p);
        }

        public PongPaddle getPaddle(bool playerOne)
        {
            return (playerOne) ? paddleOne : paddleTwo;
        }

        public void increaseScore(bool playerOne)
        {
            if (playerOne)
            {
                scoreOne++;
                labelOne.setText("Score: " + scoreOne);
            }
            else
            {
                scoreTwo++;
                labelTwo.setText("Score: " + scoreTwo);
            }
        }
    }
}
