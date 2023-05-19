using CefSharp.DevTools.CSS;
using GameLauncher.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameLauncher.Controls
{
    /// <summary>
    /// Interaction logic for GameIconControl.xaml
    /// </summary>
    public partial class GameIconControl : UserControl
    {
        public GameInfo GameInfo { get; }

        public event EventHandler<GameControlEventArgs> GameControlSelected;

        public GameIconControl ()
        {
            InitializeComponent ();
        }

        public GameIconControl (ref GameInfo gameInfo)
        {
            InitializeComponent ();
            GameInfo = gameInfo;
        }

        public static readonly DependencyProperty ImageURLProperty = DependencyProperty.Register ("ImageURL", typeof (string), typeof (GameIconControl), new PropertyMetadata (null, OnImageURLPropertyChanged));

        public string ImageURL
        {
            get { return (string)GetValue (ImageURLProperty); }
            set { SetValue (ImageURLProperty, value); }
        }

        private static void OnImageURLPropertyChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GameIconControl control = d as GameIconControl;
            if (control != null)
            {
                string imageUrl = e.NewValue as string;
                Utility.LoadImage (imageUrl, control.Image_Game);
            }
        }

        public Visibility SelectedVisibility
        {
            get { return Selected.Visibility; }
            set { Selected.Visibility = value; }
        }

        // Propiedad pública para exponer la propiedad Background del Grid Root
        public Brush RootBackground
        {
            get { return Root.Background; }
            set { Root.Background = value; }
        }

        public bool? IsSelected
        {
            get { return (bool?)GetValue (IsSelectedProperty); }
            set { SetValue (IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register ("IsSelected", typeof (bool?), typeof (GameIconControl), new PropertyMetadata (null, OnIsSelectedChanged));

        private static void OnIsSelectedChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GameIconControl gameControl)
            {
                if (e.NewValue != null && (bool)e.NewValue)
                {
                    gameControl.SelectedVisibility = Visibility.Visible;
                    gameControl.RootBackground = new SolidColorBrush ((Color)ColorConverter.ConvertFromString ("#26FFFFFF"));
                    gameControl.OnGameControlSelected (new GameControlEventArgs (gameControl));
                }
                else
                {
                    gameControl.SelectedVisibility = Visibility.Collapsed;
                    gameControl.RootBackground = Brushes.Transparent;
                }
            }
        }

        private void GameControl_MouseEnter (object sender, MouseEventArgs e)
        {
            if ((bool)!IsSelected)
            {
                Root.Background = new SolidColorBrush (Color.FromArgb (70, 255, 255, 255));
            }
            else
            {
                Root.Background = new SolidColorBrush (Color.FromArgb (100, 255, 255, 255));
            }
        }

        private void GameControl_MouseLeave (object sender, MouseEventArgs e)
        {
            if ((bool)IsSelected)
            {
                Root.Background = (Brush)new BrushConverter ().ConvertFrom ("#26FFFFFF");
            }
            else
            {
                Root.Background = Brushes.Transparent;
            }
        }

        private void GameControl_PreviewMouseDown (object sender, MouseButtonEventArgs e)
        {
            if (!IsSelected.HasValue || !IsSelected.Value)
            {
                IsSelected = true;
            }

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.SelectGame (GameInfo);
        }

        protected virtual void OnGameControlSelected (GameControlEventArgs e)
        {
            EventHandler<GameControlEventArgs> handler = GameControlSelected;
            if (handler != null)
            {
                handler (this, e);
            }
        }
    }

    public class GameControlEventArgs : EventArgs
    {
        public GameIconControl GameControl { get; set; }

        public GameControlEventArgs (GameIconControl gameControl)
        {
            GameControl = gameControl;
        }
    }
}
