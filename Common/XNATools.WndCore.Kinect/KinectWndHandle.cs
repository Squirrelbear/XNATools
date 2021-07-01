using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNATools.WndCore.Kinect
{
    /// <summary>
    /// KinectWndHandle extends the default implementation of the WndHandle by prodviding 
    /// code that will automatically load the KinectInputManager in as a shared resource.
    /// Any Wnd that uses this class will benefit from automatic configurable Kinect interaction.
    /// The automatic interaction with Kinect can be disabled and the manual methods used instead via
    /// the KinectInputManager object.
    /// </summary>
    public abstract class KinectWndHandle : WndHandle
    {
        #region Enum definitions
        /// <summary>
        /// Provides a definition of the interaction types that should be automatically controlled by this class 
        /// when dealing with the Kinect. None means that there will be no automatic Kinect Interaction, AsMouseOnly will 
        /// trigger the events as if the Kinect input is a mouse cursor. AsKinectOnly will only trigger the kinect specific 
        /// versions of the events, and if MouseAndKinect is used it will run both the Mouse and Kinect codes. 
        /// Note that a mouse can be used simultaneously while interacting with the kinect and the setEnableMouseInteraction(bool)
        /// should be used to disable this code if necessary.
        /// </summary>
        public enum KinectAutoInteraction { None, AsMouseOnly, AsKinectOnly, MouseAndKinect };

        /// <summary>
        /// KinectActionMap defines a group of the types of interactions that can be performed. This is used
        /// for lookup to map what action  will call methods or for other similar purposes. It is intended to have a pair
        /// made between a KinectActionMap entry and a MouseActionMap.
        /// </summary>
        public enum KinectActionMap { None, GripBeginLeft, GripEndLeft, GripBeginRight, GripEndRight, PushLeft, PushRight };

        /// <summary>
        /// MouseActionMap defines a group of the types of mouse interactions that can be performed. This 
        /// is used for lookup to map methods. It is intended to have a pair made between KinectActionMap and 
        /// a MouseActionMap entry.
        /// </summary>
        public enum MouseActionMap {  PressedRight, ReleasedRight, PressedLeft, ReleasedLeft };
        #endregion

        #region Instance Variables
        /// <summary>
        /// A reference to a kinectInputManager. This allows access to all the functionality of the Kinect via the wrapper class.
        /// </summary>
        protected KinectInputManager kinectInputManager;

        // TODO: Need to add Get/Set methods for the kinectInteraction variable
        /// <summary>
        /// kinectInteraction defines the mode of interaction that there is between the Kinect and this class automatically.
        /// See KinectAutoInteraction for further details of how this should be used.
        /// </summary>
        protected KinectAutoInteraction kinectInteraction;

        // TODO: NEED TO IMPLEMENT KinectActionMap still
        protected KinectActionMap[] kinectToMouseMap; 

        // TODO: Need interface methods for this variable
        /// <summary>
        /// This variables determines which hand is the main one and therefore the one that should be used
        /// for the left and right clicks. For example if this is set to true the left hand would provide the 
        /// mouseMoved events when kinectInteraction is set to a mode that enables auto hand interaction. 
        /// When true the left hand would call left button methods with right hand the right button. 
        /// The opposite would occur if the variable was set to false.
        /// </summary>
        protected bool leftAsMainHand;

        protected bool automaticMultiplayer;
        protected int maxPlayers;
        #endregion

        /// <summary>
        /// Creates a KinectWndHandle with a wndCode, displayRect, and parent.
        /// </summary>
        /// <param name="wndCode">Can be used to make windows uniquely idenfiable through a code.</param>
        /// <param name="displayRect">The bounds for the window.</param>
        /// <param name="parent">The parent to link this class to. A parent that knows where the Game object is, is required!</param>
        public KinectWndHandle(int wndCode, Rectangle displayRect, WndGroup parent)
            : base(wndCode, displayRect, parent)
        {
            kinectInputManager = KinectInputManager.getSharedKinectInputManager(parent.getAppRef());
            kinectInteraction = KinectAutoInteraction.AsMouseOnly;
            leftAsMainHand = false;
            automaticMultiplayer = false;
            maxPlayers = 1;
        }

        /*
        public override void onWndClosing()
        {
            base.onWndClosing();

            // NOTE: removed because calling unload here would trigger an obvious crash if another is still using the manager.
            //kinectInputManager.unloadContent();
        }*/

        /// <summary>
        /// This class will perform an update on the kinectInputManager object and then the base WndHandle content.
        /// After these update steps have been completed the automatic kinect interaction based on the kinectInteraction
        /// property will control the behaviour. If the property is set to AsMouseOnly or MouseAndKinect the 
        /// approprate mouseMoved, mouseClickedLeft, mouseClickedRight, mousePushedLeft, and mousePushedRight methods
        /// will be called in the WndComponents dependent on the hand interaction defined by leftAsMainHand.
        /// When the auto interaction mode is set to KinectOnly or MouseAndKinect the Kinect specific methods
        /// that are defined as part of this class will be called when the events occur. These provide convienient 
        /// ways to do something when an event occurs. If it is found for any player the events have occured the methods
        /// will be called. 
        /// </summary>
        public override void update(GameTime gameTime)
        {
            // TODO: This will not work properly if there are multiple KinectWnds that are actively being updated
            kinectInputManager.update(gameTime);
            base.update(gameTime);

            int maxPlayerID = Math.Max(maxPlayers, kinectInputManager.getPlayerCount());

            for (int playerID = 0; playerID < maxPlayerID; playerID++)
            {
                KinectPlayer player = kinectInputManager.getPlayer(playerID);
                if (player == null) continue;
                if (player.PlayerCurState == KinectPlayer.PlayerState.Entered)
                {
                    this.playerEntered(playerID, player.Skeleton.TrackingState == Microsoft.Kinect.SkeletonTrackingState.Tracked);
                }
                else if (player.PlayerCurState == KinectPlayer.PlayerState.Exited)
                {
                    this.playerExited(playerID, player.Skeleton.TrackingState == Microsoft.Kinect.SkeletonTrackingState.Tracked);
                }
                
                if (playerID == 0 && (kinectInteraction == KinectAutoInteraction.AsMouseOnly || kinectInteraction == KinectAutoInteraction.MouseAndKinect))
                {
                    bool mouseMoved = kinectInputManager.getHandMoved(leftAsMainHand, playerID);//getGripMoved(leftAsMainHand);
                    bool mouseClickedLeft = kinectInputManager.getGripEnded(leftAsMainHand, playerID);
                    bool mouseClickedRight = kinectInputManager.getGripEnded(!leftAsMainHand, playerID);
                    bool mousePressedLeft = kinectInputManager.getGripBegun(leftAsMainHand, playerID);
                    bool mousePressedRight = kinectInputManager.getGripBegun(!leftAsMainHand, playerID);

                    if (mouseMoved)
                    {
                        /*this.mouseMoved(inputManager.getLastCursor(), inputManager.getCursor());

                        foreach (WndComponent c in components)
                        {
                            if (c.isVisible())
                                c.mouseMoved(inputManager.getLastCursor(), inputManager.getCursor());
                        }*/

                        Point oldPoint = kinectInputManager.kinectVectorToPoint(kinectInputManager.getOldGripPos(leftAsMainHand, playerID), displayRect);
                        Point newPoint = kinectInputManager.kinectVectorToPoint(kinectInputManager.getGripPos(leftAsMainHand, playerID), displayRect);
                        this.mouseMoved(oldPoint, newPoint);

                        foreach (WndComponent c in components)
                        {
                            if (c.isVisible())
                                c.mouseMoved(oldPoint, newPoint);
                        }
                    }

                    if (mouseClickedLeft)
                    {
                        Point point = kinectInputManager.kinectVectorToPoint(kinectInputManager.getGripPos(leftAsMainHand, playerID), displayRect);
                        this.mouseClickedLeft(point);

                        foreach (WndComponent c in components)
                        {
                            if (c.isVisible())
                                c.mouseClickedLeft(point);
                        }
                    }

                    if (mouseClickedRight)
                    {
                        Point point = kinectInputManager.kinectVectorToPoint(kinectInputManager.getGripPos(leftAsMainHand, playerID), displayRect);
                        this.mouseClickedRight(point);

                        foreach (WndComponent c in components)
                        {
                            if (c.isVisible())
                                c.mouseClickedRight(point);
                        }
                    }

                    if (mousePressedLeft)
                    {
                        Point point = kinectInputManager.kinectVectorToPoint(kinectInputManager.getGripPos(leftAsMainHand, playerID), displayRect);
                        this.mousePressedLeft(point);

                        foreach (WndComponent c in components)
                        {
                            if (c.isVisible())
                                c.mousePressedLeft(point);
                        }
                    }

                    if (mousePressedRight)
                    {
                        Point point = kinectInputManager.kinectVectorToPoint(kinectInputManager.getGripPos(leftAsMainHand, playerID), displayRect);
                        this.mousePressedRight(point);

                        foreach (WndComponent c in components)
                        {
                            if (c.isVisible())
                                c.mousePressedRight(point);
                        }
                    }
                }

                if (kinectInteraction == KinectAutoInteraction.AsKinectOnly || kinectInteraction == KinectAutoInteraction.MouseAndKinect)
                {
                    bool handMovedLeft = kinectInputManager.getHandMoved(true, playerID);
                    bool handMovedRight = kinectInputManager.getHandMoved(false, playerID);
                    bool handGripEndLeft = kinectInputManager.getGripEnded(true, playerID);
                    bool handGripEndRight = kinectInputManager.getGripEnded(false, playerID);
                    bool handGripBeginLeft = kinectInputManager.getGripBegun(true, playerID);
                    bool handGripBeginRight = kinectInputManager.getGripBegun(false, playerID);
                    bool handPushedLeft = kinectInputManager.getPushValue(true, true, playerID) >= 1;
                    bool handPushedRight = kinectInputManager.getPushValue(false, true, playerID) >= 1;

                    // TODO: Change to use KinectActionMap to target which action to run
                    if (handMovedLeft)
                    {
                        Point oldPoint = kinectInputManager.kinectVectorToPoint(kinectInputManager.getOldGripPos(true, playerID), displayRect);
                        Point newPoint = kinectInputManager.kinectVectorToPoint(kinectInputManager.getGripPos(true, playerID), displayRect);
                        this.handMoved(oldPoint, newPoint, true, playerID);
                    }
                    if (handMovedRight)
                    {
                        Point oldPoint = kinectInputManager.kinectVectorToPoint(kinectInputManager.getOldGripPos(false, playerID), displayRect);
                        Point newPoint = kinectInputManager.kinectVectorToPoint(kinectInputManager.getGripPos(false, playerID), displayRect);
                        this.handMoved(oldPoint, newPoint, false, playerID);
                    }
                    if (handGripBeginLeft)
                    {
                        Point point = kinectInputManager.kinectVectorToPoint(kinectInputManager.getGripPos(true, playerID), displayRect);
                        this.handGripBegun(point, true, playerID);
                    }

                    if (handGripBeginRight)
                    {
                        Point point = kinectInputManager.kinectVectorToPoint(kinectInputManager.getGripPos(false, playerID), displayRect);
                        this.handGripBegun(point, false, playerID);
                    }

                    if (handGripEndLeft)
                    {
                        Point point = kinectInputManager.kinectVectorToPoint(kinectInputManager.getGripPos(true, playerID), displayRect);
                        this.handGripEnded(point, true, playerID);
                    }

                    if (handGripEndRight)
                    {
                        Point point = kinectInputManager.kinectVectorToPoint(kinectInputManager.getGripPos(false, playerID), displayRect);
                        this.handGripEnded(point, false, playerID);
                    }

                    if (handPushedLeft)
                    {
                        Point point = kinectInputManager.kinectVectorToPoint(kinectInputManager.getGripPos(true, playerID), displayRect);
                        this.handPushed(point, true, playerID);
                    }

                    if (handPushedRight)
                    {
                        Point point = kinectInputManager.kinectVectorToPoint(kinectInputManager.getGripPos(false, playerID), displayRect);
                        this.handPushed(point, false, playerID);
                    }
                }
            }
        }

        /// <summary>
        /// Called when the event handMoved for a hand from some player occurs in update.
        /// </summary>
        /// <param name="oldP">The previous point.</param>
        /// <param name="newP">The new point.</param>
        /// <param name="isLeft">Is it the left or right hand.</param>
        /// <param name="playerID">Which player is it. Or leave blank for player 0.</param>
        public virtual void handMoved(Point oldP, Point newP, bool isLeft, int playerID = 0)
        {
            
        }

        /// <summary>
        /// Called when the event handGripBegun for a hand from some player occurs in update.
        /// </summary>
        /// <param name="p">The point of the event.</param>
        /// <param name="isLeft">Is it the left or right hand.</param>
        /// <param name="playerID">Which player is it. Or leave blank for player 0.</param>
        public virtual void handGripBegun(Point p, bool isLeft, int playerID = 0)
        {
            
        }

        /// <summary>
        /// Called when the event handGripEnded for a hand from some player occurs in update.
        /// </summary>
        /// <param name="p">The point of the event.</param>
        /// <param name="isLeft">Is it the left or right hand.</param>
        /// <param name="playerID">Which player is it. Or leave blank for player 0.</param>
        public virtual void handGripEnded(Point p, bool isLeft, int playerID = 0)
        {

        }

        /// <summary>
        /// Called when the event handPushed for a hand from some player occurs in update.
        /// </summary>
        /// <param name="p">The point of the event.</param>
        /// <param name="isLeft">Is it the left or right hand.</param>
        /// <param name="playerID">Which player is it. Or leave blank for player 0.</param>
        public virtual void handPushed(Point p, bool isLeft, int playerID = 0)
        {

        }

        /// <summary>
        /// Called when a player becomes active after not being active. (a player is not active when
        /// they have been not found in the scene)
        /// </summary>
        /// <param name="playerID">The id of the player that has just entered.</param>
        /// <param name="isTracked">Whether the more extensive data is being kept for this player 
        /// (you may find if it is false that you will not want to do anything)</param>
        public virtual void playerEntered(int playerID, bool isTracked)
        {

        }

        /// <summary>
        /// Called when a player becomes inactive. (typically when they walk out of the view of the kinect)
        /// </summary>
        /// <param name="playerID">The id of the player that has just exited.</param>
        /// <param name="isTracked">Whether the more extensive data is being kept for this player 
        /// (you may find if it is false that you will not want to do anything)</param>
        public virtual void playerExited(int playerID, bool isTracked)
        {

        }

        /// <summary>
        /// Gets the KinectInputManager object from this KinectWndHandle.
        /// </summary>
        public KinectInputManager getKinectInputManager()
        {
            return kinectInputManager;
        }
    }
}
