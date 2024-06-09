using CineSync.Components.PopUps;
using CineSync.Data;
using CineSync.DbManagers;
using CineSync.Data.Models;
using CineSync.Services;
using Microsoft.AspNetCore.Components;

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
                AuthenticatedUser = LayoutService.MainLayout.AuthenticatedUser;
                movieCollections = await CollectionManager.GetUserCollections(AuthenticatedUser.Id);
                _visit = false;
            }
            else
            {
                AuthenticatedUser = await UserManager.GetFirstByConditionAsync(u => u.Id == UserId);
                _visit = true;
            }

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                UserImage = await UserImageManager.GetFirstByConditionAsync(image => image.UserId == AuthenticatedUser.Id);
                StateHasChanged();
            }
        }

        private async void OnProfileEdit()
        {
            AuthenticatedUser = LayoutService.MainLayout.AuthenticatedUser;
            StateHasChanged();
            await LayoutService.MainLayout.TriggerNavBarReRender();
        }

        private void OnTabChange(string tabName)
        {
            _activeTab = tabName;
            InvokeAsync(StateHasChanged);
        }
    }
}
