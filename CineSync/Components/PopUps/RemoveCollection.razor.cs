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

        private string ErrorMessage = string.Empty;

        protected override void OnInitialized()
        {
            AuthenticatedUser = LayoutService.MainLayout.AuthenticatedUser;
        }


        private async void ExecuteRemoveCollection()
        {
            Console.WriteLine("Remove Collection: " + Collection.Id);
            if (await CollectionsManager.RemoveCollectionAsync(AuthenticatedUser.Id, Collection.Id))
            {
                await OnRemove.InvokeAsync();
            }
            else
            {
                ErrorMessage = "Something occured an we were unable to remove the Collection.";
            }
        }
    }
}
