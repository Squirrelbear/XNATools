using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XNATools.WndCore;

namespace KinectLibraryTest
{
    public class NumberScorePanel : Panel
    {
        private NumberGameWnd wnd;
        private int score, pairs;
        private Label scoreLabel, pairsLabel;
        private ButtonCollection menu;
        private TextButton handModeBtn;

        public NumberScorePanel(Rectangle dest, NumberGameWnd wnd)
            : base(dest)
        {
            this.wnd = wnd;
            Texture2D bgColor = new Texture2D(wnd.getParent().getAppRef().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            bgColor.SetData(new[] { Color.LightGray });
            setTexture(bgColor);

            SpriteFont font = wnd.loadFont("hugeFont");
            LayoutManger lm = new LayoutManger(new Rectangle(dest.X, dest.Y, dest.Width, (int)(0.3 * dest.Height)), 3, 1);
            lm.setPadding(new Rectangle(5, 0, 5, 0));
            Label titleLabel = new Label(lm.nextRect(), "Number Game", font);
            titleLabel.centreInRect();
            scoreLabel = new Label(lm.nextRect(), "Score: 0", font);
            scoreLabel.centreInRect(Label.CentreMode.CentreVertical);
            pairsLabel = new Label(lm.nextRect(), "Pairs: 0", font);
            pairsLabel.centreInRect(Label.CentreMode.CentreVertical);
            addComponent(titleLabel);
            addComponent(scoreLabel);
            addComponent(pairsLabel);

            Texture2D nonSel = new Texture2D(wnd.getParent().getAppRef().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            nonSel.SetData(new[] { Color.DimGray });
            Texture2D sel = new Texture2D(wnd.getParent().getAppRef().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            sel.SetData(new[] { Color.Black });

            Rectangle bottomRect = new Rectangle(dest.X, dest.Bottom - (int)(0.4 * dest.Height), dest.Width, (int)(0.4 * dest.Height));
            lm = new LayoutManger(bottomRect, 4, 1);
            lm.setPadding(new Rectangle(10,10,10,10));
            handModeBtn = new TextButton(lm.nextRect(), "One Hand", font, sel, nonSel, false, 3);
            handModeBtn.setSelectedFontColor(Color.White);
            handModeBtn.setFontColor(Color.White);
            TextButton repopBtn = new TextButton(lm.nextRect(), "Repopulate", font, sel, nonSel, false, 0);
            repopBtn.setSelectedFontColor(Color.White);
            repopBtn.setFontColor(Color.White);
            TextButton restartBtn = new TextButton(lm.nextRect(), "Restart", font, sel, nonSel, false, 1);
            restartBtn.setSelectedFontColor(Color.White);
            restartBtn.setFontColor(Color.White);
            TextButton quitBtn = new TextButton(lm.nextRect(), "Quit", font, sel, nonSel, false, 2);
            quitBtn.setSelectedFontColor(Color.White);
            quitBtn.setFontColor(Color.White);

            menu = new ButtonCollection(bottomRect);
            menu.setKeepOneSelected(false);
            menu.add(handModeBtn);
            menu.add(repopBtn);
            menu.add(restartBtn);
            menu.add(quitBtn);
            addComponent(menu);
        }

        public void addScore(int score)
        {
            this.score += score;
            this.pairs++;
            scoreLabel.setText("Score: " + this.score);
            pairsLabel.setText("Pairs: " + pairs);
        }

        public void resetScore()
        {
            this.score = this.pairs = 0;
            scoreLabel.setText("Score: " + score);
            pairsLabel.setText("Pairs: " + pairs);
        }

        public override void update(GameTime gameTime)
        {
            if (menu.isBtnClicked())
            {
                if (menu.getSelected().getActionID() == 0)
                {
                    wnd.repopulateGame();
                }
                else if (menu.getSelected().getActionID() == 1)
                {
                    wnd.restartGame();
                }
                else if (menu.getSelected().getActionID() == 2)
                {
                    ((KinectGame)wnd.getParent().getAppRef()).exitGame();
                }
                else if (menu.getSelected().getActionID() == 3)
                {
                    bool next = !wnd.getTwoHandMode();
                    wnd.setTwoHandMode(next);
                    handModeBtn.setText((next) ? "Two Hands" : "One Hand");
                }
            }

            base.update(gameTime);
        }
    }
}
