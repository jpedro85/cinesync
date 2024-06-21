using CineSync.DbManagers;
using Microsoft.AspNetCore.Components;
using CineSync.Data.Models;
using CineSync.Services;
using CineSync.Data;
using CineSync.Components.Comments;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Components.Web;
using CineSync.Components.Layout;

namespace CineSync.Components.Discussions
{
    public partial class ItemDiscussion
    {
		[CascadingParameter(Name = "PageLayout")]
		public PageLayout PageLayout { get; set; }


		[Inject]
        private UserManager UserManager { get; set; }

        [Inject]
        private DiscussionManager DiscussionManager {  get; set; }

        [Inject]
        private CommentManager CommentManager { get; set; }


        [Parameter]
        public ICollection<UserLikedDiscussion> LikedDiscussions { get; set; }

        [Parameter]
        public ICollection<UserDislikedDiscussion> DislikedDiscussions { get; set; }

        [Parameter]
        public Discussion Discussion { get; set; }


        [Parameter]
        public bool Liked { get; set; } = false;
        private bool _Liked { get; set; }

        [Parameter]
        public bool DisLiked { get; set; } = false;
        private bool _Disliked { get; set; }


        [Parameter]
        public bool AllowFollow { get; set; } = true;

        [Parameter]
        public EventCallback OnChange { get; set; }

        [Parameter]
        public EventCallback<uint> OnRemove { get; set; }

        private ApplicationUser _authenticatedUser;

        private ICollection<string> _userRoles;

        private NewComment _newComment;

        protected override void OnInitialized()
        {
            _authenticatedUser = PageLayout.AuthenticatedUser!;
            _userRoles = PageLayout.UserRoles;
            _Liked = Liked;
            _Disliked = DisLiked;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) 
            {
                Discussion.Comments = await CommentManager.GetCommentsOfDiscussion(Discussion.Id);
                StateHasChanged();
            }
        }

        private async void Follow(string id)
        {
            if (await UserManager.Follow(_authenticatedUser.Id, id))
            {
                if (_authenticatedUser.Following == null)
                    _authenticatedUser.Following = new List<ApplicationUser>();

                _authenticatedUser.Following.Add(Discussion.Autor!);

                StateHasChanged();
				await PageLayout.Menu.ReRender();
                await OnChange.InvokeAsync();
            }
        }

        private async void UnFollow(string id)
        {
            if (await UserManager.UnFollow(_authenticatedUser.Id, id))
            {
                if (_authenticatedUser.Following == null)
                    _authenticatedUser.Following = new List<ApplicationUser>();

                _authenticatedUser.Following = _authenticatedUser.Following.Where(u => u.Id != Discussion.Autor!.Id).ToList();

                StateHasChanged();
				await PageLayout.Menu.ReRender();
				await OnChange.InvokeAsync();
            }
        }

        private async void AddLike()
        {
            if (_Disliked)
            {
                await DiscussionManager.RemoveDesLikeAsync(Discussion, _authenticatedUser.Id);
                _Disliked = false;
                UpdateDislike(_Disliked);
            }

            if (_Liked)
            {
                await DiscussionManager.RemoveLikeAsync(Discussion, _authenticatedUser.Id);
                _Liked = false;
            }
            else
            {
                await DiscussionManager.AddLikeAsync(Discussion, _authenticatedUser.Id);
                _Liked = true;
            }

            UpdateLike(_Liked);
            StateHasChanged();
        }

        private async void AddDeslike()
        {

            if (_Liked)
            {
                await DiscussionManager.RemoveLikeAsync(Discussion, _authenticatedUser.Id);
                _Liked = false;
                UpdateLike(_Liked);
            }

            if (_Disliked)
            {
                await DiscussionManager.RemoveDesLikeAsync(Discussion, _authenticatedUser.Id);
                _Disliked = false;
            }
            else
            {
                await DiscussionManager.AddDesLikeAsync(Discussion, _authenticatedUser.Id);
                _Disliked = true;
            }
            UpdateDislike(_Disliked);
            StateHasChanged();
        }

        private void UpdateLike(bool newState)
        {
            if (newState)
            {
                if (!LikedDiscussions.Any(u => u.Discussion.Equals(Discussion)))
                {
                    LikedDiscussions.Add(
                            new UserLikedDiscussion()
                            {
                                User = _authenticatedUser,
                                Discussion = Discussion,
                                UserId = _authenticatedUser.Id,
                                DiscussionId = Discussion.Id
                            }
                        );
                }
            }
            else
            {
                if (LikedDiscussions.Any(u => u.Discussion.Equals(Discussion)))
                {
                    foreach (var item in LikedDiscussions)
                    {
                        if (item.Discussion.Equals(Discussion))
                        {
                            LikedDiscussions.Remove(item);
                            break;
                        }
                    }
                }
            }

            StateHasChanged();
        }

        private void UpdateDislike(bool newState)
        {
            if (newState)
            {
                if (!DislikedDiscussions.Any(u => u.Discussion.Equals(Discussion)))
                {
                    DislikedDiscussions.Add(
                            new UserDislikedDiscussion()
                            {
                                User = _authenticatedUser,
                                Discussion = Discussion,
                                UserId = _authenticatedUser.Id,
                                DiscussionId = Discussion.Id
                            }
                        );
                }
            }
            else
            {
                if (DislikedDiscussions.Any(u => u.Discussion.Equals(Discussion)))
                {
                    foreach (var item in DislikedDiscussions)
                    {
                        if (item.Discussion.Equals(Discussion))
                        {
                            DislikedDiscussions.Remove(item);
                            break;
                        }
                    }
                }
            }

            StateHasChanged();
        }

        private async void AddComment(MouseEventArgs e) 
        {
            Comment commentToAdd = _newComment.GetComment();
            commentToAdd.Autor = _authenticatedUser;

            if (!commentToAdd.Content.IsNullOrEmpty()) 
            {
                await CommentManager.AddCommentToDiscussion(commentToAdd, Discussion.Id, _authenticatedUser.Id );
                _newComment.Reset();
                StateHasChanged();
            }
        }

        private async void Remove() 
        {
            uint id = (Discussion.Id);
            await DiscussionManager.RemoveAsync(Discussion);
            await OnRemove.InvokeAsync(id);
        }
    }
}
