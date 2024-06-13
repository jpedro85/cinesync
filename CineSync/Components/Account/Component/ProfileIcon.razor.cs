using CineSync.Data.Models;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.Account.Component
{
    public partial class ProfileIcon : ComponentBase
    {
        [Parameter]
        public int ProfileWidth { get; set; } = 100;

        [Parameter]
        public UserImage ProfileImage { get; set; }

        private string GetProfileWidthStyle()
        {
            return $"width: {ProfileWidth}px; height: {ProfileWidth}px;";
        }

    }
}
