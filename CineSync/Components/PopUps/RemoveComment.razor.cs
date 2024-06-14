using CineSync.DbManagers;
using CineSync.Data.Models;
using Microsoft.AspNetCore.Components;


namespace CineSync.Components.PopUps
{
    public partial class RemoveComment : ComponentBase
    {
        [Parameter]
        public Comment Comment { get; set; }

        [Parameter]
        public EventCallback OnRemove { get; set; }

        [Inject]
        private CommentManager CommentManager { get; set; }

        private PopUpLayout PopUpLayout { get; set; }

        private string Id { get; set; } = "RemoveCommentModal_";

        private string ErrorMessage { get; set; } = string.Empty;

        protected override void OnInitialized()
        {
            Id += Comment.Id;
        }


        private async void ExecuteRemoveComment()
        {
            if (await CommentManager.RemoveAsync(Comment))
            {
                await OnRemove.InvokeAsync();
            }
            else
            {
                ErrorMessage = "Something occured an we were unable to remove the Comment.";
            }
        }
    }
}
