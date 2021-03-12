using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using Mlurple_WebApp.Services;
using Mlurple_WebApp.Models;
using System.Net.Http;
using System.Collections.Generic;

namespace Mlurple_WebApp.Pages
{
    public class RegisterPageBase : ComponentBase
    {
        public UserModel userModel = new UserModel();
        public string RegisterStatus;
        [Inject]
        public NavigationManager NavManager { get; set; }
        public async void HandleValidSubmit()
        {
            char[] _username = userModel.Username.ToCharArray();
            bool isValidUsername = _username.All(Char.IsLetterOrDigit);
            bool usernameIsLongEnough = _username.Length >= 5;
            bool usernameIsTooLong = _username.Length > 15;

            if (!isValidUsername)
            {
                RegisterStatus = "Username can only contain alphanumeric characters.";
            }
            if (!usernameIsLongEnough)
            {
                RegisterStatus = "Username must be at least 5 characters long.";
            }
            if (usernameIsTooLong)
            {
                RegisterStatus = "Username cannot be more than 10 characters long.";
            }

            char[] _password = userModel.Password.ToCharArray();
            bool passwordIsLongEnough = _password.Length >= 6;
        
            if (!passwordIsLongEnough)
            {
                RegisterStatus = "Password must be at least 6 characters long.";
            }

            if (_password.Contains(' '))
            {
                RegisterStatus = "Password cannot contain whitespace characters.";
            }
            else if (isValidUsername)
            {
                if (passwordIsLongEnough)
                {
                    string encryptedUsername = EncryptAndDecryptService.Encrypt("key goes here", userModel.Username);
                    string encryptedPassword = EncryptAndDecryptService.Encrypt("key goes here", userModel.Password);

                    HttpClient client = new HttpClient();
                    HttpRequestMessage request = new HttpRequestMessage()
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new
                        Uri($"https://mysupersecretapi.com/api/User?username={encryptedUsername}&&password={encryptedPassword}")
                    };
                    using (var response = await client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        var body = response.Content.ReadAsStringAsync();
                        if (body.Result == "User was succesfully registered.")
                        {
                            SessionUser.username = userModel.Username;
                            Session.isAuthorized = true;
                            RegisterStatus = SessionUser.username;
                            RegisterStatus = body.Result;
                            NavManager.NavigateTo("/home");
                        }
                        else
                        {
                            Session.isAuthorized = false;
                            RegisterStatus = body.Result;
                        }
                    }
                }
                else if (!passwordIsLongEnough)
                {
                    RegisterStatus = "Password must be at least 6 characters long.";
                }
            }
        }
    }
}