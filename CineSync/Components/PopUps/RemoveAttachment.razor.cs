using CineSync.DbManagers;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.PopUps
{
    public partial class RemoveAttachment : ComponentBase
    {

        [Parameter]
        public int KeyHash { get; set; }

        [Parameter]
        public EventCallback<int> OnRemove { get; set; }

        [Inject]
        public CommentManager CommentManager { get; set; }

        private PopUpLayout PopUpLayout { get; set; }

        protected override void OnInitialized()
        {
        }

        private async void RemoveAttachmentFromComment()
        {
            await OnRemove.InvokeAsync(KeyHash);
        }

    }
}
