namespace CineSync.Data.Models
{
    public class MessageAttachement
    {
        public uint Id { get; set; }

        public uint MessageId { get; set; }

        public byte[] Attachment { get; set; }

        public Message Message { get; set; }
    }
}
