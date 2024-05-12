using CineSync.Components.Layout;
using CineSync.Data;
using CineSync.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using CineSync.DbManagers;
using CineSync.Data.Models;


namespace CineSync.Components.Pages
{
    public partial class ManageCollection : ComponentBase
    {

        //TODO: get the list of all movies from the collection
        [Inject]
        private CollectionsManager CollectionsManager { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        private MainLayout MainLayout { get; set; }

        private ApplicationUser? AuthenticatedUser { get; set; }

        private ICollection<CollectionsMovies> AllMovies { get; set; } = new List<CollectionsMovies> (0);

        [Parameter]
        public string? CollectionName { get; set; }

        private bool isEditing = false;

        protected override async Task OnInitializedAsync()
        {
            MainLayout = LayoutService.MainLayout;
            AuthenticatedUser = MainLayout.AuthenticatedUser;
            await GetCollection();
        }

        private async Task GetCollection()
        {
            var collections = await CollectionsManager.GetUserCollections(AuthenticatedUser.Id);
            var collection = collections.FirstOrDefault(collection => collection.Name == CollectionName);
            if (collection != null)
                AllMovies = collection.CollectionMovies.ToList();
        }

        private void StartEditing()
        {
            isEditing = true;
        }

        private void ConfirmEdit(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                isEditing = false;
            }
        }

    }
}
