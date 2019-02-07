using System.Windows;
using System.Windows.Controls;

namespace Pomodoro
{
    /// <summary>
    /// Interaction logic for AddIntervalDialog.xaml
    /// </summary>
    public partial class AddIntervalDialog : Window
    {
        public AddIntervalDialog()
        {
            InitializeComponent();
            this.TypeComboBox.IsDropDownOpen = true;
        }

        private bool userSelectedAType = false;
        private IntervalType currentSelected;
        public Interval ReturnInterval = null;

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeComboBox.SelectedIndex == 1)
            {
                currentSelected = IntervalType.PAUSE;
            }
            else
            {
                currentSelected = IntervalType.WORK;
            }
            userSelectedAType = true;
        }

        private bool CheckTimeInputCorrect()
        {
            int result = 0;
            if (int.TryParse(hourBox.Text, out result) && int.TryParse(minuteBox.Text, out result) && int.TryParse(secondBox.Text, out result))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Fills up textboxes that were left empty with a 0
        /// </summary>
        private void FillEmptyInputBoxes()
        {
            //Hourbox
            if (hourBox.Text == "")
            {
                hourBox.Text = "0";
            }

            //Minutebox
            if (minuteBox.Text == "")
            {
                minuteBox.Text = "0";
            }

            //Secondbox
            if (secondBox.Text == "")
            {
                secondBox.Text = "0";
            }
        }

        private int GetTimeInput()
        {
            int hours = int.Parse(hourBox.Text);
            int minutes = int.Parse(minuteBox.Text);
            int seconds = int.Parse(secondBox.Text);

            minutes += hours * 60;
            seconds += minutes * 60;
            return seconds;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Fill up boxes that were left empty with a 0
            FillEmptyInputBoxes();

            // Check if the time input is in the correct format
            if (!CheckTimeInputCorrect())
            {
                MessageBox.Show("Time input is not in the correct format.");
                return;
            }

            //Check if user selected an Interval Type
            if (userSelectedAType == false)
            {
                MessageBox.Show("Select an Interval Type!");
                return;
            }

            ReturnInterval = new Interval(currentSelected, GetTimeInput());
            this.DialogResult = true;
            this.Close();
        }
    }
}