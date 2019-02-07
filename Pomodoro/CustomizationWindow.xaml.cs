using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Pomodoro
{
    /// <summary>
    /// Interaction logic for CustomizationWindow.xaml
    /// </summary>
    public partial class CustomizationWindow : Window
    {
        #region ReturnValues
        #region HolderValues
        /// <summary>
        /// Holder value for the BackgroundFilePath property
        /// </summary>
        private string _BackgroundFilePath = string.Empty;
        #endregion

        /// <summary>
        /// Set: Changes the background to the specified image; Get: returns the last path given
        /// </summary>
        public string BackgroundImagePath
        {
            get
            {
                return _BackgroundFilePath;
            }
            set
            {
                _BackgroundFilePath = value;
                if (value == null)
                    return;

                BackgroundImageBrush.ImageSource = new BitmapImage(new Uri(value));
            }
        } 
        #endregion


        public CustomizationWindow(MainWindow sourceWindow)
        {
            //Retrieve customization data
            this.BackgroundImagePath = sourceWindow.BackgroundImagePath;

            InitializeComponent();
        }

        private void ImageSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Title = "Select a background image";
            openFileDialog.ShowDialog();
            BackgroundImagePath = openFileDialog.FileName;
        }
    }
}
