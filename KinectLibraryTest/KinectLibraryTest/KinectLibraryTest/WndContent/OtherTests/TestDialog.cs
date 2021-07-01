using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XNATools.WndCore;
using XNATools.WndExtra;

namespace KinectLibraryTest
{
    public class TestDialog : WndHandle
    {
        protected Button testButton;

        public TestDialog(Rectangle displayRect, WndGroup parent)
            : base(WndFactory.getCode(WndFactory.WndCodes.TestDialog), displayRect, parent)
        {
            SpriteFont font = loadFont("largeFont");
            WndComponent background = new WndComponent(displayRect, loadTexture("DialogBackground"));
            addComponent(background);

            LayoutManger lm = new LayoutManger(displayRect, 4, 2);
            for (int i = 0; i < 8; i++)
            {
                if (i == 1)
                {
                    testButton = new Button(lm.nextRect(), loadTexture("DialogBackground"), loadTexture("mainmenubg"));
                    addComponent(testButton);
                }
                else
                {
                    Label testLabel = new Label(lm.nextRect(), "Sample " + i, font);
                    addComponent(testLabel);
                }
            }

            Rectangle grabRect = new Rectangle(displayRect.X, displayRect.Y, displayRect.Width, 30);
            addComponent(new GrabBar(grabRect, this));
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (testButton.getIsClicked())
            {
                parent.removeWnd(this);
            }
        }
    }
}
