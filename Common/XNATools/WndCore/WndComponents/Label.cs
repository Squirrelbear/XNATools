using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace XNATools.WndCore
{
    /// <summary>
    /// The Label class is used to draw text onto the screen and
    /// control how that text is display with a variety of different 
    /// options for controlling the structure.
    /// </summary>
    public class Label : WndComponent
    {
        /// <summary>
        /// The CentreMode is used to define an alignment inside the
        /// defined Rectangle destination. These enumerated types
        /// can be used to tell the text to centre on either or both.
        /// </summary>
        public enum CentreMode { CentreVertical, CentreHorizontal, CentreBoth };

        #region Instance Variables
        /// <summary>
        /// The text that is displayed as part of the label.
        /// </summary>
        protected string text;

        /// <summary>
        /// The font that defines the primary appearance of any text displayed.
        /// </summary>
        protected SpriteFont font;

        /// <summary>
        /// The color that the font should be rendered with.
        /// </summary>
        protected Color fontColor;

        /// <summary>
        /// The top left corner of where the text should be rendered from.
        /// </summary>
        protected Vector2 textPos;

        /// <summary>
        /// The multiline property defines whether the text should
        /// be split over multiple lines if any line exceeds the width of the 
        /// rectangle. If this is the case the splitStrings variable will 
        /// automatically be used for rendering instead and the text 
        /// will be split into defined lengths.
        /// </summary>
        protected bool isMultiline;

        protected bool wordWrap;

        /// <summary>
        /// Stores the divided string that is updated and used when the isMultiline
        /// variable is set to true. This will automatically divide the text variable 
        /// of the Label based on the width of the Rectangle the label is defined with.
        /// </summary>
        protected List<string> splitStrings;
        #endregion

        /// <summary>
        /// Creates a black single line label located at dest with the text 
        /// and font specified. By default the Label will be located with the
        /// top left corner of the dest being used as the starting location.
        /// </summary>
        /// <param name="dest">The location to place the label at.</param>
        /// <param name="text">The string to initially set the label to.</param>
        /// <param name="font">The font to use for rendering the text.</param>
        public Label(Rectangle dest, string text, SpriteFont font)
            : this(dest, text, font, Color.Black, false)
        {
        }

        /// <summary>
        /// Creates a black single (or multiline if multiline is true) line label located at dest with the text 
        /// and font specified. By default the Label will be located with the
        /// top left corner of the dest being used as the starting location.
        /// Additionally the multiline property can be enabled or disabled 
        /// depending on desired functionality for automatic word wrapping.
        /// </summary>
        /// <param name="dest">The location to place the label at.</param>
        /// <param name="text">The string to initially set the label to.</param>
        /// <param name="font">The font to use for rendering the text.</param>
        /// <param name="multiline">Defines whether the Label should automatically 
        /// split over multiple lines based on the width of the dest.</param>
        public Label(Rectangle dest, string text, SpriteFont font, bool multiline)
            : this(dest, text, font, Color.Black, multiline)
        {
        }

        /// <summary>
        /// Creates a black single line label located at dest with the text 
        /// and font specified. By default the Label will be located with the
        /// top left corner of the dest being used as the starting location.
        /// Additionally the multiline property can be enabled or disabled 
        /// depending on desired functionality for automatic word wrapping.
        /// </summary>
        /// <param name="dest">The location to place the label at.</param>
        /// <param name="text">The string to initially set the label to.</param>
        /// <param name="font">The font to use for rendering the text.</param>
        /// <param name="fontColor">The colour to set the Label to render with.</param>
        public Label(Rectangle dest, string text, SpriteFont font, Color fontColor)
            : this(dest, text, font, fontColor, false)
        {
        }

        /// <summary>
        /// Creates a label located at dest with the text rendering using a specified
        /// font and colour. If the multiline variable is set to true the text
        /// will automatically be word wrapped based on the dimensions of the defined rectangle.
        /// </summary>
        /// <param name="dest">The location to place the label at.</param>
        /// <param name="text">The string to intially set the label to.</param>
        /// <param name="font">The font to use for rendering the text.</param>
        /// <param name="fontColor">The colour to set the Label to render with.</param>
        /// <param name="multiline">Defines whether the Label should automatically 
        /// split over multiple lines based on the width of the dest.</param>
        public Label(Rectangle dest, string text, SpriteFont font, Color fontColor, bool multiline)
            : base(dest)
        {
            this.isMultiline = multiline;
            wordWrap = false;
            this.font = font;
            this.fontColor = fontColor;

            textPos = new Vector2(dest.X, dest.Y);
            setText(text);
        }

        /// <summary>
        /// If the multiline property has been enabled the text will
        /// automatically be rendered using the precalculted divions.
        /// </summary>
        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);

            if (!isMultiline)
                spriteBatch.DrawString(font, text, textPos, fontColor);
            else
                drawMultiline(spriteBatch);
        }

        /// <summary>
        /// Sets the text to be displayed by the Label, if the multiline
        /// property is enabled the text will be automatically divided
        /// into new groups.
        /// </summary>
        /// <param name="text">The new text to display.</param>
        public void setText(string text)
        {
            this.text = text;
            if (isMultiline)
            {
                updateMultiline();
            }
        }

        /// <summary>
        /// Gets the current text being displayed by the Label.
        /// </summary>
        public string getText()
        {
            return text;
        }

        public void setWordWrap(bool enabled)
        {
            this.wordWrap = enabled;
            updateMultiline();
        }

        /// <summary>
        /// Sets the color that the text in the Label will be 
        /// rendered to the screen with.
        /// </summary>
        public void setColor(Color color)
        {
            this.fontColor = color;
        }

        /// <summary>
        /// Sets the location of the text for where the top left corner of the 
        /// text will start appearing at.
        /// </summary>
        /// <param name="location">The new location on the screen.</param>
        public override void setLocation(Vector2 location)
        {
            base.setLocation(location);

            textPos = new Vector2(location.X, location.Y);
        }

        /// <summary>
        /// Sets the location of the text in the same way the setLocation 
        /// method would, except the size of the bounding Rectangle may be changed as well.
        /// </summary>
        public override void setRect(Rectangle newRect)
        {
            base.setRect(newRect);

            // TODO: perhaps do a new pass with the multiline here if it is enabled.
            textPos = new Vector2(newRect.X, newRect.Y);
        }

        /// <summary>
        /// Moves the label by a particular amount. This overrides the default
        /// implementation so that it can correctly update the text that if 
        /// any sort of centering has been applied to it will correctly be reapplied
        /// after the rectangle has been moved.
        /// </summary>
        /// <param name="translation">The amount to move the label by.</param>
        public override void moveLocationBy(Vector2 translation)
        {
            Vector2 newTextPos = textPos + translation;
            base.moveLocationBy(translation);
            textPos = newTextPos;
        }

        /// <summary>
        /// Centers the text using both the horizontal and vertical axis.
        /// </summary>
        public void centreInRect()
        {
            centreInRect(dest, CentreMode.CentreBoth);
        }

        /// <summary>
        /// Centers the text using the specified mode. The mode can be 
        /// set to use Horizontal, Vertical or Both.
        /// </summary>
        public void centreInRect(CentreMode mode)
        {
            centreInRect(dest, mode);
        }

        /// <summary>
        /// Centers the text on both the vertical and horizontal axis.
        /// The Rectangle to center the text in is defined by dest.
        /// </summary>
        public void centreInRect(Rectangle dest)
        {
            centreInRect(dest, CentreMode.CentreBoth);
        }

        /// <summary>
        /// Centers the text inside the Rectangle dest using the
        /// specified mode for whether it should be Horizontal, 
        /// Vertical or for both.
        /// </summary>
        public void centreInRect(Rectangle dest, CentreMode mode)
        {
            // TODO: perhaps this code should have a setRect method call
            if (isMultiline)
            {
                int height = font.LineSpacing * splitStrings.Count;
                int maxChars = 0;
                string maxS = "";
                foreach (string s in splitStrings)
                {
                    if (s.Length > maxChars)
                    {
                        maxChars = s.Length;
                        maxS = s;
                    }
                }
                int width = (int)font.MeasureString(maxS).X;

                Point centre = dest.Center;
                int posX = (mode != CentreMode.CentreVertical) ? centre.X - width / 2 : dest.X;
                int posY = (mode != CentreMode.CentreHorizontal) ? centre.Y - height / 2 : dest.Y;
                textPos = new Vector2(posX, posY);
            }
            else
            {
                Vector2 stringDims = font.MeasureString(text);
                Point centre = dest.Center;
                int posX = (mode != CentreMode.CentreVertical) ? centre.X - (int)stringDims.X / 2 : dest.X;
                int posY = (mode != CentreMode.CentreHorizontal) ? centre.Y - (int)stringDims.Y / 2 : dest.Y;
                textPos = new Vector2(posX, posY);
            }
        }

        /// <summary>
        /// Divides the text text to be displayed inside the Label into the 
        /// strings that should be displayed on each line of the multiline label.
        /// </summary>
        private void updateMultiline()
        {
            splitStrings = new List<string>();
            if (text.Length == 0)
            {
                return;
            }

            int lineHeight = font.LineSpacing;
            Vector2 curPos = new Vector2(textPos.X, textPos.Y);
            string remainingText = text;

            do{
                // if there is no text left or there is no more room in the block
                if(remainingText.Length == 0 || dest.Bottom  < curPos.Y + lineHeight) 
                {
                    break;
                }

                // determine number of characters that can fit
                // start by assuming that all the remaining text can fit
                string trialString = remainingText;
                do{
                    if(font.MeasureString(trialString).X > dest.Width)
                    {
                        if(wordWrap)
                        {
                            int spaceIndex = trialString.LastIndexOf(" ");
                            string word = trialString.Substring(spaceIndex);

                            // if the word to wrap is too big to wrap then use the default method
                            if(font.MeasureString(word).X > dest.Width)
                            {
                                trialString = trialString.Substring(0, trialString.Length-1);
                            }
                            else
                            {
                                trialString = trialString.Substring(0, trialString.Length - word.Length);
                            }
                        }
                        else
                        {
                            trialString = trialString.Substring(0, trialString.Length-1);
                        }
                    }
                    else
                    {
                        break;
                    }
                } while(true);

                // add the string and update variables
                splitStrings.Add(trialString);
                remainingText = remainingText.Substring(trialString.Length);
                curPos.Y += lineHeight;
            } while(true);
        }

        /// <summary>
        /// Draw the precalculated multiline strings. They are drawn from the
        /// start location and increase onto each next line by the line spacing
        /// that is defined by the font's line spacing.
        /// </summary>
        private void drawMultiline(SpriteBatch spriteBatch)
        {
            if (splitStrings.Count == 0) return;

            int lineHeight = font.LineSpacing;
            Vector2 curPos = new Vector2(textPos.X, textPos.Y);

            foreach (string s in splitStrings)
            {
                spriteBatch.DrawString(font, s, curPos, fontColor);
                curPos.Y += lineHeight;
            }
        }
    }
}
