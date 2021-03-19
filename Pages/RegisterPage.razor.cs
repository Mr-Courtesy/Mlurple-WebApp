using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using Mlurple_WebApp.Classes;
using System.Net.Http;
using NETCore.Encrypt;
using System.Collections.Generic;
using Blazored.LocalStorage;
using System.Threading.Tasks;

namespace Mlurple_WebApp.Pages
{
    public class RegisterPageBase : ComponentBase
    {
        [Inject]
        protected ILocalStorageService StorageService { get; set; }
        protected string Username;
        protected string Password;
        protected string CurrentUser;
        public string RegisterStatus;
        [Inject]
        public NavigationManager NavManager { get; set; }
        protected override async Task OnInitializedAsync()
        {
            CurrentUser = await StorageService.GetItemAsync<string>("username");
        }
        public async void HandleValidSubmit()
        {
            char[] _username = Username.ToCharArray();
            bool isValidUsername = _username.All(Char.IsLetterOrDigit);
            bool hasOnlyNumbers = _username.All(char.IsNumber);
            bool usernameIsLongEnough = _username.Length >= 3;
            bool usernameIsTooLong = _username.Length > 15;


            if (!isValidUsername)
            {
                RegisterStatus = "Username can only contain alphanumeric characters.";
            }
            if (hasOnlyNumbers)
            {
                RegisterStatus = "Username cannot only be numbers.";
            }
            if (!usernameIsLongEnough)
            {
                RegisterStatus = "Username must be at least 3 characters long.";
            }
            if (usernameIsTooLong)
            {
                RegisterStatus = "Username cannot be more than 10 characters long.";
            }

            char[] _password = Password.ToCharArray();
            bool passwordIsLongEnough = _password.Length >= 6;
            bool passwordIsNotValid = _password.All(Char.IsNumber);
            bool passwordHasWhitespace = _password.Contains(' ');
            bool passwordIsTooLong = _password.Length > 20;
    
            if (passwordIsNotValid)
            {
                RegisterStatus = "Passwords must contain at least 1 letter or symbol.";
            }
            if (!passwordIsLongEnough)
            {
                RegisterStatus = "Password must be at least 6 characters long.";
            }

            if (passwordHasWhitespace)
            {
                RegisterStatus = "Password cannot contain whitespace characters.";
            }
            
            if (passwordIsTooLong)
            {
                RegisterStatus = "Password should not be longer than 20 characters.";
            }
            else if (isValidUsername)
            {
                if (usernameIsLongEnough)
                {
                    if (!hasOnlyNumbers)
                    {
                        if (passwordIsLongEnough && !passwordIsNotValid && !passwordHasWhitespace && !passwordIsTooLong)
                        {
                            string encryptedUsername = EncryptProvider.AESEncrypt(Username, "key");
                            string encryptedPassword = EncryptProvider.AESEncrypt(Password, "key");

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
                                if (response.IsSuccessStatusCode)
                                {
                                    var body = response.Content.ReadAsStringAsync();
                                    if (body.Result == "User was succesfully registered.")
                                    {
                                        await StorageService.SetItemAsync("username", Username);
                                        NavManager.NavigateTo("/home");
                                    }
                                    else
                                    {
                                        bool containsUser = await StorageService.ContainKeyAsync("username");
                                        if (containsUser)
                                        {
                                            await StorageService.RemoveItemAsync("username");
                                        }
                                        RegisterStatus = body.Result;
                                    }
                                }
                                else if (!response.IsSuccessStatusCode)
                                {
                                    RegisterStatus = "An error occured.";
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}