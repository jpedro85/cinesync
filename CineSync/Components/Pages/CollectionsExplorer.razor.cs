using CineSync.Components.Layout;
using CineSync.Components.PopUps;
using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.Pages
{
    public partial class CollectionsExplorer : ComponentBase
    {
        [Inject]
        public CollectionsManager CollectionManager { get; set; }

        [Inject]
        public UserManager UserManager { get; set; }

        public ApplicationUser AuthenticatedUser { get; set; }

        private ICollection<MovieCollection>? movieCollections { get; set; }

        private string[] _tabNames = { "Collections", "Comments", "Discutions", "Following", "Followers" };

        private PageLayout _pageLayout;

        public async void Initialize()
        {
            AuthenticatedUser = _pageLayout.AuthenticatedUser!;
            movieCollections = await CollectionManager.GetUserCollections(AuthenticatedUser.Id);
        }

        private void GetPageLayout(PageLayout instance)
        {
            if (_pageLayout == null)
                _pageLayout = instance;
        }

        private async Task OnCollectionsEdit()
        {
            await InvokeAsync(StateHasChanged);
        }

    }
}

