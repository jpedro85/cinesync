using CineSync.Components.Utils;
using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.Navs
{
    public partial class NavBar : ComponentBase, IDisposable
    {
        [Parameter]
        public bool HasSearch { get; set; } = false;

        [Inject]
        public UserImageManager UserImageManager { get; set; }

        [Inject]
        public LayoutService LayoutService { get; set; }

        [Inject]
        public NavBarEvents NavBarEvents { get; set; }

        public ApplicationUser? User { get; set; }

        private UserImage? UserImage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            NavBarEvents.OnRequestNavBarReRender += ReRender;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            User = LayoutService.MainLayout.AuthenticatedUser;
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

        public async Task ReRender()
        {
            Console.WriteLine("Was called");
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            NavBarEvents.OnRequestNavBarReRender -= ReRender;
        }
    }
}
