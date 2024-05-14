using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.Movies
{
    public partial class ManagerCollectionMovies : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public CollectionsManager CollectionsManager { get; set; }

        [Parameter]
        public ApplicationUser? AuthenticatedUser { get; set; }

        [Parameter]
        public ICollection<CollectionsMovies> AllMovies { get; set; }

        [Parameter]
        public MovieCollection Collection { get; set; }

        private async void RemoveMovieFromCollection(CollectionsMovies collectionsMovies)
        {
            await CollectionsManager.RemoveMovieFromCollectionAsync(AuthenticatedUser.Id, collectionsMovies.MovieCollection.Name, collectionsMovies.Movie.Id);
            AllMovies.Remove(collectionsMovies);
            StateHasChanged();
        }

        private void MovieClickHandler(Movie movie)
        {
            NavigationManager.NavigateTo($"/MovieDetails/{movie.MovieId}");
        }
    }
}
