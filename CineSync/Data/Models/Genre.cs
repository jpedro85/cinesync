using System.ComponentModel.DataAnnotations;

namespace CineSync.Data.Models
{
	public class Genre
	{
		public int Id { get; set; }

		public int? TmdbId { get; set; }

		[Required]
		public string? Name { get; set; }
	}
}
