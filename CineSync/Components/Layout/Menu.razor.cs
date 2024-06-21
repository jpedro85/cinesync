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

        [Parameter]
        public GetInstanceDelegate GetInstance { get; set; } = (m) => { };
        public delegate void GetInstanceDelegate(Menu menu);

        [Parameter,EditorRequired]
        public ApplicationUser? AuthenticatedUser { get; set; }

        [Parameter, EditorRequired]
        public ICollection<string> UserRoles { get; set; }

        [Parameter, EditorRequired]
        public NavBarEvents NavBarEvents { get; set; }


        private const int MAX_NOT_OPEN_ALLOWED_FOLLOWING_USERS = 4;
        private const int MAX_OPEN_ALLOWED_FOLLOWING_USERS = 8;
        private bool _showAll = false;

        private string IsActive { get; set; } = string.Empty;

        public int Teste { get; set; } = 69;

        protected override void OnInitialized()
        {
            NavBarEvents.OnMenuChange += this.ChangeState;
            GetInstance(this);
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

        public Task ReRender() 
        {
            return InvokeAsync(StateHasChanged);
        }
    }
}
