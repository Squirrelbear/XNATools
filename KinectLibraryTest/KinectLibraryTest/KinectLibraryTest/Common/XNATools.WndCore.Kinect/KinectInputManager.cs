using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Kinect.Toolkit.Interaction;

namespace XNATools.WndCore.Kinect
{
    public class KinectInputManager : Microsoft.Kinect.Toolkit.Interaction.IInteractionClient
    {
        public enum InputState { NoKinect = 0, Connected = 1, Disconnected = 2, NotPowered = 3, Error = 4 };
        //public enum GripState { Unknown = 0, GripBegun = 1, GripHeld = 2, GripEnded = 3, GripReleased = 4 };

        // Required variables for convert depth frame
        // color divisors for tinting depth pixels
        private static readonly int[] IntensityShiftByPlayerR = { 1, 2, 0, 2, 0, 0, 2, 0 };
        private static readonly int[] IntensityShiftByPlayerG = { 1, 2, 2, 0, 2, 0, 0, 1 };
        private static readonly int[] IntensityShiftByPlayerB = { 1, 0, 2, 2, 0, 2, 0, 2 };

        private const int RedIndex = 2;
        private const int GreenIndex = 1;
        private const int BlueIndex = 0;

        private Texture2D kinectRGBVideo;
        private Texture2D backDepthTexture;
        private KinectSensor kinectSensor;
        private String connectedStatus;
        private InputState inputState;
        private bool listenDepth, listenColor;
        private bool detectPlayersOnUpdate;

        private InteractionStream intStream;

        private Game appRef;
        private static KinectInputManager selfReference;

       /* public class KinectPlayer
        {
            public int playerID;

            public GripState handGripLeft, handGripRight;
            public GripState curhandGripLeft, curhandGripRight;
            public GripState oldhandGripLeft, oldhandGripRight;
            public Vector2 gripPointLeft, gripPointRight;
            public Vector2 curGripPointLeft, curGripPointRight;
            public Vector2 oldGripPointLeft, oldGripPointRight;

            public UserInfo directUserInfo;
            public UserInfo curUserInfo, oldUserInfo;

            public Skeleton skeleton;
        }*/

        private KinectPlayer[] players;
        private Skeleton[] skeletons;

        // XNA State variables:
        /*private GripState curhandGripLeft, curhandGripRight;
        private GripState oldhandGripLeft, oldhandGripRight;
        private Vector2 curGripPointLeft, curGripPointRight;
        private Vector2 oldGripPointLeft, oldGripPointRight;

        private bool curhandPushLeft, curhandPushRight;
        private bool oldhandPushLeft, oldhandPushRight;
        private float curvalPushLeft, curvalPushRight;
        private float oldvalPushLeft, oldvalPushRight;*/

        private KinectInputManager(Game appRef)
        {
            this.appRef = appRef;
            KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
            inputState = InputState.NoKinect;
            listenColor = listenDepth = false;
            kinectRGBVideo = null;
            connectedStatus = "Unknown";
            backDepthTexture = null;

            players = new KinectPlayer[6];
            /*for (int i = 0; i < players.Length; i++)
            {
                players[i] = new KinectPlayer(i);
                
            }*/
            DiscoverKinectSensor();

            detectPlayersOnUpdate = true;
        }

        ~KinectInputManager()
        {
            unloadContent();
        }

        public static KinectInputManager getSharedKinectInputManager(Game appRef)
        {
            if (selfReference == null)
            {
                selfReference = new KinectInputManager(appRef);
            }

            return selfReference;
        }

