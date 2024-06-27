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
        [CascadingParameter(Name = "PageLayout")]
        public PageLayout PageLayout { get; set; } = default!;


        [Inject]
        private DiscussionManager DiscussionManager { get; set; } = default!;

        [Inject]
        private CommentManager CommentManager { get; set; } = default!;


        [Parameter]
		public int MovieId { get; set; }

        [Parameter]
        public ICollection<UserLikedDiscussion> LikedDiscussions { get; set; } = default!;

        [Parameter]
        public ICollection<UserDislikedDiscussion> DislikedDiscussions { get; set; } = default!;


        private ApplicationUser _authenticatedUser = default!;
        private ICollection<string> _authenticatedUserRoles { get; set; } = new List<string>();
		private ICollection<string> _userRoles { get; set; } = default!;

        private ICollection<Discussion> _movieDiscussions = default!;

        private NewDiscussion _newDiscussion = default!;

        protected override void OnInitialized()
		{
            _authenticatedUserRoles = PageLayout.UserRoles;
            _authenticatedUser = PageLayout.AuthenticatedUser!;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if( firstRender )
            {
                _movieDiscussions = await DiscussionManager.GetDiscussionsOfMovie( MovieId );
                StateHasChanged();
            }
        }

        private async void StartDiscussion( MouseEventArgs e)
        {

            Discussion newDiscussion = _newDiscussion.Discussion;

            if (newDiscussion.Title.IsNullOrEmpty())
                return;

            Comment firstComment = _newDiscussion.FirstComment.GetComment();

            await DiscussionManager.AddDiscussion(newDiscussion, MovieId, _authenticatedUser.Id);
            
            if (!firstComment.Content.IsNullOrEmpty()) 
            {
                await CommentManager.AddCommentToDiscussion(firstComment, newDiscussion.Id,_authenticatedUser.Id);
            }

            _newDiscussion.Reset();
        }

    }
}
