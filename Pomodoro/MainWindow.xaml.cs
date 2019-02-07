using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Pomodoro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties

        #region General

        /// <summary>
        /// Decides if the timer is running
        /// </summary>
        private bool Running;

        /// <summary>
        /// Used to pick up when Running has been changed and when it hasn't by the "this.Refresh()" Method
        /// </summary>
        private bool LastRefreshRunning;

        /// <summary>
        /// Index of the currently active Interval
        /// </summary>
        public int Active
        {
            /// <summary>
            /// Gets the active interval, and if time has run out sets the next one in the list as active, or alternatively the first one if there is no next one
            /// </summary>
            /// <returns></returns>
            get
            {
                //Check if the list is long enough
                if (CurrentIntervalList.Count < 1)
                {
                    return -1;
                }

                //for each element in the current list, look if it's active
                int activeIndex = -1;
                for (int i = 0; i < CurrentIntervalList.Count; i++)
                {
                    if (CurrentIntervalList[i].Active)
                    {
                        activeIndex = i;
                        break;
                    }
                }

                //if no active interval was found, set the first item as active
                if (activeIndex < 0)
                {
                    this.Active = 0;
                    activeIndex = 0;
                }

                //Check if the time has run out
                if (CurrentIntervalList[activeIndex].Seconds < 1)
                {
                    CurrentIntervalList[activeIndex].Active = false;

                    //Check if there is a next interval to jump to
                    if (CurrentIntervalList.Count > (activeIndex + 1))
                    {
                        //Change the active interval
                        this.Active += 1;

                        //Reset the interval that came before
                        ResetInterval(activeIndex);

                        //Reflect the change in the active index
                        activeIndex++;

                        //Change the color of the window, reflecting the changes made to the list
                        ColorWindow(activeIndex);

                        //Return the currently active index
                        return activeIndex;
                    }
                    //if not, reset the list, stop the timer
                    else
                    {
                        Running = false;
                        ResetIntervals(); //Set the CurrentList to the contents of the DefaultList
                        this.Active = 0; //Set the first interval to be active
                        return 0;
                    }
                }
                return activeIndex;
            }

            /// <summary>
            /// Sets the active interval and makes all others not active
            /// </summary>
            set
            {
                if (CurrentIntervalList.Count <= value || value < 0)
                    return;

                foreach (Interval interval in CurrentIntervalList)
                {
                    interval.Active = false;
                }

                CurrentIntervalList[value].Active = true;
            }
        }

        /// <summary>
        /// Runs the this.Tick() Method once per second (while the timer should be running) to count down the time
        /// </summary>
        private DispatcherTimer TickDispatcherTimer = new DispatcherTimer();

        /// <summary>
        /// Background list, that is holding unmodified intervals from when they have been created
        /// </summary>
        private List<Interval> ResetIntervalList;

        /// <summary>
        /// Visible list in the UI, that is being modified
        /// </summary>
        private List<Interval> CurrentIntervalList;

        /// <summary>
        /// Saves the default color of the window, used by the this.ColorWindow() Method
        /// </summary>
        private Brush DefaultWindowBackground;

        #endregion

        #region Customization
        #region Holders

        private string _BackgroundImagePath;

        #endregion

        public string BackgroundImagePath
        {
            get
            {
                return _BackgroundImagePath;
            }
            set
            {
                _BackgroundImagePath = value;
                this.BackgroundImageBrush.ImageSource = new BitmapImage(new Uri(value));
            }
        }

        #endregion

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Runs when the program starts. Constructor of the Main Application Window
        /// </summary>
        public MainWindow()
        {
            //Initialize Variables
            Running = false;
            LastRefreshRunning = true;
            ResetIntervalList = new List<Interval>();
            CurrentIntervalList = new List<Interval>();

            //Load UI
            InitializeComponent();

            //Save the default background color
            DefaultWindowBackground = this.Background;

            //Load the presets and refresh the UI
            LoadPresetIntervals();
            this.Refresh();

            // Configure the TickDispatchtimer to 1 tick per second
            TickDispatcherTimer.Tick += new EventHandler(Tick);
            TickDispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Deletes all intervals and loads the default ones
        /// </summary>
        public void LoadPresetIntervals()
        {
            //Stop it from running
            Running = false;

            //Clear list
            RemoveIntervals();

            // 25 Minutes work
            AddInterval(new Interval(IntervalType.WORK, 1500));
            // 5 Minutes break
            AddInterval(new Interval(IntervalType.PAUSE, 300));
            // 25 Minutes work
            AddInterval(new Interval(IntervalType.WORK, 1500));
            // 5 Minutes break
            AddInterval(new Interval(IntervalType.PAUSE, 300));
            // 25 Minutes work
            AddInterval(new Interval(IntervalType.WORK, 1500));
            // 5 Minutes break
            AddInterval(new Interval(IntervalType.PAUSE, 300));
            // 25 Minutes work
            AddInterval(new Interval(IntervalType.WORK, 1500));
            // 20 Minute break
            AddInterval(new Interval(IntervalType.PAUSE, 1200));

            //Refresh the visible items
            this.Refresh();
        }

        /// <summary>
        /// Stops the timer and removes all intervals
        /// </summary>
        public void RemoveIntervals()
        {
            //stop the timer
            Running = false;

            //remove all intervals
            CurrentIntervalList.Clear();
            ResetIntervalList.Clear();
        }

        /// <summary>
        /// Removes an interval at the specified index from the Lists
        /// </summary>
        /// <param name="index">Position in the index</param>
        public void RemoveInterval(int index)
        {
            //Check if the index is out of range
            if (CurrentIntervalList.Count <= index)
                return;

            //Remove the interval from both lists
            CurrentIntervalList.RemoveAt(index);
            ResetIntervalList.RemoveAt(index);
        }

        /// <summary>
        /// Set the CurrentList to the contents of the DefaultList. This is needed to "deep copy" the
        /// list instead of making references
        /// </summary>
        public void ResetIntervals()
        {
            //Clear the current list
            CurrentIntervalList.Clear();

            //deep copy the default list to the current list
            foreach (Interval item in ResetIntervalList)
            {
                CurrentIntervalList.Add(new Interval(item.Type, item.Seconds));
            }
        }

        /// <summary>
        /// Resets one certain interval to it's default
        /// </summary>
        /// <param name="index">The interval that should be reset to it's default</param>
        public void ResetInterval(int index)
        {
            CurrentIntervalList[index] = new Interval(ResetIntervalList[index].Type, ResetIntervalList[index].Seconds);
        }

        /// <summary>
        /// Adds a new Deep-Copied interval to both lists
        /// </summary>
        /// <param name="interval"></param>
        public void AddInterval(Interval interval)
        {
            CurrentIntervalList.Add(new Interval(interval.Type, interval.Seconds));
            ResetIntervalList.Add(new Interval(interval.Type, interval.Seconds));
        }

        /// <summary>
        /// This function counts down 1 second of the active interval each time it's run
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tick(object sender, EventArgs e)
        {
            // Check if the timer should even tick
            if (!Running)
            {
                return;
            }

            //Get the active index
            int activeIndex = this.Active;
            //Check if the function returned -1, meaning there is no active interval
            if (activeIndex < 0)
            {
                return;
            }

            //Lower the amount of time left in that interval by 1 second
            CurrentIntervalList[activeIndex].Seconds -= 1;

            //Refresh the UI to reflect the changes
            this.Refresh();
        }

        /// <summary>
        /// This method refreshes the UI
        /// </summary>
        private void Refresh()
        {
            //Run the code ONLY when the "Running" status has changed since we last refreshed the UI
            if (LastRefreshRunning != Running)
            {
                //Deactivate Buttons depending on if the counter is running...
                if (Running)
                {
                    TickDispatcherTimer.Start();
                    PauseButton.Content = "Stop";
                }
                else
                {
                    TickDispatcherTimer.Stop();
                    PauseButton.Content = "Start";
                }
                // Color the window according to the active interval
                ColorWindow();
            }

            //If the UI and the IntervalList have different counts, completely refresh the UI list anew
            if (CurrentIntervalList.Count != IntervalListView.Items.Count)
            {
                IntervalListView.Items.Clear();
                foreach (Interval interval in CurrentIntervalList)
                {
                    IntervalListView.Items.Add(new IntervalListItem(interval));
                }
            }
            //Otherwise, just refresh each view by itself
            else
            {
                for (int i = 0; i < IntervalListView.Items.Count; i++)
                {
                    // Generate an expected item
                    IntervalListItem expectedItem = new IntervalListItem(CurrentIntervalList[i]);
                    string debugItem = IntervalListView.Items[i].ToString();

                    //if the items Intervals differ...
                    if (IntervalListView.Items[i].ToString() != CurrentIntervalList[i].ToString())
                    {
                        //... replace the item with a new, refreshed one
                        IntervalListView.Items[i] = expectedItem;
                    }
                    //else, no tinkering with the UI is needed
                }
            }

            //Save what Running as set as this time, to detect when it's being changed
            LastRefreshRunning = Running;

            //Check if the list is empty
            if (Running && CurrentIntervalList.Count < 1)
            {
                Running = false;
                this.Refresh();
                return;
            }
        }

        /// <summary>
        /// Color the window according to the active interval
        /// </summary>
        private void ColorWindow()
        {
            //Get the active item in the list automaticly
            ColorWindow(this.Active);
        }

        /// <summary>
        /// /// Color the window according to the given interval
        /// </summary>
        /// <param name="activeIndex">Index of the activity status that should be displayed</param>
        private void ColorWindow(int activeIndex)
        {
            // If it's running color it
            if (Running)
            {
                //If the active index is valid
                if (activeIndex >= 0)
                {
                    //It's pause, give it green
                    if (CurrentIntervalList[activeIndex].Type == IntervalType.PAUSE)
                    {
                        this.Background = Brushes.GreenYellow;
                    }
                    //It's work, give it blue
                    else
                    {
                        this.Background = Brushes.RoyalBlue;
                    }
                }
                //if nothing is active, color it with the default color (for example when there is no intervals in the list, but the start button has been pressed
                else
                {
                    this.Background = DefaultWindowBackground;
                }
            }
            else
            {
                // nothing is running, so color it with the default color
                this.Background = DefaultWindowBackground;
            }
        }

        /// <summary>
        /// Opens the customization window and applies the changes
        /// </summary>
        public void OpenCustomizationMenu()
        {
            CustomizationWindow window = new CustomizationWindow(this);
            window.ShowDialog();
            this.BackgroundImagePath = window.BackgroundImagePath;
        }

        #region EventHandlerMethods

        /// <summary>
        /// Gets triggered when the "Add" Button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddIntervalDialog();
            Interval interval = null;
            if (dialog.ShowDialog() == true)
            {
                interval = dialog.ReturnInterval;
            }

            if (interval == null)
            {
                return;
            }
            else
            {
                AddInterval(interval);
            }

            this.Refresh();
        }

        /// <summary>
        /// Gets triggered when the "Remove" Button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            //Extra security
            if (ResetIntervalList.Count < 1)
            {
                return;
            }
            //else

            //Remove last interval
            RemoveInterval(CurrentIntervalList.Count - 1);

            this.Refresh();
        }

        /// <summary>
        /// Gets triggered when the "Reset" Button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            //Stop the timer from running
            Running = false;

            //Reset the CurrentIntervalList list to the ResetIntervalList
            ResetIntervals();

            //Set the first one as active
            this.Active = 0;

            //Refresh the UI
            this.Refresh();
        }
        
        /// <summary>
        /// Gets triggered when the "PausePlay" Button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PausePlayButton_Click(object sender, RoutedEventArgs e)
        {
            //Don't let the user start the timer, if there is no objects anymore
            if (!Running && CurrentIntervalList.Count < 1)
                return;

            Running = !Running;
            this.Refresh();
        }

        /// <summary>
        /// Gets triggered when the "Clear" Button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            //Stop the timer
            Running = false;

            //Clear the lists
            CurrentIntervalList.Clear();
            ResetIntervalList.Clear();

            //Refresh the UI
            this.Refresh();
        }

        /// <summary>
        /// Gets triggered when the "Import" Menuitem is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Import_Click(object sender, RoutedEventArgs e)
        {
            //Create the dialog
            OpenFileDialog dialog = new OpenFileDialog();

            //Give it a nice title and configure it properly
            dialog.Title = "Import Intervals";
            dialog.CheckFileExists = true;

            //Show it to the user and get the users answer
            dialog.ShowDialog();

            //Create a new list
            List<Interval> intervals = new List<Interval>();

            try
            {
                //open the file
                StreamReader reader = new StreamReader(dialog.FileName);

                //Read the contents
                int lineCount = 0;
                while (!reader.EndOfStream)
                {
                    lineCount++; //increment the line counter

                    string line = reader.ReadLine().ToUpper(); //read the line

                    //Check if the line is empty, if it is, continue to the next
                    if (line.Length == 0)
                    {
                        continue; //End of file may have been reached
                    }

                    //Check if it's work or pause
                    IntervalType type;
                    if (line.StartsWith("W"))
                    {
                        type = IntervalType.WORK; //work
                    }
                    else if (line.StartsWith("P"))
                    {
                        type = IntervalType.PAUSE; //pause
                    }
                    else
                    {
                        throw new Exception("Beginning of line " + lineCount + " is invalid. (Expected 'P' or 'W', but instead saw '" + line.ToCharArray()[0] + "'"); //throw exception
                    }

                    //Import the number
                    line = line.Remove(0, 1); //remove the first character

                    //Check if it can be parse, if not, throw an exception
                    if (!int.TryParse(line, out int parsedSeconds))
                    {
                        throw new Exception("Could not parse the amount of seconds of line " + lineCount); //throw exception
                    }

                    //Everything has been parsed successfully, add the new interval to the list
                    intervals.Add(new Interval(type, parsedSeconds));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Import failed.\nMessage:\n" + ex.Message + "\n\nNothing has been imported.");
                return;
            }

            //Stop the timer
            Running = false;

            //Remove all intervals
            this.RemoveIntervals();

            //Add the new intervals
            foreach (Interval interval in intervals)
            {
                AddInterval(interval);
            }

            //Set the first one as active
            this.Active = 0;

            this.Refresh(); // refresh UI
        }

        /// <summary>
        /// Gets triggered when the "Export" Menuitem is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Export_Click(object sender, RoutedEventArgs e)
        {
            //Create the dialog
            SaveFileDialog dialog = new SaveFileDialog();

            //Give it a nice title and configure it properly
            dialog.Title = "Export Intervals";
            dialog.CheckPathExists = true;

            //Show it to the user and get the users answer
            dialog.ShowDialog();

            //Build the string to write to the file
            string fileContent = "";
            foreach (Interval interval in ResetIntervalList)
            {
                //Build line
                string line = "";

                //Define Work or Pause at the beginning of the line
                if (interval.Type == IntervalType.WORK)
                {
                    line += "W"; //work
                }
                else
                {
                    line += "P"; //pause
                }

                //write the amount of seconds behind it
                line += interval.Seconds + "\n";

                //add the line to the file
                fileContent += line;
            }

            //Try writing to the file
            try
            {
                //Create the file and get ready to write
                StreamWriter writer = new StreamWriter(dialog.FileName);

                //Write to it
                writer.Write(fileContent);

                //Flush
                writer.Flush();

                //Close the writer and dispose it
                writer.Close();
                writer.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong when writing to the file!\nMessage:\n\n" + ex.Message);
                return;
            }

            MessageBox.Show("Exported successfully to '" + dialog.FileName + "'");
        }

        /// <summary>
        /// Gets triggered when the "Load default" Menuitem has been clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadDefault_Click(object sender, RoutedEventArgs e)
        {
            LoadPresetIntervals();
        }

        /// <summary>
        /// Gets triggered when the "Customize" MenuItem has been clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomizeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenCustomizationMenu();
        }

        #endregion EventHandlerMethods

        #endregion Methods
    }
}