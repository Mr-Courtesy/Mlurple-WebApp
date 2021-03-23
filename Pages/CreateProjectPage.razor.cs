using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Mlurple_WebApp.Classes;
using System;
using System.Net.Http;
using NETCore.Encrypt;
using Blazored.LocalStorage;
using System.Threading.Tasks;

namespace Mlurple_WebApp.Pages
{
    public class CreateProjectBase : ComponentBase
    {
        [Inject]
        ILocalStorageService StorageService { get ;set ;}
        [Inject]
        NavigationManager NavigationManager { get; set; }
        protected string Name;
        protected string username;
        protected async Task CreateNewProject()
        {
            username = await StorageService.GetItemAsync<string>("username");
            string encryptedUsername = EncryptProvider.AESEncrypt(username, "MwjMBBUhXpUTwELvG3BJ4Xqkszqai1vT");
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://testp-blazor-api.herokuapp.com/api/ProjectSpace?username={encryptedUsername}&projectSpaceName={Name}")
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