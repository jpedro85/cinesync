using CineSync.DbManagers;
using Microsoft.AspNetCore.Components;
using CineSync.Data.Models;
using CineSync.Data;
using CineSync.Components.Comments;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Components.Web;
using CineSync.Components.Layout;
using CineSync.Components.PopUps;
using Microsoft.AspNetCore.SignalR.Client;
using SQLitePCL;
using Microsoft.JSInterop;

namespace CineSync.Components.Discussions
{
    public partial class ItemDiscussion : ComponentBase
    {
        [CascadingParameter(Name = "PageLayout")]
        public PageLayout PageLayout { get; set; } = default!;

        [CascadingParameter(Name = "DiscussionHubConnection")]
        public HubConnection DiscussionHubConnection { get; set; } = default!;

        [Inject]
        private MovieManager MovieManager { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        private UserManager UserManager { get; set; } = default!;

        [Inject]
        private DiscussionManager DiscussionManager { get; set; } = default!;

        [Inject]
        private CommentManager CommentManager { get; set; } = default!;

        [Inject]
        private DbManager<UserLikedComment> DbUserLikedComment { get; set; } = default!;

        [Inject]
        private DbManager<UserDislikedComment> DbUserDislikedComment { get; set; } = default!;

        [Parameter, EditorRequired]
        public ICollection<UserLikedDiscussion> LikedDiscussions { get; set; } = default!;

        [Parameter, EditorRequired]
        public ICollection<UserDislikedDiscussion> DislikedDiscussions { get; set; } = default!;

        public ICollection<UserLikedComment> _likedComments { get; set; } = default!;

        public ICollection<UserDislikedComment> _dislikedComments { get; set; } = default!;

        [Parameter]
        public Discussion Discussion { get; set; } = default!;

        [Parameter]
        public bool AllowFollow { get; set; } = true;

        [Parameter]
        public EventCallback OnChange { get; set; }

        [Parameter]
        public EventCallback<uint> OnRemove { get; set; }

        [Parameter]
        public EventCallback<Discussion> OnCreate { get; set; } = default;

        [Parameter]
        public bool AllowNavegation { get; set; } = false;
        //private bool _isNaveGationClick = true;

        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        private RemoveDiscussion _popupRemove = default!;

        private bool _Liked = false;

        private bool _Disliked = false;

        private ApplicationUser? _authenticatedUser;

        private ICollection<string> _userRoles = default!;

        private NewComment _newComment = default!;

        private bool _DoComment = false;

        private bool _allowSee = false;

        private bool _fetchedInfo = false;

        private string _GroupName = default!;

        protected override void OnInitialized()
        {
            _GroupName = DiscussionManager.GetGroupName(Discussion);
            DiscussionHubConnection.InvokeAsync("JoinRoom", _GroupName);
            SubscriveToEvents();
            _authenticatedUser = PageLayout.AuthenticatedUser;
            _userRoles = PageLayout.UserRoles;
            _Liked = LikedDiscussions.Any(uLike => uLike.Discussion.Equals(Discussion));
            _Disliked = DislikedDiscussions.Any( uDisLike => uDisLike.Discussion.Equals( Discussion) );
            _allowSee = !Discussion.HasSpoiler;
        }
        
        private async void GetDiscutionInfo()
        {
            Discussion.Comments = await CommentManager.GetCommentsOfDiscussion(Discussion.Id);
            GetUserStatusComments();
            UpdateSpoilerState();
            StateHasChanged();
            _fetchedInfo = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
                await JS.InvokeVoidAsync("scrollToBottom", "scrollable_messages");
        }

        private void SubscriveToEvents() 
        {

            DiscussionHubConnection.On<string, int, int>("Update", 
                    (roomName, nlikes, nDislikes) => 
                    { 
                        if (roomName == _GroupName) UpdateLikesDislikes(nlikes, nDislikes); 
                    }
                );

            DiscussionHubConnection.On<string, uint>("NewComment",
                    (roomName, commentId) =>
                    {
                        if (roomName == _GroupName) UpdateNewComment(commentId);
                    }
                );

            DiscussionHubConnection.On<string, uint>("RemoveComment",
                    (roomName, commentId) =>
                    {
                        if (roomName == _GroupName) UpdateRemoveComment(commentId);
                    }
                );
        }

        private void GetUserStatusComments()
        {
            if (_authenticatedUser != null)
            {
                _likedComments = DbUserLikedComment.GetByConditionAsync(
                            likedComment =>
                            likedComment.Comment.DiscussionId == Discussion.Id &&
                            likedComment.UserId == _authenticatedUser.Id
                            ).Result.ToList();

                _dislikedComments = DbUserDislikedComment.GetByConditionAsync(
                            likedComment =>
                            likedComment.Comment.DiscussionId == Discussion.Id &&
                            likedComment.UserId == _authenticatedUser.Id
                            ).Result.ToList();
            }
            else 
            {
                _likedComments = new List<UserLikedComment>(0);
                _dislikedComments = new List<UserDislikedComment>(0);
            }
        }


        private async void Follow(string id)
        {
            if (await UserManager.Follow(_authenticatedUser!.Id, id))
            {
                StateHasChanged();
                await PageLayout.Menu.ReRender();
                await OnChange.InvokeAsync();
            }
        }

        private async void UnFollow(string id)
        {
            if (await UserManager.UnFollow(_authenticatedUser!.Id, id))
            {
                StateHasChanged();
                await PageLayout.Menu.ReRender();
                await OnChange.InvokeAsync();
            }
        }

        private async void AddLike()
        {
            if (_Disliked)
            {
                await DiscussionManager.RemoveDesLikeAsync(Discussion, _authenticatedUser!.Id);
                _Disliked = false;
                UpdateDislike(_Disliked);
            }

            if (_Liked)
            {
                await DiscussionManager.RemoveLikeAsync(Discussion, _authenticatedUser!.Id);
                _Liked = false;
            }
            else
            {
                await DiscussionManager.AddLikeAsync(Discussion, _authenticatedUser!.Id);
                _Liked = true;
            }

            UpdateLike(_Liked);
            StateHasChanged();
        }

        private async void AddDeslike()
        {

            if (_Liked)
            {
                await DiscussionManager.RemoveLikeAsync(Discussion, _authenticatedUser!.Id);
                _Liked = false;
                UpdateLike(_Liked);
            }

            if (_Disliked)
            {
                await DiscussionManager.RemoveDesLikeAsync(Discussion, _authenticatedUser!.Id);
                _Disliked = false;
            }
            else
            {
                await DiscussionManager.AddDesLikeAsync(Discussion, _authenticatedUser!.Id);
                _Disliked = true;
            }

            DiscussionHubConnection.InvokeAsync("NotifyGroupUpdateDiscussion",
                                                    _GroupName,
                                                    Discussion.NumberOfLikes,
                                                    Discussion.NumberOfDeslikes
                                                );

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
                                User = _authenticatedUser!,
                                Discussion = Discussion,
                                UserId = _authenticatedUser!.Id,
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
                                User = _authenticatedUser!,
                                Discussion = Discussion,
                                UserId = _authenticatedUser!.Id,
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
            commentToAdd.Autor = _authenticatedUser!;

            if (!commentToAdd.Content.IsNullOrEmpty())
            {
                await CommentManager.AddCommentToDiscussion(commentToAdd, Discussion.Id, _authenticatedUser!.Id);
                _newComment.Reset();
                UpdateSpoilerState();
                StateHasChanged();
            }

            await DiscussionHubConnection.InvokeAsync("NotifyGroupNewComment", _GroupName, commentToAdd.Id);

            await JS.InvokeVoidAsync("scrollToBottom", "scrollable_messages");

        }

        private async void Remove()
        {
            uint id = (Discussion.Id);
            await DiscussionManager.DeleteAsync(Discussion);
            await OnRemove.InvokeAsync(id);
        }

        private void ToogleComment()
        {
            _DoComment = !_DoComment;
        }

        public async void UpdateSpoilerState()
        {
            bool newSpoilerState = false;

            if(Discussion.Comments != null) 
            {
                foreach (var item in Discussion.Comments)
                {
                    if (item.HasSpoiler) 
                    {
                        newSpoilerState = true;
                        break;
                    }
                }
            }

            Discussion.HasSpoiler = newSpoilerState;

            await DiscussionManager.EditAsync(Discussion);
        }

        private async void Navegate()
        {
            if (AllowNavegation)
            {
                int? MovieId = (await MovieManager.GetFirstByConditionAsync(m => m.Id == Discussion.MovieId))?.MovieId;

                if (MovieId != null)
                    NavigationManager.NavigateTo($"MovieDetails/{MovieId}/Discussions");
            }

            //_isNaveGationClick = true;
        }

        private void UpdateLikesDislikes(int nLikes,int nDislikes) 
        {
            Discussion.NumberOfLikes = nLikes;
            Discussion.NumberOfDeslikes = nDislikes;

            InvokeAsync(StateHasChanged);
        }

        private async void UpdateNewComment(uint commentId) 
        {

            Comment? comment = await CommentManager.GetFirstByConditionAsync(c => c.Id == commentId, "Attachements", "Autor");

            if (comment != null)
            {
                if (Discussion.Comments == null)
                    Discussion.Comments = new List<Comment>();

                Discussion.Comments.Add(comment);

                await InvokeAsync(StateHasChanged);

                await JS.InvokeVoidAsync("scrollToBottom", "scrollable_messages");
            }
        }

        private void UpdateRemoveComment(uint commentId)
        {
            Comment? comment = Discussion.Comments?.Where(c => c.Id == commentId).FirstOrDefault();

            if (comment != null)
            {

                Discussion.Comments!.Remove(comment);

                InvokeAsync(StateHasChanged);
            }
        }
    }
}
