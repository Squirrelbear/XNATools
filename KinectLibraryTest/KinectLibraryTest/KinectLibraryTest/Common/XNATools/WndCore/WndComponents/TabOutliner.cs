using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNATools.WndCore
{
    public class TabOutliner : WndComponent
    {
        protected TabOrder tabOrder;
        protected List<Rectangle> rects;

        public TabOutliner(Texture2D outline, TabOrder tabOrder, List<Rectangle> rects)
            : base (rects[0], outline)
        {
            this.tabOrder = tabOrder;
            this.rects = rects;
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            setRect(rects[tabOrder.getTabOrderID()]);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            //base.draw(spriteBatch);

            spriteBatch.Draw(texture, dest, new Rectangle(0, 0, texture.Width, texture.Height), Color.White);
        }
    }
}
