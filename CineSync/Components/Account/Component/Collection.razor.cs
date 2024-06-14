using CineSync.Data.Models;
using CineSync.Data;
using Microsoft.AspNetCore.Components;


namespace CineSync.Components.Account.Component
{
    public partial class Collection : ComponentBase
    {

        [Parameter]
        public MovieCollection MovieCollection { get; set; }

        [Parameter]
        public ApplicationUser? AuthenticatedUser { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private ICollection<MovieCollection>? movieCollections { get; set; }

        private ICollection<string> DefaultCollectionsNames { get; set; } = new string[] { "Favorites", "Watched", "Classified", "Watch Later" };

        private void MovieClickHandler(Movie movie)
        {
            NavigationManager.NavigateTo($"MovieDetails/{movie.MovieId}");
        }

        private void GoToCollectionManager()
        {
            NavigationManager.NavigateTo($"ManageCollection/{MovieCollection.Name!.Replace(" ", "_")}");
        }

        private async void RemoveCollection()
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
