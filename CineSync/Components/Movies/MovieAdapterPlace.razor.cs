using CineSync.Core.Adapters.ApiAdapters;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.Movies
{
    public partial class MovieAdapterPlace : ComponentBase
    {
        private string MoviePosterBase64 = string.Empty;

        [Parameter]
        public MovieSearchAdapter? Movie { get; set; }

        public bool UseRatingCs { get; set; } = false;

        public bool UseIMDBRating { get; set; } = false;

        public bool UseTitle { get; set; } = false;

        [Parameter]
        public int Width { get; set; } = 300;

        [Parameter]
        public int Heigth { get; set; } = 450;

        [Parameter]
        public bool UseHoverTitle { get; set; } = true;

        protected override void OnInitialized()
        {
            if (Movie != null && Movie.PosterImage != null)
                MoviePosterBase64 = Convert.ToBase64String(Movie.PosterImage);
        }

        protected override void OnParametersSet()
        {
            if (Movie != null && Movie.PosterImage != null && Movie.PosterImage.Length > 0)
                MoviePosterBase64 = Convert.ToBase64String(Movie.PosterImage);
        }
    }
}
