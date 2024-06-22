using CineSync.Components.Layout;
using CineSync.Data;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CineSync.Components.PopUps
{
    public partial class UsernameEdit : ComponentBase
    {
        [CascadingParameter(Name = "PageLayout")]
        public PageLayout PageLayout { get; set; }

        [Inject]
        private UserManager UserManager { get; set; }

        [Parameter]
        public EventCallback OnUsernameChange { get; set; }

        private PopUpLayout PopUpLayout;

        private ApplicationUser? AuthenticatedUser { get; set; }

        private string? ErrorMessage { get; set; } = string.Empty;

        private string _newUserName = "";

        protected override async Task OnInitializedAsync()
        {
            AuthenticatedUser = PageLayout!.AuthenticatedUser;
        }

        private void HandleInputChange(ChangeEventArgs e)
        {
            _newUserName = e.Value?.ToString() ?? string.Empty;
            ErrorMessage = string.Empty;
        }

        private async void RenameUsername()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(_newUserName))
            {
                ErrorMessage = "Username cannot be empty.";
                StateHasChanged();
                return;
            }
            if (await UserManager.ChangeUsernameAsync(AuthenticatedUser.Id, _newUserName))
            {
                PopUpLayout.Close();
                await OnUsernameChange.InvokeAsync();
                return;
            }
            else
            {
                ErrorMessage = "Username is already taken or something went wrong.";
                StateHasChanged();
                return;
            }

        }

    }

}
