using CineSync.Data;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using CineSync.Data.Models;
using CineSync.Components.Buttons;

namespace CineSync.Components.Pages
{
    public partial class MoviePage : ComponentBase
    {

        [Parameter]
        public int MovieId { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        public ApplicationDbContext ApplicationDbContext { get; set; }

        private bool InViewed { get; set; }

        private bool InFavourites { get; set; }

        private string MovieTitle { get; set; }

        private readonly string _youtubeLink = "https://www.youtube.com/embed/";

        private string MoviePosterBase64;

        private Movie Movie { get; set; }

        private TabButton TabButtonComments { get; set; }

        private TabButton TabButtonDiscution { get; set; }

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

        private void OnRatingSaved()
        {
            StateHasChanged();
        }

    }
}
