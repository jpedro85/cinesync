using CineSync.Components.Layout;
using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;


namespace CineSync.Components.PopUps
{
    public partial class RemoveCollection : ComponentBase
    {
        [CascadingParameter(Name ="PageLayout")]
        public PageLayout PageLayout { get; set; }

        [Parameter]
        public MovieCollection Collection { get; set; }

        [Parameter]
        public EventCallback OnRemove { get; set; }

        [Inject]
        public CollectionsManager CollectionsManager { get; set; }

        private PopUpLayout PopUpLayout { get; set; }

        private ApplicationUser AuthenticatedUser { get; set; }

        private string ErrorMessage = string.Empty;

        protected override void OnInitialized()
        {
            AuthenticatedUser = PageLayout.AuthenticatedUser;
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

        public void Open() 
        {
            PopUpLayout.Open();
        }

        public void Close()
        {
            PopUpLayout.Close();
        }
    }
}
