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
        [CascadingParameter(Name = "PageLayout")]
        public PageLayout PageLayout { get; set; }


        [Inject]
        private UserManager UserManager { get; set; }

        [Inject]
        private CommentManager CommentManager { get; set; }


        [Parameter,EditorRequired]
        public ICollection<UserLikedComment> LikedComments { get; set; }

        [Parameter,EditorRequired]
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


        private bool _allowSee;

        private ApplicationUser _authenticatedUser;

        private ICollection<string> _userRoles;

        private PopUpAttachementView _attachementView;

        private RemoveComment _popUpRemove;

        protected override void OnInitialized()
		{
            _authenticatedUser = PageLayout.AuthenticatedUser!;
            _userRoles = PageLayout.UserRoles;
            _Liked = Liked;
            _Disliked = DisLiked;
            _allowSee = !Comment.HasSpoiler;
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
                StateHasChanged();
                await PageLayout.Menu.ReRender();
                await OnChange.InvokeAsync();
            }
        }

        private async void UnFollow(string id)
        {
            if (await UserManager.UnFollow(_authenticatedUser.Id, id))
            {
                StateHasChanged();
                await PageLayout.Menu.ReRender();
                await OnChange.InvokeAsync();
            }
        }

        private void OpenAttachment(byte[] attachment)
        {
            _attachementView.Attachment = attachment;
            _attachementView.Name = "View Attachement";
            _attachementView.TrigerStatehasChanged();
            _attachementView.Open();
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
