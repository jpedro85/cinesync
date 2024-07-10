using CineSync.Components.Buttons;
using CineSync.Components.Layout;
using CineSync.Controllers.MovieEndpoint;
using CineSync.Core.Adapters.ApiAdapters;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Microsoft.JSInterop;

namespace CineSync.Components.Pages
{
    public partial class Search : ComponentBase
    {

        [Parameter]
        public string Query { get; set; }

        [Parameter]
        public SearchButton SearchButton { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private PageLayout _pageLayout;

        private string _currentSearchQuery = string.Empty;

        private List<MovieSearchAdapter> SearchResults { get; set; } = new();

        private int _currentPage = 1;

        private bool _isLastpage = false;

        private string _isLoading = string.Empty;

        private const int MOVIELIMIT = 18;
        protected override async Task OnParametersSetAsync()
        {;
            if (!string.IsNullOrEmpty(Query))
            {
                _currentSearchQuery = Query;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _pageLayout.NavBar.SetVisibleSearchButton(false);
                await SearchMovies(Query);
                StateHasChanged();
            }

        }

        private async Task SearchMovies(string searchQuery)
        {
            _currentPage = 1;
            _isLastpage = false;
            SearchResults.Clear();
            _currentSearchQuery = searchQuery.Trim();

            await LoadPage(searchQuery, _currentPage);
        }


        private async void SearchMoviesSearchButtonHandler(string searchQuery)
        {
            _currentSearchQuery = searchQuery;
			await UpdateUrlWithSearchQuery(searchQuery);
            await SearchMovies(searchQuery);
            StateHasChanged();
        }

        private async Task LoadNextPageMovies()
        {

            if (_isLastpage)
                return;

            await LoadPage(_currentSearchQuery, ++_currentPage);
            StateHasChanged();

        }

        private async Task LoadPage(string searchQuery, int page)
        {

            List<MovieSearchAdapter> results = await GetMovies($"/movie/search?Query={searchQuery}&Page={page}");

            if ((results.Count == 0) && page > 1 || (results.Count <= MOVIELIMIT))
            {
                _isLastpage = true;
            }
            else
            {
                SearchResults.AddRange(results);
            }

            _isLoading = string.Empty;

        }

        private async void FecthUpcoming()
        {
            List<MovieSearchAdapter> results = await GetMovies($"/movie/upcoming");
            SearchResults.AddRange(results);
            _isLoading = string.Empty;
            StateHasChanged();
        }

        private async Task<List<MovieSearchAdapter>> GetMovies(string url)
        {

            _isLoading = "Active";
            StateHasChanged();

            HttpResponseMessage response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ApiSearchResponse>(jsonResponse)?.Results!;
            }

            return new List<MovieSearchAdapter>(0);
        }

        private void MovieClickhandler(MovieSearchAdapter movie)
        {
            NavigationManager.NavigateTo($"MovieDetails/{movie.MovieId}");
        }

        private void GetPagelayout(PageLayout instance)
        {
            if (_pageLayout == null)
                _pageLayout = instance;
        }

        private async Task UpdateUrlWithSearchQuery(string searchQuery)
        {
            Query = searchQuery;
			await JSRuntime.InvokeVoidAsync("updateUrl", searchQuery);
        }
    }
}
