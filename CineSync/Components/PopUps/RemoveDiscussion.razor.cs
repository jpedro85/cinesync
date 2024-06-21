using CineSync.Data.Models;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.PopUps
{
    public partial class RemoveDiscussion
    {
        [Parameter]
        public Discussion Discussion { get; set; }

        [Parameter]
        public EventCallback OnRemove { get; set; }

        private PopUpLayout PopUpLayout { get; set; }

        private string Id { get; set; } = "RemoveDiscussionModal_";

        protected override void OnInitialized()
        {
            Id += Discussion.Id;
        }

        private async void ExecuteRemoveComment()
        {
            await OnRemove.InvokeAsync();
        }
    }
}
