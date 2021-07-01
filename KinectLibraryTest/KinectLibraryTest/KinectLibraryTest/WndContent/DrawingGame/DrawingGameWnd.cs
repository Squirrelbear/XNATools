using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XNATools.WndCore;
using XNATools.WndCore.Kinect;
using XNATools;

namespace KinectLibraryTest
{
    public class DrawingGameWnd : KinectWndHandle
    {
        protected Texture2D drawingSurface;
        protected Color[] colourArray;
        protected Rectangle drawingArea;
        protected bool enableDraw;
        protected bool changedImage;
        protected bool smoothLine;
        public Color DrawColor { get; set; }
        public int BrushSize { get; set; }
        public int DrawShapeType { get; set; }

        protected ImageTools imageTools;

        public DrawingGameWnd(Rectangle displayRect, WndGroup parent)
            : base((int)WndFactory.WndCodes.DrawingGameWnd, displayRect, parent)
        {
            imageTools = ImageTools.getSingleton(parent.getAppRef());

            drawingArea = new Rectangle(displayRect.X, displayRect.Y, (int)(displayRect.Width * 0.8), displayRect.Height);
            drawingSurface = imageTools.createColorTexture(Color.White, drawingArea.Width, drawingArea.Height);
            colourArray = new Color[drawingArea.Width * drawingArea.Height];
            drawingSurface.GetData(colourArray);
            enableDraw = false;
            DrawColor = Color.Black;
            BrushSize = 20;
            DrawShapeType = 0;

            /*// Draw polygon example
            List<Point> polygon = new List<Point>();
            polygon.Add(new Point(200, 200));
            polygon.Add(new Point(200 - 2 * BrushSize, 200 - 2 * BrushSize));
            polygon.Add(new Point(200 - 2 * BrushSize, 200 + 2 * BrushSize));
            polygon.Add(new Point(200 + 2 * BrushSize, 200 - 2 * BrushSize));
            polygon.Add(new Point(200 + 2 * BrushSize, 200 + 2 * BrushSize));
            polygon.Add(new Point(200, 200));
            colourArray = imageTools.drawPolygon(colourArray, polygon, drawingArea.Width, drawingArea.Height, Color.Black);
            changedImage = true;*/

            Rectangle controlRect = new Rectangle(drawingArea.Right, displayRect.Y, (int)(displayRect.Width * 0.2), displayRect.Height);
            DrawingCtrlPanel controls = new DrawingCtrlPanel(controlRect, this);
            addComponent(controls);

            Cursor cursor = new Cursor(leftAsMainHand, loadTexture("hand"), this);
            addComponent(cursor);
            smoothLine = true;
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            parent.getAppRef().GraphicsDevice.Clear(Color.White);
            if (changedImage)
            {
                drawingSurface.SetData(colourArray);
                changedImage = false;
            }
            spriteBatch.Draw(drawingSurface, drawingArea, Color.White);
            base.draw(spriteBatch);
        }

        public override void mousePressedLeft(Point p)
        {
            base.mousePressedLeft(p);

            if (drawingArea.Contains(p))
            {
                enableDraw = true;
            }
        }

        public override void mouseClickedLeft(Point p)
        {
            base.mouseClickedLeft(p);

            enableDraw = false;
        }

        public override void mouseMoved(Point oldP, Point newP)
        {
            base.mouseMoved(oldP, newP);

            if (!drawingArea.Contains(newP))
            {
                enableDraw = false;
            }

            if (enableDraw)
            {
                if (smoothLine && DrawShapeType != 0)
                {
                    drawSmoothed(oldP.X, oldP.Y, newP.X, newP.Y, drawingSurface.Width, drawingSurface.Height, DrawColor);
                }
                else
                {
                    drawPoint(newP);
                }
            }
        }

        public void drawPoint(Point newP)
        {
            switch (DrawShapeType)
            {
                case 0:
                    List<Vector2> polygon = new List<Vector2>();
                    polygon.Add(new Vector2(newP.X, newP.Y));
                    polygon.Add(new Vector2(newP.X - 2 * BrushSize, newP.Y - 2 * BrushSize));
                    polygon.Add(new Vector2(newP.X - 2 * BrushSize, newP.Y + 2 * BrushSize));
                    polygon.Add(new Vector2(newP.X + 2 * BrushSize, newP.Y - 2 * BrushSize));
                    polygon.Add(new Vector2(newP.X + 2 * BrushSize, newP.Y + 2 * BrushSize));
                    colourArray = imageTools.fillPolygon(colourArray, polygon, drawingSurface.Width, drawingSurface.Height, DrawColor);//, true);
                    break;
                case 1:
                    colourArray = imageTools.fillOval(colourArray, new Rectangle(newP.X - BrushSize / 2, newP.Y - BrushSize / 2, BrushSize, BrushSize),
                                                        drawingSurface.Width, drawingSurface.Height, DrawColor);
                    break;
            }

            changedImage = true;
        }

        public void drawSmoothed(int x1, int y1, int x2, int y2, int textureWidth, int textureHeight, Color c)
        {
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;
            int err = dx - dy;

            for (; ; )
            {
                if (x1 >= 0 && x1 < textureWidth && y1 >= 0 && y1 < textureHeight)
                    drawPoint(new Point(x1, y1));
                if (x1 == x2 && y1 == y2) break;
                double e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    x1 = x1 + sx;
                }
                if (x1 == x2 && y1 == y2)
                {
                    if (x1 >= 0 && x1 < textureWidth && y1 >= 0 && y1 < textureHeight)
                        drawPoint(new Point(x1, y1));
                    break;
                }
                if (e2 < dx)
                {
                    err = err + dx;
                    y1 = y1 + sy;
                }
            }
        }
    }
}
