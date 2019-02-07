using System;

namespace Pomodoro
{
    public enum IntervalType
    { PAUSE = 0, WORK = 1 };

    public class Interval
    {
        #region Properties

        private IntervalType _Type;

        public IntervalType Type
        {
            get { return _Type; }
            set
            {
                _Type = value;
                OnTypeChanged(new EventArgs());
            }
        }

        private bool _Active;

        public bool Active
        {
            get { return _Active; }
            set
            {
                _Active = value;
                OnActiveChanged(new EventArgs());
            }
        }

        private int _Seconds;

        public int Seconds
        {
            get { return _Seconds; }
            set
            {
                _Seconds = value;
                if (_Seconds < 0)
                {
                    _Seconds = 0;
                }
                OnSecondsChanged(new EventArgs());
            }
        }

        #endregion Properties

        #region EventHandlers

        /// <summary>
        /// Fires if the intervals type has changed
        /// </summary>
        public event EventHandler<EventArgs> TypeChanged;

        /// <summary>
        /// Fires if the intervals active status has been changed
        /// </summary>
        public event EventHandler<EventArgs> ActiveChanged;

        /// <summary>
        /// Fires if the amount of seconds has been changed
        /// </summary>
        public event EventHandler<EventArgs> SecondsChanged;

        /// <summary>
        /// Fires if the Interval has been changed in any way
        /// </summary>
        public event EventHandler<EventArgs> Changed;

        /// <summary>
        /// Function belonging to the TypeChanged EventHandler
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTypeChanged(EventArgs e)
        {
            if (TypeChanged != null)
            {
                TypeChanged(this, e);
            }
            OnChanged(e);
        }

        /// <summary>
        /// Function belonging to the ActiveChanged EventHandler
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnActiveChanged(EventArgs e)
        {
            if (ActiveChanged != null)
            {
                ActiveChanged(this, e);
            }
            OnChanged(e);
        }

        /// <summary>
        /// Function belonging to the SecondsChanged EventHandler
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSecondsChanged(EventArgs e)
        {
            //If there is a function assigned to the event handler, run it
            if (SecondsChanged != null)
            {
                SecondsChanged(this, e);
            }
            OnChanged(e);
        }

        /// <summary>
        /// Function belonging to the Changed EventHandler
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        #endregion EventHandlers

        /// <summary>
        /// Creates a new Interval object
        /// </summary>
        /// <param name="intervalType">The type of the interval</param>
        /// <param name="timeSpanSeconds">The length of the interval in seconds</param>
        public Interval(IntervalType intervalType, int timeSeconds)
        {
            this.Type = intervalType;
            this.Seconds = timeSeconds;
            this.Active = false;
        }

        /// <summary>
        /// Creates a new Interval object
        /// </summary>
        /// <param name="intervalType">The type of the interval</param>
        public Interval(IntervalType intervalType)
        {
            this.Type = intervalType;
            this.Active = false;
        }

        /// <summary>
        /// Custom string conversion method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return (this.Type.ToString() + this.Seconds.ToString());
        }
    }
}