using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNATools.WndCore;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNATools.WndExtra
{
    public class SimpleGraph : WndComponent
    {
        private Texture2D graph;
        private Color[] emptyGraph;
        private Color[] newGraph;
        private List<Vector2> data;
        private bool reTextureNeeded;
        public Color LineColour { get; set; }
        private ImageTools imgTools;

        public SimpleGraph(Rectangle rect, List<Vector2> data = null)
            : base(rect)
        {
            this.data = data;
            LineColour = Color.Black;
            imgTools = ImageTools.getSingleton();
            graph = imgTools.createTransparentTexture(rect.Width, rect.Height);
            emptyGraph = new Color[rect.Width * rect.Height];
            graph.GetData(emptyGraph);
            setTexture(graph);
            reTextureNeeded = false;
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            if (reTextureNeeded)
            {
                generateGraph();
                reTextureNeeded = false;
            }

            spriteBatch.Draw(graph, dest, Color.White);
        }

        public void setGraphData(List<Vector2> data)
        {
            this.data = data;
            reTextureNeeded = true;
        }

        public void setGraphData(List<int> dataOverTime)
        {
            data = new List<Vector2>();
            for (int i = 0; i < dataOverTime.Count; i++)
            {
                data.Add(new Vector2(i, (float)dataOverTime[i]));
            }
            setGraphData(data);
        }

        public void setGraphData(List<object> dataOverTime)
        {
            data = new List<Vector2>();
            for (int i = 0; i < dataOverTime.Count; i++)
            {
                data.Add(new Vector2(i, (float)dataOverTime[i]));
            }
            setGraphData(data);
        }

        public void setGraphData(List<double> dataOverTime)
        {
            data = new List<Vector2>();
            for (int i = 0; i < dataOverTime.Count; i++)
            {
                data.Add(new Vector2(i, (float)dataOverTime[i]));
            }
            setGraphData(data);
        }

        public void setGraphData(List<float> dataOverTime)
        {
            data = new List<Vector2>();
            for (int i = 0; i < dataOverTime.Count; i++)
            {
                data.Add(new Vector2(i, dataOverTime[i]));
            }
            setGraphData(data);
        }

        public void generateGraph()
        {
            if (data == null || data.Count == 0)
                return;

            ImageTools imgTools = ImageTools.getSingleton();
            newGraph = (Color[])emptyGraph.Clone();

            float minX = data[0].X, maxX = data[0].X, minY = data[0].Y, maxY = data[0].Y;
            for (int i = 1; i < data.Count; i++)
            {
                if (data[i].X < minX) minX = data[i].X;
                if (data[i].X > maxX) maxX = data[i].X;
                if (data[i].Y < minY) minY = data[i].Y;
                if (data[i].Y > maxY) maxY = data[i].Y;
            }
            float scaleX = (maxX - minX) / (dest.Width-1);
            float scaleY = (maxY - minY) / (dest.Height-1);
            List<Point> poly = new List<Point>();
            for (int i = 0; i < data.Count; i++)
            {
                poly.Add(new Point((int)((data[i].X - minX) / scaleX),
                                    (int)((data[i].Y - minY) / scaleY)));
            }
            newGraph = imgTools.drawPolygon(newGraph, poly, dest.Width, dest.Height, LineColour);
            graph.Dispose();
            graph = imgTools.createColorTexture(newGraph, dest.Width, dest.Height);
            setTexture(graph);
        }

        private float rescale(float value, float rangeMin, float rangeMax, float targetMin, float targetMax)
        {
            float scale = (rangeMax - rangeMax) / (targetMax - targetMin);
            return targetMin + (value - rangeMin) / scale;
        }
    }
}
