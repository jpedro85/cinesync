﻿using Microsoft.AspNetCore.Components;
using CineSync.Utils.Adapters.ApiAdapters;

namespace CineSync.Components.Movies
{
    public partial class MoviePlace : ComponentBase
    {

        private string MoviePosterBase64 = string.Empty;

        [Parameter]
        public MovieSearchAdapter? Movie { get; set; }

        [Parameter]
        public bool UseRatingCs { get; set; } = false;

        [Parameter]
        public bool UseIMDBRating { get; set; } = false;

        [Parameter]
        public bool UseTitle { get; set; } = false;

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