using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNATools;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace KinectLibraryTest
{
    public class ScriptCube : AnimatedObject
    {
        private string updateScript;
        private object lastResult;
        private GameTime curTime;
        private ScriptCubeWnd wndRef;
        private bool boundsEnforced;

        public ScriptCube(Random gen, ScriptCubeWnd wndRef)
            : base(ImageTools.getSingleton().createColorTexture(new Color(gen.Next(256)/256.0f, gen.Next(256)/256.0f, gen.Next(256)/256.0f)),
                     1, 1, new Rectangle(gen.Next(100, 400), gen.Next(100, 400), gen.Next(10, 45), gen.Next(10,45)))
        {
            this.wndRef = wndRef;
            updateScript = "";
            boundsEnforced = false;
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            boundsEnforced = false;
            curTime = gameTime;
            lastResult = this;
            if(updateScript.Length > 0)
                execute(updateScript);

            if (!boundsEnforced)
                enforceBounds(wndRef.getRect().X, wndRef.getRect().Y, wndRef.getRect().Width, wndRef.getRect().Height);
        }

        public void enforceBounds(int x, int y, int width, int height)
        {
            if (getX() < x || getY() < y || getX() + getRect().Width > x + width || getY() + getRect().Height > y + height)
            {
                float newX = Math.Max(Math.Min(getX(), x + width), x);
                float newY = Math.Max(Math.Min(getY(), y + height), y);
                setLocation(newX, newY);
            }
            boundsEnforced = true;
        }

        public void rotateLeft(int speed)
        {
            rotation -= speed * curTime.ElapsedGameTime.Milliseconds / 1000.0f;
        }

        public void rotateRight(int speed)
        {
            rotation += speed * curTime.ElapsedGameTime.Milliseconds / 1000.0f;
        }

        public void move(int xMove, int yMove)
        {
            moveBy(xMove * curTime.ElapsedGameTime.Milliseconds / 1000.0f, yMove * curTime.ElapsedGameTime.Milliseconds / 1000.0f);
        }

        public void setScript(string script)
        {
            this.updateScript = script;
        }

        public string getScript()
        {
            return updateScript;
        }

        private void execute(string command)
        {
            string[] lines = command.Split(';');
            foreach(string line in lines)
            {
                string[] partition = line.Split('(');
                string[] data = partition[0].Split('.');

                if (partition.Length > 1)
                    invoke(lastResult, data[0], partition[1]);
                else
                    invoke(lastResult, data[0]);
            }
        }

        private string invoke(object target, string methodName, string paramData = null)
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
                if (pSplitData.Length != parameters.Length)
                {
                    return "Error! Invalid number of parameters supplied.";
                }
                for (int i = 0; i < parameters.Length; i++)
                {
                    Type t = parameters[i].ParameterType;
                    if (t == typeof(int))
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
    }
}
