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
        public LayoutService LayoutService { get; set; }

        [Inject]
        public UserManager UserManager { get; set; }

        public ApplicationUser AuthenticatedUser { get; set; }

        private ICollection<MovieCollection>? movieCollections { get; set; }

        private string[] _tabNames = { "Collections", "Comments", "Discutions", "Following", "Followers" };

        protected override async Task OnInitializedAsync()
        {
            AuthenticatedUser = LayoutService.MainLayout.AuthenticatedUser;

            movieCollections = await CollectionManager.GetUserCollections(AuthenticatedUser.Id);
        }

        private async Task OnCollectionsEdit()
        {
            Console.WriteLine("Was Called to ReRender");
            await InvokeAsync(StateHasChanged);
        }

    }
}

