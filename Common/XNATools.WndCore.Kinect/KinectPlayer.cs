using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Kinect.Toolkit.Interaction;
using Microsoft.Kinect;

namespace XNATools.WndCore.Kinect
{
    public class KinectPlayer
    {
        public enum GripState { Unknown = 0, GripBegun = 1, GripHeld = 2, GripEnded = 3, GripReleased = 4 };
        // Entered -> Active -> Exited -> Inactive -> Entered etc
        public enum PlayerState { Inactive = 0, Entered = 1, Active = 2, Exited = 3 };

        public class KinectHand
        {
            public GripState InputGripState { get; set; }
            public GripState CurGripState { get; set; }
            public GripState OldGripState { get; set; }

            public Vector2 InputGripPoint { get; set; }
            public Vector2 CurGripPoint { get; set; }
            public Vector2 OldGripPoint { get; set; }
        }
        
        public int PlayerID { get; set; }
        public int SkeletonID { get; set; }

        public KinectHand LeftHand { get; set; }
        public KinectHand RightHand { get; set; }

        public UserInfo InputUserInfo { get; set; }
        public UserInfo CurUserInfo  { get; set; }
        public UserInfo OldUserInfo  { get; set; }

        public Skeleton Skeleton  { get; set; }
        public Skeleton OldSkeleton { get; set; }

        public PlayerState PlayerCurState { get; set; }
        public int LastSeenAt { get; set; }

        public KinectPlayer(int playerID)
        {
            PlayerID = playerID;
            SkeletonID = -1;
            LeftHand.InputGripState = RightHand.InputGripState = KinectPlayer.GripState.GripReleased;
            LeftHand.CurGripState = RightHand.CurGripState = KinectPlayer.GripState.GripReleased;
            LeftHand.OldGripState = RightHand.OldGripState = KinectPlayer.GripState.GripReleased;
            LeftHand.InputGripPoint = new Vector2(0, 0);
            RightHand.InputGripPoint = new Vector2(0, 0);
            LeftHand.CurGripPoint = new Vector2(0, 0);
            RightHand.CurGripPoint = new Vector2(0, 0);
            LeftHand.OldGripPoint = new Vector2(0, 0);
            RightHand.OldGripPoint = new Vector2(0, 0);
            InputUserInfo = CurUserInfo = OldUserInfo = null;
            Skeleton = OldSkeleton = null;
            LastSeenAt = 0;
            PlayerCurState = PlayerState.Inactive;
        }

        public void update(GameTime gameTime, Skeleton updatedSkeleton)
        {
            // previous current is now the new old
            LeftHand.OldGripState = LeftHand.CurGripState;
            RightHand.OldGripState = RightHand.CurGripState;
            LeftHand.OldGripPoint = LeftHand.CurGripPoint;
            RightHand.OldGripPoint = RightHand.CurGripPoint;

            // current input state is now the input state
            LeftHand.CurGripState = LeftHand.InputGripState;
            RightHand.CurGripState = RightHand.InputGripState; 
            LeftHand.CurGripPoint = LeftHand.InputGripPoint; ;
            RightHand.CurGripPoint = RightHand.InputGripPoint; 

            // Update the user info variables
            OldUserInfo = CurUserInfo;
            CurUserInfo = InputUserInfo;
            OldSkeleton = Skeleton;
            Skeleton = updatedSkeleton;

            if (updatedSkeleton != null)
            {
                LastSeenAt = gameTime.TotalGameTime.Milliseconds;
                if (OldSkeleton == null)
                    PlayerCurState = PlayerState.Entered;
                else
                    PlayerCurState = PlayerState.Active;
            }
            else if (PlayerCurState == PlayerState.Exited)
            {
                PlayerCurState = PlayerState.Inactive;
            }
            else
            {
                PlayerCurState = PlayerState.Exited;
            }
        }
    }
}
