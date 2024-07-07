namespace CineSync.Data.Models
{
    public class Reaction
    {   
        public uint Id { get; set; }
        public ApplicationUser Autor { get; set; } = default!;
        public string ReactionContent { get; set; } = default!;
    }
}
