using Microsoft.AspNetCore.Components;
using CineSync.Data.Models;
using CineSync.DbManagers;
using CineSync.Services;
using CineSync.Data;

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

        [Parameter]
        public bool Liked { get; set; } = false;

        [Parameter]
        public bool DisLiked { get; set; } = false;

        private bool _Liked { get; set; }
        private bool _Disliked { get; set; }

        public delegate void Action();

        [Parameter]
        public Action OnRemove { get; set; } = () => { };

        private ApplicationUser _authenticatedUser;
        private ICollection<string> _userRoles;

        protected override void OnInitialized()
        {
            _authenticatedUser = LayoutService.MainLayout.AuthenticatedUser!;
            _userRoles = LayoutService.MainLayout.UserRoles;
            _Liked = Liked;
            _Disliked = DisLiked;

        }

        private async void AddLike(Comment commentAddLike)
        {
            if (_Disliked)
            {
                await CommentManager.RemoveDesLikeAsync(commentAddLike, _authenticatedUser.Id);
                _Disliked = false;
            }

            if (_Liked)
            {
                await CommentManager.RemoveLikeAsync(commentAddLike, _authenticatedUser.Id);
                _Liked = false;
            }
            else
            {
                await CommentManager.AddLikeAsync(commentAddLike, _authenticatedUser.Id);
                _Liked = true;
            }

            Console.WriteLine(_Liked + ":" + _Disliked);

            StateHasChanged();
        }

        private async void AddDeslike(Comment commentAddDesLike)
        {
            if (_Liked)
            {
                await CommentManager.RemoveLikeAsync(commentAddDesLike, _authenticatedUser.Id);
                _Liked = false;
            }

            if (_Disliked)
            {
                await CommentManager.RemoveDesLikeAsync(commentAddDesLike, _authenticatedUser.Id);
                _Disliked = false;
            }
            else
            {
                await CommentManager.AddDesLikeAsync(commentAddDesLike, _authenticatedUser.Id);
                _Disliked = true;
            }

            Console.WriteLine(_Liked + ":" + _Disliked);

            StateHasChanged();
        }

        private async void Follow(string id)
        {
            if (await UserManager.Follow(_authenticatedUser.Id, id))
            {
                if (_authenticatedUser.Following == null)
                    _authenticatedUser.Following = new List<ApplicationUser>();

                _authenticatedUser.Following.Add(Comment.Autor!);

                StateHasChanged();
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
            }
        }

        private async void RemoveComment()
        {
            await CommentManager.RemoveAsync(Comment);
            OnRemove();
        }

        private async void RemoveAttachment(Comment comment, CommentAttachment attachment)
        {
            await CommentManager.RemoveAttachmentAsync(comment, attachment);
            OnRemove();
        }

    }
}
