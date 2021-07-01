using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNATools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KinectLibraryTest
{
    public class Car : AnimatedObject
    {
        public static Color[] CARCOLOURS = { Color.Black, Color.Blue, Color.Green, Color.Red, Color.Yellow, Color.Brown, Color.Black };
        private int[] CARSPEEDS = { 3, 5, 8, 10, 20, 10, 2 };
        private int carType;
        private CarRoadPanel road;
        public bool Active { get; set; }

        public Car(Rectangle rect, CarRoadPanel road, bool isPlayer = false)
            : base(ImageTools.getSingleton().createColorTexture(Color.Black, rect.Width, rect.Height),  rect.Width, rect.Height, rect)
        {
            this.road = road;

            if (isPlayer)
            {
                setSpriteSheet(road.getCarSprite(0));
            }
            else
            {
                resetCar();
            }
        }

        public void update(GameTime gameTime, float timeMultiplier)
        {
            if (!Active)
                return;

            base.update(gameTime);

            if(carType == 4)
                moveBy(0, road.getSharedRandom().Next(1,21) * gameTime.ElapsedGameTime.Milliseconds / 10.0f * timeMultiplier);
            else if(carType != 0)
                moveBy(0, CARSPEEDS[carType] * gameTime.ElapsedGameTime.Milliseconds / 10.0f * timeMultiplier);

            if (getY() > road.getRect().Bottom)
                road.handleCarExited(this);
        }

        public void resetCar()
        {
            KeyValuePair<int, Texture2D> nextCarType = road.getNextCarType();
            carType = nextCarType.Key;
            setSpriteSheet(nextCarType.Value);
            setLocation(getX(), -getRect().Height);
        }

        public int getExitScore()
        {
            return (carType < 5) ? carType : 0;
        }

        public int getBulletScore()
        {
            return (carType == 5) ? 20 : -5;
        }
    }
}