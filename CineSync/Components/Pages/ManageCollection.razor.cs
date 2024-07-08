using System.Data;
using System.Runtime.InteropServices;
using CineSync.Components.Layout;
using CineSync.Data;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using CineSync.DbManagers;
using CineSync.Data.Models;
using Microsoft.IdentityModel.Tokens;


namespace CineSync.Components.Pages
{
    public partial class ManageCollection : ComponentBase
    {
        [Inject] 
        private CollectionsManager CollectionsManager { get; set; }
        
        [Inject]
        private DbManager<FollowedCollection> DbFollowedCollections { get; set; }

        [Parameter] 
        public string CollectionId { get; set; }

        private PageLayout _pageLayout;
        private bool _inicialized = false;

        private ApplicationUser? AuthenticatedUser { get; set; }

        private ICollection<CollectionsMovies> AllMovies { get; set; } = new List<CollectionsMovies>(0);
        private ICollection<FollowedCollection> FollowedCollections { get; set; } = new List<FollowedCollection>(0);

        private ICollection<string> DefaultCollection { get; set; } = new string[] { "Favorites", "Watched", "Classified", "Watch Later" };

        public MovieCollection Collection { get; set; } = default!;

        private ICollection<MovieCollection> UserCollections { get; set; }

        private bool isEditing = false;

        private bool _visit = false;
        
        private bool _isInvalid = false;

        private uint _collectionId = default;

        private bool _follows = false;


        private async void Initialize()
        {
            if (!uint.TryParse(CollectionId, out _collectionId))
            {
                _isInvalid = true;
                return;
            }
            AuthenticatedUser = _pageLayout.AuthenticatedUser;
            UserCollections = await CollectionsManager.GetUserCollections(AuthenticatedUser.Id);
            FollowedCollections = DbFollowedCollections.GetByConditionAsync(c=> c.ApplicationUserId == AuthenticatedUser.Id,"MovieCollection.CollectionMovies.Movie").Result.ToList();
            await GetCollection();
            _inicialized = true;
        }

        private async Task GetCollection()
        {
            Collection = await CollectionsManager.GetFirstByConditionAsync(c => c.Id == _collectionId, "CollectionMovies.Movie");
            if (Collection == null)
            {
                _isInvalid = true;
            }

            _visit = !UserCollections.Contains(Collection!);
            _follows = FollowedCollections.Any(c => c.MovieCollection.Equals(Collection));
            AllMovies = Collection!.CollectionMovies?.ToList() ?? new List<CollectionsMovies>(0);
        }

        private async void OnChangeName(string newName)
        {
            if (newName.IsNullOrEmpty()) return;
            await CollectionsManager.ChangeCollectionName(Collection.Id, newName);
            StateHasChanged();
        }

        private async void FollowCollection()
        {
            FollowedCollection collectionToFollow = new()
            {
                ApplicationUserId = AuthenticatedUser!.Id,
                MovieCollectionId = Collection.Id
            };
            
            if (await DbFollowedCollections.AddAsync(collectionToFollow))
            {
                _follows = true;
            }
            
            StateHasChanged();
        }
        
        private async void UnFollowCollection()
        {
            var collectionToUnFollow = FollowedCollections.Where(c => c.MovieCollection.Equals(Collection)).First();
            if (await DbFollowedCollections.RemoveAsync(collectionToUnFollow))
            {
                _follows = false;
            }
            
            StateHasChanged();
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