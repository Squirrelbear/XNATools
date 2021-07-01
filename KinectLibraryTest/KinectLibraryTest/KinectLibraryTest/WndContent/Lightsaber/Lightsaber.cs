using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Audio;

using XNATools;
using XNATools.WndCore;
using XNATools.WndCore.Kinect;

namespace KinectLibraryTest
{
    public class Lightsaber : WndComponent
    {
        private Texture2D fullSprite, noneSprite, bladeSprite;
        private KinectWndHandle wndHandle;
        private bool trackLeft;
        private AnimatedObject animatedObj;
        private AnimatedObject blade;
        private float gripPercent;
        private SoundEffect bladeExtendSound, bladeShrinkSound;
        private SoundEffect[] swingSounds;
        private Timer soundCooldown;
        private Random gen;

        public Lightsaber(Rectangle dest, KinectWndHandle wndHandle)
            : base(dest) 
        {
            this.wndHandle = wndHandle;

            noneSprite = wndHandle.loadTexture("lightsabernone");
            bladeSprite = wndHandle.loadTexture("lighersaberblade");
            fullSprite = wndHandle.loadTexture("lightsaberfull");

            bladeExtendSound = wndHandle.loadSound("fx4");
            bladeShrinkSound = wndHandle.loadSound("fx5");
            swingSounds = new SoundEffect[4];
            for (int i = 0; i < swingSounds.GetLength(0); i++)
                swingSounds[i] = wndHandle.loadSound("Hum" + (i + 1));

            gen = new Random();

            animatedObj = new AnimatedObject(noneSprite, 143, 1300, dest);
            animatedObj.setOrigin(new Vector2(143 / 2, 3 * 1300 / 4));
            blade = new AnimatedObject(bladeSprite, 143, 1300, dest);
            blade.setOrigin(new Vector2(143 / 2, 3 * 1300 / 4));
            trackLeft = false;
            gripPercent = 0;
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            //base.draw(spriteBatch);
            if (gripPercent < 1)
            {
                blade.draw(spriteBatch);
                animatedObj.draw(spriteBatch);
            }
            else
            {
                animatedObj.draw(spriteBatch);
                blade.draw(spriteBatch);
            }
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (wndHandle.getKinectInputManager().getGripState(false) == KinectPlayer.GripState.GripHeld)
            {
                if (gripPercent == 0)
                    bladeExtendSound.Play();

                gripPercent += gameTime.ElapsedGameTime.Milliseconds / 1000.0f * 2;
                if(gripPercent > 1) gripPercent = 1;

                if (soundCooldown == null && gripPercent == 1)
                {
                    soundCooldown = new Timer(gen.Next(300, 600));
                }
                else if (soundCooldown != null)
                {
                    soundCooldown.update(gameTime);
                    if (soundCooldown.wasTriggered())
                    {
                        int nextSound = gen.Next(swingSounds.GetLength(0));
                        swingSounds[nextSound].Play();
                        int duration = swingSounds[nextSound].Duration.Milliseconds;
                        soundCooldown.setInterval(gen.Next(duration + 200, duration + 800));
                    }
                }
            }
            else
            {
                soundCooldown = null;
                if (gripPercent == 1)
                    bladeShrinkSound.Play();

                gripPercent -= gameTime.ElapsedGameTime.Milliseconds / 1000.0f * 2;
                if (gripPercent < 0) gripPercent = 0;
            }
            Rectangle bladeDest = blade.getRect();
            bladeDest.Height = (int)(animatedObj.getRect().Height * gripPercent);
            blade.setDest(bladeDest);

            Vector2 loc = wndHandle.getKinectInputManager().getScaledVector(
                            wndHandle.getKinectInputManager().getGripPos(trackLeft), wndHandle.getRect())
                            - new Vector2(getRect().Width / 2, getRect().Height / 2);
            animatedObj.setLocation(loc.X, loc.Y);
            blade.setLocation(loc.X, loc.Y);

            Skeleton s = wndHandle.getKinectInputManager().getSkeleton();
            if (s != null)
            {
                Microsoft.Kinect.Vector4 v = s.BoneOrientations[JointType.HandRight].AbsoluteRotation.Quaternion;
                Quaternion q = new Quaternion(v.X, v.Y, v.Z, v.W);
                Vector3 axis = Vector3.UnitZ;
                float newRotation = 0.5f * (float)Math.PI - (FindQuaternionTwist(q, axis)) * (float)Math.PI;
                animatedObj.setRotation(newRotation);
                blade.setRotation(newRotation);
            }
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
    }
}
