using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using CineSync.Components.Layout;
using CineSync.Data;

namespace CineSync.Components.PopUps
{
    public partial class UsernameEdit
    {
        [Inject]
        private UserManager UserManager { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        private MainLayout MainLayout { get; set; }

        [Parameter]
        public string ActualUserName { get; set; } = string.Empty;

        public async Task SearchAsync()
        {
            MainLayout = LayoutService.MainLayout;
            ApplicationUser user = MainLayout.AuthenticatedUser;

            if (await UserManager.ChangeUsernameAsync(user.Id, ActualUserName))
            {
                // TODO: Add the missing logic for the frontend
            }
            else
            {
                // TODO: Add the missing logic for the frontend
            }
        }

    }
}
