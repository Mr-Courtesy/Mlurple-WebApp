using Microsoft.AspNetCore.Components;
using Mlurple_WebApp.Classes;
using Blazored.LocalStorage;
using System.Threading.Tasks;

namespace Mlurple_WebApp.Pages
{
    public class LogoutBase : ComponentBase
    {
        [Inject]
        protected ILocalStorageService StorageService { get; set; }
        [Inject]
        public NavigationManager NavManager { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await StorageService.ClearAsync();
            NavManager.NavigateTo("/", true);
        }
    }
}