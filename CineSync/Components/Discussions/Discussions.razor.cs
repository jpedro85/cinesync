using CineSync.Data;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using CineSync.Data.Models;
using CineSync.DbManagers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.IdentityModel.Tokens;
using CineSync.Components.Layout;

namespace CineSync.Components.Discussions
{
	public partial class Discussions
	{

		[Inject]
		private LayoutService LayoutService { get; set; }

        [Inject]
        private DiscussionManager DiscussionManager { get; set; }

        [Parameter]
		public int MovieId { get; set; }

        [Parameter]
        public ICollection<UserLikedDiscussion> LikedDiscussions { get; set; }

        [Parameter]
        public ICollection<UserDislikedDiscussion> DislikedDiscussions { get; set; }

        private ApplicationUser _authenticatedUser;
        private ICollection<string> _authenticatedUserRoles { get; set; } = new List<string>();
		private ICollection<string> _userRoles { get; set; }

        private ICollection<Discussion> _movieDiscussions;

        private NewDiscussion _newDiscussion;

		protected override void OnInitialized()
		{
            _authenticatedUserRoles = LayoutService.MainLayout.UserRoles;
            _authenticatedUser = LayoutService.MainLayout.AuthenticatedUser;
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            if( firstRender )
            {
                _movieDiscussions = await DiscussionManager.GetDiscussionsOfMovie( MovieId );
                StateHasChanged();
            }
        }

        private async void StartDiscussion( MouseEventArgs e)
        {

            Discussion newDiscussion = _newDiscussion.GetDiscussion();

            if( !newDiscussion.Title.IsNullOrEmpty() )
            { 

                _newDiscussion.Reset();
                if ( await DiscussionManager.AddDiscussion(newDiscussion, MovieId, LayoutService.MainLayout.AuthenticatedUser.Id) )
                { 
                    _movieDiscussions.Add(newDiscussion);
                    StateHasChanged();
                }
            }

        }

    }
}
