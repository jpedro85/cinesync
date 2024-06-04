using Microsoft.AspNetCore.Components;
using CineSync.Data.Models;
using CineSync.DbManagers;

namespace CineSync.Components.Comments
{
    public partial class ItemComment
    {
        [Inject]
        private CommentManager CommentManager { get; set; }

        [Parameter]
        public Comment Comment { get; set; }

        private async void AddLike(Comment commentAddLike)
        {
            await CommentManager.AddLikeAsync(commentAddLike);
            StateHasChanged();
        }

        private async void AddDeslike(Comment commentAddDesLike)
        {
            await CommentManager.AddDesLikeAsync(commentAddDesLike);
            StateHasChanged();
        }

    }
}
