using CineSync.Components.Utils;
using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;


namespace CineSync.Components.Navs
{
    public partial class NavBar : ComponentBase
    {
        [Parameter]
        public bool HasSearch { get; set; } = false;

        [Parameter]
        public ApplicationUser? User { get; set; }

        [Inject]
        public NavBarEvents NavBarEvents { get; set; }

        [Inject]
        public UserImageManager UserImageManager { get; set; }

        private UserImage? UserImage { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (User != null)
                {
                    UserImage = await UserImageManager.GetFirstByConditionAsync(image => image.UserId == User.Id);
                    StateHasChanged();
                }
            }
        }

        private string GetImage()
        {
            string imageBase64 = ImageConverter.ConverBytesTo64(UserImage.ImageData);
            return imageBase64;
        }

    }
}
