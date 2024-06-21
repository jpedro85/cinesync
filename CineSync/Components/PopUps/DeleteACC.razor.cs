using CineSync.Components.Layout;
using CineSync.Data;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.PopUps
{
    public partial class DeleteACC : ComponentBase
    {
        [CascadingParameter(Name = "PageLayout")]
        public PageLayout PageLayout { get; set; }


        [Inject]
        private UserManager UserManager { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }


        private string ErrorMessage { get; set; } = string.Empty;

        private ApplicationUser? AuthenticatedUser { get; set; }

        protected override void OnInitialized()
        {
            AuthenticatedUser = PageLayout!.AuthenticatedUser;
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
