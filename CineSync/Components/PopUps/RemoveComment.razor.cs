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

        private PopUpLayout PopUpLayout { get; set; }

        private string Id { get; set; } = "RemoveCommentModal_";

        protected override void OnInitialized()
        {
            Id += Comment.Id;
        }


        private async void ExecuteRemoveComment()
        {
            await OnRemove.InvokeAsync();
        }
    }
}