        public void update(GameTime gameTime)
        {
            for (int i = 0; i < players.Count(); i++)
            {
                if (skeletons != null && players[i].CurUserInfo != null)
                {
                    Skeleton newSkeleton = null;
                    foreach (Skeleton s in skeletons)
                    {
                        if (s.TrackingId == players[i].SkeletonID)
                        {
                            newSkeleton = s;
                            break;
                        }
                    }
                    players[i].update(gameTime, newSkeleton);
                }
            }

         /*   
            oldGripPointLeft = curGripPointLeft;
            oldGripPointRight = curGripPointRight;

            curhandGripLeft = handGripLeft;
            curhandGripRight = handGripRight;
            curGripPointLeft = gripPointLeft;
            curGripPointRight = gripPointRight;

            oldhandPushLeft = curhandPushLeft;
            oldhandPushRight = curhandPushRight;
            oldvalPushLeft = curvalPushLeft;
            oldvalPushRight = curvalPushRight;

            if (curData != null)
            {
                curhandPushLeft = curData.HandPointers[0].IsPressed;
                curhandPushRight = curData.HandPointers[1].IsPressed;
                curvalPushLeft = (float)curData.HandPointers[0].PressExtent;
                curvalPushRight = (float)curData.HandPointers[1].PressExtent;
            }*/
        }

        public void unloadContent()
        {
            if (kinectSensor != null)
            {
                kinectSensor.ColorStream.Disable();
                kinectSensor.DepthStream.Disable();
                kinectSensor.SkeletonStream.Disable();
            }

            if (intStream != null)
            {
                intStream.Dispose();
            }

            if (kinectSensor != null)
            {
                kinectSensor.Stop();
                kinectSensor.Dispose();
            }
        }

        #region Player Variable Methods
        public KinectPlayer getPlayer(int playerID)
        {
            if (playerID < players.Length)
                return players[playerID];
            else
                return null;
        }

        public KinectPlayer[] getAllPlayers()
        {
            return players;
        }

        public int getPlayerCount()
        {
            int count = 0;
            for (int i = 0; i < players.Length; i++)
                if (players[i].Skeleton.TrackingState == SkeletonTrackingState.Tracked)
                    count++;
                else
                    break;
            return players.Length;
        }

        /// <summary>
        /// This method will potentially shuffle the players array to get a 
        /// more appropriate "player 1" and "player 2". Note that it will
        /// only support 2 players.
        /// </summary>
        public void beginPlayerDetection()
        {
            detectPlayersOnUpdate = true;
        }

        /// <summary>
        /// This method will perform a shuffle on the players array to 
        /// determine appropriate player numbers
        /// </summary>
        private void performPlayerDetection()
        {
            // TODO FIX THIS COMMENTED OUT IMPORTANT CODE
            //players = players.OrderBy(x => x.Skeleton.TrackingState == SkeletonTrackingState.Tracked)
            //                    .ThenBy(x => x.Skeleton.Position.Z).ToList();
            if (players[0].Skeleton.TrackingState == SkeletonTrackingState.Tracked
                && players[1].Skeleton.TrackingState == SkeletonTrackingState.Tracked
                && players[0].Skeleton.Position.X > players[1].Skeleton.Position.X)
            {
                KinectPlayer temp = players[0];
                players[0] = players[1];
                players[1] = temp;
            }

            /*if (this.kinectSensor != null && this.kinectSensor.SkeletonStream != null)
            {
                if (!this.kinectSensor.SkeletonStream.AppChoosesSkeletons)
                {
                    this.kinectSensor.SkeletonStream.AppChoosesSkeletons = true; // Ensure AppChoosesSkeletons is set
                }

                if (players.Count == 1)
                {
                    this.kinectSensor.SkeletonStream.ChooseSkeletons(players[0].SkeletonID); // Track this skeleton
                }
                else if (players.Count > 1)
                {
                    this.kinectSensor.SkeletonStream.ChooseSkeletons(players[0].SkeletonID, players[1].SkeletonID); // Track this skeleton
                }
            }*/
        }
        #endregion

        #region Grip Detection
        public KinectPlayer.GripState getGripState(bool left, int playerID = 0)
        {
            // TODO FIX THIS COMMENTED OUT IMPORTANT CODE
           // if (playerID >= players.Count) return KinectPlayer.GripState.Unknown;
            return (left) ? players[playerID].LeftHand.CurGripState : players[playerID].RightHand.CurGripState;
        }

