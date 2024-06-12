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
        public EventCallback OnChange { get; set; }

        [Parameter]
        public Comment Comment { get; set; }

        [Parameter]
        public bool Liked { get; set; } = false;

		[Parameter]
        public bool DisLiked { get; set; } = false;

        [Parameter]
        public bool AllowFollow { get; set; } = true;

        private bool _Liked {  get; set; }
		private bool _Disliked { get; set; }

		public delegate void Action();
        public delegate void LikeStatusChange(bool newStatus);

        [Parameter]
        public Action OnRemove { get; set; } = () => { };
        [Parameter]
        public LikeStatusChange OnDislikeChange { get; set; } = (s) => { };
        [Parameter]
        public LikeStatusChange OnlikeChange { get; set; } = (s) => { };

        [Inject]
        private UserManager UserManager { get; set; }

        [Inject]
        private CommentManager CommentManager { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        private bool _Liked { get; set; }

        private bool _Disliked { get; set; }

        private Comment _commentToRemove;

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
                OnDislikeChange(_Disliked);
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

            OnlikeChange(_Liked);
            StateHasChanged();
        }

        private async void AddDeslike(Comment commentAddDesLike)
        {
            if (_Liked)
            {
                await CommentManager.RemoveLikeAsync(commentAddDesLike, _authenticatedUser.Id);
                _Liked = false;
                OnlikeChange(_Liked);
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

            OnDislikeChange(_Disliked);
            StateHasChanged();
        }

        private async void Follow(string id)
        {
            if (await UserManager.Follow(_authenticatedUser.Id, id))
            {
                if (_authenticatedUser.Following == null)
                    _authenticatedUser.Following = new List<ApplicationUser>();

                _authenticatedUser.Following.Add(Comment.Autor!);

                await OnChange.InvokeAsync();
                StateHasChanged();
                LayoutService.MainLayout.TriggerMenuReRender();
            }
        }

        private async void UnFollow(string id)
        {
            if (await UserManager.UnFollow(_authenticatedUser.Id, id))
            {
                if (_authenticatedUser.Following == null)
                    _authenticatedUser.Following = new List<ApplicationUser>();

                _authenticatedUser.Following = _authenticatedUser.Following.Where(u => u.Id != Comment.Autor!.Id).ToList();

                await OnChange.InvokeAsync();
                StateHasChanged();
                LayoutService.MainLayout.TriggerMenuReRender();
            }
        }

        private async void RemoveComment()
        {
            await CommentManager.RemoveAsync(Comment);
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
