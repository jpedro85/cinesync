using CineSync.Components.Navs;
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
        public UserManager DbUserManager { get; set; }

        [Inject]
        public UserRoleManager<ApplicationUser> DbUserRoleManager { get; set; }

        public ApplicationUser? AuthenticatedUser { get; set; }

        private Menu? _menu;
        public Menu Menu
        {
            get
            {
                if (_menu == null)
                {
                    throw new Exception("layout Menu was not set.");
                }

                return _menu;
            }
        }

        public NavBar NavBar { get; private set; }

        private string UserId { get; set; }

        public ICollection<string> UserRoles { get; set; } = new List<string>();

        private bool _hasSearch = true;

        private MainLayoutService _mainLayoutService;

        private string _test;
        protected override void OnInitialized()
        {
            _mainLayoutService = new MainLayoutService(this, _menu!);
            Console.WriteLine("Merda");

            _test = "AAAAAAAAAAA";
        }

        protected override async Task OnInitializedAsync()
        {
            //await CheckLoginState();
            //await GetUserRoles();
            //LayoutService.MainLayout = this;
        }

        private async Task CheckLoginState()
        {
            //         AuthenticationState authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();   
            //         string? userId = UserManager.GetUserId(authState.User);

            //Console.WriteLine($"is User {userId == null}");
            //         if(userId != null)
            //             AuthenticatedUser = await DbUserManager.GetFirstByConditionAsync(u => u.Id == userId, "Following", "Followers" );
            //         Console.WriteLine($"BrawserUser {AuthenticatedUser?.Following?.Count},{AuthenticatedUser?.Followers?.Count}");

        }

        private async Task GetUserRoles()
        {
            if (AuthenticatedUser != null)
                UserRoles = await DbUserRoleManager.GetRolesOfUserAsync(AuthenticatedUser);
        }

        public void RemoveSearchButton()
        {
            //_hasSearch = false;

            //InvokeAsync(() =>
            //{
            //    StateHasChanged();
            //});
        }

        public async Task TriggerNavBarReRender()
        {
            // await NavBarEvents.RequestNavBarReRender();
        }

        public void TriggerMenuReRender()
        {
            //  MenuService.RequestMenuReRender();
        }

        private void GetMenu(Menu menu)
        {
            //_menu = menu;
        }

    }
}
