using CineSync.Components.Navs;
using CineSync.Data;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CineSync.Components.Layout
{
    public partial class MainLayout 
    {

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        public UserManager<ApplicationUser> UserManager { get; set; }

        [Inject]
        public LayoutService LayoutService { get; set; }

        public ApplicationUser? AuthenticatedUser { get; set; }

        protected override void OnInitialized()
        {
            LayoutService.MainLayout = this;
            CheckLoginState();
        }

        private async void CheckLoginState()
        {
            AuthenticationState authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            ClaimsPrincipal claimsPrincipal = authState.User;

            AuthenticatedUser = await UserManager.GetUserAsync((ClaimsPrincipal)authState.User);

            Console.WriteLine("is User" + AuthenticatedUser == null);
        }

    }
}
