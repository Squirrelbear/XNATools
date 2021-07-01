using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using XNATools.WndCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNATools.WndExtra
{
    public class TreeWnd : WndHandle
    {
        protected Button closeButton;
        protected TreeView treeView;

        public TreeWnd(Rectangle displayRect, WndGroup parent)
            : base(778, displayRect, parent)
        {

            ImageTools imgTools = ImageTools.getSingleton(parent.getAppRef());
            SpriteFont fontSmall = loadFont("smallFont");
            Rectangle dragBarDrawRect = new Rectangle(3, 3, displayRect.Width - 6, 15);
            Rectangle graphBGDrawRect = new Rectangle(3, dragBarDrawRect.Bottom + 3, dragBarDrawRect.Width,
                                                        displayRect.Height - 9 - dragBarDrawRect.Height);

            Rectangle dragBarRect = new Rectangle(dragBarDrawRect.X + displayRect.X, dragBarDrawRect.Y + displayRect.Y,
                                                    dragBarDrawRect.Width, dragBarDrawRect.Height);

            Rectangle graphRect = new Rectangle(graphBGDrawRect.X + displayRect.X, graphBGDrawRect.Y + displayRect.Y,
                                                    graphBGDrawRect.Width, graphBGDrawRect.Height);

            Texture2D background = imgTools.createColorTexture(Color.Black * 0.7f, displayRect.Width, displayRect.Height);
            background = imgTools.fillRect(background, dragBarDrawRect, Color.DarkGray * 0.8f);
            background = imgTools.fillRect(background, graphBGDrawRect, Color.LightGray * 0.6f);
            addComponent(new WndComponent(displayRect, background));
            addComponent(new GrabBar(dragBarRect, this));

            Texture2D closeButtonImg = imgTools.createColorTexture(Color.Maroon, 11, 11);
            closeButtonImg = imgTools.drawLine(closeButtonImg, 1, 1, 9, 9, Color.Black);
            closeButtonImg = imgTools.drawLine(closeButtonImg, 9, 1, 1, 9, Color.Black);
            Texture2D closeButtonSelImg = imgTools.createColorTexture(Color.Red, 11, 11);
            closeButtonSelImg = imgTools.drawLine(closeButtonSelImg, 1, 1, 9, 9, Color.Black);
            closeButtonSelImg = imgTools.drawLine(closeButtonSelImg, 9, 1, 1, 9, Color.Black);
            closeButton = new Button(new Rectangle(dragBarRect.Right - 13, dragBarRect.Top + 2, 11, 11), closeButtonSelImg, closeButtonImg);
            closeButton.setEnableMouseOverSwap(true);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (closeButton.getIsClicked())
            {
                parent.removeWnd(this);
                return;
            }
        }
    }
}
