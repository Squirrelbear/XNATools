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
#if DEBUG
    public class AutoChartWnd : WndHandle
    {
        protected Button closeButton;
        protected SimpleGraph graph;
        protected object targetObj;
        protected MethodInfo targetMethod;
        protected Timer timer;
        protected List<float> data;
        protected object[] paramData;

        public AutoChartWnd(Rectangle displayRect, WndGroup parent, object targetObj, MethodInfo method, object[] paramData = null)
            : base(777, displayRect, parent)
        {
            this.targetObj = targetObj;
            this.targetMethod = method;
            this.paramData = paramData;

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

            string titleStr = method.Name;
            if (paramData != null && paramData.Length == 1)
                titleStr += ": " + paramData[0].ToString();
            Label dragBarTitle = new Label(dragBarRect, titleStr, fontSmall, Color.Black);
            dragBarTitle.centreInRect();
            addComponent(dragBarTitle);
            addComponent(closeButton);

            graph = new SimpleGraph(graphRect);
            addComponent(graph);

            data = new List<float>();
            timer = new Timer(200);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (closeButton.getIsClicked())
            {
                parent.removeWnd(this);
                return;
            }

            timer.update(gameTime);
            if (timer.wasTriggered())
            {
                data.Add(getCurValue());
                if (data.Count > 25)
                    data.RemoveAt(0);
                graph.setGraphData(data);
            }
        }

        private float getCurValue()
        {
            if (targetMethod == null || targetObj == null)
            {
                // TODO: Perhaps put an error here if this happens
                // eg, show label instead.
                return 0;
            }

            return (float)targetMethod.Invoke(targetObj, paramData);
        }
    }
#endif
}
