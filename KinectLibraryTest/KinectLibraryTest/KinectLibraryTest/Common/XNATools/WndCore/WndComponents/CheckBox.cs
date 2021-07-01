using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNATools.WndCore
{
    /// <summary>
    /// Check boxes are a special type of panel that are designed to have a selected and an 
    /// unselected state. They contain a Button object and this button can be used to switch 
    /// the state of the checkbox.
    /// </summary>
    public class CheckBox : Panel
    {
        // TODO: Extend this class to enable more control over label and button settings
        // TODO: Also should extend this class to make RadioButton type things

        /// <summary>
        /// The button that can be clicked to iterate the checkbox to the next state.
        /// </summary>
        protected Button check;

        /// <summary>
        /// The text that is displayed next to the checkbox.
        /// </summary>
        protected Label text;

        /// <summary>
        /// A variable that indicates this component's state has changed. This can be used
        /// to perform conditional events or change other external components.
        /// </summary>
        protected bool stateChanged;

        /// <summary>
        /// Basic constructor for the Checkbox class that allows specification of the dest for the
        /// checkbox, the images for selected and unselected, the text to display next to the checkbox, 
        /// and the font to use for drawing the text. The checkbox image will be placed on the left
        /// side and it's height and width will be the height of the rectangle. So the rectangle 
        /// should be wider than it is tall to assume enough space for placement of text alongside it to
        /// the right of the graphic.
        /// </summary>
        /// <param name="dest">The region to place the checkbox at. The image will take up size (height, height)</param>
        /// <param name="selected">The image shown when the checkbox is selected.</param>
        /// <param name="unselected">The image shown when the checkbox is not selected.</param>
        /// <param name="text">The text to show alongside the checkbox.</param>
        /// <param name="font">The font to render the text with.</param>
        public CheckBox(Rectangle dest, Texture2D selected, Texture2D unselected, string text, SpriteFont font)
            : base(dest)
        {
            check = new Button(new Rectangle(dest.X, dest.Y, dest.Height, dest.Height), selected, unselected);
            check.setSelected(true);
            this.text = new Label(new Rectangle(dest.X + dest.Height + 5, dest.Y, dest.Width - dest.Height, dest.Height), text, font);
            this.text.centreInRect(Label.CentreMode.CentreVertical);
            stateChanged = false;

            addComponent(check);
            addComponent(this.text);
        }

        /// <summary>
        /// Clears the state changed returning the state back to the default.
        /// </summary>
        public override void update(GameTime gameTime)
        {
            stateChanged = false;
            base.update(gameTime);
        }

        /// <summary>
        /// If the mouse click was inside the button the checkbox
        /// will swap the state to the other state.
        /// </summary>
        /// <param name="p">The point to test if inside the button region.</param>
        public override void mouseClickedLeft(Point p)
        {
            base.mouseClickedLeft(p);
            if (check.getIsClicked())
            {
                next();
            }
        }

        /// <summary>
        /// Sets the state of the button to be whatever it wasn't
        /// before this method was called. This method will also
        /// flag the stateChanged variable to true, so the 
        /// getChanged() method can be used until the next update
        /// to determine if this checkbox has changed its value.
        /// </summary>
        public override void next()
        {
            check.setSelected(!check.getSelected());
            stateChanged = true;
        }

        /// <summary>
        /// Calls next to toggle the selected state of
        /// the Checkbox object.
        /// </summary>
        public override void previous()
        {
            next();
        }

        /// <summary>
        /// Gets the current selected state.
        /// </summary>
        public bool isSelected()
        {
            return check.getSelected();
        }

        /// <summary>
        /// Gets if the state has been changed since the last update.
        /// This will be true if the checkbox has been toggled using the
        /// next method(). The value is reset to false whenever the 
        /// update method is called.
        /// </summary>
        public bool getChanged()
        {
            return stateChanged;
        }
    }
}
