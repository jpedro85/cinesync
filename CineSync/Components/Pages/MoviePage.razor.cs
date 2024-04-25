using Microsoft.AspNetCore.Components;

namespace CineSync.Components.Pages
{
	public partial class MoviePage : ComponentBase
	{
		private string MoviePosterBase64;

		[Parameter]
		public int MovieId { get; set; }

		private bool InViewed { get; set; }
	}
}
