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


        [Parameter]
        public string? CollectionName { get; set; }

        private PageLayout _pageLayout;
        private bool _inicialized = false;

        private ApplicationUser? AuthenticatedUser { get; set; }

        private ICollection<CollectionsMovies> AllMovies { get; set; } = new List<CollectionsMovies>(0);

        private ICollection<string> DefaultCollection { get; set; } = new string[] { "Favorites", "Watched", "Classified", "Watch Later" };

        public MovieCollection Collection { get; set; }

        private bool isEditing = false;

        private async void Initialize()
        {
            AuthenticatedUser = _pageLayout.AuthenticatedUser;
            await GetCollection();
            _inicialized = true;
        }

        private async Task GetCollection()
        {
            var collections = await CollectionsManager.GetUserCollections(AuthenticatedUser.Id);
            Collection = collections.FirstOrDefault(collection => collection.Name == CollectionName.Replace("_", " "));
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
            if (!newName.IsNullOrEmpty())
            {
                await CollectionsManager.ChangeCollectioName(Collection.Id, newName);
                StateHasChanged();
            }
        }

        private async void OnChangePublic(bool newState)
        {
            await CollectionsManager.ChangePublicSate(Collection.Id, newState);
            StateHasChanged();
        }

        private void GetPagelayout(PageLayout instance)
        {
            if (_pageLayout == null)
                _pageLayout = instance;
        }
    }
}
