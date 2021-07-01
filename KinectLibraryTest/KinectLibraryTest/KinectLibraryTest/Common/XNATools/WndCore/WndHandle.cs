using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Reflection;

namespace XNATools.WndCore
{
    // TODO: need to add methods to provide further interaction with components

    /// <summary>
    /// Defines a template for groups of components that can be added
    /// to a class that extends WndHandle through components being
    /// defined as WndComponent type objects.
    /// </summary>
    public abstract class WndHandle
    {
        /// <summary>
        /// The default directory containing fonts to enable fast loading without 
        /// needing to specify the directory.
        /// </summary>
        public static string fontDirectory = "WndContent\\Fonts\\";

        #region Instance Variables
        /// <summary>
        /// Represents a code that can be used to reference wnds.</summary>
        protected int wndCode;

        /// <summary>
        /// A reference to the object that this WndHandle is controlled by.
        /// It can be null if there is no parent assigned.</summary>
        protected WndGroup parent;

        /// <summary>
        /// A reference to an instance of input manager. If a parent is 
        /// configured the input manager will be automatically shared to this
        /// class. If thre is not a parent the inpt manager is created when object 
        /// creation occurs. This object provides mouse, gamepad, and keyboard helpers.</summary>
        protected InputManager inputManager;

        /// <summary>
        /// The audio manager must be configured by classes that wish to use it currently.
        /// By default this will be null. This implementation may change at some later stage.
        /// </summary>
        protected AudioManager audioManager;

        /// <summary>
        /// The displayRect defines the bounds of the window. </summary>
        protected Rectangle displayRect;

        /// <summary>
        /// The isVisible property defines whether the window will be drawn. 
        /// Interactions with the window will not be performed unless the
        /// window is visible. </summary>
        protected bool isVisible;

        /// <summary>
        /// The enabled property can be used to determine if a window is enabled.
        /// When enabled is set to false the input class will update, but 
        /// not other components in the class will be updated. No mouse
        /// events will be handled either.
        /// </summary>
        protected bool enabled;

        /// <summary>
        /// A collection of WndComponents that can be controlled by the window.
        /// Once added to the window these components will automatically be updated
        /// and also have mouse events passed to them to be handled
        /// as defined by each WndComponent type.
        /// </summary>
        protected List<WndComponent> components;

        /// <summary>
        /// Enables or disables the automatic mouse interactions with
        /// WndComponents that have been added to the class.
        /// </summary>
        protected bool enableMouseInteraction;
        #endregion

        /// <summary>
        /// Creates a WndHande with a code that can be used to identify wnds in WndGroups.
        /// A displayRect is defined to specify the bounds for the window and the parent indicates
        /// the controlling object. This may be null if this is either a seperate handle
        /// or it is the parent of all the other windows. This method will configure the class to 
        /// either get the existing input manager and audio manager from a parent or a 
        /// new instance of InputManager (wth 2 players as default) and a null object for the 
        /// audio manager will be created when there is no parent.
        /// </summary>
        /// <param name="wndCode">A code that can be used to identify a window.</param>
        /// <param name="displayRect">The location and dimensions for the window.</param>
        /// <param name="parent">The group object that contains this window or null if there is no parent</param>
        public WndHandle(int wndCode, Rectangle displayRect, WndGroup parent)
        {
            this.parent = parent;
            this.displayRect = displayRect;
            this.wndCode = wndCode;
            if (parent != null)
            {
                this.inputManager = parent.getInputManager();
                this.audioManager = parent.getAudioManager();
            }
            else
            {
                inputManager = new InputManager(2);
                audioManager = null;
            }
            isVisible = true;
            enabled = true;
            enableMouseInteraction = true;
            components = new List<WndComponent>();
        }

