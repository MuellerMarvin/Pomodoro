using System.Windows.Controls;
using System.Windows.Media;

namespace Pomodoro
{
    /// <summary>
    /// Interaction logic for IntervalListItem.xaml
    /// </summary>
    public partial class IntervalListItem : UserControl
    {
        public IntervalListItem(Interval interval)
        {
            InitializeComponent();

            //Set the interval and the used divider to display the time left
            this.InternalInterval = interval;
        }

        #region Properties

        //Properties
        internal Interval _InternalInterval;

        public Interval InternalInterval
        {
            get
            {
                return _InternalInterval;
            }
            set
            {
                //Replace the interval with the new one
                _InternalInterval = value;

                //Make sure it refreshes when the interval is changed
                InternalInterval.Changed += this.RefreshOnChange;
                InternalInterval.ActiveChanged += this.ChangeActiveDisplay;

                //Refresh the interfact
                this.Refresh();

                // Make sure the Activity display is being Refreshed as well
                ChangeActiveDisplay(this, new System.EventArgs());
            }
        }

        #endregion Properties

        #region Functions

        /// <summary>
        /// Refreshes the Items visible objects
        /// </summary>
        internal void Refresh()
        {
            //Set Typelabel
            if (InternalInterval.Type == IntervalType.WORK)
            {
                this.TypeLabel.Content = "Work";
                this.Background = Brushes.RoyalBlue;
            }
            else
            {
                this.TypeLabel.Content = "Pause";
                this.Background = Brushes.GreenYellow;
            }

            //Set time label
            this.TimeLabel.Content = GetTimeString(InternalInterval.Seconds);
        }

        /// <summary>
        /// Fires when the activity status of the interval has changed to change the appearance of the item accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeActiveDisplay(object sender, System.EventArgs e)
        {
            //Hide or show the red ellipse (ActiveDisplay) according to if the internal interval is active or not
            if (InternalInterval.Active)
            {
                this.ActiveDisplay.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.ActiveDisplay.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        /// <summary>
        /// Fires if the Interval has been changed in any way
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshOnChange(object sender, System.EventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// Converts a number of seconds into a string, that can be displayed to the user in the format hours:minutes:seconds
        /// </summary>
        /// <param name="ticks">Amount of time in seconds</param>
        /// <returns></returns>
        internal string GetTimeString(int ticks)
        {
            //Get the correct timeintegers
            //seconds
            int intSeconds = ticks % 60;
            ticks -= intSeconds;
            ticks = ticks / 60;

            int intMinutes = ticks % 60;
            ticks -= intMinutes;
            ticks = ticks / 60;

            int intHours = ticks;

            //Turn them into string
            string seconds = intSeconds.ToString();
            string minutes = intMinutes.ToString();
            string hours = intHours.ToString();

            //Fill them up
            seconds = FillTimeStringUp(seconds, 2);
            minutes = FillTimeStringUp(minutes, 2);
            hours = FillTimeStringUp(hours, 2);

            return hours + ":" + minutes + ":" + seconds;
        }

        private string FillTimeStringUp(string timeString, int fillLength)
        {
            while (timeString.Length < fillLength)
            {
                timeString = '0' + timeString;
            }
            return timeString;
        }

        public override string ToString()
        {
            return InternalInterval.ToString();
        }

        #endregion Functions
    }
}