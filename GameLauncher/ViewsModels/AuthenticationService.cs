using GameLauncher.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.ViewsModels
{
    // AuthenticationService.cs
    public class AuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _hostUrl;

        // PHP files
        public string LoginFilename = "login.php";
        public string RegisterFilename = "register.php";
        public string SendRecoveryCodeFilename = "password_recovery.php";
        public string CheckUserExistsFilename = "check_user_exists.php";
        public string CheckRecoveryCodeExistsFilename = "check_recovery_code.php";
        public string ChangePasswordFilename = "change_password.php";
        public string RegisterDeviceFilename = "register_device.php";
        public string DeviceExistsFilename = "device_exists.php";

        public AuthenticationService (HttpClient httpClient, string hostUrl)
        {
            _httpClient = httpClient;
            _hostUrl = hostUrl;
        }

        public async Task<bool> ValidateLoginAsync (string email, string password, bool googleLogin = false)
        {
            var host = new Uri (_hostUrl + LoginFilename);

            var values = new Dictionary<string, string>
            {
                { "email", email },
                { "password", password },
                { "google_login", googleLogin.ToString() }
            };

            var content = new FormUrlEncodedContent (values);
            var response = await _httpClient.PostAsync (host, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync ();
                Debug.WriteLine (responseContent);

                if (responseContent == "LOGIN SUCCESS")
                {
                    return true;
                }
                else
                {
                    throw new InvalidOperationException (responseContent);
                }
            }
            else
            {
                throw new InvalidOperationException ($"Cannot locate the host: {host}");
            }
        }

        /// <summary>
        /// Check if user exists in database
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> DeviceExists (string hardwareId)
        {
            var host = new Uri (_hostUrl + DeviceExistsFilename);

            using (var client = new HttpClient ())
            {
                var values = new Dictionary<string, string>
                {
                    { "hardwareId", Utility.GetDeviceUniqueIdentifier() }
                };

                var content = new FormUrlEncodedContent (values);
                var response = await client.PostAsync (host, content);

                if (response.IsSuccessStatusCode)
                {
                    // Get the response content as a string
                    var responseContent = await response.Content.ReadAsStringAsync ();

                    Debug.WriteLine (responseContent);

                    // Deserialize the response JSON string into a dictionary
                    var responseObject = JsonConvert.DeserializeObject<Dictionary<string, bool>> (responseContent);


                    // Return the "exists" value from the dictionary
                    return responseObject["exists"];
                }
                else
                {
                    // Throw an exception if the server request fails
                    throw new InvalidOperationException ($"Cannot locate the host: {host}");
                }
            }
        }



        /// <summary>
        /// Send user registration data to server
        /// </summary>
        public async Task<bool> RegisterDevice()
        {
            var HOST = new Uri(_hostUrl + RegisterDeviceFilename);

            using (HttpClient client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "deviceName", Environment.MachineName },
                    { "hardwareId", Utility.GetDeviceUniqueIdentifier() },
                    { "operSystem", Utility.GetOperSystem() },
                    { "memSize", Utility.GetMemSize() },
                    { "graphicDeviceName", Utility.GetGraphicDeviceName()},
                    { "graphicMemSize",Utility.GetGraphicMemSize() },
                    { "processorName", Utility.GetProcessorName() }
                };

                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync(HOST, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(responseContent);

                    if (responseContent == "SUCCESS")
                    {
                        return true;
                    }
                    else
                    {
                        // Registration failed
                        throw new InvalidOperationException(responseContent);
                    }
                }
                else
                {
                    // Host error
                    throw new InvalidOperationException($"Cannot locate the host: {HOST}");
                }
            }
        }

        /// <summary>
        /// Send user registration data to server
        /// </summary>
        public async Task<bool> RegisterUser (string email, string password, bool googleLogin = false)
        {
            var HOST = new Uri (_hostUrl + RegisterFilename);

            using (HttpClient client = new HttpClient ())
            {
                var values = new Dictionary<string, string>
                {
                    { "email", email },
                    { "password", password },
                    { "google_login", googleLogin.ToString() }
                };

                var content = new FormUrlEncodedContent (values);
                var response = await client.PostAsync (HOST, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync ();
                    Debug.WriteLine (responseContent);

                    if (responseContent == "REGISTRATION SUCCESS")
                    {
                        return true;
                    }
                    else
                    {
                        // Registration failed
                        throw new InvalidOperationException (responseContent);
                    }
                }
                else
                {
                    // Host error
                    throw new InvalidOperationException ($"Cannot locate the host: {HOST}");
                }
            }
        }

        public async Task<bool> SendRecoveryCode (string email)
        {
            var host = new Uri (_hostUrl + SendRecoveryCodeFilename);

            using (var client = new HttpClient ())
            {
                var values = new Dictionary<string, string>
                {
                    { "email", email }
                };

                var content = new FormUrlEncodedContent (values);
                var response = await client.PostAsync (host, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync ();
                    Debug.WriteLine (responseContent);
                    return true;
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync ();
                    throw new Exception ($"Error: {response.StatusCode}. {errorMessage}");
                }
            }
        }

        public async Task<bool> RecoveryCodeExists (string email, string recoveryCode)
        {
            var host = new Uri (_hostUrl + CheckRecoveryCodeExistsFilename);

            using (var client = new HttpClient ())
            {
                var values = new Dictionary<string, string>
                {
                    { "email", email },
                    { "recovery_code", recoveryCode }
                };

                var content = new FormUrlEncodedContent (values);
                var response = await client.PostAsync (host, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync ();
                    var responseObject = JsonConvert.DeserializeObject<Dictionary<string, bool>> (responseContent);

                    Debug.WriteLine (responseContent);
                    return responseObject["exists"];
                }
                else
                {
                    Debug.WriteLine ($"Cannot locate the host: {host}");
                    throw new InvalidOperationException ($"Cannot locate the host: {host}");
                }
            }
        }

        public async Task<bool> ChangePasswordAsync (string email, string newPassword)
        {
            var host = new Uri (_hostUrl + ChangePasswordFilename);

            using (var client = new HttpClient ())
            {
                var values = new Dictionary<string, string>
                {
                    { "email", email },
                    { "new_password", newPassword }
                };

                var content = new FormUrlEncodedContent (values);
                var response = await client.PostAsync (host, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync ();
                    Debug.WriteLine (responseContent);
                    return true;
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync ();
                    Debug.WriteLine (responseContent);
                    return false;
                }
            }
        }
    }
}