        /// <summary>
        /// Updates components if the window is enabled, and then if 
        /// mouse interaction is enabled mouse events will be detected 
        /// and passed to the WndComponents contained in the class.
        /// </summary>
        public virtual void update(GameTime gameTime)
        {
            // if this is the top of the chain
            if (parent == null && inputManager != null)
            {
                inputManager.update(gameTime);
            }

            if (!enabled)
                return;

            foreach (WndComponent c in components)
            {
                c.update(gameTime);
            }

            if (!enableMouseInteraction)
                return;

            bool mouseMoved = inputManager.hasMouseMoved();
            bool mouseClickedLeft = inputManager.getMouseReleased(true);
            bool mouseClickedRight = inputManager.getMouseReleased(false);
            bool mousePressedLeft = inputManager.getMousePressed(true);
            bool mousePressedRight = inputManager.getMousePressed(false);

            if (mouseMoved)
            {
                this.mouseMoved(inputManager.getLastCursor(), inputManager.getCursor());

                foreach (WndComponent c in components)
                {
                    if (c.isVisible())
                        c.mouseMoved(inputManager.getLastCursor(), inputManager.getCursor());
                }
            }

            if (mouseClickedLeft)
            {
                this.mouseClickedLeft(inputManager.getCursor());

                foreach (WndComponent c in components)
                {
                    if (c.isVisible())
                        c.mouseClickedLeft(inputManager.getCursor());
                }
            }

            if (mouseClickedRight)
            {
                this.mouseClickedRight(inputManager.getCursor());

                foreach (WndComponent c in components)
                {
                    if (c.isVisible())
                        c.mouseClickedRight(inputManager.getCursor());
                }
            }

            if (mousePressedLeft)
            {
                this.mousePressedLeft(inputManager.getCursor());

                foreach (WndComponent c in components)
                {
                    if (c.isVisible())
                        c.mousePressedLeft(inputManager.getCursor());
                }
            }

            if (mousePressedRight)
            {
                this.mousePressedRight(inputManager.getCursor());

                foreach (WndComponent c in components)
                {
                    if (c.isVisible())
                        c.mousePressedRight(inputManager.getCursor());
                }
            }
        }

        /// <summary>
        /// Draws all the components contained in the WndHandle.
        /// </summary>
        public virtual void draw(SpriteBatch spriteBatch)
        {
            if (!getVisible())
                return;

            foreach (WndComponent c in components)
            {
                if(c.isVisible())
                    c.draw(spriteBatch);
            }
        }

        #region General Get/Set Methods
        /// <summary>
        /// Gets the WndCode, this is a value that can
        /// be used to make windows uniquely identifiable 
        /// within WndGroups.</summary>
        public int getWndCode()
        {
            return wndCode;
        }

        /// <summary>
        /// Gets the parent object for this WndHandle.</summary>
        public WndGroup getParent()
        {
            return parent;
        }

        /// <summary>
        /// Gets the bounds for this WndHandle.</summary>
        public Rectangle getRect()
        {
            return displayRect;
        }

        /// <summary>
        /// Sets whether the content for this window should be drawn
        /// to the screen.</summary>
        public void setVisible(bool isVisible)
        {
            this.isVisible = isVisible;
        }

        /// <summary>
        /// Gets whether the WndHandle is to be drawn to the screen.</summary>
        public bool getVisible()
        {
            return isVisible;
        }

        /// <summary>
        /// Sets whether the object should be enabled. If not enabled
        /// the class will not update the components nor call mouse
        /// event handlers.</summary>
        public void setEnabled(bool isEnabled)
        {
            this.enabled = isEnabled;
        }

        /// <summary>
        /// Gets whether the object should update components and call
        /// mouse event handlers.</summary>
        public bool getEnabled()
        {
            return enabled;
        }

        /// <summary>
        /// Enables or disables the mouse interaction for this object.
        /// When enabled mouse events will be triggered during the update method of this class.
        /// </summary>
        public void setEnableMouseInteraction(bool enableMouseInteraction)
        {
            this.enableMouseInteraction = enableMouseInteraction;
        }

        /// <summary>
        /// Gets whether the mouse interaction is enabled for this object.
        /// When enabled mouse events will be triggered during the update method of this class.
        /// </summary>
        public bool getEnableMouseInteraction()
        {
            return enableMouseInteraction;
        }

        /// <summary>
        /// Adds a component to the window.</summary>
        /// <param name="component">The component to add.</param>
        public void addComponent(WndComponent component)
        {
            components.Add(component);
        }

        /// <summary>
        /// Gets the full set of components associated with this WndHandle
        /// </summary>
        public List<WndComponent> getComponents()
        {
            return components;
        }

