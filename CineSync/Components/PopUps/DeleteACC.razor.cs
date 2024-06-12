using CineSync.Data;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.PopUps
{
    public partial class DeleteACC : ComponentBase
    {

        [Inject]
        private UserManager UserManager { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private string ErrorMessage { get; set; } = string.Empty;

        private ApplicationUser AuthenticatedUser { get; set; }

        protected override void OnInitialized()
        {
            AuthenticatedUser = LayoutService.MainLayout.AuthenticatedUser;
        }

        private async void DeleteAccount()
        {
            ErrorMessage = string.Empty;
            if (await UserManager.DeleteAccountAsync(AuthenticatedUser.Id))
            {
                NavigationManager.NavigateTo("/");
            }
            else
            {
                ErrorMessage = "It seems an error occurred while deleting your account";
            }
            StateHasChanged();
        }

    }
}
