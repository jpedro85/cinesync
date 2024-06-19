using Microsoft.AspNetCore.Components;
using CineSync.Data.Models;
using CineSync.DbManagers;
using CineSync.Services;
using CineSync.Data;
using CineSync.Components.PopUps;
using Microsoft.AspNetCore.Components.Web;
using CineSync.Components.Layout;

namespace CineSync.Components.Comments
{
    public partial class ItemComment : ComponentBase
    {

        [Parameter]
        public ICollection<UserLikedComment> LikedComments { get; set; }

        [Parameter]
        public ICollection<UserDislikedComment> DislikedComments { get; set; }

        [Parameter]
        public Comment Comment { get; set; }

        [Parameter]
        public bool Liked { get; set; } = false;
        private bool _Liked {  get; set; }

        [Parameter]
        public bool DisLiked { get; set; } = false;
		private bool _Disliked { get; set; }

        [Parameter]
        public bool AllowFollow { get; set; } = true;

        [Parameter]
        public EventCallback OnChange { get; set; }

        [Inject]
        private UserManager UserManager { get; set; }

        [Inject]
        private CommentManager CommentManager { get; set; }


        [Inject]
        private LayoutService LayoutService { get; set; }

        public delegate void Action();

        public delegate void LikeStatusChange(bool newStatus);

        private bool _Liked { get; set; }

        private bool _Disliked { get; set; }

        private ApplicationUser _authenticatedUser;

        private ICollection<string> _userRoles;

        protected override void OnInitialized()
        {
            _authenticatedUser = LayoutService.MainLayout.AuthenticatedUser!;
            _userRoles = LayoutService.MainLayout.UserRoles;
            _Liked = Liked;
            _Disliked = DisLiked;
        }

		private async void AddLike()
        {
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
                    if (item.Equals(Comment))
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
                    if (item.Equals(Comment)) 
                    {
                        DislikedComments.Remove(item);
                        break;
                    }
                }
            }
        }

        private async void Follow(string id)
        {
            if (await UserManager.Follow(_authenticatedUser.Id, id))
            {
                if (_authenticatedUser.Following == null)
                    _authenticatedUser.Following = new List<ApplicationUser>();

                _authenticatedUser.Following.Add(Comment.Autor!);

                StateHasChanged();
                LayoutService.MainLayout.TriggerMenuReRender();
                await OnChange.InvokeAsync();
            }
        }

        private async void UnFollow(string id)
        {
            if (await UserManager.UnFollow(_authenticatedUser.Id, id))
            {
                if (_authenticatedUser.Following == null)
                    _authenticatedUser.Following = new List<ApplicationUser>();

                _authenticatedUser.Following = _authenticatedUser.Following.Where(u => u.Id != Comment.Autor!.Id).ToList();

                StateHasChanged();
                LayoutService.MainLayout.TriggerMenuReRender();
                await OnChange.InvokeAsync();
            }
        }

        private async void RemoveComment()
        {
            await OnChange.InvokeAsync();
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
