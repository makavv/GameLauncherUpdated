using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameLauncher.Controls
{
    public partial class SearchBarControl : UserControl
    {
        public SearchBarControl ()
        {
            InitializeComponent ();
        }

        public void RefreshLanguage ()
        {
            Placeholder.Content = GameLauncherCore.Localization.Get (GameLauncherCore.LocalizationID.SearchBar_Placeholder);
        }

        private void SearchField_TextChanged (object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty (SearchField.Text))
            {
                Placeholder.Visibility = Visibility.Collapsed;
                ClearIcon.Visibility = Visibility.Visible;
            }
            else
            {
                Placeholder.Visibility = Visibility.Visible;
                ClearIcon.Visibility = Visibility.Collapsed;
            }

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            
            if (mainWindow != null)
            {
                mainWindow.SearchFieldTextChanged (SearchField.Text);
            }


        }

        private void SearchField_GotFocus (object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty (SearchField.Text))
            {
                Placeholder.Visibility = Visibility.Collapsed;
            }
        }

        private void SearchField_LostFocus (object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty (SearchField.Text))
            {
                Placeholder.Visibility = Visibility.Visible;
            }
        }

        private void ClearIcon_MouseDown (object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SearchField.Clear ();
            Placeholder.Visibility = Visibility.Visible;
            ClearIcon.Visibility = Visibility.Collapsed;

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.SearchFieldTextChanged ();
            }
        }
    }
}
