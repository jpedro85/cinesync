using CineSync.Components.Navs;
using CineSync.Data;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CineSync.Components.Layout
{
    public partial class PageLayout : ComponentBase
    {
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        public UserManager<ApplicationUser> UserManager { get; set; }

        [Inject]
        public UserManager DbUserManager { get; set; }

        [Inject]
        public UserRoleManager<ApplicationUser> DbUserRoleManager { get; set; }


        [Parameter]
        public GetInstanceDelegate GetInstance { get; set; } = (m) => { };
        public delegate void GetInstanceDelegate(PageLayout layout);

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool HasSearch { get; set; } = true;


        public NavBar NavBar
        {
            get
            {
                if (_navBar == null)
                    throw new NullReferenceException("PageLayout: NavBar was not set.");

                return _navBar;
            }
        }
        private NavBar _navBar;

        public Menu Menu
        {
            get
            {
                if (_navBar == null)
                    throw new NullReferenceException("PageLayout: Menu was not set.");

                return _menu;
            }
        }
        private Menu _menu;

        public ApplicationUser? AuthenticatedUser { get; set; }

        public ICollection<string> UserRoles { get; set; } = new List<string>();

        private NavBarEvents _navBarEvents = new NavBarEvents();

        protected override async Task OnInitializedAsync()
        {
            await CheckLoginState();
            await GetUserRoles();
            GetInstance(this);
        }

        private async Task CheckLoginState()
        {
            AuthenticationState authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            string? userId = UserManager.GetUserId(authState.User);

            if (userId != null)
                AuthenticatedUser = await DbUserManager.GetFirstByConditionAsync(u => u.Id == userId, "Following", "Followers");
        }

        private async Task GetUserRoles()
        {
            if (AuthenticatedUser != null)
                UserRoles = await DbUserRoleManager.GetRolesOfUserAsync(AuthenticatedUser);
        }

        private void GetNavbarInstance( NavBar instance )
        {
            if(_navBar == null)
                _navBar = instance;
        }

        private void GetMenuInstance(Menu instance)
        {
            if (_menu == null)
                _menu = instance;
        }

        private bool _hasSearch = true;
        public void RemoveSearchButton()
        {
            _hasSearch = false;

            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        //public async Task TriggerNavBarReRender()
        //{
        //    // await NavBarEvents.RequestNavBarReRender();
        //}

        //public void TriggerMenuReRender()
        //{
        //    //  MenuService.RequestMenuReRender();
        //}
    }
}
