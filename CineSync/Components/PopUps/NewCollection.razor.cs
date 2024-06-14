using CineSync.Data;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;

namespace CineSync.Components.PopUps
{
    public partial class NewCollection
    {
        [Parameter]
        public EventCallback OnNewCollection { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        [Inject]
        private CollectionsManager CollectionsManager { get; set; }

        private PopUpLayout PopUpLayout { get; set; }

        private ApplicationUser AuthenticatedUser { get; set; }

        private string _newCollectionName = "";
        private string ErrorMessage = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            AuthenticatedUser = LayoutService.MainLayout.AuthenticatedUser;
        }

        public async void OnSave()
        {
            if (!_newCollectionName.IsNullOrEmpty())
            {
                if (await CollectionsManager.CreateNewCollectionAsync(AuthenticatedUser.Id, _newCollectionName))
                {
                    PopUpLayout.Close();
                    await OnNewCollection.InvokeAsync();
                }
                else
                {
                    ErrorMessage = "Something occured an we were unable to remove the Comment.";
                }

            }
        }

    }
}
