using CineSync.Data;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using CineSync.Data.Models;

namespace CineSync.Components.Discussions
{
	public partial class Discussions
	{

		[Inject]
		private LayoutService LayoutService { get; set; }

		[Parameter]
		public ApplicationUser AuthenticatedUser { get; set; }

		private ICollection<string> AuthenticatedUserRoles { get; set; } = new List<string>();

		private ICollection<string> UserRoles { get; set; }

		protected override void OnInitialized()
		{
			AuthenticatedUserRoles = LayoutService.MainLayout.UserRoles;
        }

	}
}
