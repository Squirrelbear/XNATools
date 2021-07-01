using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using XNATools;
using Microsoft.Xna.Framework;
using XNATools.WndCore;

namespace KinectLibraryTest
{
    public class CarRoadPanel : Panel
    {
        private int[] DIFFICULTYCARS = { 3, 5, 10, 5, 10, 13, 10, 13, 10, 13 };
        private float[] DIFFICULTYTIME = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        private Random sharedRandom;
        private List<Car> cars, inactiveCars;
        private Car player;
        private bool isArcade, isDragging;
        public bool Paused { set; get; }
        private Texture2D[] carSprites;
        private int difficulty, score, dodged, crashes;
        private CarGameWnd mp;

        public CarRoadPanel(Rectangle dest, CarGameWnd mp)
            : base(dest)
        {
            this.mp = mp;
            sharedRandom = new Random();

            int carWidth = dest.Width / 13 - 15;
            int carHeight = (int)(carWidth * 1.5);

            carSprites = new Texture2D[Car.CARCOLOURS.Length];
            for (int i = 0; i < carSprites.Length - 1; i++)
            {
                Texture2D carSprite = ImageTools.getSingleton().createColorTexture(Car.CARCOLOURS[i], carWidth, carHeight);
                //ImageTools.getSingleton().fillRect(carSprite, 
                carSprites[i] = carSprite;
            }
            carSprites[carSprites.Length - 1] = ImageTools.getSingleton().createTransparentTexture(carWidth, carHeight);
            carSprites[carSprites.Length - 1] = ImageTools.getSingleton().fillOval(carSprites[carSprites.Length - 1], 
                                                            new Rectangle(0, 0, carWidth, carHeight), Car.CARCOLOURS[6]);

            LayoutManger lm = new LayoutManger(new Rectangle(dest.Left, -carHeight, dest.Width, carHeight), 1, 13);
            lm.setPadding(new Rectangle(7, 0, 8, 0));
            cars = new List<Car>();
            inactiveCars = new List<Car>();

            for (int i = 0; i < 13; i++)
            {
                Car car = new Car(lm.nextRect(), this);
                inactiveCars.Add(car);
                cars.Add(car);
            }
            player = new Car(new Rectangle(dest.Center.X - carWidth / 2, dest.Bottom - carHeight - 30, carWidth, carHeight), this, true);
            difficulty = 1;
            isArcade = false;
            newGame();
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (Paused)
                return;

            for (int i = 0; i < cars.Count; i++)
                cars[i].update(gameTime, DIFFICULTYTIME[difficulty-1]);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);

            for (int i = 0; i < cars.Count; i++)
                cars[i].draw(spriteBatch);
            player.draw(spriteBatch);
        }

        public override void mousePressedLeft(Point p)
        {
            if (getRect().Contains(p))
                isDragging = true;
        }

        public override void mouseClickedLeft(Point p)
        {
            isDragging = false;
        }

        public override void mouseMoved(Point oldP, Point newP)
        {
            if (isDragging)
                player.setLocation(Math.Min(Math.Max(newP.X, dest.X), dest.Right - player.getRect().Width), player.getY());
        }

        public void newGame(bool resetScores = false)
        {
            inactiveCars.Clear();
            foreach (Car car in cars)
            {
                car.resetCar();
                car.Active = false;
            }
            inactiveCars.AddRange(cars);

            for (int i = 0; i < DIFFICULTYCARS[difficulty-1]; i++)
                spawnCar();

            if (resetScores)
                score = dodged = 0;
            Paused = false;
        }

        public void handleCarExited(Car car, bool bulletDestroyed = false)
        {
            car.Active = false;
            car.resetCar();
            inactiveCars.Add(car);
            score += (bulletDestroyed) ? car.getBulletScore() : car.getExitScore();
            mp.updateScore(score, (bulletDestroyed) ? dodged : ++dodged);
            spawnCar();
        }

        public void spawnCar()
        {
            inactiveCars = inactiveCars.OrderBy(a => sharedRandom.Next()).ToList();
            inactiveCars[0].Active = true;
            inactiveCars.RemoveAt(0);
        }

        public Random getSharedRandom()
        {
            return sharedRandom;
        }

        public KeyValuePair<int, Texture2D> getNextCarType()
        {
            int carType = sharedRandom.Next(1, (isArcade) ? 7 : 5);
            return new KeyValuePair<int,Texture2D>(carType, carSprites[carType]);
        }

        public Texture2D getCarSprite(int carType)
        {
            return carSprites[carType];
        }

        public void setIsArcade(bool isArcade)
        {
            this.isArcade = isArcade;
        }

        public float getCarYPos()
        {
            return cars[0].getRect().Y;
        }

        public float getCarYPosV(int i)
        {
            return cars[i].getRect().Y;
        }
    }
}
