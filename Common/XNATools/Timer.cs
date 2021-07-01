using System;
using Microsoft.Xna.Framework;

namespace XNATools
{
    /// <summary>
    /// Provides a basic Timer implementation that will trigger events
    /// based on a time interval. The timer should be updated every 
    /// update cycle and the wasTriggered() method should be used after 
    /// an update to determine if the timer event has occured. Note
    /// that this object should not be used for highly precise timer
    /// operations. The firing of events is limited to one per update 
    /// and so the time between updates will determine the minimum interval.
    /// </summary>
    public class Timer
    {
        #region Instance Variables
        /// <summary>
        /// The interval between events occuring. This is a time in milliseconds.
        /// </summary>
        protected float interval;

        /// <summary>
        /// The variable used to count where current time is up to. 
        /// When this variable hits 0 the event is triggered.
        /// </summary>
        protected float countDown;

        /// <summary>
        /// This variable can be used to pause the timer until it should
        /// be resumed at any later time.
        /// </summary>
        protected bool paused;

        /// <summary>
        /// This variable will be set to true after a timer update to indicate that the 
        /// timer's interval has ended and that the timer has continued to the next interval.
        /// During the following update cycle this will always be reset back to false.
        /// </summary>
        protected bool timerTriggered;
        #endregion

        /// <summary>
        /// Creates a timer object that has a specified interval.
        /// </summary>
        /// <param name="interval">The interval in milliseconds for the timer to use.</param>
        public Timer(float interval)
            : this(interval, interval)
        {
        }

        /// <summary>
        /// Creates a timer object that has a specified countDown and interval.
        /// CountDown represents the initial interval length so that it is 
        /// possible to make it different from the interval cycles that follow 
        /// after that. Both times use milliseconds as their unit of time.
        /// </summary>
        /// <param name="countDown">The initial interval.</param>
        /// <param name="interval">The standard interval to be used after the first one.</param>
        public Timer(float countDown, float interval)
        {
            this.countDown = countDown;
            this.interval = interval;
        }

        /// <summary>
        /// This method should be called to update the timer so that it can
        /// progress the time forward. The timer will clear any previous triggering.
        /// The timer will not be updated if it is paused. When a timer event has
        /// occured the wasTriggered() method should be used to determine
        /// if it has occured. Once an event has occured the next time interval will
        /// immediately start and any extra time from the previous iteration will be
        /// clocked over to the new one.
        /// </summary>
        public void update(GameTime gameTime)
        {
            timerTriggered = false;
            if (!paused)
            {
                countDown -= gameTime.ElapsedGameTime.Milliseconds;

                if (countDown < 0)
                {
                    timerTriggered = true;
                    countDown += interval;
                }
            }
        }

        /// <summary>
        /// Sets the time that will be counted down from on the timer.
        /// </summary>
        /// <param name="time">The time to set the clock to. (in milliseconds)</param>
        public void setTime(float time)
        {
            this.countDown = time;
        }

        /// <summary>
        /// Gets the time remaining before the next event in whole milliseconds.
        /// </summary>
        public int getTime()
        {
            return (int)countDown;
        }

        /// <summary>
        /// Gets if the previous update triggered a timer event.
        /// If this is true it means the timer has begun it's next 
        /// cycle and if there hsould be any program changes based on
        /// the event they should be performed right away.
        /// </summary>
        public bool wasTriggered()
        {
            return timerTriggered;
        }

        /// <summary>
        /// Sets the interval to use in milliseconds.
        /// </summary>
        public void setInterval(float interval)
        {
            this.interval = interval;
        }

        /// <summary>
        /// Gets the time interval rounded down to the closest millisecond.
        /// </summary>
        public int getInterval()
        {
            return (int)interval;
        }

        /// <summary>
        /// Gets the percent of progress toward the next interval. 
        /// Value will increase from 0 to 1 as it approaches the end.
        /// </summary>
        public float getTimePercent()
        {
            return (interval - countDown) / interval; 
        }

        /// <summary>
        /// Sets whether the timer is paused. When paused the timer
        /// will not continue to increase the clock.
        /// </summary>
        public void setPaused(bool paused)
        {
            this.paused = paused;
        }

        /// <summary>
        /// Gets whether the timer is paused. When paused the timer
        /// will not continue to increase the clock.
        /// </summary>
        public bool getPaused()
        {
            return paused;
        }

        /// <summary>
        /// Gets the time in seconds with no decimal places.
        /// </summary>
        public double getTimeInSeconds()
        {
            return getTimeInSeconds(0);
        }

        /// <summary>
        /// Gets the time in seconds up to the number of decimal places specified.
        /// </summary>
        /// <param name="decimalPlaces"></param>
        /// <returns></returns>
        public double getTimeInSeconds(int decimalPlaces)
        {
            double time = (int)(Math.Pow(10.0, decimalPlaces - 3) * countDown);
            return time * Math.Pow(10.0, -decimalPlaces);
        }

        /// <summary>
        /// Uses the getTimeInSeconds() method to get the time
        /// and then represent it as a formatted string with:
        /// M:SS format. If the seconds is less than 10 a 0 will
        /// be supplemented in place of the first S.
        /// </summary>
        public string getFormatedTime()
        {
            int fullTime = (int)getTimeInSeconds();
            int minutes = fullTime / 60;
            int seconds = fullTime % 60;

            string result = "";
            string modString = (seconds < 10) ? "0" : "";
            result = minutes + ":" + modString + seconds;
            return result;
        }
    }
}