        /// <summary>
        /// Set the parent object to reference.</summary>
        /// <param name="parent">The parent.</param>
        public void setParent(WndGroup parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Moves the window and all the components in the object.
        /// It has been defined so that the WndGroups can override this method.
        /// </summary>
        /// <param name="location">The new window location.</param>
        public virtual void moveToAndChildren(Vector2 location)
        {
            foreach(WndComponent c in components)
                c.moveToAndChildren(location, displayRect);

            setLocation(location);
        }

        /// <summary>
        /// Moves the window and all components in the object by a translation.
        /// </summary>
        /// <param name="translation">The amount to move the window and components by.</param>
        public virtual void moveByAndChildren(Vector2 translation)
        {
            foreach (WndComponent c in components)
                c.moveByAndChildren(translation);

            moveLocationBy(translation);
        }

        /// <summary>
        /// Sets the location of the window object. NOTE this will NOT move components!
        /// If the location should change components too, either the moveToAndChildren
        /// or moveByAndChildren methods should be used.
        /// </summary>
        /// <param name="location">The location to move this WndHandle to.</param>
        public void setLocation(Vector2 location)
        {
            this.displayRect.X = (int)location.X;
            this.displayRect.Y = (int)location.Y;
        }

        /// <summary>
        /// Translates only this window and not the components. If movement of
        /// the components is required the moveToAndChildren or moveByAndChildren methods
        /// should be used instead.
        /// </summary>
        /// <param name="translation">The amount to translate this WndHandle by.</param>
        public void moveLocationBy(Vector2 translation)
        {
            setLocation(new Vector2(displayRect.X, displayRect.Y) + translation);
        }

        /// <summary>
        /// Sets the audio manager to the specified instance.</summary>
        public virtual void setAudioManager(AudioManager audioManager)
        {
            this.audioManager = audioManager;
        }

        /// <summary>
        /// Sets the input manager to the specified instance.</summary>
        public virtual void setInputManager(InputManager inputManager)
        {
            this.inputManager = inputManager;
        }
        #endregion

        #region Helper Methods for Loading Content
        /// <summary>
        /// If a parent is not null the specified Texture2D file will be loaded.
        /// </summary>
        /// <param name="file">The file to load.</param>
        public Texture2D loadTexture(string file)
        {
            if (parent == null)
                return null;
            return parent.getAppRef().Content.Load<Texture2D>(file);
        }

        /// <summary>
        /// If a parent is not null the specified SpriteFont file will be loaded.
        /// Note that only the name of the file needs to be used, unless the
        /// fontDirectory variable is not correctly set to the directory
        /// containing the fonts.
        /// </summary>
        /// <param name="file">The file to load.</param>
        public SpriteFont loadFont(string file)
        {
            if (parent == null)
                return null;
            return parent.getAppRef().Content.Load<SpriteFont>(fontDirectory + file);
        }

        /// <summary>
        /// If a parent is not null the specified SoundEffect file will be loaded.
        /// </summary>
        /// <param name="file">The file to load.</param>
        public SoundEffect loadSound(string file)
        {
            if (parent == null)
                return null;
            return parent.getAppRef().Content.Load<SoundEffect>(file);
        }
        #endregion

        #region Overridable Methods for window events
        /// <summary>
        /// This method can be used to define functionality that must be called processed right before a WndHandle is 
        /// closed. At this time the object will still be referenced inside of a WndGroup, so references
        /// to the parent can still be used.
        /// </summary>
        public virtual void onWndClosing()
        {
        }

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
        /// This method will be called by the WndHandle
        /// when a left mouse button release occurs. By default it will not do
        /// anything and should be overidden by components to perform
        /// any interaction that is required of them.
        /// </summary>
        /// <param name="p">The point that a mouse even has occured.</param>
        public virtual void mouseClickedLeft(Point p)
        {

        }

        /// <summary>
        /// This method will be called by the WndHandle
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

        #region Window Template Creation
        /// <summary>
        /// Used to create a WndHandle with only needing to know the Type of the object.
        /// </summary>
        /// <param name="type">The object type to be created, this MUST extend WndHandle.</param>
        /// <param name="displayRect">Rectangle that defines the size and location of the window.</param>
        /// <param name="parent">The parent that controls the WndHandle.</param>
        /// <returns>A new object of type WndHandle or null if object creation failed.</returns>
        public static WndHandle createWnd(Type type, Rectangle displayRect, WndGroup parent)
        {
            try
            {
                ConstructorInfo ctor = type.GetConstructor(new[] { typeof(Rectangle), typeof(WndGroup) });
                return (WndHandle)ctor.Invoke(new object[] { displayRect, parent });
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Used to create a WndHandle with only needing to know the Type of the object.
        /// </summary>
        /// <typeparam name="T">A class name of type WndHandle that can be instantiated using a 
        /// Rectangle and WndGroup constructor.</typeparam>
        /// <param name="displayRect">Rectangle that defines the size and location of the window.</param>
        /// <param name="parent">The parent that controls the WndHandle.</param>
        /// <returns>A new object of type WndHandle or null if object creation failed.</returns>
        public static T createWnd<T>(Rectangle displayRect, WndGroup parent) where T : WndHandle
        {
            return (T)createWnd(typeof(T), displayRect, parent);
            /*try
            {
                Type type = typeof(T);
                ConstructorInfo ctor = type.GetConstructor(new[] { typeof(Rectangle), typeof(WndGroup) });
                return (T)ctor.Invoke(new object[] { displayRect, parent });
            }
            catch
            {
                return default(T);
            }*/
        }
        #endregion
    }
}
