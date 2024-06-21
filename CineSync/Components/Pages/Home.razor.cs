using CineSync.Data;
using CineSync.Controllers.MovieEndpoint;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Components;
using System.Timers;
using CineSync.Core.Adapters.ApiAdapters;
using CineSync.Services;
using CineSync.Components.Layout;
using CineSync.Components.Buttons;
using System;

namespace CineSync.Components.Pages
{
    public partial class Home : ComponentBase
    {
      
        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }


        private bool showNavMenu = false;
        private System.Timers.Timer _timer;
        private SearchButton SearchButton { get; set; }

        private PageLayout _pageLayout;

        protected override void OnInitialized()
        {

            
        }

        //protected override async Task OnInitializedAsync()
        //{
        //    //MainLayout = LayoutService.MainLayout;
        //    //MainLayout?.RemoveSearchButton();
        //    //Console.WriteLine(PotatoValue);
        //    Console.WriteLine($"Teste :{MainLayouta == null}");

        //}

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                SearchButton.OnSearch += OnSearch;
            }

            Console.WriteLine($"aa {_pageLayout != null}");
			Console.WriteLine($"aa1 {_pageLayout?.Menu != null}");
			Console.WriteLine($"a2 {_pageLayout?.NavBar != null}");
		}

        //private void ToggleNavMenu()
        //{
        //    //showNavMenu = !showNavMenu;
        //}

        public void OnSearch(string searchQuery)
        {
            if (searchQuery != string.Empty)
                NavigationManager.NavigateTo($"/Search/{searchQuery}");
        }

        private void GetPagelayout(PageLayout instance) 
        {
            if(_pageLayout == null)
                _pageLayout = instance;
		}
	}
}
