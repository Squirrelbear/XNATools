using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace XNATools.WndCore
{
    /// <summary>
    /// Extends the functionality of the Label class to 
    /// have multiple different strings that can be displayed.
    /// These strings can be automatically cycled based on the timer.
    /// </summary>
    public class CyclingLabel : Label
    {
        #region Instance Variables
        /// <summary>
        /// The ID of the current string being displayed by the label.
        /// </summary>
        protected int curID;

        /// <summary>
        /// Whether the cycling of the automatic label is random or 
        /// if it cycles in the order the strings have been defined.
        /// </summary>
        protected bool random;

        /// <summary>
        /// The random generator that can be used to generate the 
        /// random sequence of strings.
        /// </summary>
        protected Random gen;

        /// <summary>
        /// The collection of text that can be displayed by the label.
        /// </summary>
        protected List<string> textList;

        /// <summary>
        /// The timer object that can be used for the automatic 
        /// sequence of string changes.
        /// </summary>
        protected Timer timer;

        /// <summary>
        /// Whether the strings should automatically be iterated through
        /// using the nextText() if true, or previousText() if false.
        /// </summary>
        protected bool autoForward;
        #endregion

        /// <summary>
        /// Defines a basic constructor that includes the Rectangle defining
        /// the region it is displayed, the list of text elements that can be displayed,
        /// the font, and whether the sequence should be randomly generated or 
        /// based on the sequence they have been supplied.
        /// </summary>
        /// <param name="dest">The location to place the label at.</param>
        /// <param name="textList">The list of text elements to be cycled through.</param>
        /// <param name="font">The font to use for rendering the text.</param>
        /// <param name="random">If this is true the label will automatically cycle randomly.</param>
        public CyclingLabel(Rectangle dest, List<string> textList, SpriteFont font, bool random)
            : this(dest, textList, font, Color.Black, random)
        {
        }

        /// <summary>
        /// Creates a Cycling label with a defined Rectangle for the location of 
        /// the string, a list of the strings to be cycled through, the font, 
        /// the colour to draw the text with and whether the strings should
        /// be randomly cycled or if the sequence should be based on the 
        /// order the strings have been supplied.
        /// </summary>
        /// <param name="dest">The rectangle the label is bounded by.</param>
        /// <param name="textList">The list of text that the cycling label will use.</param>
        /// <param name="font">The font to use when rendering the text.</param>
        /// <param name="fontColor">The colour to use when rendering the text.</param>
        /// <param name="random">If this is true the label will automatically cycle randomly.</param>
        public CyclingLabel(Rectangle dest, List<string> textList, SpriteFont font, Color fontColor, bool random)
            : base(dest, textList[0], font, fontColor)
        {
            curID = 0;
            this.random = random;
            this.textList = textList;
            if (random)
            {
                gen = new Random();
                setRandomText();
            }

            timer = null;
            autoForward = true;
        }

        /// <summary>
        /// Updates the label. If a timer has been defined using
        /// the setTimer method automatic cycling will occur after
        /// each timer event. If autoforward is true the nextText()
        /// method will be called, otherwise the previousText will trigger.
        /// </summary>
        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (timer != null)
            {
                timer.update(gameTime);
                if (timer.wasTriggered())
                {
                    if (autoForward)
                        nextText();
                    else
                        previousText();
                }
            }
        }

        /// <summary>
        /// Set the text to a random element from the array of 
        /// strings. The method assumes there is at least one string.
        /// </summary>
        public void setRandomText()
        {
            curID = gen.Next(textList.Count);
            setText(textList[curID]);
        }

        /// <summary>
        /// Sets the text to the specified index in the array.
        /// </summary>
        public void setSelection(int nextID)
        {
            this.curID = nextID;
            setText(textList[curID]);
        }

        /// <summary>
        /// Iterates to the next text element. If random is enabled
        /// the next element will be random, otherwise the pointer will 
        /// increment and cycle to either the next index or back to the 
        /// first element.
        /// </summary>
        public void nextText()
        {
            if (random)
            {
                setRandomText();
            }
            else
            {
                curID++;
                if (curID >= textList.Count)
                    curID = 0;
                setText(textList[curID]);
            }
        }

        /// <summary>
        /// Iterates to the previous text element. If random is enabled 
        /// the previous element will be random and not be based on the 
        /// previous element. Otherwise the index in the array of strings
        /// will be decremented and either that index or the last index
        /// in the array will be used.
        /// </summary>
        public void previousText()
        {
            // note that this does not do it for random
            if (random)
            {
                setRandomText();
            }
            else
            {
                curID--;
                if (curID < 0)
                    curID = textList.Count - 1;
                setText(textList[curID]);
            }
        }

        /// <summary>
        /// Gets the current element ID that is displayed from
        /// the array of strings.
        /// </summary>
        public int getElementID()
        {
            return curID;
        }

        /// <summary>
        /// Gets the currently displayed string.
        /// </summary>
        public string getElementString()
        {
            return textList[curID];
        }

        /// <summary>
        /// Gets the total number of elements contained in the list
        /// of strings.
        /// </summary>
        public int getTotalElements()
        {
            return textList.Count;
        }

        /// <summary>
        /// Adds the specified text element to the list of text elements.
        /// </summary>
        public void addText(string newText)
        {
            textList.Add(newText);
        }

        /// <summary>
        /// Sets the timer to have the application automatically increment.
        /// By default this will increment forward.
        /// </summary>
        /// <param name="timer"></param>
        public void setTimer(Timer timer)
        {
            setTimer(timer, true);
        }

        /// <summary>
        /// Sets the timer to have the application automatically increment
        /// or decrement over time. If loopForward is enabled nextText()
        /// will be used; otherwise the previousText() will be used.
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="loopForward"></param>
        public void setTimer(Timer timer, bool loopForward)
        {
            this.timer = timer;
            autoForward = loopForward;
        }
    }
}
