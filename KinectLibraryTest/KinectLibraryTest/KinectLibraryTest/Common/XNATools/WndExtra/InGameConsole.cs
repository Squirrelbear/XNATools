using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNATools.WndCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace XNATools.WndExtra
{
#if DEBUG
    public class InGameConsole : WndHandle
    {
        protected List<Label> outputs;
        protected Button closeButton;
        protected TextInput textInput;
        protected int insertID;
        protected object lastResult;

        public InGameConsole(WndGroup parent)
            : this(new Rectangle(50, 50, 350, 200), parent)
        {
        }

        public InGameConsole(Rectangle rect, WndGroup parent)
            : base(99999, rect, parent)
        {
            ImageTools imgTools = ImageTools.getSingleton(parent.getAppRef());
            SpriteFont fontSmall = loadFont("smallFont");
            Rectangle dragBarDrawRect = new Rectangle(3, 3, rect.Width - 6, 15);
            Rectangle textInputDrawRect = new Rectangle(3, rect.Height - fontSmall.LineSpacing - 3, dragBarDrawRect.Width, fontSmall.LineSpacing);
            Rectangle textAreaDrawRect = new Rectangle(3, dragBarDrawRect.Bottom + 3, dragBarDrawRect.Width, (textInputDrawRect.Top - 6) - dragBarDrawRect.Bottom);

            Rectangle dragBarRect = new Rectangle(dragBarDrawRect.X + rect.X, dragBarDrawRect.Y + rect.Y, dragBarDrawRect.Width, dragBarDrawRect.Height);
            Rectangle textAreaRect = new Rectangle(textAreaDrawRect.X + rect.X, textAreaDrawRect.Y + rect.Y, textAreaDrawRect.Width, textAreaDrawRect.Height);
            Rectangle textInputRect = new Rectangle(textInputDrawRect.X + rect.X, textInputDrawRect.Y + rect.Y, textInputDrawRect.Width, textInputDrawRect.Height);

            Texture2D background = imgTools.createColorTexture(Color.Black * 0.7f, rect.Width, rect.Height);
            background = imgTools.fillRect(background, dragBarDrawRect, Color.DarkGray * 0.8f);
            background = imgTools.fillRect(background, textInputDrawRect, Color.DarkGreen * 0.6f);
            background = imgTools.fillRect(background, textAreaDrawRect, Color.DarkGreen * 0.7f);
            addComponent(new WndComponent(rect, background));
            addComponent(new GrabBar(dragBarRect, this));

            Texture2D closeButtonImg = imgTools.createColorTexture(Color.Maroon, 11, 11);
            closeButtonImg = imgTools.drawLine(closeButtonImg, 1, 1, 9, 9, Color.Black);
            closeButtonImg = imgTools.drawLine(closeButtonImg, 9, 1, 1, 9, Color.Black);
            Texture2D closeButtonSelImg = imgTools.createColorTexture(Color.Red, 11, 11);
            closeButtonSelImg = imgTools.drawLine(closeButtonSelImg, 1, 1, 9, 9, Color.Black);
            closeButtonSelImg = imgTools.drawLine(closeButtonSelImg, 9, 1, 1, 9, Color.Black);
            closeButton = new Button(new Rectangle(dragBarRect.Right - 13, dragBarRect.Top + 2, 11, 11), closeButtonSelImg, closeButtonImg);
            closeButton.setEnableMouseOverSwap(true);

            Label dragBarTitle = new Label(dragBarRect, "Console", fontSmall, Color.Black);
            dragBarTitle.centreInRect();
            addComponent(dragBarTitle);
            addComponent(closeButton);

            outputs = new List<Label>();
            int count = textAreaRect.Height / fontSmall.LineSpacing;
            LayoutManger lm = new LayoutManger(textAreaRect, count, 1);
            for (int i = 0; i < count; i++)
            {
                Label testLabel = new Label(lm.nextRect(), "Sample label " + (i + 1), fontSmall, Color.White);
                outputs.Add(testLabel);
                addComponent(testLabel);
            }

            insertID = 0;

            Texture2D focusTextInputBG = imgTools.createColorTexture(Color.White * 0.7f);
            textInput = new TextInput(parent.getInputManager(), textInputRect, "12345", fontSmall);
            textInput.setFocusBG(focusTextInputBG);
            textInput.setBaseColor(Color.White);
            textInput.setSelectedColor(Color.Black);
            addComponent(textInput);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (closeButton.getIsClicked())
            {
                exitConsole();
            }
            else if (textInput.getIsTriggered())
            {
                processCommand(textInput.getText());
            }
        }

        public void processCommand(string command)
        {
            addTextToLog(command);

            string[] partition = command.Split(new char[]{ '(' }, 2);
            string[] data = partition[0].Split('.');
            if (data[0].Equals("graph"))
            {
                graph(data, (partition.Length > 1) ? partition[1] : null);
            }
            else
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (i != 0)
                        data[i] = "*" + data[i];
                    string result = executeCommand(data[i], (partition.Length > 1) ? partition[1] : null);
                    addTextToLog(result);
                }
            }
            textInput.setText("");
        }

        public string executeCommand(string command, string paramData = null)
        {
            string[] data = command.Split(' ');
            if (data[0][0] == '*')
            {
                data[0] = data[0].Remove(0, 1);
            }
            else
            {
                lastResult = this;
            }
            return invoke(lastResult, data[0], paramData);
        }

        public string dataTest()
        {
            return "Wow this works";
        }

        // All error checking omitted. In particular, check the results
        // of Type.GetType, and make sure you call it with a fully qualified
        // type name, including the assembly if it's not in mscorlib or
        // the current assembly. The method has to be a public instance
        // method with no parameters. (Use BindingFlags with GetMethod
        // to change this.)
        public string invoke(object target, string methodName, string paramData = null)
        {
            Type type = target.GetType();//Type.GetType(typeName);
            object instance = target;//Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod(methodName);
            object[] paramArray = null;
            if (paramData != null)
            {
                ParameterInfo[] parameters = method.GetParameters();
                paramArray = new object[parameters.Length];
                string[] pSplitData = paramData.Split(',');
                // TODO: disabling this warning is a BAD IDEA
                /*if (pSplitData.Length != parameters.Length)
                {
                    return "Error! Invalid number of parameters supplied.";
                }*/
                for (int i = 0; i < parameters.Length; i++)
                {
                    Type t = parameters[i].ParameterType;
                    if(t == typeof(int))
                    {
                        int temp;
                        bool result = int.TryParse(pSplitData[i], out temp);
                        paramArray[i] = temp;
                        if (!result)
                            return "Error! Parameter " + i + " could not be cast to int."; 
                    }
                    else if (t == typeof(float))
                    {
                        float temp;
                        bool result = float.TryParse(pSplitData[i], out temp);
                        paramArray[i] = temp;
                        if (!result)
                            return "Error! Parameter " + i + " could not be cast to float.";
                    }
                    else if (t == typeof(double))
                    {
                        double temp;
                        bool result = double.TryParse(pSplitData[i], out temp);
                        paramArray[i] = temp;
                        if (!result)
                            return "Error! Parameter " + i + " could not be cast to double.";
                    }
                    else if (t == typeof(string))
                    {
                        // TODO: this is a "hack"
                        paramArray[i] = "";
                        for (int j = i; j < pSplitData.Length; j++)
                            paramArray[i] += pSplitData[j] +
                                ((j < pSplitData.Length - 1) ? "," : "");
                        i = paramArray.Length;
                    }
                    else
                    {
                        return ("Error! Unsupported param type in field: " + i + ".");
                    }
                }
            }
            if (method == null)
            {
                return "Invalid method call!";
            }
            else if (method.ReturnType == typeof(void))
            {
                method.Invoke(instance, paramArray);
                return "Method called: " + methodName;
            }
            else if (method.ReturnType != typeof(string))
            {
                lastResult = method.Invoke(instance, paramArray);
                return "Method called: " + lastResult.ToString();
            }

            return (string)method.Invoke(instance, paramArray);
        }

        public void graph(string[] data, string paramData)
        {
            if (data.Length == 1)
            {
                addTextToLog("Error: No target set.");
            }

            int i = 1;
            for (; i < data.Length-1; i++)
            {
                if (i != 1)
                    data[i] = "*" + data[i];
                string result = executeCommand(data[i]);
                addTextToLog(result);
            }

            MethodInfo method = lastResult.GetType().GetMethod(data[data.Length-1]);
            object[] paramArray = null;
            if (paramData != null)
            {
                ParameterInfo[] parameters = method.GetParameters();
                paramArray = new object[parameters.Length];
                string[] pSplitData = paramData.Split(',');
                if (pSplitData.Length != parameters.Length)
                {
                    addTextToLog("Error! Invalid number of parameters supplied.");
                    return;
                }
                for (int j = 0; j < parameters.Length; j++)
                {
                    Type t = parameters[j].ParameterType;
                    if (t == typeof(Int32))
                    {
                        int temp;
                        bool result = int.TryParse(pSplitData[j], out temp);
                        paramArray[j] = temp;
                        if (!result)
                        {
                            addTextToLog("Error! Parameter " + j + " could not be cast to int.");
                            return;
                        }
                    }
                    else
                    {
                        addTextToLog("Error! Unsupported param type in field: " + j + ".");
                        return;
                    }
                }
            }

            Rectangle targetRect = new Rectangle(getParent().getRect().Right - 300, 30, 300, 250);
            parent.addWnd(new AutoChartWnd(targetRect, getParent(), lastResult, method, paramArray));
        }

        public void addTextToLog(string text)
        {
            outputs[insertID++].setText(text);
            if (insertID >= outputs.Count)
                insertID = 0;
        }

        public int addTest(int a, int b, int c)
        {
            return a + b + c;
        }

        public WndHandle getBackWnd()
        {
            return getParent().getWndAt(0);
        }

        public void openConsole()
        {
            if (parent.getWnd(this) == null)
                parent.addWnd(this);
        }

        public void exitConsole()
        {
            parent.removeWnd(this);
        }
    }
#endif
}
