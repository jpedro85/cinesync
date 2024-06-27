using CineSync.Data.Models;
using CineSync.Data;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using CineSync.Components.Comments;
using Microsoft.IdentityModel.Tokens;

namespace CineSync.Components.Discussions
{
    public partial class NewDiscussion
    {
        [Parameter]
        public bool AllowFirsComment { get; set; } = true;

        [Parameter]
        public string Title { get; set; } = string.Empty;

        public Discussion Discussion { get; private set; } = new Discussion();

        private bool _addFirstComment = false;

        public NewComment FirstComment { get; private set; }

        protected override void OnInitialized()
        {
            Discussion.Title = Title;
        }

        private void ToogleFistComment()
        {
            _addFirstComment = !_addFirstComment;
            InvokeAsync(StateHasChanged);
        }

        public void Reset()
        {
            Discussion = new Discussion();
            Discussion.Title = Title;

            InvokeAsync(StateHasChanged);

            if (_addFirstComment)
            {
                FirstComment?.Reset();
            }
        }
    }
}
