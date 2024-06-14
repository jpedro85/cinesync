using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using System.Collections.ObjectModel;

namespace CineSync.Components.PopUps
{
    public partial class NewCollection
    {
        [Parameter]
        public uint MovieID { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        [Inject]
        private CollectionsManager CollectionsManager { get; set; }

        private ApplicationUser AuthenticatedUser { get; set; }

        private ICollection<MovieCollection> Collections { get; set; } = new List<MovieCollection>();

        private IDictionary<string, bool> CollectionsMovieStatus { get; set; } = new Dictionary<string, bool>();
        private ICollection<string> edited { get; set; } = new List<string>();

        private string _newCollectionName = "";

        protected override async Task OnInitializedAsync()
        {
            AuthenticatedUser = LayoutService.MainLayout.AuthenticatedUser;

            Collections = await FetchCollections();
            CollectionsMovieStatus = UpdateStateCollections();

        }

        private async Task<ICollection<MovieCollection>> FetchCollections()
        {
            return await CollectionsManager.GetUserCollections(AuthenticatedUser.Id);
        }

        private IDictionary<string, bool> UpdateStateCollections()
        {
            IDictionary<string, bool> dic = new Dictionary<string, bool>();

            foreach (MovieCollection collection in Collections)
            {
                bool containMovie = collection.CollectionMovies.Any(relation => relation.MovieId == MovieID);
                dic.Add(collection.Name, containMovie);
            }

            return dic;
        }

        public void UpdateState(string key, bool isCheecked)
        {
            CollectionsMovieStatus[key] = isCheecked;

            if (!edited.Contains(key))
                edited.Add(key);
        }

        public async void OnSave()
        {
            if (!_newCollectionName.IsNullOrEmpty())
            {
                if (await CollectionsManager.CreateNewCollectionAsync(AuthenticatedUser.Id, _newCollectionName))
                {
                    await CollectionsManager.AddMovieToCollectionAsync(AuthenticatedUser.Id, _newCollectionName, MovieID);
                    _newCollectionName = "";
                }

            }

            foreach (var collection in CollectionsMovieStatus)
            {
                if (edited.Contains(collection.Key))
                {
                    if (collection.Value)
                    {
                        await CollectionsManager.AddMovieToCollectionAsync(AuthenticatedUser.Id, collection.Key, MovieID);
                    }
                    else
                    {
                        await CollectionsManager.RemoveMovieFromCollectionAsync(AuthenticatedUser.Id, collection.Key, MovieID);
                    }
                }
            }

        }
    }
}
