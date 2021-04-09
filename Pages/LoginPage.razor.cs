using Microsoft.AspNetCore.Components;
using Mlurple_WebApp.Classes;
using System.Net.Http;
using System;
using NETCore.Encrypt;
using System.Linq;
using Blazored.LocalStorage;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Mlurple_WebApp.Pages
{
    public class LoginPageBase : ComponentBase
    {
        protected string CurrentUser;
        [Inject]
        protected ILocalStorageService StorageService { get; set; }
        protected string _Username;
        protected string _Password;
        protected string LoginStatus;
        [Inject]
        public NavigationManager NavManager { get; set; }
        protected override async Task OnInitializedAsync()
        {
            CurrentUser = await StorageService.GetItemAsync<string>("username");
            if (CurrentUser != null)
            {
                NavManager.NavigateTo("/home");
            }
        }

        public async void HandleValidSubmit()
        {
            char[] _username = _Username.ToCharArray();
            bool hasWhitespace = false;
            foreach (char c in _username)
            {
                if (char.IsWhiteSpace(c))
                {
                    hasWhitespace = true;
                }
            }

            bool hasOnlyNumbers = _username.All(char.IsNumber);
            bool usernameIsLongEnough = _username.Length >= 3;
            char[] _password = _Password.ToCharArray();
            bool passwordIsLongEnough = _password.Length >= 6;
            bool passwordIsNotValid = _password.All(Char.IsNumber);
            bool passwordHasWhitespace = _password.Contains(' ');
            bool passwordIsTooLong = _password.Length > 20;
            if (!hasWhitespace || hasOnlyNumbers || !usernameIsLongEnough || passwordIsNotValid || !passwordIsLongEnough || passwordHasWhitespace || passwordIsTooLong)
            {
                LoginStatus = "Email or password is incorrect.";
            }

            string encryptedUsername = EncryptProvider.AESEncrypt(_Username, "key");
            string encryptedPassword = EncryptProvider.AESEncrypt(_Password, "key");
            LoginStatus += $":{encryptedUsername} and {encryptedPassword}";
            if (!hasWhitespace && usernameIsLongEnough && !hasOnlyNumbers && passwordIsLongEnough && !passwordIsNotValid && !passwordHasWhitespace && !passwordIsTooLong)
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new
                    Uri($"https://supersecretapi.com/api/User?email={encryptedUsername}&&password={encryptedPassword}")
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        var body = response.Content.ReadAsStringAsync();

                        if (body.Result == "false")
                        {
                            bool containsUser = await StorageService.ContainKeyAsync("username");
                            await StorageService.SetItemAsync<string>("authstate", "notAuthorized");
                            LoginStatus = $"{body.Result}: Username or password is incorrect";
                        }
                        else 
                        {
                            await StorageService.SetItemAsync("username", body.Result);
                            await StorageService.SetItemAsync("email", _Username);
                            await StorageService.SetItemAsync("password", encryptedPassword);
                            await StorageService.SetItemAsync("authstate", "authorized");
                            await GetUserProjects(_Username);
                            NavManager.NavigateTo("/home");
                        }
                    }
                }
            }
        }
        protected async Task GetUserProjects(string username)
        {
            int projectCount;
            string encryptedUsername = EncryptProvider.AESEncrypt(username, "key");

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://supersecretapi.com/api/ProjectSpace?email={encryptedUsername}")
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = response.Content.ReadAsStringAsync().Result;
                if (body == "No projects found.")
                {
                    await StorageService.SetItemAsync("projects", "No projects found");
                    await StorageService.SetItemAsync("projectcount", 0);
                }
                else
                {
                    var projects = body.Split("&");
                    List<string> userProjects = new List<string>();

                    foreach (var proj in projects)
                    {
                        if (proj != null && !userProjects.Contains(proj))
                        {
                            userProjects.Add(proj);
                        }
                    }
                    projectCount = projects.Count();
                    await StorageService.SetItemAsync("projects", userProjects);
                    await StorageService.SetItemAsync("projectcount", projectCount);
                }
            }
        }
    }
}