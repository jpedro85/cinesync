using CineSync.Data;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CineSync.Components.PopUps
{
    public partial class UsernameEdit : ComponentBase
    {
        [Parameter]
        public EventCallback OnUsernameChange { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private UserManager UserManager { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        private PopUpLayout PopUpLayout;

        private ApplicationUser AuthenticatedUser { get; set; }

        private string? ErrorMessage { get; set; } = string.Empty;

        private string _newUserName = "";

        protected override async Task OnInitializedAsync()
        {
            AuthenticatedUser = LayoutService.MainLayout.AuthenticatedUser;
        }

        private void HandleInputChange(ChangeEventArgs e)
        {
            _newUserName = e.Value?.ToString() ?? string.Empty;
            ErrorMessage = string.Empty;
        }

        private async Task RenameUsername()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(_newUserName))
            {
                ErrorMessage = "Username cannot be empty.";
                return;
            }

            if (await UserManager.ChangeUsernameAsync(AuthenticatedUser.Id, _newUserName))
            {
                LayoutService.MainLayout.AuthenticatedUser.UserName = _newUserName;
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
