using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace XNATools.WndCore
{
    /// <summary>
    /// The Button object is used to display simple WndComponents that
    /// can swap between a selected and an unselected state. TextButton 
    /// provides additional functionality for this class by adding a text
    /// field, and ButtonCollection can be used for button control groups.
    /// </summary>
    public class Button : WndComponent
    {
        #region Instance Variables
        /// <summary>
        /// Contains the texture to be shown when the button is selected.</summary>
        protected Texture2D selected;

        /// <summary>
        /// Contains the texture to be shown when the button is not selected.</summary>
        protected Texture2D unselected;

        /// <summary>
        /// Stores the current selection state of a button. This will be changed to true 
        /// automatically when a mouse is over the object. The exception is when the 
        /// enableMouseOverSwap property has been set to false. Then manual selection
        /// will be required.</summary>
        protected bool isSelected;

        /// <summary>
        /// The actionID can be used to indicate which button has been selected or a 
        /// piece of information that will be useful for determining what is to be done
        /// when a button has been "clicked".</summary>
        protected int actionID;

        /// <summary>
        /// Whenever a click occurs and the button contains the point this will be enabled.
        /// This property can be accessed via the getIsClicked() method.</summary>
        protected bool isClicked;

        /// <summary>
        /// This when set to false will disable the mouseover type interaction.</summary>
        protected bool enableMouseOverSwap;
        #endregion

        /// <summary>
        /// Creates a generic button with a location, and textures for the selected,
        /// and unselected states. It will default to not selected and an action id of 0.
        /// </summary>
        /// <param name="dest">The location and dimensions for the button.</param>
        /// <param name="selected">The texture to be shown when the object is selected.</param>
        /// <param name="unselected">The texture to be shown when the object is not selected.</param>
        public Button(Rectangle dest, Texture2D selected, Texture2D unselected)
            : this(dest, selected, unselected, false, 0)
        {
        }

        /// <summary>
        /// Creates a button with a location, and textures for the selected,
        /// and unselected states. The initial selection and action ID values can also be set.
        /// </summary>
        /// <param name="dest">The location and dimensions for the button.</param>
        /// <param name="selected">The texture to be shown when the object is selected.</param>
        /// <param name="unselected">The texture to be shown when the object is not selected.</param>
        /// <param name="isSelected">The initial selection state.</param>
        /// <param name="actionID">A reference number that can be later retrieved for event handling.</param>
        public Button(Rectangle dest, Texture2D selected, Texture2D unselected, bool isSelected, int actionID)
            : base(dest, ((isSelected) ? selected : unselected))
        {
            this.isSelected = isSelected;
            this.selected = selected;
            this.unselected = unselected;
            this.actionID = actionID;
            enableMouseOverSwap = false;
        }

        /// <summary>
        /// When update is called the clicked state will be reset to false.</summary>
        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            clearClick();
        }

        /// <summary>
        /// When a click occurs the isClicked property can be accessed 
        /// via the getIsClicked() method to determine if the click was 
        /// inside this button. </summary>
        /// <param name="p">The point where the click occured.</param>
        public override void mouseClickedLeft(Point p)
        {
            isClicked = dest.Contains(p);
        }

        /// <summary>
        /// While the mouse over swap is enabled the selection state of 
        /// this button will change depedent on whether the new position 
        /// is inside the object's bounds.</summary>
        /// <param name="oldP">Old point.</param>
        /// <param name="newP">New Point</param>
        public override void mouseMoved(Point oldP, Point newP)
        {
            base.mouseMoved(oldP, newP);

            if (enableMouseOverSwap) 
                setSelected(dest.Contains(newP));
        }

        /// <summary>
        /// Clears the clicked state of the button.</summary>
        public void clearClick()
        {
            isClicked = false;
        }

        #region Get/Set methods
        /// <summary>
        /// Set the state of the button and update the displayed texture
        /// to the correct one based on the new selection state.</summary>
        /// <param name="isSelected">The new selection state.</param>
        public void setSelected(bool isSelected)
        {
            this.isSelected = isSelected;

            setTexture((isSelected) ? selected : unselected);
        }

        /// <summary>
        /// Gets the current state of whether the button is selected.</summary>
        public bool getSelected()
        {
            return isSelected;
        }

        /// <summary>
        /// Set the actionID to actionID. This property is used for button event handling.</summary>
        /// <param name="actionID">The actionID to set as the value for the button.</param>
        public void setActionID(int actionID)
        {
            this.actionID = actionID;
        }

        /// <summary>
        /// Get the actionID, this can be used to determine what a 
        /// button does when it is clicked. </summary>
        /// <returns>The actionID.</returns>
        public int getActionID()
        {
            return actionID;
        }

        /// <summary>
        /// Gets if the button has been clicked after the last update.</summary>
        /// <returns>If the button has been clicked.</returns>
        public bool getIsClicked()
        {
            return isClicked;
        }

        /// <summary>
        /// Gets if the mouse over event triggers the select and unselected 
        /// state swapping. </summary>
        /// <returns>If mouseover triggers selection.</returns>
        public bool getEnableMouseOverSwap()
        {
            return enableMouseOverSwap;
        }

        /// <summary>
        /// Set if the mouse over event triggers the select and unselected 
        /// state swapping. </summary>
        /// <param name="enableMouseOverSwap">Set to true if mouseover should change the appearance.</param>
        public void setEnableMouseOverSwap(bool enableMouseOverSwap)
        {
            this.enableMouseOverSwap = enableMouseOverSwap;
        }
        #endregion
    }
}
