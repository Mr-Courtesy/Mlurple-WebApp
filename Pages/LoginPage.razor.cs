using Microsoft.AspNetCore.Components;
using Mlurple_WebApp.Classes;
using System.Net.Http;
using System;
using NETCore.Encrypt;
using System.Linq;
using Blazored.LocalStorage;

namespace Mlurple_WebApp.Pages
{
    public class LoginPageBase : ComponentBase
    {
        [Inject]
        protected ILocalStorageService StorageService { get; set; }
        [Inject]
        protected UserModel UserModel { get; set; }
        protected string _Username;
        protected string _Password;
        protected string _password;
        protected string LoginStatus;
        // public UserModel userModel = new UserModel();
        [Inject]
        public NavigationManager NavManager { get; set; }

        public async void HandleValidSubmit()
        {
            char[] _username = _Username.ToCharArray();
            bool isValidUsername = _username.All(Char.IsLetterOrDigit);
            bool hasOnlyNumbers = _username.All(char.IsNumber);
            bool usernameIsLongEnough = _username.Length >= 3;
            bool usernameIsTooLong = _username.Length > 15;
            char[] _password = _Password.ToCharArray();
            bool passwordIsLongEnough = _password.Length >= 6;
            bool passwordIsNotValid = _password.All(Char.IsNumber);
            bool passwordHasWhitespace = _password.Contains(' ');
            bool passwordIsTooLong = _password.Length > 20;
            if (!isValidUsername)
            {
                LoginStatus = "Username or password is incorrect.";
            }
            if (hasOnlyNumbers)
            {
                LoginStatus = "Username or password is incorrect.";
            }
            if (!usernameIsLongEnough)
            {
                LoginStatus = "Username or password is incorrect.";
            }
            if (usernameIsTooLong)
            {
                LoginStatus = "Username or password is incorrect.";
            }
            if (passwordIsNotValid)
            {
                LoginStatus = "Username or password is incorrect.";
            }
            if (!passwordIsLongEnough)
            {
                LoginStatus = "Username or password is incorrect.";
            }

            if (passwordHasWhitespace)
            {
                LoginStatus = "Username or password is incorrect.";
            }

            if (passwordIsTooLong)
            {
                LoginStatus = "Username or password is incorrect.";
            }
            var aeskey = EncryptProvider.CreateAesKey();
            var key = aeskey.Key;

            string encryptedUsername = EncryptProvider.AESEncrypt(_Username, "0uNFCkLHqc5G0l3lhfZ4q3SZskhBn6jt");
            string encryptedPassword = EncryptProvider.AESEncrypt(_Password, "0uNFCkLHqc5G0l3lhfZ4q3SZskhBn6jt");

            if (isValidUsername && usernameIsLongEnough && !hasOnlyNumbers && passwordIsLongEnough && !passwordIsNotValid && !passwordHasWhitespace && !passwordIsTooLong)
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new
                    Uri($"https://testp-blazor-api.herokuapp.com/api/User?username={encryptedUsername}&&password={encryptedPassword}")
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        var body = response.Content.ReadAsStringAsync();
                        if (body.Result == "true")
                        {
                            await StorageService.SetItemAsync("username", _Username);
                            await StorageService.SetItemAsync("authorized", true);
                            NavManager.NavigateTo("/home");
                        }
                        else if (body.Result == "false")
                        {
                            await StorageService.SetItemAsync("authorized", false);
                            LoginStatus = $"{body.Result}: Username or password is incorrect {UserModel.Username} {UserModel.Password}";
                        }
                        else
                        {
                            await StorageService.SetItemAsync("authorized", false);
                            LoginStatus = "Whoops! An error occured.";
                        }
                    }
                    else if (!response.IsSuccessStatusCode)
                    {
                        LoginStatus = "An error occured.";
                    }
                }
            }
        }
    }
}