using CineSync.Components.Buttons;
using CineSync.Components.Layout;
using CineSync.Controllers.MovieEndpoint;
using CineSync.Core.Adapters.ApiAdapters;
using CineSync.Data.Models;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;


namespace CineSync.Components.Pages
{
	public partial class Search
	{
		[Inject]
		private HttpClient _client { get; set; }

		[Inject]
		private NavBarEvents NavBarEvents { get; set; }

		[Inject]
		private NavigationManager NavigationManager { get; set; }

		[Parameter]
		public string Query { get; set; }

		[Parameter]
		public SearchButton SearchButton { get; set; }

		private string _currentSearchQuery = string.Empty;

		private List<MovieSearchAdapter> SearchResults { get; set; } = new () ;
		
		private int _currentPage = 1;
		private bool _isLastpage = false;
		private string _isLoading = string.Empty;

		protected override void OnInitialized()
		{

			NavBarEvents.OnSearchFromNavBar += SearchMoviesNavSearchHandler;


			if ( ! string.IsNullOrEmpty(Query) )
			{
				SearchMoviesMainSearchButtonHandler( Query );
			}
			else
			{
				FecthUpcoming();
			}
		}

		protected override void OnAfterRender(bool firstRender)
		{
			if (firstRender)
			{
				SearchButton.OnSearch += SearchMoviesMainSearchButtonHandler;
				_currentPage = 1;
			}
		}

		private async Task SearchMovies (string searchQuery) 
		{
			_currentPage = 1;
			_isLastpage = false;
			SearchResults.Clear();
			_currentSearchQuery = searchQuery;

			await LoadPage(searchQuery, _currentPage);
		}


		private async void SearchMoviesMainSearchButtonHandler( string searchQuery )
		{
			NavBarEvents.InvokeOnSearchFromPage(searchQuery);
			await SearchMovies(searchQuery);
			StateHasChanged();
		}

		private void SearchMoviesNavSearchHandler(string searchQuery)
		{
			InvokeAsync( async () =>
			{
				await SearchMovies( searchQuery );
				SearchButton.setSearchInput(searchQuery);
				StateHasChanged();
			});
		}

		private async Task LoadNextPageMovies()
		{

			if ( _isLastpage )
				return;
			
			await LoadPage( _currentSearchQuery , ++_currentPage);
			StateHasChanged();
			
		}

		private async Task LoadPage(string searchQuery, int page)
		{

			List<MovieSearchAdapter> results = await GetMovies($"/movie/search?Query={searchQuery}&Page={page}");

			if ( results.Count == 0 && page > 1)
			{
				_isLastpage = true;
			}
			else
			{
				SearchResults.AddRange( results );
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

		private async Task< List<MovieSearchAdapter> > GetMovies( string url )
		{

			_isLoading = "Active";
			StateHasChanged();

			HttpResponseMessage response = await _client.GetAsync(url);

			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<ApiSearchResponse>(jsonResponse)?.Results !;
			}

			return new List<MovieSearchAdapter>(0);
		}

		private void MovieClickhandler(MovieSearchAdapter movie)
		{
			NavigationManager.NavigateTo($"MovieDetails/{movie.MovieId}");
		}
	}
}
