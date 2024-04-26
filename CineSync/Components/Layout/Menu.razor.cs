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
        private NavBarEvents NavBarEvents { get; set; }

        [Inject]
        private SignInManager<ApplicationUser> SignInManager { get; set; }

        [Inject]
        private NavigationManager NavManager { get; set; }

        [Parameter]
        public ApplicationUser? User { get; set; }

        private string IsActive { get; set; } = string.Empty;


        public void OverlayClick( MouseEventArgs e)
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

        protected override void OnInitialized()
        {
            NavBarEvents.OnMenuChange += this.ChangeState;
        }

    }
}
