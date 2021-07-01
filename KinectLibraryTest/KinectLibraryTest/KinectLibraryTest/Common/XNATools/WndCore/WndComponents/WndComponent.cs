using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace XNATools.WndCore
{
    /// <summary>
    /// This is the base class for all window elements that can
    /// be placed inside of WndHandles. It provides the basic 
    /// information necessary to represent an object with 
    /// methods that can be overridden to provide functionality.</summary>
    public class WndComponent
    {
        #region Instance Variables
        /// <summary>
        /// Location stores the real location that the object is to be drawn on screen.</summary>
        protected Vector2 location;
        
        /// <summary>
        /// Object Size represents the objects width and height properties.</summary>
        protected Vector2 objectSize;
        
        /// <summary>
        /// Dest contains the combination of the location and width/height.</summary>
        protected Rectangle dest;
        
        /// <summary>
        /// Texture contains the background graphic that will be rendered 
        /// with size dest if texture is valid and visibible.</summary>
        protected Texture2D texture;
        
        /// <summary>
        /// Visible indicates if the object should be drawn to the screen.</summary>
        protected bool visible;
        
        /// <summary>
        /// Has Focus is used for objects that can have focus placed on them.
        /// This is used to determine interaction properties.</summary>
        protected bool hasFocus;
        
        /// <summary>
        /// When the texture initially supplied is null this will be set to false.
        /// The texture will only be drawn if this variable has been set to true.</summary>
        protected bool bgTextureSet;
        
        /// <summary>
        /// FocusRect defines the bounds that are used to gain focus of this object within.
        /// A rectangle defined as (-1.-1,0,0) will yield a non-focusable object.</summary>
        protected Rectangle focusRect;
        #endregion

        // TODO: Implement these
        protected WndHandle parentWnd;
        protected WndComponent parentComponent;

        /// <summary>
        /// Basic constructor that will set the screen region
        /// for display to dest with no background. The object will
        /// be visible, but the texture set property will be disabled.
        /// </summary>
        /// <param name="dest">The screen region for the object to be rendered at.</param>
        public WndComponent(Rectangle dest)
            : this(dest, null)
        {
            bgTextureSet = false;
        }

        /// <summary>
        /// Primary constructor that sets a screen region and background texture 
        /// for the object. The object will be visible and default to a focus rect
        /// that uses the objects dest instead of a separate Rectangle.
        /// If the texture supplied in null the texture will not be drawn.
        /// </summary>
        /// <param name="dest">The screen region for the object to be rendered at.</param>
        /// <param name="texture">The texture to be drawn as a background or null.</param>
        public WndComponent(Rectangle dest, Texture2D texture)
        {
            setRect(dest);
            this.texture = texture;
            visible = true;
            hasFocus = false;
            bgTextureSet = texture != null;
            focusRect = new Rectangle(-1, -1, 0, 0);
        }

        /// <summary>
        /// Basic implementation of the update method. This is used to 
        /// provide a generic update method that can be called for any WndComponent.
        /// </summary>
        public virtual void update(GameTime gameTime)
        {

        }

        /// <summary>
        /// Basic implementation of the draw method. This is used to 
        /// provide a generic update method that can be called for any WndComponent.
        /// </summary>
        public virtual void draw(SpriteBatch spriteBatch)
        {
            if (visible && bgTextureSet)
            {
                //, new Rectangle(0,0,texture.Width, texture.Height)
                spriteBatch.Draw(texture, dest, Color.White);
            }
        }

        /// <summary>
        /// Set the location and dimenions for the component to be placed at.
        /// <param name="dest">The location and size for the object to be drawn at.</param>
        public virtual void setRect(Rectangle dest)
        {
            Rectangle newDest = new Rectangle();
            newDest.X = dest.X;
            newDest.Y = dest.Y;
            newDest.Width = dest.Width;
            newDest.Height = dest.Height;
            this.dest = newDest;

            location = new Vector2(newDest.X, newDest.Y);
            objectSize = new Vector2(newDest.Width, newDest.Height);
        }

        /// <summary>
        /// Updates only the location of the component without changing size.
        /// </summary>
        /// <param name="location">The new location to place the object at.</param>
        public virtual void setLocation(Vector2 location)
        {
            this.location.X = location.X;
            this.location.Y = location.Y;
            dest.X = (int)location.X;
            dest.Y = (int)location.Y;
        }

        /// <summary>
        /// Sets the background texture for this component. If the texture is
        /// null no background will be displayed.
        /// </summary>
        /// <param name="texture">The new texture to shows as the background or null.</param>
        public void setTexture(Texture2D texture)
        {
            this.texture = texture;
            bgTextureSet = texture != null;
        }

        /// <summary>
        /// Moves this object by a translation based on the existing location.
        /// </summary>
        /// <param name="translation">The offset to move the component by.</param>
        public virtual void moveLocationBy(Vector2 translation)
        {
            setLocation(getLocation() + translation);
        }

        /// <summary>
        /// Moves the component this is applied to by determining the translation
        /// that needs to be applied when a parent object is moved and this must 
        /// move with the same translation.
        /// </summary>
        /// <param name="location">The new location the parent or this object (if parent) will be located at.</param>
        /// <param name="parentRect">The parent component or window's position relative to this object.</param>
        public virtual void moveToAndChildren(Vector2 location, Rectangle parentRect)
        {
            Vector2 translation = new Vector2(location.X - parentRect.X, location.Y - parentRect.Y);
            moveByAndChildren(translation);
        }

        /// <summary>
        /// Moves the component and its children by a translation in location.
        /// </summary>
        /// <param name="translation">The translation to move the component and children by.</param>
        public virtual void moveByAndChildren(Vector2 translation)
        {
            moveLocationBy(translation);
        }

        /// <summary>
        /// Sets the size of the component.
        /// </summary>
        /// <param name="dimensions">The new width and height.</param>
        public void setSize(Vector2 dimensions)
        {
            this.objectSize.X = dimensions.X;
            this.objectSize.Y = dimensions.Y;
            dest.Width = (int)objectSize.X;
            dest.Height = (int)objectSize.Y;
        }

        /// <summary>
        /// Gets the size of the component.
        /// </summary>
        public Vector2 getSize()
        {
            return objectSize;
        }

        /// <summary>
        /// Gets the location of the component.
        /// </summary>
        public Vector2 getLocation()
        {
            return location;
        }

        /// <summary>
        /// Gets the location and size of the object represented as a Rectangle.
        /// </summary>
        public Rectangle getRect()
        {
            return dest;
        }

        /// <summary>
        /// Gets the focus rectangle that is used by interactive components.
        /// If the x coordinate of the specified rectangle is -1 the base
        /// dest object will be returned instead. (using getRect()).
        /// </summary>
        public Rectangle getFocusRect()
        {
            if (focusRect.X == -1)
                return getRect();
            else
                return focusRect;
        }

        /// <summary>
        /// Sets the focus rect to the specified rectangle.
        /// </summary>
        /// <param name="focusRect">The new focus rect to set.</param>
        public void setFocusRect(Rectangle focusRect)
        {
            this.focusRect = focusRect;
        }

        /// <summary>
        /// Sets the visible property. If an object is not visible it will
        /// not be rendered to the screen.
        /// </summary>
        /// <param name="visible">Set to true to make visible.</param>
        public void setVisible(bool visible)
        {
            this.visible = visible;
        }

        /// <summary>
        /// Gets the visible property. If this is true the object will
        /// be rendered to the screen.
        /// </summary>
        /// <returns></returns>
        public bool isVisible()
        {
            return visible;
        }

        /// <summary>
        /// Sets this component to have focus. This is used for when 
        /// an interactive component becomes active and is craving attention.
        /// </summary>
        /// <param name="hasFocus">Whether this object is in focus.</param>
        public virtual void setFocus(bool hasFocus)
        {
            this.hasFocus = hasFocus;
        }

        /// <summary>
        /// Gets the hasFocus property that represents if 
        /// this object is interactive and has focus for performing
        /// some action.</summary>
        public bool getHasFocus()
        {
            return hasFocus;
        }

        /// <summary>
        /// Next is used for some interactive components that require iteration>
        /// The default implementation will not do anything at all, but it can be 
        /// overridden to provide iteration functionality that goes to the next.
        /// </summary>
        public virtual void next()
        {

        }

        /// <summary>
        /// Previous is used for some interactive components that require iteration.
        /// The default implementation will not do anything at all, but it can be
        /// overrdden to provide iteration functionality that goes to the previous.
        /// </summary>
        public virtual void previous()
        {

        }

        #region Mouse Events
        /// <summary>
        /// This method is called when the mouse cursor is moved and is triggered by 
        /// the WndHandle object or some other component in a window. Using this method
        /// it can be determined if a mouseEntered or mouseExited event has occured if
        /// necessary. Eg. if(getRect().contains(old) && !getRect.contains(newP)) 
        /// would detect a mouse exited event. By default this method will not
        /// perform any action and it should be overidden by other classes to provide
        /// any necessary functonality.
        /// </summary>
        /// <param name="oldP">The previous point the mouse was at before moving.</param>
        /// <param name="newP">The point the mouse is currently at.</param>
        public virtual void mouseMoved(Point oldP, Point newP)
        {

        }

        /// <summary>
        /// This method will be called by the WndHandle or another component
        /// when a left mouse button release occurs. By default it will not do
        /// anything and should be overidden by components to perform
        /// any interaction that is required of them.
        /// </summary>
        /// <param name="p">The point that a mouse even has occured.</param>
        public virtual void mouseClickedLeft(Point p)
        {

        }

        /// <summary>
        /// This method will be called by the WndHandle or another component
        /// when a right mouse button release occurs. By default it will not do
        /// anything and should be overidden by components to perform
        /// any interaction that is required of them.
        /// </summary>
        /// <param name="p">The point that a mouse even has occured.</param>
        public virtual void mouseClickedRight(Point p)
        {

        }

        /// <summary>
        /// This method will be called by the WndHandle or another component
        /// when a left mouse button press occurs. By default it will not do
        /// anything and should be overidden by components to perform
        /// any interaction that is required of them.
        /// </summary>
        /// <param name="p">The point that a mouse even has occured.</param>
        public virtual void mousePressedLeft(Point p)
        {

        }

        /// <summary>
        /// This method will be called by the WndHandle or another component
        /// when a right mouse button press occurs. By default it will not do
        /// anything and should be overidden by components to perform
        /// any interaction that is required of them.
        /// </summary>
        /// <param name="p">The point that a mouse even has occured.</param>
        public virtual void mousePressedRight(Point p)
        {

        }
        #endregion
    }
}
