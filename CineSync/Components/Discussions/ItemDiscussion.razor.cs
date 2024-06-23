using CineSync.DbManagers;
using Microsoft.AspNetCore.Components;
using CineSync.Data.Models;
using CineSync.Data;
using CineSync.Components.Comments;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Components.Web;
using CineSync.Components.Layout;
using CineSync.Components.PopUps;

namespace CineSync.Components.Discussions
{
    public partial class ItemDiscussion : ComponentBase
    {
        [CascadingParameter(Name = "PageLayout")]
        public PageLayout PageLayout { get; set; }

        [Inject]
        private UserManager UserManager { get; set; }

        [Inject]
        private DiscussionManager DiscussionManager { get; set; }

        [Inject]
        private CommentManager CommentManager { get; set; }

        [Inject]
        private DbManager<UserLikedComment> DbUserLikedComment { get; set; }

        [Inject]
        private DbManager<UserDislikedComment> DbUserDislikedComment { get; set; }

        [Parameter, EditorRequired]
        public ICollection<UserLikedDiscussion> LikedDiscussions { get; set; }

        [Parameter, EditorRequired]
        public ICollection<UserDislikedDiscussion> DislikedDiscussions { get; set; }

        public ICollection<UserLikedComment> _likedComments { get; set; } = new List<UserLikedComment>(0);

        public ICollection<UserDislikedComment> _dislikedComments { get; set; } = new List<UserDislikedComment>(0);

        [Parameter]
        public Discussion Discussion { get; set; }

        [Parameter]
        public bool AllowFollow { get; set; } = true;

        [Parameter]
        public EventCallback OnChange { get; set; }

        [Parameter]
        public EventCallback<uint> OnRemove { get; set; }


        private RemoveDiscussion _popupRemove;

        private bool _Liked = false;

        private bool _Disliked = false;

        private ApplicationUser? _authenticatedUser;

        private ICollection<string> _userRoles;

        private NewComment _newComment;

        private bool _DoComment = false;

        private bool _allowSee = false;

        protected override void OnInitialized()
        {
            _authenticatedUser = PageLayout.AuthenticatedUser;
            _userRoles = PageLayout.UserRoles;
            _Liked = LikedDiscussions.Any(uLike => uLike.Discussion.Equals(Discussion));
            _Disliked = DislikedDiscussions.Any( uDisLike => uDisLike.Discussion.Equals( Discussion) );
            _allowSee = !Discussion.HasSpoiler;
        }
        
        protected async void GetDiscutionInfo()
        {
            Console.WriteLine(Discussion);
            Discussion.Comments = await CommentManager.GetCommentsOfDiscussion(Discussion.Id);
            GetUserStatusComments();
            UpdateSpoilerState();
            StateHasChanged();
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


        }

        private async void Remove()
        {
            uint id = (Discussion.Id);
            await DiscussionManager.RemoveAsync(Discussion);
            await OnRemove.InvokeAsync(id);
        }

        private void ToogleComment()
        {
            _DoComment = !_DoComment;
        }

        //TODO call this when a comment is removed or edited
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
    }
}
