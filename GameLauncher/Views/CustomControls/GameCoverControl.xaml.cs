using GameLauncher.Models;
using System.Net;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.IO;

namespace GameLauncher.Controls
{
    /// <summary>
    /// Interaction logic for GameCoverControl.xaml
    /// </summary>
    public partial class GameCoverControl : UserControl
    {

        public GameInfo GameInfo { get; }

        public GameCoverControl ()
        {
            InitializeComponent ();
            IsInstalled = false;

            FavoriteButtonBorder.Visibility = Visibility.Collapsed;
            InstalledText.Text = GameLauncherCore.Localization.Get (GameLauncherCore.LocalizationID.GameSettings_Installed);
        }

        public GameCoverControl (ref GameInfo gameInfo)
        {
            InitializeComponent ();
            GameInfo = gameInfo;
            IsInstalled = false;
            InstalledText.Text = GameLauncherCore.Localization.Get (GameLauncherCore.LocalizationID.GameSettings_Installed);

            if (GameInfo.IsFavorite)
            {
                FavoriteButtonImage.Source = new BitmapImage (new Uri ("pack://application:,,,/Resources/Icons/Status/status_favorite.png"));
            }
            else
            {
                FavoriteButtonImage.Source = new BitmapImage (new Uri ("pack://application:,,,/Resources/Icons/Status/status_favorite_unchecked.png"));
            }
        }



        public string ImageURL
        {
            get { return (string)GetValue (ImageURLProperty); }
            set { SetValue (ImageURLProperty, value); }
        }

        public static readonly DependencyProperty ImageURLProperty = DependencyProperty.Register ("ImageURL", typeof (string), typeof (GameCoverControl), new PropertyMetadata (null, OnImageURLPropertyChanged));


        private static void OnImageURLPropertyChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GameCoverControl control = d as GameCoverControl;
            if (control != null)
            {
                string imageUrl = e.NewValue as string;
                Utility.LoadImage (imageUrl, control.Background);
            }
        }

        public string Title
        {
            get { return (string)GetValue (TitleProperty); }
            set { SetValue (TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register ("Title", typeof (string), typeof (GameCoverControl), new PropertyMetadata (null, OnTitlePropertyChanged));

        private static void OnTitlePropertyChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GameCoverControl control && e.NewValue is string title)
            {
                control.GameTitle.Text = title;
            }
        }

        public string Subtitle
        {
            get { return (string)GetValue (SubtitleProperty); }
            set { SetValue (SubtitleProperty, value); }
        }

        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register ("Subtitle", typeof (string), typeof (GameCoverControl), new PropertyMetadata (null, OnSubtitlePropertyChanged));

        private static void OnSubtitlePropertyChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GameCoverControl control && e.NewValue is string subtitle)
            {
                control.GameSubtitle.Text = subtitle;
            }
        }

        public bool IsInstalled
        {
            get { return (bool)GetValue (IsInstalledProperty); }
            set { SetValue (IsInstalledProperty, value); }
        }

        public static readonly DependencyProperty IsInstalledProperty =
            DependencyProperty.Register("IsInstalled", typeof(bool?), typeof(GameCoverControl), new PropertyMetadata(null, OnIsInstalledPropertyChanged));

        private static void OnIsInstalledPropertyChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GameCoverControl control)
            {
                if (e.NewValue != null && (bool)e.NewValue)
                {
                    control.GameImageInstalled.Visibility = Visibility.Visible;
                    control.InstalledText.Visibility = Visibility.Visible;
                    control.GameSubtitle.Visibility = Visibility.Collapsed;
                }
                else
                {
                    control.GameImageInstalled.Visibility = Visibility.Collapsed;
                    control.InstalledText.Visibility = Visibility.Collapsed;
                    control.GameSubtitle.Visibility = Visibility.Visible;
                }
            }
        }

        private void Background_MouseEnter (object sender, MouseEventArgs e)
        {
            Storyboard scaleAnimation = (Storyboard)FindResource ("ScaleAnimation");
            scaleAnimation.Begin (Content);

        }

        private void Background_MouseLeave (object sender, MouseEventArgs e)
        {
            Storyboard resetScaleAnimation = (Storyboard)FindResource ("ResetScaleAnimation");
            resetScaleAnimation.Begin (Content);
        }

        private void FavoriteButton_Click (object sender, RoutedEventArgs e)
        {

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            GameInfo.IsFavorite = !GameInfo.IsFavorite;

            if (GameInfo.IsFavorite)
            {
                FavoriteButtonImage.Source = new BitmapImage (new Uri ("pack://application:,,,/Resources/Icons/Status/status_favorite.png"));
                mainWindow.AddTopSideIcon (GameInfo);
            } else
            {
                FavoriteButtonImage.Source = new BitmapImage (new Uri ("pack://application:,,,/Resources/Icons/Status/status_favorite_unchecked.png"));
                mainWindow.RemoveTopSideIcon (GameInfo);
            }

            mainWindow.UpdateCategoryListsCount ();
            mainWindow.SaveGamesInfo ();
        }

        private void Background_PreviewMouseDown (object sender, MouseButtonEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.SelectGame (GameInfo);
        }
    }
}
