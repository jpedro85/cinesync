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
        public Discussion Discussion { get; private set; } = new Discussion();

        private bool _addFirstComment = false;

        public NewComment FirstComment { get; private set; }

        private void ToogleFistComment()
        {
            _addFirstComment = !_addFirstComment;
            InvokeAsync(StateHasChanged);
        }

        public void Reset()
        {
            Discussion = new Discussion();

            InvokeAsync(StateHasChanged);

            if (_addFirstComment)
            {
                FirstComment?.Reset();
            }
        }
    }
}
