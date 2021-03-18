using Microsoft.AspNetCore.Components;
using Mlurple_WebApp.Classes;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using Microsoft.AspNetCore.Components.Web;
using NETCore.Encrypt;
using Blazored.LocalStorage;

namespace Mlurple_WebApp.Pages
{
    public class HomePageBase : ComponentBase
    {
        protected string CurrentUser;
        [Inject]
        protected ILocalStorageService StorageService { get; set; }
        [Inject]
        public NavigationManager navigationManager { get; set; }
        public static bool hasProjects { get; set; }
        protected bool _authState { get; set; }
        protected override async Task OnInitializedAsync()
        {
            CurrentUser = await StorageService.GetItemAsync<string>("username");
            bool authState = await StorageService.GetItemAsync<bool>("authorized"); 
            authState = _authState;
            if (authState)
            {
                string username = await StorageService.GetItemAsync<string>("username");
                string encryptedUsername = EncryptProvider.AESEncrypt(username, "key");

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://mysupersecretapi.com/api/ProjectSpace?username={encryptedUsername}")
                };

                using (var response = client.Send(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = response.Content.ReadAsStringAsync().Result;

                    if (body == "No projects found.")
                    {
                        hasProjects = false;
                    }
                    else 
                    {
                        hasProjects = true;
                        var projects = body.Split("&");

                        foreach (var proj in projects)
                        {
                            if (proj != null && !SessionUser.projects.Contains(proj))
                            {
                                SessionUser.projects.Add(proj);
                            }
                        }
                    }
                }
            }
        }

        public void OpenCreationPage(MouseEventArgs e)
        {
            navigationManager.NavigateTo("/create");
        }
    }
}