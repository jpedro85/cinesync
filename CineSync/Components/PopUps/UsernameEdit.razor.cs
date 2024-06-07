using CineSync.Components.Layout;
using CineSync.Data;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CineSync.Components.PopUps
{
    public partial class UsernameEdit
    {
        [Parameter]
        public string ActualUserName { get; set; } = string.Empty;

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private UserManager UserManager { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        private MainLayout MainLayout { get; set; }

        private string? ErrorMessage { get; set; } = string.Empty;

        private string _newUserName = "";
        
        public async Task RenameUsername()
        {
            MainLayout = LayoutService.MainLayout;
            ApplicationUser user = MainLayout.AuthenticatedUser;

            if (await UserManager.ChangeUsernameAsync(user.Id, _newUserName))
            {
                await JSRuntime.InvokeVoidAsync("window.location.reload");
            }
            else
            {
                ErrorMessage = "Something went wrong could not change your username";
            }
        }

    }
}
