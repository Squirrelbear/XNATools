using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace XNATools
{
    public class ScriptRunner
    {
        private List<KeyValuePair<int, string>> output;
        private int commandID;
        private bool verboseOutput;
        private object defaultObject, currentReference;
        private Stack<KeyValuePair<string, object>> objectStack;
        private List<KeyValuePair<string, object>> definedVariables;

        public ScriptRunner(object defaultObject, bool verboseOutput = false)
        {
            output = new List<KeyValuePair<int, string>>();
            objectStack = new Stack<KeyValuePair<string, object>>();
            definedVariables = new List<KeyValuePair<string, object>>();
            this.verboseOutput = verboseOutput;
            commandID = 0;
            this.defaultObject = defaultObject;
            currentReference = defaultObject;
        }

        public void processCommand(string command)
        {
            output.Add(new KeyValuePair<int, string>(commandID, command));

            string[] data = command.Split('.');
            for (int i = 0; i < data.Length; i++)
            {
                if (i != 0)
                    data[i] = "*" + data[i];
                string result = executeCommand(data[i]);
                output.Add(new KeyValuePair<int, string>(commandID, result));
            }
            commandID++;
        }

        public string executeCommand(string command)
        {
            string[] data = command.Split(' ');
            if (data[0][0] == '*')
            {
                data[0] = data[0].Remove(0, 1);
            }
            else
            {
                currentReference = defaultObject;
            }
            return invoke(currentReference, data[0]);
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
        public string invoke(object target, string methodName, bool newInstance = false)
        {
            Type type = target.GetType();//Type.GetType(typeName);
           // object instance = (newInstance) ? Activator.CreateInstance(type) : target ;

            object instance = target;//Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod(methodName);
            if (method == null)
            {
                return "Invalid method call!";
            }
            else if (method.ReturnType == typeof(void))
            {
                method.Invoke(instance, null);
                return "Method called: " + methodName;
            }
            else if (method.ReturnType != typeof(string))
            {
                currentReference = method.Invoke(instance, null);
                return "Method called: " + currentReference.ToString();
            }

            return (string)method.Invoke(instance, null);
        }
    }
}