        public Vector2 getGripPos(bool left, int playerID = 0)
        {
            // TODO FIX THIS COMMENTED OUT IMPORTANT CODE
          //  if (playerID >= players.Count) return new Vector2(-100, -100);
            return (left) ? players[playerID].LeftHand.CurGripPoint : players[playerID].RightHand.CurGripPoint;
        }

        public Vector2 getGripPos(bool left, bool current, int playerID = 0)
        {
            return (current) ? getGripPos(left, playerID) : getOldGripPos(left, playerID);
        }

        public Vector2 getOldGripPos(bool left, int playerID = 0)
        {
            if (playerID >= players.Count()) return new Vector2(-100, -100);
            return (left) ? players[playerID].LeftHand.OldGripPoint : players[playerID].RightHand.OldGripPoint;
        }

        public bool getGripMoved(bool left, int playerID = 0)
        {
            if (getGripState(left, playerID) != KinectPlayer.GripState.GripHeld)
                return false;

            return !getGripPos(left, playerID).Equals(getOldGripPos(left, playerID));
        }

        public bool getHandMoved(bool left, int playerID = 0)
        {
            return !getGripPos(left, playerID).Equals(getOldGripPos(left, playerID));
        }

        public KinectPlayer.GripState getGripState(bool left, bool cur, int playerID = 0)
        {
            if (playerID >= players.Count()) return KinectPlayer.GripState.Unknown;
            return (left) ? ((cur) ? players[playerID].LeftHand.CurGripState : players[playerID].LeftHand.OldGripState)
                            : ((cur) ? players[playerID].RightHand.CurGripState : players[playerID].RightHand.OldGripState);
        }

        public bool getGripBegun(bool left, int playerID = 0)
        {
            if (playerID >= players.Count()) return false;
            return (left) ? (players[playerID].LeftHand.CurGripState == KinectPlayer.GripState.GripHeld
                                && players[playerID].LeftHand.OldGripState != KinectPlayer.GripState.GripHeld)
                            : (players[playerID].RightHand.CurGripState == KinectPlayer.GripState.GripHeld
                                && players[playerID].RightHand.OldGripState != KinectPlayer.GripState.GripHeld);
        }

        public bool getGripEnded(bool left, int playerID = 0)
        {
            if (playerID >= players.Count()) return false;
            return (left) ? (players[playerID].LeftHand.CurGripState == KinectPlayer.GripState.GripReleased
                                && players[playerID].LeftHand.OldGripState != KinectPlayer.GripState.GripReleased)
                            : (players[playerID].RightHand.CurGripState == KinectPlayer.GripState.GripReleased
                                && players[playerID].RightHand.OldGripState != KinectPlayer.GripState.GripReleased);
        }
        #endregion

        #region Push Detection
        public bool getPushBegin(bool left, int playerID = 0)
        {
            if (playerID >= players.Count()) return false;
            if (players[playerID].InputUserInfo == null) return false;
            return (left) ? (players[playerID].CurUserInfo.HandPointers[0].IsPressed == true && players[playerID].OldUserInfo.HandPointers[0].IsPressed == false)
                            : (players[playerID].CurUserInfo.HandPointers[1].IsPressed == true && players[playerID].OldUserInfo.HandPointers[1].IsPressed == false);
        }

        public bool getPushEnd(bool left, int playerID = 0)
        {
            if (playerID >= players.Count()) return false;
            if (players[playerID].InputUserInfo == null) return false;
            return (left) ? (players[playerID].CurUserInfo.HandPointers[0].IsPressed == false && players[playerID].OldUserInfo.HandPointers[0].IsPressed == true)
                            : (players[playerID].CurUserInfo.HandPointers[1].IsPressed == false && players[playerID].OldUserInfo.HandPointers[1].IsPressed == true);
        }

