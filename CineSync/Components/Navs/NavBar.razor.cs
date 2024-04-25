using CineSync.Data;
using Microsoft.AspNetCore.Components;
using CineSync.Services;


namespace CineSync.Components.Navs
{
    public partial class NavBar : ComponentBase
    {
        [Parameter]
        public bool HasSearch { get; set; } = false;

        [Parameter]
        public ApplicationUser? User { get; set; }

        [Inject]
        public NavBarEvents NavBarEvents { get; set; }

    }
}
