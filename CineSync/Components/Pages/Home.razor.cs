using CineSync.Data;
using CineSync.Controllers.MovieEndpoint;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Components;
using System.Timers;
using CineSync.Core.Adapters.ApiAdapters;
using CineSync.Services;
using CineSync.Components.Layout;
using CineSync.Components.Buttons;

namespace CineSync.Components.Pages
{
    public partial class Home : ComponentBase
    {

        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private MainLayout MainLayout { get; set; }

        private bool showNavMenu = false;

        private System.Timers.Timer _timer;
        
        private SearchButton SearchButton { get; set; } = new SearchButton();

        protected override async Task OnInitializedAsync()
        {
            MainLayout = LayoutService.MainLayout;
            MainLayout.RemoveSearchButton();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                SearchButton.OnSearch += OnSearch;
            }
        }

        private void ToggleNavMenu()
        {
            showNavMenu = !showNavMenu;
        }
        
        public void OnSearch(string searchQuery)
        {
            if (searchQuery != string.Empty)
                NavigationManager.NavigateTo($"/Search/{searchQuery}");
        }
        
    }
}