        public bool getPush(bool left, bool curValue, int playerID = 0)
        {
            if (playerID >= players.Count()) return false;
            if (players[playerID].InputUserInfo == null) return false;
            return (left) ? ((curValue) ? players[playerID].CurUserInfo.HandPointers[0].IsPressed : players[playerID].OldUserInfo.HandPointers[0].IsPressed)
                        : ((curValue) ? players[playerID].CurUserInfo.HandPointers[1].IsPressed : players[playerID].OldUserInfo.HandPointers[1].IsPressed);
        }

        public float getPushValue(bool left, bool curValue, int playerID = 0)
        {
            if (playerID >= players.Count()) return 0;
            if (players[playerID].InputUserInfo == null) return 0;
            return (float)((left) ? ((curValue) ? players[playerID].CurUserInfo.HandPointers[0].PressExtent : players[playerID].OldUserInfo.HandPointers[0].PressExtent)
                        : ((curValue) ? players[playerID].CurUserInfo.HandPointers[1].PressExtent : players[playerID].OldUserInfo.HandPointers[1].PressExtent));
        }
        #endregion

        #region Skeleton Methods
        public Skeleton getSkeleton(int playerID = 0)
        {
            if (playerID >= players.Count()) return null;
            return players[playerID].Skeleton;
        }

        public float getJointRotationOnZAxis(JointType jointType, int playerID = 0)
        {
            return getJointRotationOnAxis(jointType, Vector3.UnitZ, playerID);
        }

        public float getJointRotationOnAxis(JointType jointType, Vector3 axis, int playerID = 0)
        {
            if (playerID >= players.Count()) return 0;
            Microsoft.Kinect.Vector4 v = players[playerID].Skeleton.BoneOrientations[jointType].AbsoluteRotation.Quaternion;
            Quaternion q = new Quaternion(v.X, v.Y, v.Z, v.W);
            return 0.5f * (float)Math.PI - (FindQuaternionTwist(q, axis)) * (float)Math.PI;
        }

        // http://stackoverflow.com/questions/3684269/component-of-a-quaternion-rotation-around-an-axis
        public static float FindQuaternionTwist(Quaternion q, Vector3 axis)
        {
            axis.Normalize();


            //get the plane the axis is a normal of
            Vector3 orthonormal1, orthonormal2;
            FindOrthonormals(axis, out orthonormal1, out orthonormal2);


            Vector3 transformed = Vector3.Transform(orthonormal1, q);


            //project transformed vector onto plane
            Vector3 flattened = transformed - (Vector3.Dot(transformed, axis) * axis);
            flattened.Normalize();


            //get angle between original vector and projected transform to get angle around normal
            float a = (float)Math.Acos((double)Vector3.Dot(orthonormal1, flattened));


            return a;
        }

        private static Matrix OrthoX = Matrix.CreateRotationX(MathHelper.ToRadians(90));
        private static Matrix OrthoY = Matrix.CreateRotationY(MathHelper.ToRadians(90));

        public static void FindOrthonormals(Vector3 normal, out Vector3 orthonormal1, out Vector3 orthonormal2)
        {
            Vector3 w = Vector3.Transform(normal, OrthoX);
            float dot = Vector3.Dot(normal, w);
            if (Math.Abs(dot) > 0.6)
            {
                w = Vector3.Transform(normal, OrthoY);
            }
            w.Normalize();

            orthonormal1 = Vector3.Cross(normal, w);
            orthonormal1.Normalize();
            orthonormal2 = Vector3.Cross(normal, orthonormal1);
            orthonormal2.Normalize();
        }
        #endregion

        #region General Helper Methods
        public Point kinectVectorToPoint(Vector2 kinectVector)
        {
            return new Point((int)kinectVector.X, (int)kinectVector.Y);
        }

        public Point kinectVectorToPoint(Vector2 kinectVector, Rectangle scaleRect)
        {
            return kinectVectorToPoint(getScaledVector(kinectVector, scaleRect));
        }

        public Vector2 getScaledVector(Vector2 kinectVector, Rectangle scaleRect)
        {
            return kinectVector * new Vector2(scaleRect.Width, scaleRect.Height);
        }
        #endregion

