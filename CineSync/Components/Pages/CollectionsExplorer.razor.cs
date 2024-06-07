using CineSync.Components.PopUps;
using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.Pages
{
    public partial class CollectionsExplorer
    {
        [Parameter]
        public string? UserId { get; set; }

        [Inject]
        public CollectionsManager CollectionManager { get; set; }

        [Inject]
        public UserImageManager UserImageManager { get; set; }

        [Inject]
        public LayoutService LayoutService { get; set; }

        [Inject]
        public UserManager UserManager { get; set; }

        public UsernameEdit newuserName { get; set; }


        public ApplicationUser AuthenticatedUser { get; set; }

        private ICollection<MovieCollection>? movieCollections { get; set; }

        private UserImage? UserImage { get; set; }

        private string _activeTab = "Collections";

        private string[] _tabNames = { "Collections", "Comments", "Discutions", "Following", "Followers" };
        private bool _visit = false;


        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrEmpty(UserId))
            {
                // If no UserId is provided, display the authenticated user's profile
                AuthenticatedUser = LayoutService.MainLayout.AuthenticatedUser;
                movieCollections = await CollectionManager.GetUserCollections(AuthenticatedUser.Id);
                _visit = false;
            }
            else
            {
                // Fetch the profile of the user specified by UserId
                AuthenticatedUser = await UserManager.GetFirstByConditionAsync(u => u.Id == UserId);
                _visit = true;
            }

            movieCollections = await CollectionManager.GetUserCollections(AuthenticatedUser.Id);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                UserImage = await UserImageManager.GetFirstByConditionAsync(image => image.UserId == AuthenticatedUser.Id);
                StateHasChanged();
            }
        }

        private void OnProfileEdit()
        {
            StateHasChanged();
        }

        private void OnTabChange(string tabName)
        {
            _activeTab = tabName;
            InvokeAsync(StateHasChanged);
        }
    }
}

