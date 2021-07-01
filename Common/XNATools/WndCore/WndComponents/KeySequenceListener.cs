using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XNATools.WndCore
{
    public class KeySequenceListener
    {
        public enum ListenMode { None = 0, All = 1, Custom = 2, Alpha = 3 };

        protected InputManager inputManager;
        protected ListenMode mode;
        protected List<KeyMap> keyMap;
        protected bool listen;
        protected string frameText;
        protected bool _isChanged;
        protected bool autoBackspace;
        protected bool backSpace;

        public KeySequenceListener(ListenMode mode, InputManager inputManager)
        {
            this.inputManager = inputManager;
            this.mode = mode;

            frameText = "";
            keyMap = new List<KeyMap>();
            listen = false;
            _isChanged = false;
            autoBackspace = true;

            addKeys();
        }

        public void update(GameTime gameTime)
        {
            _isChanged = false;
            backSpace = false;

            if (listen && mode != ListenMode.None)
            {
                bool shift = inputManager.isKeyDown(Keys.LeftShift) || inputManager.isKeyDown(Keys.RightShift);

                foreach (KeyMap map in keyMap)
                {
                    if (map.testKey(inputManager))
                        pushString(map.getInput(shift));
                }

                backSpace = inputManager.isKeyPressed(Keys.Back);

                if (autoBackspace && backSpace && frameText.Length > 0)
                {
                    frameText = frameText.Substring(0, frameText.Length - 1);
                    backSpace = false;
                }
            }
        }

        public void start()
        {
            listen = true;
        }

        public void stop()
        {
            listen = true;
        }

        public bool isListening()
        {
            return listen;
        }

        public void pushString(string str)
        {
            if (str.Length > 0)
            {
                _isChanged = true;
                frameText += str;
            }
        }

        public string popString()
        {
            string temp = frameText;
            frameText = "";
            return temp;
        }

        public string peekString()
        {
            return frameText;
        }

        public bool isChanged()
        {
            return _isChanged;
        }

        public bool isBackspace()
        {
            return backSpace;
        }

        public void setAutoBackspace(bool autoBackspace)
        {
            this.autoBackspace = autoBackspace;
        }

        public bool getAutoBackspace()
        {
            return autoBackspace;
        }

        public class KeyMap
        {
            public enum MapMode { Single = 0, AddShift = 1 };

            public Keys key;
            public string single, shift;
            public MapMode mapMode;

            public KeyMap(Keys key, string single)
                : this(key, single, "")
            {
            }

            public KeyMap(Keys key, string single, string shift)
            {
                if (shift.Length == 0)
                    mapMode = MapMode.Single;
                else
                    mapMode = MapMode.AddShift;

                this.key = key;
                this.single = single;
                this.shift = shift;
            }

            public bool testKey(InputManager inputManager)
            {
                return inputManager.isKeyPressed(key);
            }

            public string getInput(bool isShift)
            {
                if (mapMode == MapMode.Single || !isShift)
                    return single;
                else
                    return shift;
            }
        }


        private void addKeys()
        {
            keyMap.Add(new KeyMap(Keys.D1, "1", "!"));
            keyMap.Add(new KeyMap(Keys.D2, "2", "@"));
            keyMap.Add(new KeyMap(Keys.D3, "3", "#"));
            keyMap.Add(new KeyMap(Keys.D4, "4", "$"));
            keyMap.Add(new KeyMap(Keys.D5, "5", "%"));
            keyMap.Add(new KeyMap(Keys.D6, "6", "^"));
            keyMap.Add(new KeyMap(Keys.D7, "7", "&"));
            keyMap.Add(new KeyMap(Keys.D8, "8", "*"));
            keyMap.Add(new KeyMap(Keys.D9, "9", "("));
            keyMap.Add(new KeyMap(Keys.D0, "0", ")"));

            char letter = 'a';
            Keys key = Keys.A;
            for (int i = 0; i < 26; i++, letter++) 
                keyMap.Add(new KeyMap(key+i, ""+letter, (""+letter).ToUpper()));

            keyMap.Add(new KeyMap(Keys.OemPeriod, ".", ">"));
            keyMap.Add(new KeyMap(Keys.OemComma, ",", "<"));
            keyMap.Add(new KeyMap(Keys.OemQuotes, "\'", "\""));
            keyMap.Add(new KeyMap(Keys.OemSemicolon, ";", ":"));
            keyMap.Add(new KeyMap(Keys.OemOpenBrackets, "[", "{"));
            keyMap.Add(new KeyMap(Keys.OemCloseBrackets, "]", "}"));
            keyMap.Add(new KeyMap(Keys.OemMinus, "-", "_"));
            keyMap.Add(new KeyMap(Keys.OemPlus, "=", "+"));
            keyMap.Add(new KeyMap(Keys.OemPipe, "\\", "|"));
            keyMap.Add(new KeyMap(Keys.OemQuestion, "/", "?"));
        }
    }
}
