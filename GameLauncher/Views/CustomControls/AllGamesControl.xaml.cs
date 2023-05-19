using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GameLauncher.Controls
{
    /// <summary>
    /// Interaction logic for AllGamesControl.xaml
    /// </summary>
    public partial class AllGamesControl : UserControl
    {
        public AllGamesControl ()
        {
            InitializeComponent ();
            IsSelected = false;

            lbl_ALLGAMES.Text = GameLauncherCore.Localization.Get (GameLauncherCore.LocalizationID.MainUI_Library_Topside_AllGames);
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

        public bool IsSelected
        {
            get { return (bool)GetValue (IsSelectedProperty); }
            set { SetValue (IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register ("IsSelected", typeof (bool?), typeof (AllGamesControl), new PropertyMetadata (null, OnIsSelectedChanged));

        private static void OnIsSelectedChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AllGamesControl gameControl)
            {
                if (e.NewValue != null && (bool)e.NewValue)
                {
                    gameControl.SelectedVisibility = Visibility.Visible;
                    gameControl.RootBackground = new SolidColorBrush ((Color)ColorConverter.ConvertFromString ("#26FFFFFF"));
                }
                else
                {
                    gameControl.SelectedVisibility = Visibility.Collapsed;
                    gameControl.RootBackground = Brushes.Transparent;
                }
            }
        }

        private void AllGamesControl_MouseEnter (object sender, MouseEventArgs e)
        {
            if (!IsSelected)
            {
                Root.Background = new SolidColorBrush (Color.FromArgb (70, 255, 255, 255));
            }
            else
            {
                Root.Background = new SolidColorBrush (Color.FromArgb (100, 255, 255, 255));
            }

        }

        private void AllGamesControl_MouseLeave (object sender, MouseEventArgs e)
        {
            if (IsSelected)
            {
                Root.Background = (Brush)new BrushConverter ().ConvertFrom ("#26FFFFFF");
            }
            else
            {
                Root.Background = Brushes.Transparent;
            }
        }

        private void AllGamesControl_PreviewMouseDown (object sender, MouseButtonEventArgs e)
        {
            IsSelected = true;
        }
    }
}