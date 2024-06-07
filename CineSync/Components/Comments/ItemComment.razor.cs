using Microsoft.AspNetCore.Components;
using CineSync.Data.Models;
using CineSync.DbManagers;
using CineSync.Services;
using CineSync.Data;
using CineSync.Components.PopUps;

namespace CineSync.Components.Comments
{
    public partial class ItemComment : ComponentBase
    {
		[Inject]
		private UserManager UserManager { get; set; }

		[Inject]
        private CommentManager CommentManager { get; set; }

		[Inject]
		private LayoutService LayoutService { get; set; }

		[Parameter]
        public Comment Comment { get; set; }

		private ApplicationUser _authenticatedUser;

		private ErrorPopUp _errorPopUp;

		protected override void OnInitialized()
		{
			_authenticatedUser = LayoutService.MainLayout.AuthenticatedUser!;
		}

		protected override void OnAfterRender(bool firstRender)
		{

		}

		private async void AddLike(Comment commentAddLike)
        {
            await CommentManager.AddLikeAsync(commentAddLike);
            StateHasChanged();
        }

        private async void AddDeslike(Comment commentAddDesLike)
        {
            await CommentManager.AddDesLikeAsync(commentAddDesLike);
            StateHasChanged();
        }

        private async void Follow(string id)
        {
            await UserManager.Follow(_authenticatedUser.Id, id);
        }

		private async void UnFollow(string id)
		{
			await UserManager.UnFollow(_authenticatedUser.Id, id);
		}

	}
}
