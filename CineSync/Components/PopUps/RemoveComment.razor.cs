using CineSync.DbManagers;
using CineSync.Data.Models;
using Microsoft.AspNetCore.Components;


namespace CineSync.Components.PopUps
{
    public partial class RemoveComment : ComponentBase
    {
        private static uint _counter = 0;

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
            Id += _counter++;
            Console.WriteLine($"CommentId {Comment.Id} {Id}");
        }


        private async void ExecuteRemoveComment()
        {
            Console.WriteLine($"CommentId {Comment.Id} {Id}");
            if (await CommentManager.RemoveAsync(Comment))
            {
                await OnRemove.InvokeAsync();
            }
            else
            {
                ErrorMessage = "Something occured an we were unable to remove the Comment.";
            }
        }

        public void Open() 
        {
            Console.WriteLine($"OpenCommentId {Comment.Id} {Id}");
            PopUpLayout.Open();
            Console.WriteLine($"OpenCommentId {Comment.Id} {Id}");
        }

        public void Close()
        {
            PopUpLayout.Close();
        }
    }
}
