using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNATools
{
    public class ImageTools
    {
        private static ImageTools instance;
        private Game appRef;
        
        public ImageTools (Game appRef)
        {
            this.appRef = appRef;
        }

        public static ImageTools getSingleton(Game appRef = null)
        {
            if(appRef != null)
                instance = new ImageTools(appRef);

            return instance;
        }

        public Texture2D createColorTexture(Color c, int width = 1, int height = 1)
        {
            Texture2D result = new Texture2D(appRef.GraphicsDevice, width, height, false, SurfaceFormat.Color);
            int cells = width*height;
            Color[] Colors1D = new Color[cells];
            for (int i = 0; i < cells; i++)
                Colors1D[i] = c;
            result.SetData(Colors1D);
            return result;
        }

        public Texture2D createColorTexture(Color[] Colors1D, int width, int height)
        {
            Texture2D result = new Texture2D(appRef.GraphicsDevice, width, height, false, SurfaceFormat.Color);
            result.SetData(Colors1D);
            return result;
        }

        public Texture2D createTransparentTexture(int width = 1, int height = 1)
        {
            Color c = Color.Transparent;
            return createColorTexture(c, width, height);
        }

        public Texture2D fillRect(Texture2D texture, Rectangle rect, Color c)
        {
            Color[] Colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(Colors1D);

            Colors1D = fillRect(Colors1D, rect, texture.Width, texture.Height, c);

            texture.SetData(Colors1D);
            return texture;
        }

        public Color[] fillRect(Color[] Colors1D, Rectangle rect, int textureWidth, int textureHeight, Color c)
        {
            int newX = Math.Max(rect.X, 0);
            int newY = Math.Max(rect.Y, 0);
            int newRight = Math.Min(rect.Right, textureWidth);
            int newBottom = Math.Min(rect.Bottom, textureHeight);
            for (int x = newX; x < newRight; x++)
            {
                for (int y = newY; y < newBottom; y++)
                {
                    Colors1D[x + y * textureWidth] = c;
                }
            }

            return Colors1D;
        }

        public Texture2D drawRect(Texture2D texture, Rectangle rect, Color c)
        {
            Color[] Colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(Colors1D);

            Colors1D = drawRect(Colors1D, rect, texture.Width, texture.Height, c);

            texture.SetData(Colors1D);
            return texture;
        }

        public Color[] drawRect(Color[] Colors1D, Rectangle rect, int textureWidth, int textureHeight, Color c)
        {
            if (rect.Top >= 0 && rect.Top < textureHeight)
            {
                int endX = Math.Min(rect.Right, textureWidth);
                int startX = Math.Max(rect.Left, 0);
                for (int x = startX; x < endX; x++)
                    Colors1D[x + rect.Top * textureWidth] = c;
            }

            if (rect.Bottom >= 0 && rect.Bottom < textureHeight)
            {
                int endX = Math.Min(rect.Right, textureWidth);
                int startX = Math.Max(rect.Left, 0);
                for (int x = startX; x < endX; x++)
                    Colors1D[x + rect.Bottom * textureWidth] = c;
            }

            if (rect.Left >= 0 && rect.Left < textureWidth)
            {
                int endY = Math.Min(rect.Bottom, textureHeight);
                int startY = Math.Max(rect.Bottom, 0);
                for (int y = startY; y < endY; y++)
                    Colors1D[rect.Left + y * textureWidth] = c;
            }

            if (rect.Right >= 0 && rect.Right < textureWidth)
            {
                int endY = Math.Min(rect.Right, textureHeight);
                int startY = Math.Max(rect.Right, 0);
                for (int y = startY; y < endY; y++)
                    Colors1D[rect.Right + y * textureWidth] = c;
            }

            return Colors1D;
        }

        public Texture2D drawLine(Texture2D texture, int x1, int y1, int x2, int y2, Color c)
        {
            Color[] Colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(Colors1D);

            Colors1D = drawLine(Colors1D, x1, y1, x2, y2, texture.Width, texture.Height, c);

            texture.SetData(Colors1D);
            return texture;
        }

        // Code based on the simplification of Bresenham's algorithm
        // http://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
        public Color[] drawLine(Color[] Colors1D, int x1, int y1, int x2, int y2, int textureWidth, int textureHeight, Color c)
        {
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;
            int err = dx-dy;

            for(;;) {
                if(x1 >= 0 && x1 < textureWidth && y1 >= 0 && y1 < textureHeight)
                    Colors1D[x1 + y1 * textureWidth] = c;
                if (x1 == x2 && y1 == y2) break;
                double e2 = 2*err;
                if (e2 > -dy) 
                {
                    err = err - dy;
                    x1 = x1 + sx;
                }
                if (x1 == x2 && y1 == y2)
                {
                    if (x1 >= 0 && x1 < textureWidth && y1 >= 0 && y1 < textureHeight)
                        Colors1D[x1 + y1 * textureWidth] = c;
                    break;
                }
                if (e2 <  dx)
                {
                    err = err + dx;
                    y1 = y1 + sy;
                }
            }

            return Colors1D;
        }

        public Texture2D fillOval(Texture2D texture, Rectangle rect, Color c)
        {
            Color[] Colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(Colors1D);

            Colors1D = fillOval(Colors1D, rect, texture.Width, texture.Height, c);

            texture.SetData(Colors1D);
            return texture;
        }

        public Color[] fillOval(Color[] Colors1D, Rectangle rect, int textureWidth, int textureHeight, Color c)
        {
            int brushSizeHeight = rect.Height;
            int brushSizeWidth = rect.Width;
            for (int x = rect.X; x < textureWidth && x < rect.Right; x++)
            {
                if (x < 0) x = 0;
                for (int y = rect.Y; y < textureHeight && y < rect.Bottom; y++)
                {
                    if (y < 0) y = 0;
                    Vector2 p = new Vector2(x, y);
                    if (CollisionTools.pointInOval(p, rect))
                    {

                        /*float dx = (p.X - rect.Center.X) / (rect.Width / 2);
                        float dy = (p.Y - rect.Center.Y) / (rect.Height / 2);

                        float alpha = 1 - (float)Math.Min((dx * dx + dy * dy), 0.6);*/
                        Colors1D[x + y * textureWidth] = c;// *alpha; ;
                    }
                }
            }
            return Colors1D;
        }

        /// <summary>
        /// Convert an XNA Texture2D to a 2D array of Color.
        /// </summary>
        /// <param name="Texture">The texture.</param>
        /// <returns></returns>
        public static Color[,] Texture2DToColorArray2D(Texture2D Texture)
        {
            Color[] Colors1D = new Color[Texture.Width * Texture.Height];
            Texture.GetData(Colors1D);

            Color[,] Colors2D = new Color[Texture.Width, Texture.Height];
            for (int x = 0; x < Texture.Width; x++)
            {
                for (int y = 0; y < Texture.Height; y++)
                {
                    Colors2D[x, y] = Colors1D[x + (y * Texture.Width)];
                }
            }
            return (Colors2D);
        }


        /// <summary>
        /// Convert a 2D array of Color into a full Texture2D ready to be drawn.
        /// </summary>
        /// <param name="Colors2D">The 2D array of Color data.</param>
        /// <param name="Graphics">The XNA graphics device currently in use.</param>
        /// <returns></returns>
        public static Texture2D ColorArray2DToTexture2D(Color[,] Colors2D, GraphicsDevice Graphics)
        {
            Texture2D Texture;
            // Figure out the width and height of the new texture,
            // by looking at the dimensions of the array.
            int TextureWidth = Colors2D.GetUpperBound(0);
            int TextureHeight = Colors2D.GetUpperBound(1);
            Color[] Colors1D = new Color[TextureWidth * TextureHeight];

            for (int x = 0; x < TextureWidth; x++)
            {
                for (int y = 0; y < TextureHeight; y++)
                {

                    Colors1D[x + (y * TextureWidth)] = Colors2D[x, y];

                }
            }

            Texture = new Texture2D(Graphics, TextureWidth, TextureHeight, false, SurfaceFormat.Color);
            Texture.SetData(Colors1D);

            return (Texture);
        }

        public Texture2D drawPolygon(Texture2D texture, List<Vector2> polygon, Color c, bool closePoly = false)
        {
            Color[] Colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(Colors1D);

            Colors1D = drawPolygon(Colors1D, polygon, texture.Width, texture.Height, c);

            texture.SetData(Colors1D);
            return texture;
        }

        public Texture2D drawPolygon(Texture2D texture, List<Point> polygon, Color c, bool closePoly = false)
        {
            Color[] Colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(Colors1D);

            Colors1D = drawPolygon(Colors1D, polygon, texture.Width, texture.Height, c, closePoly);

            texture.SetData(Colors1D);
            return texture;
        }

        public Color[] drawPolygon(Color[] Colors1D, List<Vector2> polygon, int textureWidth, int textureHeight, Color c, bool closePoly = false)
        {
            List<Point> pointList = new List<Point>();
            for (int i = 0; i < polygon.Count; i++)
                pointList.Add(new Point((int)Math.Round(polygon[i].X, 0),
                                        (int)Math.Round(polygon[i].Y, 0)));
            return drawPolygon(Colors1D, pointList, textureWidth, textureHeight, c, closePoly);
        }

        public Color[] drawPolygon(Color[] Colors1D, List<Point> polygon, int textureWidth, int textureHeight, Color c, bool closePoly = false)
        {
            if (closePoly)
            {
                if (polygon.Count == 0)
                {
                    return Colors1D;
                }
                else if (polygon.Count == 1 || !polygon[0].Equals(polygon[polygon.Count - 1]))
                {
                    // ensure the start and end point are the same
                    polygon.Add(new Point(polygon[0].X, polygon[0].Y));
                }
            }

            for (int i = 0; i < polygon.Count - 1; i++)
            {
                Colors1D = drawLine(Colors1D, polygon[i].X, polygon[i].Y, polygon[i+1].X, polygon[i+1].Y, textureWidth, textureHeight, c);
            }
            return Colors1D;
        }

        public Texture2D fillPolygon(Texture2D texture, List<Vector2> polygon, Color c)
        {
            Color[] Colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(Colors1D);

            Colors1D = fillPolygon(Colors1D, polygon, texture.Width, texture.Height, c);

            texture.SetData(Colors1D);
            return texture;
        }

        public Texture2D fillPolygon(Texture2D texture, List<Point> polygon, Color c)
        {
            Color[] Colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(Colors1D);

            Colors1D = fillPolygon(Colors1D, polygon, texture.Width, texture.Height, c);

            texture.SetData(Colors1D);
            return texture;
        }

        public Color[] fillPolygon(Color[] Colors1D, List<Vector2> polygon, int textureWidth, int textureHeight, Color c)
        {
            List<Point> pointList = new List<Point>();
            for (int i = 0; i < polygon.Count; i++)
                pointList.Add(new Point((int)Math.Round(polygon[i].X, 0), 
                                        (int)Math.Round(polygon[i].Y, 0)));
            return fillPolygon(Colors1D, pointList, textureWidth, textureHeight, c);
        }


        // Code for this algorithm is based on code from:
        // http://www.sunshine2k.de/coding/java/Polygon/Filling/FillPolygon.htm
        public Color[] fillPolygon(Color[] Colors1D, List<Point> polygon, int textureWidth, int textureHeight, Color c)
        {
            if (polygon.Count == 0)
            {
                return Colors1D;
            }
            else if (polygon.Count == 1 || !polygon[0].Equals(polygon[polygon.Count - 1]))
            {
                // ensure the start and end point are the same
                polygon.Add(new Point(polygon[0].X, polygon[0].Y));
            }

            // create edges array from polygon vertice vector
            // make sure that first vertice of an edge is the smaller one
            Edge[] sortedEdges = this.createEdges(polygon);
         
            // sort all edges by y coordinate, smallest one first, lousy bubblesort
            Edge tmp;
        
            for (int i = 0; i < sortedEdges.Length - 1; i++)
                for (int j = 0; j < sortedEdges.Length - 1; j++)
                {
                    if (sortedEdges[j].p1.Y > sortedEdges[j+1].p1.Y) 
                    {
                        // swap both edges
                        tmp = sortedEdges[j];
                        sortedEdges[j] = sortedEdges[j+1];
                        sortedEdges[j+1] = tmp;
                    }  
                }
        
            // find biggest y-coord of all vertices
            int scanlineEnd = 0;
            for (int i = 0; i < sortedEdges.Length; i++)
            {
                if (scanlineEnd < sortedEdges[i].p2.Y)
                    scanlineEnd = sortedEdges[i].p2.Y;
            }

            // scanline starts at smallest y coordinate
            int scanline = sortedEdges[0].p1.Y;
        
            // this list holds all cutpoints from current scanline with the polygon
            List<int> list = new List<int>();
        
            // move scanline step by step down to biggest one
            for (scanline = sortedEdges[0].p1.Y; scanline <= scanlineEnd; scanline++)
            {
                list.Clear();
            
                // loop all edges to see which are cut by the scanline
                for (int i = 0; i < sortedEdges.Length; i++)
                {   
                
                    // here the scanline intersects the smaller vertice
                    if (scanline == sortedEdges[i].p1.Y) 
                    {
                        if (scanline == sortedEdges[i].p2.Y)
                        {
                            // the current edge is horizontal, so we add both vertices
                            sortedEdges[i].deactivate();
                            list.Add((int)sortedEdges[i].curX);
                        }
                        else
                        {
                            sortedEdges[i].activate();
                            // we don't insert it in the list cause this vertice is also
                            // the (bigger) vertice of another edge and already handled
                        }
                    }
                
                    // here the scanline intersects the bigger vertice
                    if (scanline == sortedEdges[i].p2.Y)
                    {
                        sortedEdges[i].deactivate();
                        list.Add((int)sortedEdges[i].curX);
                    }
                
                    // here the scanline intersects the edge, so calc intersection point
                    if (scanline > sortedEdges[i].p1.Y && scanline < sortedEdges[i].p2.Y)
                    {
                        sortedEdges[i].update();
                        list.Add((int)sortedEdges[i].curX);
                    }
                
                }
            
                // now we have to sort our list with our x-coordinates, ascendend
                list.Sort();

                if (list.Count < 2 || list.Count % 2 != 0) 
                {
                    Console.WriteLine("This should never happen!");
                    continue;
                }
             
                // so draw all line segments on current scanline
                for (int i = 0; i < list.Count; i+=2)
                {
                    //Console.WriteLine("Drawing line: (" + list[i] + ", " + scanline + ") to (" + list[i + 1] + ", " + scanline + ")");
                    Colors1D = drawLine(Colors1D, list[i], scanline, list[i + 1], scanline, textureWidth, textureHeight, c);
                }
            
            }
            return Colors1D;
        }

        private Edge[] createEdges(List<Point> polygon)
        {
            Edge[] sortedEdges = new Edge[polygon.Count - 1];
            for (int i = 0; i < polygon.Count - 1; i++)
            {
                //if (polygon.elementAt(i).y == polygon.elementAt(i+1).y) continue;
                if (polygon[i].Y < polygon[i + 1].Y)
                    sortedEdges[i] = new Edge(polygon[i], polygon[i + 1]);
                else
                    sortedEdges[i] = new Edge(polygon[i + 1], polygon[i]);
            }
            return sortedEdges;
        }

        private class Edge
        {

            public Point p1;        // first vertice
            public Point p2;        // second vertice
            public float m;                // slope

            public float curX;             // x-coord of intersection with scanline

            /*
             * Create on edge out of two vertices
             */
            public Edge(Point a, Point b)
            {
                p1 = new Point(a.X, a.Y);
                p2 = new Point(b.X, b.Y);

                // m = dy / dx
                m = (float)((float)(p1.Y - p2.Y) / (float)(p1.X - p2.X));
            }

            /*
             * Called when scanline intersects the first vertice of this edge.
             * That simply means that the intersection point is this vertice.
             */
            public void activate()
            {
                curX = p1.X;
            }

            /*
             * Update the intersection point from the scanline and this edge.
             * Instead of explicitly calculate it we just increment with 1/m every time
             * it is intersected by the scanline.
             */
            public void update()
            {
                curX += (float)((float)1 / (float)m);
            }

            /*
             * Called when scanline intersects the second vertice, 
             * so the intersection point is exactly this vertice and from now on 
             * we are done with this edge
             */
            public void deactivate()
            {
                curX = p2.X;
            }

        }
    }
}
