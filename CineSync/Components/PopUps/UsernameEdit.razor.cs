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

        [Parameter]
        public EventCallback OnUsernameChange { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private UserManager UserManager { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        private PopUpLayout PopUpLayout;

        private ApplicationUser User { get; set; }

        private string? ErrorMessage { get; set; } = string.Empty;

        private string _newUserName = "";

        protected override async Task OnInitializedAsync()
        {
            User = LayoutService.MainLayout.AuthenticatedUser;
        }

        public async Task RenameUsername()
        {
            ErrorMessage = string.Empty;

            if (await UserManager.ChangeUsernameAsync(User.Id, _newUserName))
            {
                await OnUsernameChange.InvokeAsync();
                PopUpLayout.Close();
            }
            else
            {
                ErrorMessage = "Something went wrong could not change your username";
            }
        }

    }
}
