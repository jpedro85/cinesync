using Microsoft.AspNetCore.Components;
using CineSync.Data.Models;
using CineSync.DbManagers;
using CineSync.Services;
using CineSync.Data;
using CineSync.Components.PopUps;
using Microsoft.AspNetCore.Components.Web;
using CineSync.Components.Layout;
using Microsoft.AspNetCore.SignalR.Client;

namespace CineSync.Components.Comments
{
    public partial class ItemComment : ComponentBase
    {
        [CascadingParameter(Name = "PageLayout")]
        public PageLayout PageLayout { get; set; } = default!;

        [CascadingParameter(Name = "DiscussionHubConnection")]
        public HubConnection? DiscussionHubConnection { get; set; } = default;


        [Inject]
        private MovieManager MovieManager { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        private UserManager UserManager { get; set; } = default!;

        [Inject]
        private CommentManager CommentManager { get; set; } = default!;

        [Inject]
        private DiscussionManager DiscussionManager { get; set; } = default!;


        [Parameter,EditorRequired]
        public ICollection<UserLikedComment> LikedComments { get; set; } = default!;

        [Parameter,EditorRequired]
        public ICollection<UserDislikedComment> DislikedComments { get; set; } = default!;

        [Parameter,EditorRequired]
        public Comment Comment { get; set; } = default!;

        [Parameter]
        public bool AllowFollow { get; set; } = true;

        [Parameter]
        public bool AllowStartDiscusion { get; set; } = true;

        [Parameter]
        public bool ShowOnlyInfo { get; set; } = false;

        [Parameter]
        public bool AllowNavegation { get; set; } = false;
        private bool _isNaveGationClick = true;

        [Parameter]
        public EventCallback OnChange { get; set; }

        [Parameter]
        public EventCallback<uint> OnRemove { get; set; }

        [Parameter]
        public EventCallback OnCreateDiscussion { get; set; } = default;

        private bool _Liked = false;
		private bool _Disliked = false;
        private bool _allowSee;

        private ApplicationUser _authenticatedUser = default!;

        private ICollection<string> _userRoles = default!;

        private PopUpAttachementView _attachementView = default!;

        private RemoveComment _popUpRemove = default!;

        private PopUpStartDiscussion _newDiscussionPopUp = default!;

        private string _GroupName = default!;

        protected override void OnInitialized()
		{
            if(DiscussionHubConnection != null && Comment.DiscussionId != null)
            {
                _GroupName = DiscussionManager.GetGroupName( (uint)(Comment.DiscussionId) );
                DiscussionHubConnection.InvokeAsync("JoinRoom", _GroupName);
            }

            _authenticatedUser = PageLayout.AuthenticatedUser!;
            _userRoles = PageLayout.UserRoles;
            _Liked = LikedComments.Any(uLike => uLike.Comment.Equals(Comment));
            _Disliked = DislikedComments.Any(uDisLike => uDisLike.Comment.Equals(Comment)); 
            _allowSee = !Comment.HasSpoiler;
        }

		private async void AddLike()
        {
            _isNaveGationClick = false;

			if (_Disliked)
            {
                await CommentManager.RemoveDesLikeAsync(Comment, _authenticatedUser.Id);
                _Disliked = false;
                UpdateDislike(_Disliked);
            }

            if (_Liked)
            {
                await CommentManager.RemoveLikeAsync(Comment, _authenticatedUser.Id);
                _Liked = false;
            }
            else
            {
                await CommentManager.AddLikeAsync(Comment, _authenticatedUser.Id);
                _Liked = true;
            }

            UpdateLike(_Liked);
            StateHasChanged();

        }

        private async void AddDeslike()
        {
			_isNaveGationClick = false;

			if (_Liked)
            {
                await CommentManager.RemoveLikeAsync(Comment, _authenticatedUser.Id);
                _Liked = false;
                UpdateLike(_Liked);
            }

            if (_Disliked)
            {
                await CommentManager.RemoveDesLikeAsync(Comment, _authenticatedUser.Id);
                _Disliked = false;
            }
            else
            {
                await CommentManager.AddDesLikeAsync(Comment, _authenticatedUser.Id);
                _Disliked = true;
            }

            UpdateDislike(_Disliked);
            StateHasChanged();
        }

        public async void UpdateSpoilerState(bool newState)
        {
            Comment.HasSpoiler = newState;

            if( await CommentManager.EditAsync(Comment) )
                StateHasChanged();
        }

        private void UpdateLike(bool newState )
        {
            if (newState)
            {
                if (!LikedComments.Any(u => u.Comment.Equals(Comment)))
                {
                    LikedComments.Add(
                            new UserLikedComment()
                            {
                                User = _authenticatedUser,
                                Comment = Comment,
                                UserId = _authenticatedUser.Id,
                                CommentId = Comment.Id
                            }
                        );
                }
            }
            else
            {
                foreach (var item in LikedComments)
                {
                    if (item.Comment.Equals(Comment))
                    {
                        LikedComments.Remove(item);
                        break;
                    }
                }
            }
        }

        private void UpdateDislike(bool newState)
        {
            if (newState)
            {
                if (!DislikedComments.Any(u => u.Comment.Equals(Comment)))
                {
                    DislikedComments.Add(
                            new UserDislikedComment()
                            {
                                User = _authenticatedUser,
                                Comment = Comment,
                                UserId = _authenticatedUser.Id,
                                CommentId = Comment.Id
                            }
                        );
                }
            }
            else
            {
                foreach (var item in DislikedComments)
                {
                    if (item.Comment.Equals(Comment))
                    {
                        DislikedComments.Remove(item);
                        break;
                    }
                }
            }
        }

        private async void Follow(string id)
        {
			_isNaveGationClick = false;

			if (await UserManager.Follow(_authenticatedUser.Id, id))
            {
                StateHasChanged();
                await PageLayout.Menu.ReRender();
                await OnChange.InvokeAsync();
            }
        }

        private async void UnFollow(string id)
        {
			_isNaveGationClick = false;

			if (await UserManager.UnFollow(_authenticatedUser.Id, id))
            {
                StateHasChanged();
                await PageLayout.Menu.ReRender();
                await OnChange.InvokeAsync();
            }
        }

        private void OpenAttachment(byte[] attachment)
        {
            _isNaveGationClick = false;
            _attachementView.Attachment = attachment;
            _attachementView.Name = "View Attachement";
            _attachementView.TrigerStatehasChanged();
            _attachementView.Open();
        }

        private async void RemoveComment()
        {
			_isNaveGationClick = false;

            await OnRemove.InvokeAsync(Comment.Id);

            DiscussionHubConnection?.InvokeAsync("NotifyGroupRemovedComment",
                                                    _GroupName,
                                                    Comment.Id
                                                );
        }

        private async void Navegate() 
        {
            if (AllowNavegation && _isNaveGationClick) 
            {
                int? MovieId = ( await MovieManager.GetFirstByConditionAsync(m => m.Id == Comment.MovieId ))?.MovieId;

                if(MovieId != null)
                    NavigationManager.NavigateTo($"MovieDetails/{MovieId}/Comments");
            }

            _isNaveGationClick = true;
        }

        // WARN: May be Implemented in a later date
        private async void RemoveAttachment()
        {
            Console.WriteLine("Removing attachment with id " + Comment.Id);
            Console.WriteLine("Comment Content: " + Comment.Content);
            // await CommentManager.RemoveAttachmentAsync(comment, attachment);
            await OnChange.InvokeAsync();
        }

    }
}
