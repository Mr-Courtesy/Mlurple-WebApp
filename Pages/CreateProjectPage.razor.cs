using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Mlurple_WebApp.Models;
using Mlurple_WebApp.Services;
using System;
using System.Net.Http;

namespace Mlurple_WebApp.Pages
{
    public class CreateProjectBase : ComponentBase
    {
        [Inject]
        NavigationManager NavigationManager { get; set; }
        public ProjectSpace projectSpace = new ProjectSpace();
        protected void CreateNewProject()
        {
            if (Session.isAuthorized)
            {
                string encryptedUsername = EncryptAndDecryptService.Encrypt("key", SessionUser.username);
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"https://mysupersecretapi.com/api/ProjectSpace?username={encryptedUsername}&projectSpaceName={projectSpace.Name}")
                };

                using (var response = client.Send(request))
                {
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        NavigationManager.NavigateTo("/home");
                    }
                }
            }
        }
    }
}