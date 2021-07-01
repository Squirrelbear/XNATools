using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using XNATools.WndCore;

namespace KinectLibraryTest
{
    public class MainMenu : WndHandle
    {
        protected int c_i;
        protected Color c;

        public MainMenu(Rectangle displayRect, WndGroup parent)
            : base(WndFactory.getCode(WndFactory.WndCodes.MainMenu), displayRect, parent)
        {
            c = Color.Red;
            c_i = 0;

            parent.addWnd(WndFactory.createWnd(WndFactory.WndCodes.TestDialog, new Rectangle(50, 50, 400, 500), parent));
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (inputManager.isKeyPressed(Keys.C))
            {
                c_i++;
                if (c_i > 2)
                    c_i = 0;

                if (c_i == 0) c = Color.Red;
                else if (c_i == 1) c = Color.Green;
                else c = Color.Blue;
            }
            else if (inputManager.isKeyPressed(Keys.V))
            {
                parent.addWnd(WndFactory.createWnd(WndFactory.WndCodes.TestDialog, new Rectangle(50, 50, 400, 500), parent));
            }
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);

            parent.getAppRef().GraphicsDevice.Clear(c);
        }
    }
}
