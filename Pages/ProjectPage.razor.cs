using Microsoft.AspNetCore.Components;
using Mlurple_WebApp.Classes;
using System.Threading.Tasks;
using NETCore.Encrypt;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading;

namespace Mlurple_WebApp.Pages 
{
    public class ProjectPageBase : ComponentBase
    {
        [Inject]
        NavigationManager Navigation { get; set; }
        [Parameter]
        public string project { get; set; }
        [Inject]
        Blazored.LocalStorage.ILocalStorageService StorageService { get; set; }
        protected string _username;
        protected List<string> projectInfo = new List<string>();
        protected Dictionary<string, string> ProjectInfoDictionary = new Dictionary<string, string>();
        protected List<string> ProjectTasksList = new List<string>();
        protected override async Task OnInitializedAsync()
        {
            _username = await StorageService.GetItemAsync<string>("email");
            if (_username != null)
            {
                
                await StorageService.SetItemAsync("load", "yes");
                await GetProjectInfo();
            }
        }
        protected async Task GetProjectInfo()
        {
            await StorageService.RemoveItemAsync("project");
            if (_username != null)
            {
                await StorageService.SetItemAsync("project", project);
                string name = await StorageService.GetItemAsync<string>("project");

                string encryptedUsername = EncryptProvider.AESEncrypt(_username, "key");
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://supersecretapi.com/api/ProjectSpace/{name}?email={encryptedUsername}")
                };

                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        var body = response.Content.ReadAsStringAsync().Result;
                        var info = body.Split("**");
                        List<string> _info = new List<string>();
                        
                        foreach (var i in info)
                        {
                            if (i != null && !projectInfo.Contains(i))
                            {
                                projectInfo.Add(i);
                            }
                        }
                        foreach (var i in projectInfo)
                        {
                            var _projectInfo = i.Split(":");
                            foreach (var t in _projectInfo)
                            {
                                _info.Add(t);
                            }
                        }
                        
                        ProjectInfoDictionary.Add($"_{_info[0]}", _info[1]);
                        await StorageService.SetItemAsync(name, ProjectInfoDictionary);

                        ProjectTasksList = await GetProjectTasks(encryptedUsername, project);
                    }
                }
            }
        }
        protected async Task<List<string>> GetProjectTasks(string email, string project)
        {
            List<string> tasks = new List<string>();
            HttpClient client = new HttpClient();
            HttpRequestMessage requestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://supersecretapi.com/api/ProjectSpace/tasks?email={email}&project={project}")
            };
            
            using (var response = await client.SendAsync(requestMessage))
            {
                response.EnsureSuccessStatusCode();
                var body = response.Content.ReadAsStringAsync().Result;
                var t = body.Split("&");

                foreach (var i in t)
                {
                    if (i != "" && !tasks.Contains(i))
                    {
                        tasks.Add(i);
                    }
                }
            }
            return tasks;
        }
    }
}