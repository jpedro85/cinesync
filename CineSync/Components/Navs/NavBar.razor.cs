using CineSync.Components.Layout;
using CineSync.Components.Utils;
using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CineSync.Components.Navs
{
    public partial class NavBar : ComponentBase
    {
        [Inject]
        public UserImageManager UserImageManager { get; set; }


        [Parameter]
        public GetInstanceDelegate GetInstance { get; set; } = (m) => { };
        public delegate void GetInstanceDelegate( NavBar instance );

        [Parameter]
        public bool HasSearch { get; set; } = true;
        private bool _hasSearch;

        [Parameter,EditorRequired]
        public ApplicationUser? AuthenticatedUser { get; set; }

		[Parameter, EditorRequired]
        public NavBarEvents NavBarEvents { get; set; }


        private UserImage? UserImage { get; set; }


        protected override void OnInitialized()
        {
            _hasSearch = HasSearch;
			NavBarEvents.OnRequestNavBarReRender += ReRender;
            GetInstance(this);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
           // User = LayoutService.MainLayout.AuthenticatedUser;
            if (firstRender)
            {
                if (AuthenticatedUser != null)
                {
                    UserImage = await UserImageManager.GetFirstByConditionAsync(image => image.UserId == AuthenticatedUser.Id);
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
            if (UserImage == null && AuthenticatedUser != null)
            {
                UserImage = await UserImageManager.GetFirstByConditionAsync(image => image.UserId == AuthenticatedUser.Id);
            }
            await InvokeAsync(StateHasChanged);
        }

        private void OnMenuClick(MouseEventArgs e)
        {
            NavBarEvents.OnMenuClick(e);
        }

        private void OnNotificationClick(MouseEventArgs e)
        {
            NavBarEvents.OnClickNotification(e);
        }

        public void SetVisibleSearchButton(bool visible)
        {
            _hasSearch = visible;
			InvokeAsync(StateHasChanged);
        }
    }
}
