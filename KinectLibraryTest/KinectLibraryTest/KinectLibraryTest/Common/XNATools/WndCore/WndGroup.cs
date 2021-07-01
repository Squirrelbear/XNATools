using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace XNATools.WndCore
{
    /// <summary>
    /// WndGroup type objects should control windows defined as WndHandle
    /// type objects. Since WndGroup objects inherit from the WndHandle object
    /// is it possible to also have WndGroup objects inside of other WndGroup objects.
    /// </summary>
    public class WndGroup : WndHandle
    {
        #region Instance Variables
        /// <summary>
        /// A reference to the primary Game object. This makes it possible to 
        /// use the loadTexture and other loading methods contained in any WndHandle 
        /// through a parent->child structure. It is important that this should not be null.
        /// </summary>
        protected Game appRef;

        /// <summary>
        /// The list of active wndHandles. These windows are updated and drawn to the screen
        /// in addition to all other normal functions that should be applied. 
        /// </summary>
        protected List<WndHandle> wndHandles;

        /// <summary>
        /// The removeList contains the list of queued removals that should occur. 
        /// These are objects that were marked typically for removal from the wndHandles
        /// list during an update. Since removal during an update would cause potential 
        /// crashes this is kept as a separate tempory list of pointers to perform the update.
        /// </summary>
        protected List<WndHandle> removeList;

        /// <summary>
        /// Like the remove list array this array contains a list of elements that need to be 
        /// added to the wndHandles object at the end of an update cycle. 
        /// </summary>
        protected List<WndHandle> addList;
        #endregion

        /// <summary>
        /// Constructor to create a WndGroup. This will initialise the
        /// variables required for storing a collection of WndHandle 
        /// references. Since this object is also a WndHandle the WndGroup
        /// is initialised with the specified details.
        /// </summary>
        /// <param name="wndCode">A code that can be used to reference individual WndHandles.</param>
        /// <param name="displayRect">The dimenions that encompass the WndGroup.</param>
        /// <param name="parent">The parent for the WndGroup, often this will just be null if this is the only group.</param>
        /// <param name="appRef">This must be a reference to the application. WARNING: Not setting this to a reference may cause issues!</param>
        public WndGroup(int wndCode, Rectangle displayRect, WndGroup parent, Game appRef)
            : base(wndCode, displayRect, parent) 
        {
            this.appRef = appRef;

            wndHandles = new List<WndHandle>();
            removeList = new List<WndHandle>();
            addList = new List<WndHandle>();
        }

        /// <summary>
        /// Handles the event for when an onWndClosing() is called for a WndGroup.
        /// All of the currently active WndHandle objects are notified of this event.
        /// </summary>
        public override void onWndClosing()
        {
            foreach (WndHandle handle in wndHandles)
            {
                if (handle.getEnabled())
                    handle.onWndClosing();
            }
        }

        /// <summary>
        /// Updates all the current WndHandles and then
        /// performs adding of new WndHandles from the addList. 
        /// Then performs removals from the removeList.
        /// </summary>
        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            foreach (WndHandle handle in wndHandles)
            {
                if(handle.getEnabled())
                    handle.update(gameTime);
            }

            // do the adding
            foreach (WndHandle addHandle in addList)
            {
                wndHandles.Add(addHandle);
            }
            addList.Clear();

            // do the removals
            foreach (WndHandle removeHandle in removeList)
            {
                removeHandle.onWndClosing();
                wndHandles.Remove(removeHandle);
            }
            removeList.Clear();

            if (parent == null && audioManager != null)
            {
                audioManager.update(gameTime);
            }
        }

        /// <summary>
        /// Draws all the WndHandles onto the screen. 
        /// NOTE: This method for WndGroup objects that have parent 
        /// set to null will call spriteBatch.Begin() and
        /// spriteBatch.End(). This means that there should not
        /// be a spriteBatch.Begin called prior to calling this method
        /// unless there is a matching spriteBatch.End() call prior to 
        /// this being called. </summary>
        public override void draw(SpriteBatch spriteBatch)
        {
            if(parent == null)
                spriteBatch.Begin();

            base.draw(spriteBatch);

            foreach (WndHandle handle in wndHandles)
            {
                if (handle.getVisible())
                    handle.draw(spriteBatch);
            }

            if (parent == null)
                spriteBatch.End();
        }

        /// <summary>
        /// Moves the WndGroup to the location specified and will move any
        /// components or windows that are grouped into this object.
        /// </summary>
        /// <param name="location">The location to move to.</param>
        public override void moveToAndChildren(Vector2 location)
        {
            Vector2 translation = new Vector2(displayRect.X - location.X, displayRect.Y - location.Y);

            moveByAndChildren(translation);
        }

        /// <summary>
        /// Moves the WndGroup by a specified translation. This is
        /// applied to all child components and WndHandle objects that
        /// have been added to the WndGroup.
        /// </summary>
        /// <param name="translation">The amount to translate by.</param>
        public override void moveByAndChildren(Vector2 translation)
        {
            foreach (WndComponent c in components)
                c.moveByAndChildren(translation);

            foreach (WndHandle handle in wndHandles)
                handle.moveByAndChildren(translation);

            moveLocationBy(translation);
        }

        #region add/remove/manage wndHandles
        /// <summary>
        /// If the current WndHandle that is open is not the same
        /// type as the one that is being told to open all WndHandles 
        /// will be removed and a new object of type T will become the
        /// primary window.
        /// </summary>
        /// <typeparam name="T">A WndHandle type that can be added as the new primary WndHandle.</typeparam>
        public void setWnd<T>() where T : WndHandle
        {
            setWnd(typeof(T));
        }

        /// <summary>
        /// If the current WndHandle that is open is not the same
        /// type as the one that is being told to open all WndHandles 
        /// will be removed and a new object of type T will become the
        /// primary window.
        /// </summary>
        /// <typeparam name="T">A WndHandle type that can be added as the new primary WndHandle.</typeparam>
        /// <param name="displayRect">The rectangle to use for the display dimensions.</param>
        public void setWnd<T>(Rectangle displayRect) where T : WndHandle
        {
            setWnd(typeof(T), displayRect);
        }

        /// <summary>
        /// If the current WndHandle that is open is not the same
        /// type as the one that is being told to open all WndHandles 
        /// will be removed and a new object of type will become the
        /// primary window. The rectangle will be set to the same as this group.
        /// </summary>
        /// <param name="type">A WndHandle type object</param>
        public void setWnd(Type type)
        {
            int wndHeight = getAppRef().GraphicsDevice.Viewport.Height;
            int wndWidth = getAppRef().GraphicsDevice.Viewport.Width;
            Rectangle displayRect = new Rectangle(0, 0, wndWidth, wndHeight);
            setWnd(type, displayRect);
        }

        /// <summary>
        /// If the current WndHandle that is open is not the same
        /// type as the one that is being told to open all WndHandles 
        /// will be removed and a new object of type T will become the
        /// primary window. The rectangle will be the displayRect specified.
        /// </summary>
        /// <param name="type">The type of WndHandle to create.</param>
        /// <param name="displayRect">The rectangle to use for the WndHandles bounds.</param>
        public void setWnd(Type type, Rectangle displayRect)
        {
            WndHandle handle = getWndAt(0);
            if (handle == null || handle.GetType() != type)
            {
                removeAllWnd();
                addWnd(WndHandle.createWnd(type, displayRect, this));
            }
        }

        /// <summary>
        /// Adds a WndHandle to the WndGroup. This method will just
        /// queue the adding of the WndHandle. It will not be in the 
        /// main list of WndHandles until the end of the update cycle 
        /// for this object.
        /// </summary>
        /// <param name="wnd">The WndHandle to add.</param>
        public void addWnd(WndHandle wnd)
        {
            addList.Add(wnd);
        }

        /*public void addWnd(WndHandle wnd, int index)
        {
            if (index <= 0)
                wndHandles.Insert(0, wnd);
            else if (index >= wndHandles.Count)
                addWnd(wnd);
            else
                wndHandles.Insert(index, wnd);
        }*/

        /// <summary>
        /// Removes a WndHandle to the WndGroup. This method will just
        /// queue the removing of the WndHandle. It will not be in the 
        /// main list of WndHandles until the end of the update cycle 
        /// for this object.
        /// </summary>
        /// <param name="wnd">The WndHandle to remove.</param>
        public void removeWnd(WndHandle wnd)
        {
            if(!removeList.Contains(wnd))
                removeList.Add(wnd);
            //wndHandles.Remove(wnd);
        }

        /// <summary>
        /// Removes all WndHandles that have the specified "wndID" code.
        /// The WndHandles will not be removed until the end of the update
        /// cycle for this object.
        /// </summary>
        /// <param name="wndID">The id to find and remove.</param>
        /// <returns>Returns true if there were any WndHandles found to remove.</returns>
        public bool removeAllWnd(int wndID)
        {
            int removeCount =  0;
            foreach(WndHandle handle in wndHandles)
            {
                if (handle.getWndCode() == wndID && !removeList.Contains(handle))
                {
                    removeList.Add(handle);
                    removeCount++;
                }
            }

            return (removeCount > 0);
        }

        /// <summary>
        /// Removes all WndHandles that have the specified type.
        /// The WndHandles will not be removed until the end of the update
        /// cycle for this object.
        /// </summary>
        /// <param name="type">The WndHandle type to find and remove.</param>
        /// <returns>Returns true if there were any WndHandles found to remove.</returns>
        public bool removeAllWnd(Type type)
        {
            int removeCount = 0;
            foreach (WndHandle handle in wndHandles)
            {
                if (handle.GetType() == type && !removeList.Contains(handle))
                {
                    removeList.Add(handle);
                    removeCount++;
                }
            }

            return (removeCount > 0);
        }

        /// <summary>
        /// Removes all WndHandles.
        /// The WndHandles will not be removed until the end of the update
        /// cycle for this object.
        /// </summary>
        /// <returns>Returns true if there were any WndHandles found to remove.</returns>
        public bool removeAllWnd()
        {
            int removeCount = 0;
            foreach (WndHandle handle in wndHandles)
            {
                removeWnd(handle);
                removeCount++;
            }

            return (removeCount > 0);
        }

        /// <summary>
        /// Removes the first WndHandle found with specified wndID.
        /// The WndHandle will not be removed until the end of the
        /// update cycle for this object.
        /// </summary>
        /// <param name="wndID">The wndID to remove.</param>
        /// <returns>Returns true if there were any WndHandles found to remove.</returns>
        public bool removeFirstWnd(int wndID)
        {
            WndHandle removeHandle = null;
            foreach (WndHandle handle in wndHandles)
            {
                if (handle.getWndCode() == wndID)
                {
                    removeHandle = handle;
                    break;
                }
            }

            if (removeHandle == null)
                return false;

            if (!removeList.Contains(removeHandle))
                removeList.Add(removeHandle);
            //wndHandles.Remove(removeHandle);
            return true;
        }

        /// <summary>
        /// Removes the first WndHandle found with specified type.
        /// The WndHandle will not be removed until the end of the
        /// update cycle for this object.
        /// </summary>
        /// <param name="type">The type to remove.</param>
        /// <returns>Returns true if there were any WndHandles found to remove.</returns>
        public bool removeFirstWnd(Type type)
        {
            WndHandle removeHandle = null;
            foreach (WndHandle handle in wndHandles)
            {
                if (handle.GetType() == type)
                {
                    removeHandle = handle;
                    break;
                }
            }

            if (removeHandle == null)
                return false;

            if (!removeList.Contains(removeHandle))
                removeList.Add(removeHandle);
            //wndHandles.Remove(removeHandle);
            return true;
        }

        /// <summary>
        /// Removes the WndHandle found at the specified index.
        /// The WndHandle will not be removed until the end of the
        /// update cycle for this object.
        /// </summary>
        /// <param name="index">The index to remove at.</param>
        /// <returns>Returns true if there were any WndHandles found to remove.</returns>
        public bool removeWndAt(int index)
        {
            if (index < 0 || index >= wndHandles.Count)
                return false;

            if (!removeList.Contains(wndHandles[index]))
                removeList.Add(wndHandles[index]);
            //wndHandles.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Changes the order of the WndHandles so that the
        /// specified wndID is moved to the front. This will only
        /// apply to the first one found.
        /// </summary>
        /// <param name="wndID">The wndID to move.</param>
        public void bringWndToFront(int wndID)
        {
            WndHandle moveHandle = null;
            foreach (WndHandle handle in wndHandles)
            {
                if (handle.getWndCode() == wndID)
                {
                    moveHandle = handle;
                    break;
                }
            }

            bringWndToFront(moveHandle);
        }

        /// <summary>
        /// Moves the WndHandle at the specified index to the
        /// front. </summary>
        /// <param name="index">The index to move.</param>
        public void bringWndToFrontFrom(int index)
        {
            WndHandle moveHandle = null;
            moveHandle = (index < 0 || index >= wndHandles.Count) ? null : wndHandles[index];

            bringWndToFront(moveHandle);
        }

        public void bringWndToFront(WndHandle wndHandle)
        {
            // TODO: This method won't actually work
            if (wndHandle != null)
            {
                removeWnd(wndHandle);
                addWnd(wndHandle);
            }
        }

        /// <summary>
        /// Sends the specified WndHandle by wndID to the back.
        /// Only applies to the first one found.
        /// </summary>
        /// <param name="wndID">The wndID to move.</param>
        public void sendWndToBack(int wndID)
        {
            WndHandle moveHandle = null;
            foreach (WndHandle handle in wndHandles)
            {
                if (handle.getWndCode() == wndID)
                {
                    moveHandle = handle;
                    break;
                }
            }

            sendWndToBack(moveHandle);
        }

        /// <summary>
        /// Sends the first WndHandle with the type 
        /// specified to the back.
        /// </summary>
        /// <param name="type">The type to find and move.</param>
        public void sendWndToBackByType(Type type)
        {
            foreach (WndHandle handle in wndHandles)
            {
                if (handle.GetType() == type)
                {
                    sendWndToBack(handle);
                    break;
                }
            }            
        }

        /// <summary>
        /// Sends the WndHandle at index to the back.
        /// </summary>
        /// <param name="index">The index to move to back.</param>
        public void sendWndToBackIndex(int index)
        {
            WndHandle moveHandle = null;
            moveHandle = (index < 0 || index >= wndHandles.Count) ? null : wndHandles[index];

            sendWndToBack(moveHandle);
        }

        /// <summary>
        /// Sends the WndHandle specified to the back.
        /// </summary>
        /// <param name="wndHandle">The WndHandle to move.</param>
        public void sendWndToBack(WndHandle wndHandle)
        {
            // TODO: WARNING THIS WILL NOT WORK!!!
            if (wndHandle != null)
            {
                removeWnd(wndHandle);
                wndHandles.Insert(0, wndHandle);
                //addWnd(wndHandle, 0);
            }
        }

        /// <summary>
        /// Gets the first WndHandle of the type specified.
        /// </summary>
        /// <param name="type">The type to search for.</param>
        /// <returns>A WndHandle or null.</returns>
        public WndHandle getWnd(Type type)
        {
            WndHandle targetWnd = null;
            foreach (WndHandle handle in wndHandles)
            {
                if (handle.GetType() == type)
                {
                    targetWnd = handle;
                    break;
                }
            }

            return targetWnd;
        }

        /// <summary>
        /// Finds the first WndHandle with the reference specified.
        /// </summary>
        /// <param name="searchForHandle">The WndHandle to search for.</param>
        /// <returns>A WndHandle or null.</returns>
        public WndHandle getWnd(WndHandle searchForHandle)
        {
            WndHandle targetWnd = null;
            foreach (WndHandle handle in wndHandles)
            {
                if (handle == searchForHandle)
                {
                    targetWnd = handle;
                    break;
                }
            }

            return targetWnd;
        }

        /// <summary>
        /// Finds the first WndHandle with the wndID specified.
        /// </summary>
        /// <param name="wndID">The wndID to search for.</param>
        /// <returns>A WndHandle or null.</returns>
        public WndHandle getWnd(int wndID)
        {
            WndHandle targetWnd = null;
            foreach (WndHandle handle in wndHandles)
            {
                if (handle.getWndCode() == wndID)
                {
                    targetWnd = handle;
                    break;
                }
            }

            return targetWnd;
        }

        /// <summary>
        /// Gets the WnHandle at index specified.
        /// </summary>
        /// <param name="index">The index to get.</param>
        /// <returns>A WndHandle or null.</returns>
        public WndHandle getWndAt(int index)
        {
            if (index < 0 || index >= wndHandles.Count)
                return null;

            return wndHandles[index];
        }

        /// <summary>
        /// Gets all the WndHandles stored in this WndGroup of the
        /// type specified.
        /// </summary>
        /// <param name="type">The type of WndHandle to search for.</param>
        /// <returns>A list of zero or more WndHandles.</returns>
        public List<WndHandle> getAllWnd(Type type)
        {
            List<WndHandle> targetWnds = new List<WndHandle>();
            foreach (WndHandle handle in wndHandles)
            {
                if (handle.GetType() == type)
                {
                    targetWnds.Add(handle);
                    break;
                }
            }

            return targetWnds;
        }

        /// <summary>
        /// Gets all WndHandles with the wndID specified.
        /// </summary>
        /// <param name="wndID">The wndID to search for.</param>
        /// <returns>A list of zero or more WndHandles.</returns>
        public List<WndHandle> getAllWnd(int wndID)
        {
            List<WndHandle> targetWnds = new List<WndHandle>();
            foreach (WndHandle handle in wndHandles)
            {
                if (handle.getWndCode() == wndID)
                {
                    targetWnds.Add( handle );
                    break;
                }
            }

            return targetWnds;
        }

        /// <summary>
        /// Gets all the currently active WndHandles.
        /// </summary>
        /// <returns>A list containing zero or more WndHandles.</returns>
        public List<WndHandle> getAllWnd()
        {
            return wndHandles;
        }
        #endregion

        #region Managers
        /// <summary>
        /// Gets a reference to the primary XNA Game object.
        /// </summary>
        public Game getAppRef()
        {
            return appRef;
        }

        /// <summary>
        /// Gets a reference to the input manager stored by this object.
        /// </summary>
        public InputManager getInputManager()
        {
            return inputManager;
        }

        /// <summary>
        /// Gets a reference to the audio manager stored by this object.
        /// </summary>
        public AudioManager getAudioManager()
        {
            return audioManager;
        }

        /// <summary>
        /// Sets the audio manager for this group and all wndHandles that are children.
        /// </summary>
        /// <param name="audioManager">The new audio manager object.</param>
        public override void setAudioManager(AudioManager audioManager)
        {
            this.audioManager = audioManager;

            foreach (WndHandle handle in wndHandles)
            {
                handle.setAudioManager(audioManager);
            }
        }

        /// <summary>
        /// Sets the input manager for this group and all wndHandles that are children.
        /// </summary>
        /// <param name="inputManager">The new input manager object.</param>
        public override void setInputManager(InputManager inputManager)
        {
            base.setInputManager(inputManager);

            foreach (WndHandle handle in wndHandles)
            {
                handle.setInputManager(inputManager);
            }
        }
        #endregion
    }
}
