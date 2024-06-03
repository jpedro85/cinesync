using CineSync.Components.Layout;
using CineSync.Data;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CineSync.Components.PopUps
{
    public partial class Classification : ComponentBase
    {
        [Parameter]
        public int MovieAPIId { get; set; }

        [Parameter]
        public uint MovieId { get; set; }

        [Inject]
        private MovieManager MovieManager { get; set; }

        [Inject]
        private CollectionsManager CollectionsManager { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private MainLayout MainLayout { get; set; }


        public ApplicationUser AuthenticatedUser { get; set; }
        private int Rating { get; set; }

        protected override void OnInitialized()
        {
            MainLayout = LayoutService.MainLayout;
        }

        private void SetRating(ChangeEventArgs e)
        {
            Rating = Convert.ToInt32(e.Value);
        }

        private async void SaveRating()
        {
            await MovieManager.AddRating(Rating, MovieAPIId, MainLayout.AuthenticatedUser.Id);
            await JSRuntime.InvokeVoidAsync("window.location.reload");
        }
    }
}

