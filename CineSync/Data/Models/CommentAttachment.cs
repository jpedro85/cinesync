namespace CineSync.Data.Models
{
    public class CommentAttachment
    {
        public uint Id { get; set; }
        public byte[] Attachment { get; set; }

        public Comment Comment {get; set;}
    }
}
