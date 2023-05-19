using GameLauncher.Models;
using System.Windows;
using System.Windows.Controls;

namespace GameLauncher.Controls
{
    /// <summary>
    /// Interaction logic for NewsSlideshowControl.xaml
    /// </summary>
    public partial class NewsSlideshowControl : UserControl
    {
        public NewsSlideshowControl ()
        {
            InitializeComponent ();
        }

        private void News_Button_Click (object sender, RoutedEventArgs e)
        {

            // Get the current news item
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            // Open URL
            Utility.OpenURL (mainWindow.NEWS_CURRENT_URL);
        }
    }
}
