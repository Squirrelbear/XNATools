using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNATools.WndCore.Kinect
{
    /// <summary>
    /// Defines an example cursor implementation that will automatically update itself
    /// to the current location of a left or right hand. The position will centre the 
    /// texture on that location.
    /// </summary>
    public class Cursor : WndComponent
    {
        /// <summary>
        /// This variable indicates whether the left or right hand should be tracked.
        /// When this is set to true the left hand will be tracked.
        /// </summary>
        protected bool trackLeft;

        /// <summary>
        /// A reference to the KinectWndHandle so that it is possible to get the 
        /// connection to the KinectInputManager for determining the current hand position.
        /// </summary>
        protected KinectWndHandle wndHandle;

        /// <summary>
        /// Basic constructor that allows definition of the left or right hand, the texture to 
        /// render, and the reference to the WndHandle. Currently this method will default
        /// the rendering dimensions to 50 by 50 pixels.
        /// </summary>
        /// <param name="trackLeft">Tracks left if true, tracks right if false.</param>
        /// <param name="texture">The texture to show centered on the hand's location.</param>
        /// <param name="wndHandle">The reference to a KinectWndHandle that has a KinectInputManager.</param>
        public Cursor(bool trackLeft, Texture2D texture, KinectWndHandle wndHandle)
            : base(new Rectangle(-50,-50,50,50), texture)
        {
            this.trackLeft = trackLeft;
            this.wndHandle = wndHandle;
        }

        /// <summary>
        /// Updates the location of the cursor to the current relative position of the hand.
        /// The scaling is based on the dimensions of the window.
        /// </summary>
        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            Vector2 loc = wndHandle.getKinectInputManager().getScaledVector(
                            wndHandle.getKinectInputManager().getGripPos(trackLeft), wndHandle.getRect())
                            - new Vector2(getRect().Width/2, getRect().Height/2);
            setLocation(loc);
                
        }
    }
}
