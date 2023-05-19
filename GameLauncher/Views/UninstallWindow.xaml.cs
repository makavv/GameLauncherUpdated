using GameLauncher.Models;
using GameLauncherCore;
using System;
using System.Windows;
using System.Windows.Xps.Packaging;

namespace GameLauncher.Views
{
    /// <summary>
    /// Interaction logic for UninstallWindow.xaml
    /// </summary>
    public partial class UninstallWindow : Window
    {
        public bool Confirmed { get; private set; } = false;

        public GameInfo gameInfo { get; private set; }

        public UninstallWindow (ref GameInfo gameInfo)
        {
            InitializeComponent ();

            this.gameInfo = gameInfo;

            Utility.LoadImage (gameInfo.IconURL, AppIcon);

            RefreshLanguage ();
        }

        public void RefreshLanguage ()
        {
            UninstallText.Text = GameLauncherCore.Localization.Get (GameLauncherCore.LocalizationID.Uninstall_UninstallText);
            UninstallText.Text = UninstallText.Text.Replace ("{0}", gameInfo.Title);
            
            ButtonYes.Content = GameLauncherCore.Localization.Get (GameLauncherCore.LocalizationID.Uninstall_Yes);
            ButtonNo.Content = GameLauncherCore.Localization.Get (GameLauncherCore.LocalizationID.Uninstall_No);
        }

        private void YesButton_Click (object sender, RoutedEventArgs e)
        {
            Confirmed = true;
            Close ();
        }

        private void NoButton_Click (object sender, RoutedEventArgs e)
        {
            Close ();
        }
    }
}
