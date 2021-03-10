using Microsoft.AspNetCore.Components;
using Mlurple_WebApp.Models;
using System.Net.Http;
using System;

namespace Mlurple_WebApp.Pages
{
    public class LoginPageBase : ComponentBase
    {
        public string LoginStatus;
        public UserModel userModel = new UserModel();

        public async void HandleValidSubmit()
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new
                Uri($"https://mysupersecretapi.com/api/User?username={userModel.Username}&&password={userModel.Password}")
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = response.Content.ReadAsStringAsync();
                if (body.Result == "true")
                {
                    LoginStatus = "";
                    LoginStatus = "bonjour";
                    SessionUser.username = userModel.Username;
                    LoginStatus = $"{SessionUser.username} hi";
                }
                else if (body.Result == "false")
                {
                    LoginStatus = "Username or password is incorrect";
                }
            }
        }
    }
}