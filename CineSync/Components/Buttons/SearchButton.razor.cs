using CineSync.Controllers.MovieEndpoint;
using CineSync.Core.Adapters.ApiAdapters;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;

namespace CineSync.Components.Buttons
{
    public partial class SearchButton
    {

        [Inject]
        private HttpClient _client { get; set; }

        [Parameter]
        public string SearchInput { get; set; } = string.Empty;

        public delegate void SearchResultHandler( string searchResults );
        public event SearchResultHandler? OnSearch;

        private void SearchHandler( KeyboardEventArgs  e )
        {
            if ( e.Key == "Enter" && !string.IsNullOrEmpty(SearchInput) )
            {
                OnSearch?.Invoke(SearchInput);
            }
        }

        private void SearchHandler( MouseEventArgs e )
        {
            if( !string.IsNullOrEmpty(SearchInput) )
                OnSearch?.Invoke(SearchInput);
        }

	}
}
