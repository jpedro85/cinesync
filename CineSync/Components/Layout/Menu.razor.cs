using CineSync.Data;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CineSync.Components.Layout
{
    public partial class Menu : ComponentBase
    {
        [Inject]
        private LayoutService LayoutService { get; set; }

        [Inject]
        private NavBarEvents NavBarEvents { get; set; }

        [Inject]
        private MenuService MenuService { get; set; }

        [Inject]
        private SignInManager<ApplicationUser> SignInManager { get; set; }

        public ApplicationUser? AuthenticatedUser { get; set; }

        [Parameter]
        public ICollection<string> UserRoles { get; set; }

        private const int MAX_NOT_OPEN_ALLOWED_FOLLOWING_USERS = 4;
        private const int MAX_OPEN_ALLOWED_FOLLOWING_USERS = 8;
        private bool _showAll = false;

        private string IsActive { get; set; } = string.Empty;

        protected override void OnInitialized()
        {
            MenuService.OnRequestMenuReRender += ReRender;
            NavBarEvents.OnMenuChange += this.ChangeState;
            AuthenticatedUser = LayoutService.MainLayout.AuthenticatedUser;
        }

        public void CloseMenu( MouseEventArgs e)
        {
            IsActive = "";

            NavBarEvents.IsMenuOpen = false;

            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        public void ChangeState( bool active)
        {
            if (active)
            {
                IsActive = "Active";
            }
            else
            {
                IsActive = "";
            }

            InvokeAsync( () =>
            {
                StateHasChanged();
            });

        }

        private void OnClickShowAll(MouseEventArgs e) 
        {
            _showAll = !_showAll;
            IsActive = "Active";
            Console.WriteLine($"IsActive: {IsActive}");
            StateHasChanged();
        }

        private Task ReRender() 
        {
            AuthenticatedUser = LayoutService.MainLayout.AuthenticatedUser;
            return InvokeAsync(StateHasChanged);
        }
    }
}
