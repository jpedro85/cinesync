using CineSync.Data;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.Pages
{
    public partial class Profile
    {
        [Parameter]
        public ApplicationUser? User { get; set; }
    }
}
