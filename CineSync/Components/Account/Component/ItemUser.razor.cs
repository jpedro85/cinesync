using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using Microsoft.AspNetCore.Components;


namespace CineSync.Components.Account.Component
{
    public partial class ItemUser
    {
        [Inject]
        public DbManager<UserImage> DbUserImageManager {  get; set; }

        [Parameter]
        public ApplicationUser User { get; set; }

        [Parameter]
        public bool Short { get; set; } = false;

        [Parameter]
        public string Style { get; set; } = "";

        private UserImage _image = null;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                GetImage();
                StateHasChanged();
            }
        }

        private async void GetImage()
        {
            _image = await DbUserImageManager.GetFirstByConditionAsync( uImage => uImage.UserId == User.Id );
        }

    }
}
