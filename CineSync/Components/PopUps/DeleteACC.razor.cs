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

        private ApplicationUser AuthenticatedUser { get; set; }

        protected override void OnInitialized()
        {
            AuthenticatedUser = LayoutService.MainLayout.AuthenticatedUser;
        }

        private async void DeleteAccount()
        {
            Console.WriteLine(await UserManager.DeleteAccountAsync(AuthenticatedUser.Id));
        }

    }
}
