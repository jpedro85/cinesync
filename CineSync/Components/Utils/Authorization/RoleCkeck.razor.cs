using CineSync.Data;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CineSync.Components.Utils.Authorization
{
	public partial class RoleCkeck : ComponentBase
	{
		[Inject]
		private UserRoleManager<ApplicationUser> UserRoleManager { get; set; }

		[Inject]
		private LayoutService LayoutService { get; set; }

		[Parameter]
		public RenderFragment PassAllChecks {  get; set; }

		[Parameter]
		public RenderFragment PassOneCheck { get; set; }

		[Parameter]
		public RenderFragment NotPassCheck { get; set; }

		[Parameter]
		public ICollection<string> Roles { get; set; } = [];

		[Parameter]
		public ICollection<string> AuthenticatedUserRoles { get; set; } = [];

		protected override async void OnInitialized()
		{
			if(Roles.Count == 0) 
			{
				if (LayoutService.MainLayout.AuthenticatedUser != null)
					AuthenticatedUserRoles = await UserRoleManager.GetRolesOfUserAsync(LayoutService.MainLayout.AuthenticatedUser);
			}
		}

		private bool HasAllRoles()
		{
			bool hasRoles = true;
			foreach (var role in Roles) 
			{
				hasRoles = hasRoles && AuthenticatedUserRoles.Contains(role);
			}
			return hasRoles;
		}

		private bool HasOneRole()
		{
			bool hasRoles = false;
			foreach (var role in Roles)
			{
				hasRoles = hasRoles || AuthenticatedUserRoles.Contains(role);
			}
			return hasRoles;
		}
	}
}
