using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XNATools.WndCore;
using XNATools;

namespace KinectLibraryTest
{
    public class DrawingCtrlPanel : Panel
    {
        protected Slider sizeSlider;
        protected Label brushLabel;
        protected DrawingGameWnd drawWnd;
        protected Color[] colours = { Color.Black, Color.Red, Color.Green, Color.Blue, Color.White };
        protected string[] shapes = { "Polygon", "Circle" };
        protected ButtonCollection btnColorCollection, btnShapeCollection;

        public DrawingCtrlPanel(Rectangle dest, DrawingGameWnd wnd)
            : base(dest)
        {
            this.drawWnd = wnd;
            ImageTools imgTools = ImageTools.getSingleton();
            Texture2D blank = imgTools.createColorTexture(Color.LightGray);
            Rectangle sliderRect = new Rectangle(dest.X + 30, dest.Y + 30, dest.Width - 60, (int)(0.25 * dest.Height));
            sizeSlider = new Slider(sliderRect, 5, 100, 20, wnd, true);
            SpriteFont font = wnd.loadFont("hugeFont");
            brushLabel = new Label(new Rectangle(sliderRect.X, sliderRect.Bottom + 50, 50, 50), "Brush Size: " + 20, font);

            addComponent(new WndComponent(dest, blank));
            addComponent(sizeSlider);
            addComponent(brushLabel);

            Rectangle bottomRect = new Rectangle(dest.X, dest.Bottom - (int)(0.6 * dest.Height), dest.Width, (int)(0.6 * dest.Height));
            LayoutManger lm = new LayoutManger(bottomRect, 7, 1);
            lm.setPadding(new Rectangle(10, 10, 10, 10));

            btnShapeCollection = new ButtonCollection(bottomRect);
            Texture2D btnShapeNotSelected = imgTools.createColorTexture(Color.DarkGray);
            Texture2D btnShapeSelected = imgTools.createColorTexture(Color.Black, lm.getRect(0, 0).Width, lm.getRect(0, 0).Height);
            btnShapeSelected = imgTools.fillRect(btnShapeSelected, new Rectangle(3, 3, lm.getRect(0, 0).Width - 6, lm.getRect(0, 0).Height - 6), Color.DarkGray);
            for (int i = 0; i < shapes.GetLength(0); i++)
            {
                TextButton btn = new TextButton(lm.nextRect(), shapes[i], font, btnShapeSelected, btnShapeNotSelected, (i == 0), i);
                btn.setFontColor(Color.White);
                btn.setSelectedFontColor(Color.White);
                btnShapeCollection.add(btn);
            }

            btnColorCollection = new ButtonCollection(bottomRect);
            for (int i = 0; i < colours.GetLength(0); i++)
            {
                Texture2D buttonBG = imgTools.createColorTexture(colours[i]);
                TextButton btn = new TextButton(lm.nextRect(), "", "Active", font, buttonBG, buttonBG, (i == 0), i);
                btn.setFontColor((i == 0) ? Color.White : Color.Black);
                btn.setSelectedFontColor((i == 0) ? Color.White : Color.Black);
                btnColorCollection.add(btn);
            }
            addComponent(btnShapeCollection);
            addComponent(btnColorCollection);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);
            if (sizeSlider.getHasChanged())
            {
                brushLabel.setText("Brush Size: " + sizeSlider.getValue().ToString());
                drawWnd.BrushSize = sizeSlider.getValue();
            }

            drawWnd.DrawColor = colours[btnColorCollection.getSelected().getActionID()];
            drawWnd.DrawShapeType = btnShapeCollection.getSelected().getActionID();
        }
    }
}
