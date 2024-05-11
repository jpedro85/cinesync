using CineSync.Components.Layout;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CineSync.Components.PopUps
{
    public partial class Classification : ComponentBase
    {
        [Parameter]
        public int MovieId { get; set; }

        [Inject]
        private MovieManager MovieManager { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private MainLayout MainLayout { get; set; }

        private int Rating { get; set; }

        protected override void OnInitialized()
        {
            MainLayout = LayoutService.MainLayout;
        }

        private void SetRating(ChangeEventArgs e)
        {
            Rating = Convert.ToInt32(e.Value);
        }

        // TODO: Remove the reload after making the MovieDetails Component
        private async void SaveRating()
        {
            await MovieManager.AddRating(Rating, MovieId, MainLayout.AuthenticatedUser.Id);
            await JSRuntime.InvokeVoidAsync("window.location.reload");
        }
    }
}

