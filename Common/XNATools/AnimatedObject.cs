using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;

namespace XNATools
{
    public class AnimatedObject
    {
       protected Texture2D spriteSheet;

        //protected float speedMultiplierX;
        //protected float speedMultiplierY;
        protected float x, y;
        protected float rotation;
        protected Rectangle dest, source;

        protected int spriteWidth, spriteHeight;
        protected int spriteSheetWidth, spriteSheetHeight;
        protected int framesPerRow;
        protected int totalRowCount;
        protected int animStartID, animEndID;
        protected int animCurID;
        protected float opacity;
        protected Color color;
        protected Vector2 origin;
        protected bool visible;
        protected SpriteEffects spriteEffects;

        protected int stopFrame;
        protected int playforframes;
        private float nextFrameTimer;
        private float frameTime = 250;

        public AnimatedObject(Texture2D spriteSheet, int spriteWidth, int spriteHeight, Rectangle dest)
        {
            this.spriteSheet = spriteSheet;
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;
            this.dest = dest;

           // speedMultiplierX = 0;
            //speedMultiplierY = 0;
            x = dest.X;
            y = dest.Y;
            rotation = 0;
            spriteSheetWidth = spriteSheet.Width;
            spriteSheetHeight = spriteSheet.Height;
            framesPerRow = spriteSheetWidth / spriteWidth;
            totalRowCount = spriteSheetHeight / spriteHeight;
            animStartID = animCurID = animEndID = 0;
            source = new Rectangle(0, 0, spriteWidth, spriteHeight);

            opacity = 1.0f;
            color = Color.White;
            spriteEffects = SpriteEffects.None;
            origin = new Vector2(0, 0);//spriteWidth / 2, spriteHeight / 2);

            nextFrameTimer = 0;
            playforframes = 0;

            visible = true;
        }

        public virtual void update(GameTime gameTime)
        {
            if (animStartID != animEndID)
            {
                nextFrameTimer -= gameTime.ElapsedGameTime.Milliseconds;

                if (nextFrameTimer <= 0)
                {
                    if (animCurID == animEndID)
                    {
                        animCurID = animStartID;
                    }
                    else
                    {
                        animCurID++;
                    }
                    setFrame(animCurID);

                    if (playforframes > 0)
                    {
                        playforframes--;
                    }
                    else if (playforframes == 0)
                    {
                        beginAnimation(stopFrame, stopFrame);
                    }
                    nextFrameTimer = frameTime;
                }
            }
        }

        public virtual void draw(SpriteBatch spriteBatch)
        {
            if(visible)
                spriteBatch.Draw(spriteSheet, dest, source, color * opacity, 
                                rotation, origin, spriteEffects, 0);
        }

        public void beginAnimation(int startFrameID, int endFrameID)
        {
            animStartID = startFrameID;
            animEndID = endFrameID;
            animCurID = startFrameID;
            nextFrameTimer = frameTime;
            setFrame(animStartID);
            playforframes = -1;
        }

        public void beginAnimation(int startFrameID, int endFrameID, int playforframes)
        {
            animStartID = startFrameID;
            animEndID = endFrameID;
            animCurID = startFrameID;
            nextFrameTimer = frameTime;
            setFrame(animStartID);
            this.playforframes = playforframes;
        }

        public void setFrame(int frameID)
        {
            animCurID = frameID;
            int row = frameID / framesPerRow;
            int col = frameID % framesPerRow;
            source = new Rectangle(col*spriteWidth, row*spriteHeight, spriteWidth, spriteHeight);
        }

        public int getFrame()
        {
            return animCurID;
        }

        public int getNextFrameID()
        {
            return (animCurID + 1 > animEndID) ? animStartID : animCurID + 1;
        }

        public void setFrameTime(int frameTime)
        {
            this.frameTime = frameTime;
        }

        public int getTotalFrameCount()
        {
            return totalRowCount * framesPerRow;
        }

        public int getRowStartID(int row)
        {
            return row * framesPerRow;
        }

        public int getRowEndID(int row)
        {
            return getRowStartID(row) + framesPerRow - 1;
        }

        public int getRowCount()
        {
            return totalRowCount;
        }

        public int getRowIDFromFrame(int frame)
        {
            return animCurID / framesPerRow;
        }

        public int getColIDFromFrame(int frame)
        {
            return animCurID % framesPerRow;
        }

        public int getFramesPerRow()
        {
            return framesPerRow;
        }

        public void setColor(Color color)
        {
            this.color = color;
        }

        public void setOpacity(float opacity)
        {
            this.opacity = opacity;
        }

        public void setRotation(float rotation)
        {
            this.rotation = rotation;
        }

        public float getRotation()
        {
            return rotation;
        }

        public Rectangle getRect()
        {
            return dest;
        }

        public void setLocation(float x, float y)
        {
            this.x = x;
            this.y = y;
            dest.X = (int)this.x;
            dest.Y = (int)this.y;
        }

        public void moveBy(float x, float y)
        {
            this.x += x;
            this.y += y;
            dest.X = (int)this.x;
            dest.Y = (int)this.y;
        }

        public float getX()
        {
            return x;
        }

        public float getY()
        {
            return y;
        }

        public void setSpriteSheet(Texture2D spriteSheet)
        {
            this.spriteSheet = spriteSheet;
        }

        public void setDefaultOrigin()
        {
            origin = new Vector2(spriteWidth / 2, spriteHeight / 2);
        }

        public void setOrigin(Vector2 origin)
        {
            this.origin = origin;
        }

        public bool isAnimating()
        {
            return animStartID != animEndID;
        }

        public void setVisible(bool visible)
        {
            this.visible = visible;
        }

        public bool getVisible()
        {
            return visible;
        }

        public void setDest(Rectangle dest)
        {
            this.dest = dest;
        }

        public void setSize(float width, float height)
        {
            dest.Width = (int)width;
            dest.Height = (int)height;
        }

        public void setStopFrame(int frameID)
        {
            stopFrame = frameID;
        }

        public float getFrameTime()
        {
            return frameTime;
        }

        public void setSpriteEffect(SpriteEffects effect)
        {
            spriteEffects = effect;
        }
    }
}
