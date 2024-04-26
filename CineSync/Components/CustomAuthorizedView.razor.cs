using CineSync.Data;
using CineSync.DbManagers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CineSync.Components
{
    public partial class CustomAuthorizedView
    {
        [Parameter]
        public ApplicationUser? User { get; set; }

        [Parameter]
        public bool CheckAuthentication { get; set; } = false;

        [Parameter]
        public string Roles { get; set; } = string.Empty;

        [Parameter]
        public RenderFragment CustomAuthorized { get; set; }

        [Parameter]
        public RenderFragment CustomNotAuthorized { get; set; }

        [Parameter]
        public string DebugName { get; set; }

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        private UserRoleManager<ApplicationUser> UserRoleManager { get; set; }

        [Inject]
        private UserManager<ApplicationUser> UserManager { get; set; }

        public bool PassedRoleChecked { get; private set; } = false;

        public bool PassedAuthentication { get; private set; } = false;

        protected override async Task OnInitializedAsync()
        {
            DebugName = DebugName;

            PassedAuthentication = await PassCheckAuthentication();

            if (User != null && Roles != string.Empty)
            {
                string[] RolesToCheck = Roles.Trim().Split(',');

                foreach (string Role in RolesToCheck)
                {
                    if (await UserRoleManager.IsInRoleAsync(User, Role))
                    {
                        PassedRoleChecked = true;
                        return;
                    }
                }
            }
            else if (PassedAuthentication)
            {
                PassedRoleChecked = true;
            }

        }

        private async Task<bool> PassCheckAuthentication()
        {
            if (CheckAuthentication)
            {
                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                var user = await UserManager.GetUserAsync(authState.User); ;
                return user != null;
            }
            else
                return true;
        }

    }
}
