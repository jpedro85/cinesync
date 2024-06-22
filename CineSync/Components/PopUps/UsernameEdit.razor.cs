using CineSync.Components.Layout;
using CineSync.Data;
using CineSync.DbManagers;
using Microsoft.AspNetCore.Components;

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
            StateHasChanged();
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

            bool _hasChangedUsername = await UserManager.ChangeUsernameAsync(AuthenticatedUser.Id, _newUserName);
            if (_hasChangedUsername)
            {
                PopUpLayout.Close();
                await OnUsernameChange.InvokeAsync();
            }
            else
            {
                ErrorMessage = "The Username is already in use or Something went wrong";
                StateHasChanged();
            }

        }

    }

}
