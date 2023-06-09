using GameLauncher.Models;
using GameLauncher.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLauncher
{
    public partial class App
    {
        public bool logged;
        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!AreSettingsSet())
            {
                this.MainWindow = new SignInWindow();
                this.MainWindow.ShowDialog(); // Waits until closed.

                // Recheck the settings now that the login screen has been closed.
                if (!AreSettingsSet())
                {
                    // Tell the user there is a problem and quit.
                    this.Shutdown();
                }
            }

            var mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.Activate();
            mainWindow.Focus();
        }

        private bool AreSettingsSet()
        {
            return SettingsManager.Settings.Logged;
        }
    }
}
