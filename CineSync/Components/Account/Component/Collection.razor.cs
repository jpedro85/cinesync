using CineSync.Data.Models;
using CineSync.Data;
using Microsoft.AspNetCore.Components;
using CineSync.Components.PopUps;
using CineSync.Components.Layout;


namespace CineSync.Components.Account.Component
{
    public partial class Collection : ComponentBase
    {
        [CascadingParameter(Name = "PageLayout")]
        public PageLayout PageLayout { get; set; }

        [Parameter]
        public MovieCollection MovieCollection { get; set; }

        [Parameter]
        public ApplicationUser? AuthenticatedUser { get; set; }

        [Parameter]
        public EventCallback OnRemove { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private ICollection<string> DefaultCollectionsNames { get; set; } = new string[] { "Favorites", "Watched", "Classified", "Watch Later" };

        private RemoveCollection _popupRemove;

        protected override void OnInitialized()
        {
            AuthenticatedUser = PageLayout.AuthenticatedUser;
        }

        private void MovieClickHandler(Movie movie)
        {
            NavigationManager.NavigateTo($"MovieDetails/{movie.MovieId}");
        }

        private void GoToCollectionManager()
        {
            NavigationManager.NavigateTo($"ManageCollection/{MovieCollection.Name!.Replace(" ", "_")}");
        }

    }
}
