using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.Movies
{
    public partial class ManagerCollectionMovies : ComponentBase
    {

        [Parameter]
        public MovieCollection Collection { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public CollectionsManager CollectionsManager { get; set; }

        private ApplicationUser AuthenticatedUser { get; set; }

        private async void RemoveMovieFromCollection(CollectionsMovies collectionsMovies)
        {
            await CollectionsManager.RemoveMovieFromCollectionAsync(Collection.ApplicationUser.Id, collectionsMovies.MovieCollection.Name, collectionsMovies.Movie.Id);
            StateHasChanged();

        }

        private void MovieClickHandler(Movie movie)
        {
            NavigationManager.NavigateTo($"/MovieDetails/{movie.MovieId}");
        }
    }
}