        #region KinectHelperMethods
        public String getConnectedStatus()
        {
            return connectedStatus;
        }

        public InputState getKinectInputState()
        {
            return inputState;
        }

        private void DiscoverKinectSensor()
        {
            foreach (KinectSensor sensor in KinectSensor.KinectSensors)
            {
                if (sensor.Status == KinectStatus.Connected)
                {
                    // Found one, set our sensor to this
                    kinectSensor = sensor;
                    inputState = InputState.Connected;
                    break;
                }
            }

            if (this.kinectSensor == null)
            {
                connectedStatus = "Found none Kinect Sensors connected to USB";
                inputState = InputState.NoKinect;
                return;
            }

            // You can use the kinectSensor.Status to check for status
            // and give the user some kind of feedback
            switch (kinectSensor.Status)
            {
                case KinectStatus.Connected:
                    connectedStatus = "Status: Connected";
                    inputState = InputState.Connected;
                    break;
                case KinectStatus.Disconnected:
                    connectedStatus = "Status: Disconnected";
                    inputState = InputState.Disconnected;
                    break;
                case KinectStatus.NotPowered:
                    connectedStatus = "Status: Connect the power";
                    inputState = InputState.NotPowered;
                    break;
                default:
                    connectedStatus = "Status: Error";
                    inputState = InputState.Error;
                    break;
            }

            // Init the found and connected device
            if (kinectSensor.Status == KinectStatus.Connected)
            {
                InitializeKinect();
            }
        }

        private bool InitializeKinect()
        {
            // Color stream
            //kinectSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            //kinectSensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(kinectSensor_ColorFrameReady);

            // Depth stream
            //kinectSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            //kinectSensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(kinectSensor_DepthFrameReady);

            // Skeleton Stream
            kinectSensor.SkeletonStream.Enable(new TransformSmoothParameters()
            {
                Smoothing = 0.5f,
                Correction = 0.5f,
                Prediction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            });
            //kinectSensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinectSensor_SkeletonFrameReady);
            kinectSensor.ColorStream.Enable();
            kinectSensor.DepthStream.Enable();
            kinectSensor.SkeletonStream.Enable();
            kinectSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(ks_AllFramesReady);

            myIntClient mic = new myIntClient(this);
            intStream = new Microsoft.Kinect.Toolkit.Interaction.InteractionStream(kinectSensor, mic);
            intStream.InteractionFrameReady += new EventHandler<InteractionFrameReadyEventArgs>(intStream_InteractionFrameReady);

            try
            {
                kinectSensor.Start();
            }
            catch
            {
                connectedStatus = "Unable to start the Kinect Sensor";
                inputState = InputState.Error;
                return false;
            }

            return true;
        }

        // Converts a 16-bit grayscale depth frame which includes player indexes into a 32-bit frame
        // that displays different players in different colors
        private byte[] ConvertDepthFrame(short[] depthFrame, DepthImageStream depthStream, int depthFrame32Length)
        {
            int tooNearDepth = depthStream.TooNearDepth;
            int tooFarDepth = depthStream.TooFarDepth;
            int unknownDepth = depthStream.UnknownDepth;
            byte[] depthFrame32 = new byte[depthFrame32Length];

            for (int i16 = 0, i32 = 0; i16 < depthFrame.Length && i32 < depthFrame32.Length; i16++, i32 += 4)
            {
                int player = depthFrame[i16] & DepthImageFrame.PlayerIndexBitmask;
                int realDepth = depthFrame[i16] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                // transform 13-bit depth information into an 8-bit intensity appropriate
                // for display (we disregard information in most significant bit)
                byte intensity = (byte)(~(realDepth >> 4));

                if (player == 0 && realDepth == 0)
                {
                    // white 
                    depthFrame32[i32 + RedIndex] = 255;
                    depthFrame32[i32 + GreenIndex] = 255;
                    depthFrame32[i32 + BlueIndex] = 255;
                }
                else if (player == 0 && realDepth == tooFarDepth)
                {
                    // dark purple
                    depthFrame32[i32 + RedIndex] = 66;
                    depthFrame32[i32 + GreenIndex] = 0;
                    depthFrame32[i32 + BlueIndex] = 66;
                }
                else if (player == 0 && realDepth == unknownDepth)
                {
                    // dark brown
                    depthFrame32[i32 + RedIndex] = 66;
                    depthFrame32[i32 + GreenIndex] = 66;
                    depthFrame32[i32 + BlueIndex] = 33;
                }
                else
                {
                    // tint the intensity by dividing by per-player values
                    depthFrame32[i32 + RedIndex] = (byte)(intensity >> IntensityShiftByPlayerR[player]);
                    depthFrame32[i32 + GreenIndex] = (byte)(intensity >> IntensityShiftByPlayerG[player]);
                    depthFrame32[i32 + BlueIndex] = (byte)(intensity >> IntensityShiftByPlayerB[player]);
                }
            }

            return depthFrame32;
        }

