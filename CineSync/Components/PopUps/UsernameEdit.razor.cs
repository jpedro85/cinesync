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

        public string newUserName = string.Empty;

        public async Task SearchAsync()
        {
            MainLayout = LayoutService.MainLayout;
            ApplicationUser user = MainLayout.AuthenticatedUser;

            if (await UserManager.ChangeUsernameAsync(user.Id, newUserName))
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
