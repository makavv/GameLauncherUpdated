using GameLauncher.Models;
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
using System.Windows.Shapes;
using GameLauncherCore;
using System.IO;

namespace GameLauncher.Views
{
    /// <summary>
    /// Interaction logic for InstallGame.xaml
    /// </summary>
    public partial class InstallGame : Window
    {

        public GameInfo gameInfo;

        public long downloadSize;

        public InstallGame (ref GameInfo GameInfo, GameLauncherPatcher patcher, string MAINAPP_VERSIONINFO_URL)
        {
            InitializeComponent ();

            gameInfo = GameInfo;

            Status_Image.Visibility = Visibility.Hidden;
            Status.Visibility = Visibility.Hidden;
            InstallButton.IsEnabled = false;
            checkBox_EnableAutomaticUpdates.IsChecked = gameInfo.AutomaticUpdates;

            UpdateInfo (patcher, MAINAPP_VERSIONINFO_URL);
        }

        public async void UpdateInfo (GameLauncherPatcher patcher, string MAINAPP_VERSIONINFO_URL)
        {


            Title.Text = gameInfo.Title;
            GameCover.Title = gameInfo.Title;
            GameCover.ImageURL = gameInfo.CoverURL;
            GameCover.Subtitle = gameInfo.Subtitle;

            // Try to install in the default location
            string gamePath = System.IO.Path.Combine (Directory.GetCurrentDirectory(), MainWindow.MAINAPP_SUBDIRECTORY, gameInfo.Title);
            LocationPath.Text = gamePath;

            gameInfo.InstallPath = gamePath;

            Required_Disk_Space.Text = "Calculating...";



            try
            {
                VersionInfo versionInfo = null;
                //MessageBox.Show ($"Trying to get the VersionInfo from URL: {MAINAPP_VERSIONINFO_URL}");
                var versionInfoXML = await Utility.DownloadTextFileAsync (MAINAPP_VERSIONINFO_URL);
                //MessageBox.Show ($"VersionInfoXML: {versionInfoXML}");

                //Console.WriteLine ($"Trying to get the VersionInfo from URL: {MAINAPP_VERSIONINFO_URL}");

                versionInfo = PatchUtils.DeserializeXMLToVersionInfo (versionInfoXML);

                // Get download size
                downloadSize = patcher.GetDownloadSize (versionInfo);
                Required_Disk_Space.Text = $"{Utility.FormatBytes (downloadSize)} required";
                HasRequiredDiskSpace ();

            }
            catch (Exception e)
            {
                Required_Disk_Space.Text = "Host error";
            }

        }

        public bool HasRequiredDiskSpace ()
        {
            Status_Image.Visibility = Visibility.Visible;
            Status.Visibility = Visibility.Visible;

            // Check if we have the required space in the current disk
            if (Utility.GetFreeDiskSpace (gameInfo.InstallPath) < downloadSize)
            {
                Status_Image.Source = new BitmapImage (new Uri ("pack://application:,,,/Resources/Icons/Status/status_error.png"));
                Status.Text = "Not enough disk space";
                InstallButton.IsEnabled = false;
                return false;
            }
            else
            {
                Status_Image.Source = new BitmapImage (new Uri ("pack://application:,,,/Resources/Icons/Status/status_completed.png"));
                Status.Text = "Available";
                InstallButton.IsEnabled = true;
                return true;

            }
        }

        private void CloseWindow (object sender, RoutedEventArgs e)
        {
            this.Close ();
        }

        private void InstallButton_MouseDown (object sender, RoutedEventArgs e)
        {
            if (!HasRequiredDiskSpace ())
            {
                MessageBox.Show ("No enought disk space");
                return;
            }

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            mainWindow.mainAppDirectory = gameInfo.InstallPath;

            // Get install in default location
            string defaultGamePath = System.IO.Path.Combine (Directory.GetCurrentDirectory(), MainWindow.MAINAPP_SUBDIRECTORY, mainWindow.SelectedGame.Title);

            // Check if a path, or default path is assigned
            if (string.IsNullOrEmpty (mainWindow.SelectedGame.InstallPath) || mainWindow.SelectedGame.InstallPath == defaultGamePath)
            {
                mainWindow.mainAppDirectory = System.IO.Path.Combine (mainWindow.launcherDirectory, MainWindow.MAINAPP_SUBDIRECTORY, mainWindow.SelectedGame.Title, mainWindow.CurrentSelectedEnvironment);
            }
            else
            {
                mainWindow.mainAppDirectory = mainWindow.SelectedGame.InstallPath;
            }

            // Save the install date
            mainWindow.SelectedGame.InstallDate = DateTime.Now;
            mainWindow.SaveGamesInfo ();

            mainWindow.StartMainAppPatch (false);

            this.Close ();
        }

        private void ChangeLocationButton_MouseDown (object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog ();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog ();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string gamePath = dialog.SelectedPath;

                if (!gamePath.EndsWith (gameInfo.Title))
                {
                    gamePath = System.IO.Path.Combine (dialog.SelectedPath, gameInfo.Title);
                }

                LocationPath.Text = gamePath;

                gameInfo.InstallPath = gamePath;
            }

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.SaveGamesInfo ();
        }
        private void checkBox_EnableAutomaticUpdates_Click (object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox.IsChecked == true)
            {
                gameInfo.AutomaticUpdates = true;
            }
            else
            {
                gameInfo.AutomaticUpdates = false;
            }

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.SaveGamesInfo ();

        }

        private void checkBox_CreateDesktopIcon_Click (object sender, RoutedEventArgs e)
        {

        }
    }
}
