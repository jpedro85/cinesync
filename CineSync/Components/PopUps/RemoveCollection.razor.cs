using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;


namespace CineSync.Components.PopUps
{
    public partial class RemoveCollection : ComponentBase
    {
        [Parameter]
        public MovieCollection Collection { get; set; }

        [Parameter]
        public EventCallback OnRemove { get; set; }

        [Inject]
        public CollectionsManager CollectionsManager { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        private PopUpLayout PopUpLayout { get; set; }

        private ApplicationUser AuthenticatedUser { get; set; }

        protected override void OnInitialized()
        {
            AuthenticatedUser = LayoutService.MainLayout.AuthenticatedUser;
        }


        private async void ExecuteRemoveCollection()
        {
            await CollectionsManager.RemoveCollectionAsync(AuthenticatedUser.Id, Collection.Id);
            await OnRemove.InvokeAsync();
        }
    }
}
