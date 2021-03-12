using Microsoft.AspNetCore.Components;
using Mlurple_WebApp.Models;

namespace Mlurple_WebApp.Pages
{
    public class LogoutBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavManager { get; set; }
        protected override void OnInitialized()
        {
            Session.isAuthorized = false;
            NavManager.NavigateTo("/");
        }
    }
}