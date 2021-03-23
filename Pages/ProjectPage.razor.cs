using Microsoft.AspNetCore.Components;
using Mlurple_WebApp.Classes;
using System.Threading.Tasks;
using NETCore.Encrypt;
using System;
using System.Net.Http;
using System.Collections.Generic;

namespace Mlurple_WebApp.Pages 
{
    public class ProjectPageBase : ComponentBase
    {
        [Inject]
        Blazored.LocalStorage.ILocalStorageService StorageService { get; set; }
        protected string _username;
        protected List<string> projectInfo = new List<string>();
        protected override async Task OnInitializedAsync()
        {
            _username = await StorageService.GetItemAsync<string>("username");
            if (_username != null)
            {
                await GetProjectInfo();
            }
        }
        protected async Task GetProjectInfo()
        {
            if (_username != null)
            {
                string name = await StorageService.GetItemAsync<string>("project");
                string encryptedUsername = EncryptProvider.AESEncrypt(_username, "key");
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://mysupersecretapi.com/api/ProjectSpace/{name}?username={encryptedUsername}")
                };

                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        var body = response.Content.ReadAsStringAsync().Result;
                        var info = body.Split("**");
                        
                        foreach (var i in info)
                        {
                            if (i != null && !projectInfo.Contains(i))
                            {
                                projectInfo.Add(i);
                            }
                        }
                        await StorageService.SetItemAsync(name, projectInfo);
                        await StorageService.RemoveItemAsync("project");
                    }
                }
            }
        }
    }
}