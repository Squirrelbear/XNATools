using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XNATools.WndCore
{
    public class TextInput : Label
    {
        protected int cursorPos;
        protected KeySequenceListener keys;
        protected bool _isChanged;
        protected bool _isTriggered;
        protected Color baseColor, selectedColor;
        protected CentreMode centerMode;
        protected InputManager inputManager;
        protected Texture2D focusBG;
        protected Texture2D noFocusBG;

        public TextInput(InputManager inputManager, Rectangle dest, string text, SpriteFont font)
            : this(inputManager, dest, text, font, Color.Black, false)
        {
        }

        public TextInput(InputManager inputManager, Rectangle dest, string text, SpriteFont font, bool multiline)
            : this(inputManager, dest, text, font, Color.Black, multiline)
        {
        }

        public TextInput(InputManager inputManager, Rectangle dest, string text, SpriteFont font, Color fontColor)
            : this(inputManager, dest, text, font, fontColor, false)
        {
        }

        public TextInput(InputManager inputManager, Rectangle dest, string text, SpriteFont font, Color fontColor, bool multiline)
            : base(dest, text, font, fontColor, multiline)
        {
            this.inputManager = inputManager;
            cursorPos = text.Length;
            keys = new KeySequenceListener(KeySequenceListener.ListenMode.All, inputManager);
            keys.start();
            _isChanged = false;
            _isTriggered = false;
            baseColor = fontColor;
            selectedColor = Color.Red;
            centerMode = CentreMode.CentreVertical;
            focusBG = noFocusBG = null;
        }

        public override void update(GameTime gameTime)
        {
            _isChanged = false;
            _isTriggered = false;

            if (getHasFocus())
            {
                keys.update(gameTime);
                if (keys.isBackspace() && getText().Length > 0)
                {
                    setText(getText().Substring(0, getText().Length - 1));
                    centreInRect(centerMode);
                    _isChanged = true;
                }

                if (keys.peekString().Length > 0)
                {
                    string newS = getText() + keys.popString();
                    if (validateString(newS))
                    {
                        setText(newS);
                        centreInRect(centerMode);
                        _isChanged = true;
                    }
                }

                if (inputManager.isKeyReleased(Keys.Enter))
                {
                    _isTriggered = true;
                }
            }

            base.update(gameTime);
        }

        public override void mouseClickedLeft(Point p)
        {
            base.mouseClickedLeft(p);

            setFocus(dest.Contains(p));
        }

        public override void setFocus(bool hasFocus)
        {
            base.setFocus(hasFocus);

            if (hasFocus)
            {
                setColor(selectedColor);
                setTexture(focusBG);
            }
            else
            {
                setColor(baseColor);
                setTexture(noFocusBG);
            }
        }

        public bool getIsChanged()
        {
            return _isChanged;
        }

        public bool getIsTriggered()
        {
            return _isTriggered;
        }

        public void setBaseColor(Color c)
        {
            baseColor = c;
            if (!hasFocus)
                setColor(c);
        }

        public void setSelectedColor(Color c)
        {
            selectedColor = c;
            if (hasFocus)
                setColor(c);
        }

        public void setFocusBG(Texture2D textureBG)
        {
            focusBG = textureBG;
            if (hasFocus)
                setTexture(focusBG);
        }

        public void setNoFocusBG(Texture2D textureBG)
        {
            noFocusBG = textureBG;
            if (!hasFocus)
                setTexture(noFocusBG);
        }

        public virtual bool validateString(string s)
        {
            /*int num;
            if (!int.TryParse(s, out num))
                return false;

            if (num <= 0 || num > 99)
                return false;
            */
            return true;
        }
    }
}
