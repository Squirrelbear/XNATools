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
    public class Tile : WndComponent
    {
        protected AnimatedObject tile;
        protected int solution;
        protected bool isSolution;

        public Tile(Rectangle dest, Texture2D texture, int solution, int initRotation)
            : base(dest)
        {
            this.solution = solution;

            tile = new AnimatedObject(texture, 265,265, dest);
            tile.setDefaultOrigin();
            tile.setLocation(dest.X + dest.Width / 2, dest.Y + dest.Height / 2);
            setRotation(initRotation);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);

            tile.draw(spriteBatch);
        }

        public override void setLocation(Vector2 location)
        {
            base.setLocation(location);

            if (tile != null)
                tile.setLocation(location.X + dest.Width / 2, location.Y + dest.Height / 2);
        }

        public override void setRect(Rectangle dest)
        {
            base.setRect(dest);

            if(tile != null)
                tile.setDest(dest);
        }

        public AnimatedObject getAnimatedObject()
        {
            return tile;
        }


        // 0,1,2,3
        public int getRotation()
        {
            return (int)(tile.getRotation() / (Math.PI / 2));
        }

        public void setRotation(int rotation)
        {
            tile.setRotation((float)(rotation * (Math.PI / 2)));
        }

        public void setModRotation(int modRotation)
        {
            setRotation(getRotation() + modRotation);
        }

        public void updateIsSolution(BackTile back)
        {
            isSolution = (getRotation() % 4 == 0) && back.getSolution() == solution;
        }

        public bool getIsSolution()
        {
            return isSolution;
        }
    }
}
