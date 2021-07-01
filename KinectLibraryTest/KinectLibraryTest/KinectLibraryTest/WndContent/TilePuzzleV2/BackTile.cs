using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XNATools.WndCore;

namespace KinectLibraryTest
{
    public class BackTile : WndComponent
    {
        public int solution;

        public BackTile(Rectangle dest, Texture2D texture, int solution)
            : base(dest, texture)
        {
            this.solution = solution;
        }

        public int getSolution()
        {
            return solution;
        }
    }
}
