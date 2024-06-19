using System.ComponentModel.DataAnnotations;

namespace CineSync.Data.Models
{
    public class Discussion 
    {
        public uint Id { get; set; }

		public uint MovieId { get; set; }
        
        [Required]
        public string Title { get; set; }

        [Required]
        public ApplicationUser Autor { get; set; }

        public long NumberOfLikes { get; set; } = 0;

        public long NumberOfDeslikes { get; set; } = 0;

        public DateTime TimeStamp { get; set; } = DateTime.Now;

        [Required]
        public ICollection<Comment>? Comments { get; set; }

		public override bool Equals(object? obj)
		{

			if (obj == null || !(obj is Discussion))
			{
				return false;
			}

			return ((Discussion)obj).Id == this.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}
