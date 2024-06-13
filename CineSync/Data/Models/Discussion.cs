using System.ComponentModel.DataAnnotations;

namespace CineSync.Data.Models
{
    public class Discussion 
    {
        public uint Id { get; set; }
        
        [Required]
        public string Title { get; set; }

        [Required]
        public ApplicationUser Autor { get; set; }

        public long NumberOfLikes { get; set; } = 0;

        public long NumberOfDeslikes { get; set; } = 0;

        [Required]
        public ICollection<Comment>? Comments { get; set; }
    }
}
