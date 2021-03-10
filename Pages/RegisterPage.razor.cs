using Microsoft.AspNetCore.Components;
using System;
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
        public async void HandleValidSubmit()
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
                    RegisterStatus = SessionUser.username;
                    RegisterStatus = body.Result;
                }
                else if (body.Result == "User already exists with that username.")
                {
                    RegisterStatus = body.Result;
                }
            }
        }
    }
}