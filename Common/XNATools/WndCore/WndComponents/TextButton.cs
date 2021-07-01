using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace XNATools.WndCore
{
    public class TextButton : Button
    {
        protected string text;
        protected string selectedText;
        protected SpriteFont font;
        protected Color fontColor;
        protected Color fontColorSelected;
        protected Vector2 textPosition;
        protected Vector2 selectedTextPosition;

        public TextButton(Rectangle dest, string text, SpriteFont font, Texture2D selected, Texture2D unselected, bool isSelected = false, int actionID = 0)
            : this(dest, text, text, font, selected, unselected, isSelected, actionID)
        {
        }

        public TextButton(Rectangle dest, string text, string selectedText, SpriteFont font, Texture2D selected, Texture2D unselected, bool isSelected = false, int actionID = 0)
            : base(dest, selected, unselected, isSelected, actionID)
        {
            this.text = text;
            this.selectedText = selectedText;
            this.font = font;
            fontColor = Color.MidnightBlue;
            fontColorSelected = Color.MidnightBlue;

            Vector2 stringDims = font.MeasureString(text);
            Point centre = dest.Center;
            textPosition = new Vector2(centre.X - stringDims.X / 2, centre.Y - stringDims.Y / 2);
            Vector2 stringSelectedDims = font.MeasureString(selectedText);
            selectedTextPosition = new Vector2(centre.X - stringSelectedDims.X / 2, centre.Y - stringSelectedDims.Y / 2);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);

            if (!visible) return;

            if(isSelected)
                spriteBatch.DrawString(font, selectedText, selectedTextPosition, fontColorSelected);
            else
                spriteBatch.DrawString(font, text, textPosition, fontColor);
        }

        public void setText(string newText)
        {
            this.text = newText;

            Vector2 stringDims = font.MeasureString(text);
            Point centre = dest.Center;
            textPosition = new Vector2(centre.X - stringDims.X / 2, centre.Y - stringDims.Y / 2); 
        }

        public void setSelectedText(string newText)
        {
            this.selectedText = newText;
            Point centre = dest.Center;
            Vector2 stringSelectedDims = font.MeasureString(selectedText);
            selectedTextPosition = new Vector2(centre.X - stringSelectedDims.X / 2, centre.Y - stringSelectedDims.Y / 2);
        }

        public string getText()
        {
            return text;
        }

        public string getSelectedText()
        {
            return selectedText;
        }

        public override void setLocation(Vector2 location)
        {
            base.setLocation(location);

            if (font != null)
            {
                Vector2 stringDims = font.MeasureString(text);
                Point centre = dest.Center;
                textPosition = new Vector2(centre.X - stringDims.X / 2, centre.Y - stringDims.Y / 2);
                Vector2 stringSelectedDims = font.MeasureString(selectedText);
                selectedTextPosition = new Vector2(centre.X - stringSelectedDims.X / 2, centre.Y - stringSelectedDims.Y / 2);
            }
        }

        public override void setRect(Rectangle dest)
        {
            base.setRect(dest);

            if (font != null)
            {
                Vector2 stringDims = font.MeasureString(text);
                Point centre = dest.Center;
                textPosition = new Vector2(centre.X - stringDims.X / 2, centre.Y - stringDims.Y / 2);
                Vector2 stringSelectedDims = font.MeasureString(selectedText);
                selectedTextPosition = new Vector2(centre.X - stringSelectedDims.X / 2, centre.Y - stringSelectedDims.Y / 2);
            }
        }

        public void setFontColor(Color color)
        {
            this.fontColor = color;
        }

        public void setSelectedFontColor(Color color)
        {
            this.fontColorSelected = color;
        }
    }
}
