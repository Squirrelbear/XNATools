using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNATools
{
    public class CollisionTools
    {
        public static bool pointInOval(Vector2 point, Rectangle ovalBounds)
        {
            float dx = (point.X - ovalBounds.Center.X) / (ovalBounds.Width/2);
            float dy = (point.Y - ovalBounds.Center.Y) / (ovalBounds.Height/2);

            return dx * dx + dy * dy < 1;
        }
    }
}
