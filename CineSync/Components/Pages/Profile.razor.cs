using CineSync.Components.PopUps;
using Microsoft.AspNetCore.Components;
using CineSync.Data;
using CineSync.DbManagers;
using CineSync.Data.Models;
using CineSync.Services;
using CineSync.Core.Adapters.ApiAdapters;

namespace CineSync.Components.Pages
{
    public partial class Profile
    {
        [Inject]
        public CollectionsManager CollectionManager { get; set; }

        [Inject]
        public LayoutService LayoutService { get; set; }

        public UsernameEdit newuserName { get; set; }

        public ApplicationUser AuthenticatedUser { get; set; }

        private ICollection<MovieCollection> movieCollections { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AuthenticatedUser = LayoutService.MainLayout.AuthenticatedUser;
            movieCollections = await CollectionManager.GetUserCollections(AuthenticatedUser.Id);
        }
    }
   
}
