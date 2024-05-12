using CineSync.Data;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using CineSync.Data.Models;
using CineSync.Components.PopUps;
using Microsoft.AspNetCore.Components.Web;

namespace CineSync.Components.Pages
{
	public partial class MoviePage
	{
		private string MoviePosterBase64;

		[Inject]
		private HttpClient _client { get; set; }

		[Parameter]
		public int MovieId { get; set; }

		private bool InViewed { get; set; }

        private bool InFavourites { get; set; }

        private string MovieTitle { get; set; }

		[Inject]
		public ApplicationDbContext ApplicationDbContext { get; set; }

		private Movie Movie { get; set; }

        protected override async Task OnInitializedAsync()
		{
			Movie = await GetMovieDetails();
		}

		private async Task<Movie?> GetMovieDetails()
		{

			HttpResponseMessage response = await _client.GetAsync($"movie?id={MovieId}");

			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<Movie>(jsonResponse);
			}
			return null;

		}

	}
}
