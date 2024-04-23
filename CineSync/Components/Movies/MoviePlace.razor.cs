using Microsoft.AspNetCore.Components;
using CineSync.Data.Models;

namespace CineSync.Components.Movies
{
    public partial class MoviePlace
    {

        private string MoviePosterBase64 = string.Empty;

        [Parameter]
        public Movie Movie { get; set; }

        [Parameter]
        public bool UseRatingCs { get; set; } = false;

        [Parameter]
        public bool UseIMDBRating { get; set; } = false;

        [Parameter]
        public bool UseTitle { get; set; } = false;

        protected override void OnInitialized()
        {
            if(Movie.PosterImage != null)
                MoviePosterBase64 = Convert.ToBase64String(Movie.PosterImage);
        }
    }
}
