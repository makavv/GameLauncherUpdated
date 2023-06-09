using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GameLauncher.Controls
{
    /// <summary>
    /// Interaction logic for PasswordInputFieldControl.xaml
    /// </summary>
    public partial class PasswordInputFieldControl : UserControl
    {
        public bool IsPasswordVisible
        {
            get { return (bool)GetValue (IsPasswordVisibleProperty); }
            set { SetValue (IsPasswordVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsPasswordVisibleProperty =
            DependencyProperty.Register ("IsPasswordVisible", typeof (bool), typeof (PasswordInputFieldControl), new PropertyMetadata (false));

        public PasswordInputFieldControl ()
        {
            InitializeComponent ();
        }

        private void BtnShowPassword_Click (object sender, RoutedEventArgs e)
        {
            IsPasswordVisible = !IsPasswordVisible;

            var button = (Button)sender;
            var image = (Image)button.FindName ("btnShowPassword");

            if (IsPasswordVisible)
            {
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordTextBox.Focus ();
                PasswordTextBox.CaretIndex = PasswordTextBox.Text.Length;

                var uriSource = new Uri ("pack://application:,,,/Resources/Icons/Status/status_visible.png");
                var bitmapImage = new BitmapImage (uriSource);
                image.Source = bitmapImage;
                PasswordTextBox.Text = PasswordBox.Password;

            }
            else
            {
                PasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;

                PasswordBox.Focus ();
              
                var uriSource = new Uri ("pack://application:,,,/Resources/Icons/Status/status_hidden.png");
                var bitmapImage = new BitmapImage (uriSource);
                image.Source = bitmapImage;
                PasswordBox.Password = PasswordTextBox.Text;

            }
        }
    }
}
