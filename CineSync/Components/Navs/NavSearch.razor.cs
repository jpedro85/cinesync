using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CineSync.Components.Navs
{
    public partial class NavSearch
    {
        private string _searchActive = string.Empty;

        private string _currentSearch = string.Empty;

        [Inject]
        public NavBarEvents NavBarEvents { get; set; }

		protected override void OnInitialized()
		{
            NavBarEvents.OnSearchFromPage += updateCurrentSearch;
		}

		private void OutSearch()
        {
            _searchActive = "";
        }

        private void OnSearchClick( MouseEventArgs e )
        {
            if (_currentSearch != string.Empty)
                NavBarEvents?.OnClickSearch( _currentSearch );
        }

		private void OnSearchClick( KeyboardEventArgs e )
		{
			if (_currentSearch != string.Empty)
				NavBarEvents?.OnClickSearch( _currentSearch );
		}

		private void ClickInput(MouseEventArgs e)
        {
            _searchActive = "Active";
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
