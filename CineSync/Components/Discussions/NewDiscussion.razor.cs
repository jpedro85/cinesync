using CineSync.Data.Models;
using CineSync.Data;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using CineSync.Components.Comments;

namespace CineSync.Components.Discussions
{
    public partial class NewDiscussion
    {
        private Discussion _discussion { get; set; } = new Discussion();

        private bool _addFirstComment = false;

        private NewComment _newComment { get; set; }

        private void ToogleFistComment()
        {
            _addFirstComment = !_addFirstComment;
            InvokeAsync(StateHasChanged);
        }

        public Discussion GetDiscussion()
        {
            if (_addFirstComment)
            {
                _discussion.Comments = new List<Comment>();
                _discussion.Comments.Add(_newComment.GetComment());
            }

            return _discussion;
        }

        public void Reset()
        {
            _discussion = new Discussion();
            _newComment.Reset();
        }
    }
}
