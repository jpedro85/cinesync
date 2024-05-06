namespace CineSync.Components.Comments
{
    public partial class Comments
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string CommentText { get; set; }
        public DateTime Created { get; set; }

        public Comments commentModel = new Comments();

        public async Task addComment()
        {

        }
    }
}
