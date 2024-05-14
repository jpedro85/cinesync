using CineSync.Components.Layout;
using CineSync.Data;
using CineSync.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using CineSync.DbManagers;
using CineSync.Data.Models;
using CineSync.Core.Adapters.ApiAdapters;
using Microsoft.IdentityModel.Tokens;


namespace CineSync.Components.Pages
{
    public partial class ManageCollection : ComponentBase
    {

        [Inject]
        private CollectionsManager CollectionsManager { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        private MainLayout MainLayout { get; set; }

        private ApplicationUser? AuthenticatedUser { get; set; }

        private ICollection<CollectionsMovies> AllMovies { get; set; } = new List<CollectionsMovies> (0);

        private ICollection<string> DefaultCollection { get; set; } = new List<string>();

        [Parameter]
        public string? CollectionName { get; set; }

        public MovieCollection Collection { get; set; }

        private bool isEditing = false;

        protected override async Task OnInitializedAsync()
        {
            MainLayout = LayoutService.MainLayout;
            AuthenticatedUser = MainLayout.AuthenticatedUser;
            await GetCollection();
            startDefaultCollectionList();
        }

        private void startDefaultCollectionList()
        {
            DefaultCollection.Add("Favorites");
            DefaultCollection.Add("Watched");
            DefaultCollection.Add("Classified");
            DefaultCollection.Add("Watch Later");
 
        }

        private async Task GetCollection()
        {
            var collections = await CollectionsManager.GetUserCollections(AuthenticatedUser.Id);
            Collection = collections.FirstOrDefault(collection => collection.Name == CollectionName.Replace("_"," "));
            if (Collection != null)
                AllMovies = Collection!.CollectionMovies!.ToList();
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

        private async void OnChangeName(string newName)
        {
            if(!newName.IsNullOrEmpty()) 
            {
                await CollectionsManager.ChangeCollectioName( AuthenticatedUser.Id, Collection.Name, newName);
                StateHasChanged();
            }
        }

        private async void OnChangePublic(bool newState)
        {
            await CollectionsManager.ChangePublicSate(AuthenticatedUser.Id, Collection.Name, newState);
            StateHasChanged();
        }
    }
}
