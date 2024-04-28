using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace CineSync.Components.Navs
{
    public partial class NavSearch
    {
        private string _searchActive = string.Empty;

        private string _currentSearch = string.Empty;

        [Inject]
        public NavigationManager NavigationManager { get; set; }

		private void OnSearchClick( MouseEventArgs e )
        {
            if (_currentSearch != string.Empty)
                NavigationManager.NavigateTo($"/Search/{_currentSearch}");		
        }

		private void OnSearchClick( KeyboardEventArgs e )
		{
			if (_currentSearch != string.Empty && e.Key == "Enter")
				NavigationManager.NavigateTo($"/Search/{_currentSearch}");
		}

		private void ClickInput( MouseEventArgs e )
        {
            _searchActive = "Active";
		}
		private void OutSearch()
        {
            _searchActive = "";
        }

        private void updateCurrentSearch(string searchActive )
        {
            InvokeAsync(() =>
            {
                _currentSearch = searchActive;
                StateHasChanged();
            });
		}

	}
}