        public InteractionInfo GetInteractionInfoAtLocation(int skeletonTrackingId, InteractionHandType handType, double x, double y)
        {
            InteractionInfo ii = new InteractionInfo();

            //f.Text = x.ToString() + " " + y.ToString();

            return ii;
        }
        #endregion

        #region KinectEventHandlers
        void ks_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {


            short[] depthPix;
            using (DepthImageFrame dif = e.OpenDepthImageFrame())
            {
                if (dif == null)
                {
                    inputState = InputState.Error;
                    return;
                }

                depthPix = new short[dif.PixelDataLength];

                dif.CopyPixelDataTo(depthPix);

                intStream.ProcessDepth(dif.GetRawPixelData(), dif.Timestamp);
                //int player = depthPix[0] & DepthImageFrame.PlayerIndexBitmask;
                //int depth = depthPix[0] >> DepthImageFrame.PlayerIndexBitmaskWidth;
            }

            //Skeleton[] skeletons;
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                    intStream.ProcessSkeleton(skeletons, kinectSensor.AccelerometerGetCurrentReading(), skeletonFrame.Timestamp);
                }
            }
            //performPlayerDetection();
        }

        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (this.kinectSensor == e.Sensor)
            {
                if (e.Status == KinectStatus.Disconnected ||
                    e.Status == KinectStatus.NotPowered)
                {
                    this.kinectSensor = null;
                    this.DiscoverKinectSensor();
                }
            }
        }

        void kinectSensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];

                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    if (detectPlayersOnUpdate)
                    {
                        performPlayerDetection();
                    }

                    /*Skeleton playerSkeleton = (from s in skeletonData where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();
                    
                    if (playerSkeleton != null)
                    {*/
                        //Console.WriteLine("Success");

                        //Joint rightHand = playerSkeleton.Joints[JointType.HandRight];
                        //Joint leftHand = playerSkeleton.Joints[JointType.HandRight];
                        /*if (inputManager.getHandInputMode() == InputManager.HandInputMode.Hand_Right ||
                            inputManager.getHandInputMode() == InputManager.HandInputMode.Hand_Both)
                        {
                            Joint useHand = playerSkeleton.Joints[JointType.HandRight];
                            Vector2 handPosition = new Vector2((((0.5f * useHand.Position.X) + 0.5f) * (WNDWIDTH)), (((-0.5f * useHand.Position.Y) + 0.5f) * (WNDHEIGHT)));
                            inputManager.setHandPosition(true, handPosition);
                        }

                        if (inputManager.getHandInputMode() == InputManager.HandInputMode.Hand_Left ||
                            inputManager.getHandInputMode() == InputManager.HandInputMode.Hand_Both)
                        {
                            Joint useHand = playerSkeleton.Joints[JointType.HandLeft];
                            Vector2 handPosition = new Vector2((((0.5f * useHand.Position.X) + 0.5f) * (WNDWIDTH)), (((-0.5f * useHand.Position.Y) + 0.5f) * (WNDHEIGHT)));
                            inputManager.setHandPosition(false, handPosition);
                        }

                        if (inputManager.getUseHipCentre())
                        {
                            Joint hipJoint = playerSkeleton.Joints[JointType.HipCenter];
                            Vector2 hipPos = new Vector2((((0.5f * hipJoint.Position.X) + 0.5f) * (WNDWIDTH)), WNDHEIGHT / 2);
                            inputManager.setHipCentre(hipPos);
                        }

                        if (inputManager.getUseShoulderCentre())
                        {
                            Joint shoulderJoint = playerSkeleton.Joints[JointType.ShoulderCenter];
                            Vector2 shoulderPos = new Vector2((((0.5f * shoulderJoint.Position.X) + 0.5f) * (WNDWIDTH)), (((-0.5f * shoulderJoint.Position.Y) + 0.5f) * (WNDHEIGHT)));
                            inputManager.setShoulderCentre(shoulderPos);
                        }*/
                        //inputManager.setHandPosition(false, hipPos);
                        //inputManager.setHandPosition(true, hipPos);
                        //Joint useHand;
                        //if (useRightHand)
                        //{
                        //    useHand = playerSkeleton.Joints[JointType.HandRight];
                        //}
                        //else
                        //{
                        //    useHand = playerSkeleton.Joints[JointType.HandLeft];
                        //}
                        //handPosition = new Vector2((((0.5f * useHand.Position.X) + 0.5f) * (WNDWIDTH)), (((-0.5f * useHand.Position.Y) + 0.5f) * (WNDHEIGHT)));
                    //}
                }
            }
        }

        void kinectSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorImageFrame = e.OpenColorImageFrame())
            {
                if (colorImageFrame != null)
                {

                    byte[] pixelsFromFrame = new byte[colorImageFrame.PixelDataLength];

                    colorImageFrame.CopyPixelDataTo(pixelsFromFrame);

                    Color[] color = new Color[colorImageFrame.Height * colorImageFrame.Width];
                    kinectRGBVideo = new Texture2D(appRef.GraphicsDevice, colorImageFrame.Width, colorImageFrame.Height);

                    // Go through each pixel and set the bytes correctly.
                    // Remember, each pixel got a Rad, Green and Blue channel.
                    int index = 0;
                    for (int y = 0; y < colorImageFrame.Height; y++)
                    {
                        for (int x = 0; x < colorImageFrame.Width; x++, index += 4)
                        {
                            color[y * colorImageFrame.Width + x] = new Color(pixelsFromFrame[index + 2], pixelsFromFrame[index + 1], pixelsFromFrame[index + 0]);
                        }
                    }

                    // Set pixeldata from the ColorImageFrame to a Texture2D
                    kinectRGBVideo.SetData(color);
                }
            }
        }

        // http://digitalerr0r.wordpress.com/2011/06/21/kinect-fundamentals-3-getting-data-from-the-depth-sensor/

        void kinectSensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
            {
                if (depthImageFrame != null)
                {
                    short[] pixelsFromFrame = new short[depthImageFrame.PixelDataLength];

                    depthImageFrame.CopyPixelDataTo(pixelsFromFrame);
                    byte[] convertedPixels = ConvertDepthFrame(pixelsFromFrame, ((KinectSensor)sender).DepthStream, 640 * 480 * 4);

                    Color[] color = new Color[depthImageFrame.Height * depthImageFrame.Width];
                    kinectRGBVideo = new Texture2D(appRef.GraphicsDevice, depthImageFrame.Width, depthImageFrame.Height);

                    // Set convertedPixels from the DepthImageFrame to a the datasource for our Texture2D
                    kinectRGBVideo.SetData<byte>(convertedPixels);
                }
            }
        }

        private void updatePlayerInfo(UserInfo userInfo)
        {
            KinectPlayer player = (from p in players where p.SkeletonID == userInfo.SkeletonTrackingId select p).FirstOrDefault();

            if (player == null)
            {
                player = new KinectPlayer(players.Count());
                //players.Add(player);
            }

            player.InputUserInfo = userInfo;

            if (userInfo.HandPointers[1].HandEventType == InteractionHandEventType.Grip)
                player.RightHand.InputGripState = KinectPlayer.GripState.GripHeld;
            if (userInfo.HandPointers[1].HandEventType == InteractionHandEventType.GripRelease)
                player.RightHand.InputGripState = KinectPlayer.GripState.GripReleased;

            if (userInfo.HandPointers[0].HandEventType == InteractionHandEventType.Grip)
                player.LeftHand.InputGripState = KinectPlayer.GripState.GripHeld;
            if (userInfo.HandPointers[0].HandEventType == InteractionHandEventType.GripRelease)
                player.LeftHand.InputGripState = KinectPlayer.GripState.GripReleased;

            Vector2 temp = player.LeftHand.InputGripPoint;
            temp.X = (float)(userInfo.HandPointers[0].X);
            temp.Y = (float)(userInfo.HandPointers[0].Y);

            temp = player.RightHand.InputGripPoint;
            temp.X = (float)(userInfo.HandPointers[1].X);
            temp.Y = (float)(userInfo.HandPointers[1].Y);
        }

        void intStream_InteractionFrameReady(object sender, InteractionFrameReadyEventArgs e)
        {
            InteractionFrame iFrame = e.OpenInteractionFrame();
            if (iFrame == null) return;


            UserInfo[] usrInfo = new UserInfo[6];

            iFrame.CopyInteractionDataTo(usrInfo);

            List<UserInfo> curUsers = usrInfo.Where(x => x.SkeletonTrackingId > 0).ToList<UserInfo>();

            if (curUsers.Count > 0)
            {
                for (int i = 0; i < curUsers.Count; i++)
                {
                    updatePlayerInfo(curUsers[i]);

                    /*UserInfo curUser = curUsers[i];
                    players[i].InputUserInfo = curUser;

                    if (curUser.HandPointers[1].HandEventType == InteractionHandEventType.Grip)
                        players[i].RightHand.InputGripState = KinectPlayer.GripState.GripHeld;
                    if (curUser.HandPointers[1].HandEventType == InteractionHandEventType.GripRelease)
                        players[i].RightHand.InputGripState = KinectPlayer.GripState.GripReleased;

                    if (curUser.HandPointers[0].HandEventType == InteractionHandEventType.Grip)
                        players[i].LeftHand.InputGripState = KinectPlayer.GripState.GripHeld;
                    if (curUser.HandPointers[0].HandEventType == InteractionHandEventType.GripRelease)
                        players[i].LeftHand.InputGripState = KinectPlayer.GripState.GripReleased;

                    Vector2 temp = players[i].LeftHand.InputGripPoint;
                    temp.X = (float)(curUser.HandPointers[0].X);
                    temp.Y = (float)(curUser.HandPointers[0].Y);

                    temp = players[i].RightHand.InputGripPoint;
                    temp.X = (float)(curUser.HandPointers[1].X);
                    temp.Y = (float)(curUser.HandPointers[1].Y);*/
                }
            }
            else
            {
                for (int i = 0; i < curUsers.Count && i < players.Count(); i++)
                {
                    players[i].LeftHand.InputGripState = KinectPlayer.GripState.GripReleased;
                    players[i].RightHand.InputGripState = KinectPlayer.GripState.GripReleased;
                }
            }
        }

        private class myIntClient : Microsoft.Kinect.Toolkit.Interaction.IInteractionClient
        {
            private KinectInputManager kinectInputManager;

            public myIntClient(KinectInputManager kinectInputManager)
            {
                this.kinectInputManager = kinectInputManager;
            }

            public InteractionInfo GetInteractionInfoAtLocation(int skeletonTrackingId, InteractionHandType handType, double x, double y)
            {
                InteractionInfo ii = new InteractionInfo();

                //f.Text = x.ToString() + " " + y.ToString();

                return ii;
            }
        }

        #endregion
    }
}
