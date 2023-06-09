using System;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Net.Http;
using GameLauncher.ViewsModels;
using GameLauncher.Models;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Linq;
using System.Globalization;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameLauncher.Views
{
    /// <summary>
    /// Interaction logic for SignInWindow
    /// </summary>
    public partial class SignInWindow : Window
    {
        // API HOST URL MUST END WITH '/'
        public string HOST_URL = "https://7q8.ru/API/";

        private readonly AuthenticationService _authenticationService;
        private const string _encryptionKey = "my-secret-key-12"; // 16 characters = 128 bits

        public SignInWindow()
        {
            InitializeComponent();
            _authenticationService = new AuthenticationService(new HttpClient(), HOST_URL);

            RegisterDevice();
            // Navigate to login tab
            LoginTab();
            // Load user credentials from settings
            LoadUserCredentials();
        }
        private void RegisterDevice()
        {

            OnRegisterDevice();

        }
        private void LoadUserCredentials()
        {
            var email = SettingsManager.Settings.Email;
            InputLogin_Email.Text = email;

            if (!string.IsNullOrEmpty(SettingsManager.Settings.Email) &&
                !string.IsNullOrEmpty(SettingsManager.Settings.Password) && SettingsManager.Settings.AutoLogin)
            {
                // Decrypt password and set email and password fields in login window

                var password = DecryptPassword(SettingsManager.Settings.Password);

                // Check if the decrypted password is valid
                if (!string.IsNullOrEmpty(password))
                {
                    InputLogin_Password.PasswordBox.Password = password;
                    if (Utility.CheckForInternetConnection())
                        OnLogin();
                    else
                        OnLoginSuccess(false);
                }
                else
                {
                    // Prompt the user to login manually
                    MessageBox.Show("Could not automatically log in. Please enter your email and password manually.");
                }
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        #region TABS
        private void LoginTab(object sender = null, MouseButtonEventArgs e = null)
        {
            tabControl.SelectedIndex = 0; // Navigate to login


            lbl_ForgotPassword.Visibility = Visibility.Visible;
        }

        private void RegisterTab(object sender, MouseButtonEventArgs e)
        {
            tabControl.SelectedIndex = 1; // Navigate to register

            lbl_ForgotPassword.Visibility = Visibility.Visible;
        }

        private void ForgotPasswordTab(object sender = null, MouseButtonEventArgs e = null)
        {
            tabControl.SelectedIndex = 2; // Navigate to forgot password
            lbl_SendCodeStatus.Text = "Ready to sent";


            lbl_ForgotPassword.Visibility = Visibility.Visible;
        }

        private void ChangePasswordTab(object sender = null, MouseButtonEventArgs e = null)
        {
            tabControl.SelectedIndex = 3; // Navigate to change password


            lbl_ForgotPassword.Visibility = Visibility.Visible;
        }

        #endregion

        #region LOGIN BUTTONS

        /// <summary>
        /// Handles the Google sign-in button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GoogleSignInButton_Click(object sender, RoutedEventArgs e)
        {
            /*   try
              {
                //  var googleLogin = new GoogleLogin ();
                  var credential = await googleLogin.SignInAsync ();

                  // Checks if sign-in was successful and gets user profile.
                  if (credential != null && !string.IsNullOrEmpty (credential.UserId))
                  {
                      // Sign-in was successful, and you can access the user profile information.
                      var userInfo = await googleLogin.GetUserInfoAsync (credential);
                      var email = userInfo.Email ?? "Email unknown";

                      // Checks if user already exists in the database.
                      if (!await _authenticationService.UserExists (email))
                      {
                          // User does not exist in the database, create a new record.
                          if (await _authenticationService.RegisterUser (email, null, true))
                          {
                              OnLoginSuccess ();
                          }
                      }
                      else
                      {
                          // User already exists in the database, log in.
                          if (await _authenticationService.ValidateLoginAsync (email, null, true))
                          {
                              OnLoginSuccess ();
                          }
                          else
                          {
                              OnLoginError ("Error on Google login");
                          }
                      }
                  }
                  else
                  {
                      OnLoginError ("Google Login: CREDENTIAL ERROR");
                  }
              }
              catch (Exception ex)
              {
                  OnLoginError ($"Google Login: {ex.Message}");
              }*/
        }

        #endregion

        #region LOGIN

        /// <summary>
        /// On Login Action. Call to server
        /// </summary>
        public async void OnLogin(object sender = null, EventArgs e = null)
        {
            if (!Utility.CheckForInternetConnection())
            {
                MessageBox.Show("Подключение к интернету отсутствует!");
                return;
            }
            LoginButton.IsEnabled = false;

            // Check if email and password are valid
            if (!IsValidEmail(InputLogin_Email.Text))
            {
                MessageBox.Show("Please enter a valid email address");
                LoginButton.IsEnabled = true;
                return;
            }

            if (!IsValidPassword(InputLogin_Password.PasswordBox.Password))
            {
                MessageBox.Show("You password must contain at least 6 characters");
                LoginButton.IsEnabled = true;
                return;
            }

            // Validate Login
            try
            {
                bool isValidLogin = await _authenticationService.ValidateLoginAsync(InputLogin_Email.Text, InputLogin_Password.PasswordBox.Password);

                if (isValidLogin)
                {
                    OnLoginSuccess();
                }
            }
            catch (Exception ex)
            {
                OnLoginError(ex.Message);
            }

            LoginButton.IsEnabled = true;

        }

        /// <summary>
        /// On Login Success action.
        /// </summary>
        public void OnLoginSuccess(bool online = true)
        {
            if (online)
            {
                // Save the user email and password (encrypted) in the local storage
                // Add these [Email, Password, AutoLogin] variables if your Settings doesn't have them
                SettingsManager.Settings.Email = InputLogin_Email.Text;
                SettingsManager.Settings.Password = EncryptPassword(InputLogin_Password.PasswordBox.Password);
                SettingsManager.Settings.AutoLogin = checkBox_KeepMeLoggedIn.IsChecked.Value;

                SettingsManager.SaveSettings();


            }
            // Show Main Window
            SettingsManager.Settings.Logged = true;

            // Close current window
            this.Close();
        }

        /// <summary>
        /// On Login Error action.
        /// </summary>
        public void OnLoginError(string error)
        {
            // Focus current window
            this.Show();
            this.Activate();
            this.Focus();

            SettingsManager.Settings.AutoLogin = false;
            SettingsManager.SaveSettings();

            // Show login error message
            MessageBox.Show(error);
        }

        #endregion

        #region REGISTER


        /// <summary>
        /// On Register Action. Call to server
        /// </summary>
        public async void OnRegisterDevice(object sender = null, EventArgs e = null)
        {
            // RegisterButton.IsEnabled = false;
            if (!Utility.CheckForInternetConnection())
            {
                // MessageBox.Show("Подключение к интернету отсутствует!");
                return;
            }
            LoginButton.IsEnabled = false;

            try
            {
                if (!await _authenticationService.DeviceExists(Utility.GetDeviceUniqueIdentifier()))
                {
                    // User does not exist in the database, create a new record.
                    if (await _authenticationService.RegisterDevice())
                    {
                        OnRegisterDeviceSuccess();
                    }
                }
                // Send registration data to server
                // bool isSuccess = await _authenticationService.RegisterDevice ();

            }
            catch (Exception ex)
            {
                OnRegisterError($"Register error: {ex.Message}");
            }

            LoginButton.IsEnabled = true;


        }

        /// <summary>
        /// On Register Success action.
        /// </summary>
        public void OnRegisterDeviceSuccess()
        {
            // Auto-login after registration
            LoadUserCredentials();
        }

        /// <summary>
        /// On Register Error action.
        /// </summary>
        public void OnRegisterError(string error)
        {
            MessageBox.Show($"Registration failed: {error}");
        }

        /// <summary>
        /// Validate user input before registration
        /// </summary>
        /*private bool ValidateInputsRegister ()
        {

            if (InputRegister_Email.Text == "")
            {
                MessageBox.Show ("Please enter your email address");
                return false;
            }

            if (!IsValidEmail (InputRegister_Email.Text))
            {
                MessageBox.Show ("Please enter a valid email address");
                return false;
            }

            if (!IsValidPassword (InputRegister_Password.PasswordBox.Password))
            {
                MessageBox.Show ("You password must contain at least 6 characters");
                return false;
            }

            if (!IsValidPassword (InputRegister_ConfirmPassword.PasswordBox.Password))
            {
                MessageBox.Show ("Please confirm your password");
                return false;
            }

            if (InputRegister_Password.PasswordBox.Password != InputRegister_ConfirmPassword.PasswordBox.Password)
            {
                MessageBox.Show ("Passwords do not match");
                return false;
            }

            if (checkBox_AcceptTermsAndConditions.IsChecked == false)
            {
                MessageBox.Show ("You need to accept the terms and conditions to continue");
                return false;
            }

            return true;
        }*/

        /// <summary>
        /// Check if an email address is valid
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check if the password is valid
        /// </summary>
        private bool IsValidPassword(string password)
        {
            if (password.Length < 6)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region RECOVER PASSWORD

        /// <summary>
        /// On Recover Password Action. Call to server and check if the code is valid
        /// </summary>
        public async void OnRecoverPassword(object sender = null, EventArgs e = null)
        {
            try
            {
                // Get email and recovery code from UI
                var email = InputForgotPassword_Email.Text.Trim();
                var recoveryCode = InputForgotPassword_Code.Text.Trim();

                // Call server to check if recovery code is valid
                var isValid = await _authenticationService.RecoveryCodeExists(email, recoveryCode);

                if (isValid)
                {
                    // Recovery code is valid, navigate to Reset Password page
                    OnRecoverPasswordSuccess();
                }
            }
            catch (Exception ex)
            {
                OnRecoverPasswordError($"Recover password failed: {ex.Message}");
            }
        }

        /// <summary>
        /// On Recover Password Success action.
        /// </summary>
        public void OnRecoverPasswordSuccess()
        {
            ChangePasswordTab();
        }

        /// <summary>
        /// On Recover Password Error action.
        /// </summary>
        public void OnRecoverPasswordError(string error)
        {
            // Recover Password error
            MessageBox.Show(error);
        }

        private async void SendCodeButton_Click(object sender, RoutedEventArgs e)
        {

            // Check email is valid
            if (!IsValidEmail(InputForgotPassword_Email.Text))
            {
                MessageBox.Show("Please enter a valid email address");
                return;
            }

            // Send recovery code to email
            _authenticationService.SendRecoveryCode(InputForgotPassword_Email.Text);

            // Disable button for 60 seconds
            int remainingSeconds = 60;
            SendCodeButton.IsEnabled = false;

            while (remainingSeconds > 0)
            {
                lbl_SendCodeStatus.Text = $"Code sent. Retry in {remainingSeconds--} seconds";
                await Task.Delay(1000);
            }

            lbl_SendCodeStatus.Text = "";
            SendCodeButton.IsEnabled = true;
        }


        #endregion


        #region CHANGE PASSWORD
        /// <summary>
        /// On Change Password Action. Send the new password to server, then login
        /// </summary>
        private async void OnChangePassword(object sender = null, RoutedEventArgs e = null)
        {
            try
            {
                // Get the new password from the input fields
                string newPassword = InputChange_Password.PasswordTextBox.Text;
                string confirmPassword = InputChange_ConfirmPassword.PasswordTextBox.Text;

                // Password checks
                if (!IsValidPassword(newPassword))
                {
                    // The new password must contain at least 6 characters
                    OnChangePasswordError("The new password must contain at least 6 characters.");
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    // The new password and confirmation password do not match
                    OnChangePasswordError("The new password and confirmation password do not match.");
                    return;
                }

                // Get the email of the user associated with the recovery code
                string email = InputForgotPassword_Email.Text;

                if (string.IsNullOrEmpty(email))
                {
                    // An error occurred while retrieving the email
                    OnChangePasswordError("An error occurred while retrieving the email associated");
                    return;
                }

                // Change the user's password in the server
                if (await _authenticationService.ChangePasswordAsync(email, newPassword))
                {
                    // The password was changed successfully
                    OnChangePasswordSuccess();
                }
            }
            catch (Exception ex)
            {
                // An error occurred
                OnChangePasswordError($"An error occurred: {ex.Message}");
            }
        }

        private void OnChangePasswordSuccess()
        {
            // Password changed successfully
            OnLoginSuccess();
        }

        private void OnChangePasswordError(string errorMessage)
        {
            // Display the error message to the user
            MessageBox.Show(errorMessage);
        }

        #endregion


        #region PASSWORD ENCRYPTION

        // Encrypt the given plaintext using the Base64 algorithm
        public static string EncryptPassword(string plaintext)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plaintext);
            return Convert.ToBase64String(plainTextBytes);
        }

        // Decrypt the given Base64-encoded ciphertext
        public static string DecryptPassword(string ciphertext)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(ciphertext);
            return Encoding.UTF8.GetString(cipherTextBytes);
        }

        #endregion
       



        private void Window_Closing(object sender, CancelEventArgs e)
        {

        }
    }
}
