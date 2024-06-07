using CineSync.Components.PopUps;
using CineSync.Data;
using CineSync.DbManagers;
using CineSync.Data.Models;
using CineSync.Services;
<<<<<<< HEAD
using Microsoft.AspNetCore.Components;
=======
using CineSync.Components.Buttons;
>>>>>>> c9a0e10 (feat TabBar)

namespace CineSync.Components.Pages
{
    public partial class Profile : ComponentBase
    {
        [Parameter]
        public string? UserId { get; set; }

        [Inject]
        public CollectionsManager CollectionManager { get; set; }

        [Inject]
        public UserImageManager UserImageManager { get; set; }

        [Inject]
        public LayoutService LayoutService { get; set; }

        public UsernameEdit newuserName { get; set; }
        
        [Parameter]
        public string? UserId { get; set; }

        public ApplicationUser AuthenticatedUser { get; set; }

        private ICollection<MovieCollection>? movieCollections { get; set; }

        private UserImage? UserImage { get; set; }

        private string _activeTab = "Collections";

        private string[] _tabNames = { "Collections", "Comments", "Discutions", "Following", "Followers" };

        protected override async Task OnInitializedAsync()
        {
            AuthenticatedUser = LayoutService.MainLayout.AuthenticatedUser;
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