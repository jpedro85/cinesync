using CineSync.Data;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CineSync.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase
    {

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        public UserManager<ApplicationUser> UserManager { get; set; }

        [Inject]
        public LayoutService LayoutService { get; set; }

        public ApplicationUser? AuthenticatedUser { get; set; }

        private bool _hasSearch = true;

        protected override async Task OnInitializedAsync()
        {
            await CheckLoginState();
            LayoutService.MainLayout = this;
        }

        private async Task CheckLoginState()
        {
            AuthenticationState authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            AuthenticatedUser = await UserManager.GetUserAsync(authState.User);

            Console.WriteLine($"is User {AuthenticatedUser == null}");
        }

        public void RemoveSearchButton()
        {
            _hasSearch = false;

            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

    }
}
