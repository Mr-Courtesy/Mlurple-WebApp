using Microsoft.AspNetCore.Components;
using Mlurple_WebApp.Classes;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using Microsoft.AspNetCore.Components.Web;
using NETCore.Encrypt;
using Blazored.LocalStorage;
using System.Collections.Generic;

namespace Mlurple_WebApp.Pages
{
    public class HomePageBase : ComponentBase
    {
        protected string Result;
        [Inject]
        protected ILocalStorageService StorageService { get; set; }
        [Inject]
        public NavigationManager navigationManager { get; set; }
        public static bool hasProjects { get; set; }
        protected string _authState { get; set; }
        
        protected async Task<List<string>> GetUserProjects(string username)
        {
            string encryptedUsername = EncryptProvider.AESEncrypt(username, "key");

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://mysupersecretapi.com/api/ProjectSpace?username={encryptedUsername}")
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = response.Content.ReadAsStringAsync().Result;
                if (body == "No projects found.")
                {
                    await StorageService.SetItemAsync("projects", "No projects found");
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
                    await StorageService.SetItemAsync("projects", userProjects);
                }
                List<string> projs = await StorageService.GetItemAsync<List<string>>("projects");
                return projs;
            }
        }
        public void OpenCreationPage(MouseEventArgs e)
        {
            navigationManager.NavigateTo("/create");
        }
    }
}