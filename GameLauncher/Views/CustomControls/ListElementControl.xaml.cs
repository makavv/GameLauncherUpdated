using GameLauncher.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

public enum Category
{
    All,
    FreeToPlay,
    Multiplayer,
    MacOS,
    Mobile,
    Installed,
    Favorites
}

namespace GameLauncher.Controls
{
    /// <summary>
    /// Interaction logic for ListElementControl.xaml
    /// </summary>
    public partial class ListElementControl : UserControl
    {

        public event EventHandler<DependencyPropertyChangedEventArgs> IsSelectedChanged;


        public ListElementControl ()
        {
            InitializeComponent ();
        }

        public ListElementControl (Category category)
        {
            InitializeComponent ();
            Category = category;
        }

        public static readonly DependencyProperty CategoryProperty =
            DependencyProperty.Register ("Category", typeof (Category), typeof (ListElementControl), new PropertyMetadata (Category.All));

        public Category Category
        {
            get { return (Category)GetValue (CategoryProperty); }
            set { SetValue (CategoryProperty, value); }
        }

        public string LabelContent
        {
            get { return (string)GetValue (LabelContentProperty); }
            set { SetValue (LabelContentProperty, value); }
        }

        public static readonly DependencyProperty LabelContentProperty =
            DependencyProperty.Register ("LabelContent", typeof (string), typeof (ListElementControl), new PropertyMetadata (null, OnLabelContentPropertyChanged));

        private static void OnLabelContentPropertyChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListElementControl control && e.NewValue is string labelContent)
            {
                control.Label.Content = labelContent;
            }
        }

        public string Count
        {
            get { return (string)GetValue (CountProperty); }
            set { SetValue (CountProperty, value); }
        }

        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register ("Count", typeof (string), typeof (ListElementControl), new PropertyMetadata (null, OnCountPropertyChanged));

        private static void OnCountPropertyChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListElementControl control && e.NewValue is string labelContent)
            {
                control.CountText.Content = labelContent;
            }
        }

        public bool ShowCountBackground
        {
            get { return (bool)GetValue (ShowCountBackgroundProperty); }
            set { SetValue (ShowCountBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ShowCountBackgroundProperty =
            DependencyProperty.Register ("ShowCountBackground", typeof (bool), typeof (ListElementControl), new PropertyMetadata (false, OnShowCountBackgroundPropertyChanged));

        private static void OnShowCountBackgroundPropertyChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListElementControl control && e.NewValue is bool showCountBackground)
            {
                control.CountBackground.Visibility = showCountBackground ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool? IsSelected
        {
            get { return (bool?)GetValue (IsSelectedProperty); }
            set { SetValue (IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register ("IsSelected", typeof (bool?), typeof (ListElementControl), new PropertyMetadata (null, OnIsSelectedPropertyChanged));

        private static void OnIsSelectedPropertyChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListElementControl listElementControl)
            {
                bool? isSelected = (bool?)e.NewValue;
                if (isSelected.HasValue)
                {
                    if (isSelected.Value)
                    {
                        listElementControl.Selected.Visibility = Visibility.Visible;
                        listElementControl.List_Element.Background = (Brush)new BrushConverter ().ConvertFrom ("#26FFFFFF");
                    }
                    else
                    {
                        listElementControl.Selected.Visibility = Visibility.Collapsed;
                        listElementControl.List_Element.Background = Brushes.Transparent;
                    }

                    listElementControl.IsSelectedChanged?.Invoke (listElementControl, e);
                }
                else
                {
                    listElementControl.Selected.Visibility = Visibility.Collapsed;
                    listElementControl.List_Element.Background = Brushes.Transparent;
                }
            }
        }

        private void List_Element_MouseEnter (object sender, MouseEventArgs e)
        {
            if ((bool)!IsSelected)
            {
                List_Element.Background = new SolidColorBrush (Color.FromArgb (70, 255, 255, 255));
            }

            Label.Foreground = new SolidColorBrush (Colors.Black);
            CountText.Foreground = new SolidColorBrush (Colors.Cyan);

        }

        private void List_Element_MouseLeave (object sender, MouseEventArgs e)
        {
            if ((bool)IsSelected)
            {
                List_Element.Background = (Brush)new BrushConverter ().ConvertFrom ("#26FFFFFF");
                Label.Foreground = new SolidColorBrush (Colors.White);
            }
            else
            {
                List_Element.Background = Brushes.Transparent;
                Label.Foreground = new SolidColorBrush (Colors.White);
            }

            CountText.Foreground = new SolidColorBrush (Colors.White);
        }

        private void List_Element_PreviewMouseDown (object sender, MouseButtonEventArgs e)
        {
            // Desmarcar todos los demás elementos de lista
            var parent = this.Parent as Panel;
            if (parent != null)
            {
                foreach (var child in parent.Children)
                {
                    if (child is ListElementControl listElementControl && listElementControl != this)
                    {
                        listElementControl.IsSelected = false;
                    }
                }
            }

            // Seleccionar este elemento
            IsSelected = true;

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.SelectCategory (Category);
        }
 
    }
}