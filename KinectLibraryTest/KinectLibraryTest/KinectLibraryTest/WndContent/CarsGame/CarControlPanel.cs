using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNATools.WndCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

namespace KinectLibraryTest
{
    public class CarControlPanel : Panel
    {
        private Label scoreLabel, dodgeLabel, crashLabel;
        private ButtonCollection btnCollection;
        private CarGameWnd mp;

        public CarControlPanel(Rectangle rect, CarGameWnd mp)
            : base(rect)
        {
            this.mp = mp;
            ImageTools imgTools = ImageTools.getSingleton();
            LayoutManger lm = new LayoutManger(new Rectangle(rect.Left, rect.Top, rect.Width, (int)(rect.Height * 0.25)), 3, 1);
            SpriteFont font = mp.loadFont("largeFont");
            scoreLabel = new Label(lm.nextRect(), "Score: 0", font);
            dodgeLabel = new Label(lm.nextRect(), "Dodged: 0", font);
            crashLabel = new Label(lm.nextRect(), "Crashed: 0", font);
            addComponent(scoreLabel);
            addComponent(dodgeLabel);
            addComponent(crashLabel);

            Rectangle btnRectangle = new Rectangle(rect.Left, rect.Bottom - (int)(rect.Height * 0.3) - 10, rect.Width, (int)(rect.Height * 0.3));
            lm = new LayoutManger(btnRectangle, 4, 1);
            btnCollection = new ButtonCollection(btnRectangle);
            Texture2D btnShapeNotSelected = imgTools.createColorTexture(Color.DarkGray);
            Texture2D btnShapeSelected = imgTools.createColorTexture(Color.Black, lm.getRect(0, 0).Width, lm.getRect(0, 0).Height);
            btnShapeSelected = imgTools.fillRect(btnShapeSelected, new Rectangle(3, 3, lm.getRect(0, 0).Width - 6, lm.getRect(0, 0).Height - 6), Color.DarkGray);
            btnCollection.add(new TextButton(lm.nextRect(), "High Scores", font, btnShapeSelected, btnShapeNotSelected, false, 0));
            btnCollection.add(new TextButton(lm.nextRect(), "Quit", font, btnShapeSelected, btnShapeNotSelected, false, 1));
            btnCollection.add(new TextButton(lm.nextRect(), "New Game", font, btnShapeSelected, btnShapeNotSelected, false, 2));
            btnCollection.add(new TextButton(lm.nextRect(), "Pause", font, btnShapeSelected, btnShapeNotSelected, false, 3));
            addComponent(btnCollection);
        }

        public override void update(GameTime gameTime)
        {
            //btnCollection.update(gameTime);
            if (btnCollection.isBtnClicked())
            {
                switch (btnCollection.getSelected().getActionID())
                {
                    case 0:
                        mp.showHighScores();
                        break;
                    case 1:
                        ((KinectGame)mp.getParent().getAppRef()).exitGame();
                        break;
                    case 2:
                        mp.newGame();
                        break;
                    case 3:
                        mp.togglePause();
                        break;
                }
            }

            base.update(gameTime);
        }

        public void updateScore(int score, int dodged)
        {
            scoreLabel.setText("Score: " + score);
            dodgeLabel.setText("Dodged: " + dodged);
        }

        public void updateCrashes(int crashes)
        {
            crashLabel.setText("Crashes: " + crashes);
        }

        public void setPauseBtnText(bool isPaused)
        {
            ((TextButton)btnCollection.getButtons()[3]).setText((isPaused) ? "Play" : "Pause");
        }
    }
}
