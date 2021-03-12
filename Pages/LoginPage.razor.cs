using Microsoft.AspNetCore.Components;
using Mlurple_WebApp.Models;
using System.Net.Http;
using System;
using Mlurple_WebApp.Services;

namespace Mlurple_WebApp.Pages
{
    public class LoginPageBase : ComponentBase
    {
        public string LoginStatus;
        public UserModel userModel = new UserModel();
        [Inject]
        public NavigationManager NavManager { get; set; }
        public async void HandleValidSubmit()
        {
            string encryptedUsername = EncryptAndDecryptService.Encrypt("key goes here", userModel.Username);
            string encryptedPassword = EncryptAndDecryptService.Encrypt("key goes here", userModel.Password);

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new
                Uri($"https://mysupersecretapi.com/api/User?username={encryptedUsername}&&password={encryptedPassword}")
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = response.Content.ReadAsStringAsync();
                if (body.Result == "true")
                {
                    LoginStatus = "";
                    SessionUser.username = userModel.Username;
                    Session.isAuthorized = true;
                    LoginStatus = $"{body.Result}: Hello, {SessionUser.username}.";
                    NavManager.NavigateTo("/home");
                }
                else if (body.Result == "false")
                {
                    Session.isAuthorized = false;
                    LoginStatus = $"{body.Result}: Username or password is incorrect {userModel.Username} {userModel.Password}";
                }
            }
        }
    }
}