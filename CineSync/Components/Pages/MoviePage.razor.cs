using CineSync.Data;
using Microsoft.AspNetCore.Components;
using CineSync.Data.Models;
using CineSync.DbManagers;
using CineSync.Components.Layout;
using CineSync.Components.PopUps;
using CineSync.Components.Buttons;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.SignalR.Client;


namespace CineSync.Components.Pages
{
    public partial class MoviePage : ComponentBase
    {

        [Parameter]
        public int MovieId { get; set; }

		[Parameter]
		public string? Tab { get; set; }


        [Inject]
        private HttpClient _client { get; set; } = default!;

        [Inject]
        public CollectionsManager CollectionsManager { get; set; } = default!;

		[Inject]
        public DbManager<UserLikedComment> DbUserLikedComment { get; set; } = default!;

		[Inject]
        public DbManager<UserDislikedComment> DbUserDislikedComment { get; set; } = default!;

		[Inject]
        public DbManager<UserLikedDiscussion> DbUserLikedDiscussion { get; set; } = default!;

		[Inject]
        public DbManager<UserDislikedDiscussion> DbUserDislikedDiscussion { get; set; } = default!;

		[Inject]
        private UserManager UserManager { get; set; } = default!;

		[Inject]
        private MovieManager MovieManager { get; set; } = default!;

		[Inject]
		private NavigationManager NavigationManager { get; set; } = default!;

		[Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

		private PageLayout _pageLayout = default!;
        private bool _initialized = false;


		private readonly string _youtubeLink = "https://www.youtube.com/embed/";

        private string MoviePosterBase64 = default!;

		private Movie Movie { get; set; } = default!;

		private string _activeTab = "Comments";

        private string[] _tabNames = { "Comments", "Discussions" };

        private ICollection<string> _userRoles = default!;

        private ICollection<UserLikedComment> _likedComents = new List<UserLikedComment>();
        private ICollection<UserDislikedComment> _dislikedComents = new List<UserDislikedComment>();

        private ICollection<UserLikedDiscussion> _likedDiscussion = new List<UserLikedDiscussion>();
        private ICollection<UserDislikedDiscussion> _dislikedDiscussions= new List<UserDislikedDiscussion>();

        private ApplicationUser? _authenticatedUser;

        private bool _hasRatedMovie = false;

        private VideoTrailer VideoTrailer = default!;
		private TabButtonsBar _TabBar = default!;
        private int _initialTab = 0;
        private bool _isInvalid = false;
		private HubConnection DiscussionHubConnection { get; set; } = default!;


		protected async void Initialize()
        {

            if (!_initialized) 
            {

                if( Tab != null )
                {
                    _initialTab = Array.IndexOf(_tabNames, Tab);
                    _initialTab = _initialTab == -1 ? 0 : _initialTab;
				}
				
                _activeTab = _tabNames[_initialTab];
				_authenticatedUser = _pageLayout.AuthenticatedUser;
                _userRoles = _pageLayout.UserRoles;
                
                Movie? movieResult = await GetMovieDetails();
                if (movieResult is null)
                {
                    _isInvalid = true;
                    _initialized = true;
                    StateHasChanged();
                    return;
                }
                
                Movie = movieResult;
                GetUserStatusComments();
                GetUserStatusDiscussions();

                if (_authenticatedUser != null)
                {
                    _hasRatedMovie = await HasUserRatedMovieAsync();
                }
            
                _initialized = true;

                StateHasChanged();
            }

            await ConnectToMessageHub();
		}

		private async Task ConnectToMessageHub()
		{
			DiscussionHubConnection = new HubConnectionBuilder()
			  .WithUrl(NavigationManager.ToAbsoluteUri("/DiscussionHub"))
			  .Build();

			await DiscussionHubConnection.StartAsync();

			StateHasChanged();
		}

		private async Task<bool> HasUserRatedMovieAsync()
        {
            string ratedMoviesCollectionName = "Classified";
            var ratedMovies = await CollectionsManager.GetFirstByConditionAsync(
                mc => mc.ApplicationUser.Equals(_authenticatedUser) && mc.Name == ratedMoviesCollectionName,
                "CollectionMovies"
            );
            return ratedMovies!.CollectionMovies?.Any(cm => cm.MovieId == Movie.Id) ?? false;
        }

        private void GetUserStatusComments()
        {
            if (Movie != null && _authenticatedUser != null)
            {
                _likedComents = DbUserLikedComment.GetByConditionAsync(
                            likedComment =>
                            likedComment.Comment.MovieId == Movie.Id &&
                            likedComment.UserId == _authenticatedUser.Id
                            ).Result.ToList();

                _dislikedComents = DbUserDislikedComment.GetByConditionAsync(
                            likedComment =>
                            likedComment.Comment.MovieId == Movie.Id &&
                            likedComment.UserId == _authenticatedUser.Id
                            ).Result.ToList();
            }
            else
            {
                _likedComents = new List<UserLikedComment>(0);
                _dislikedComents = new List<UserDislikedComment>(0);
            }
        }

        private void GetUserStatusDiscussions()
        {
            if (Movie != null && _authenticatedUser != null)
            {
                _likedDiscussion = DbUserLikedDiscussion.GetByConditionAsync(
                            likedDiscussion =>
                            likedDiscussion.Discussion.MovieId == Movie.Id &&
                            likedDiscussion.UserId == _authenticatedUser.Id
                            ).Result.ToList();

                _dislikedDiscussions = DbUserDislikedDiscussion.GetByConditionAsync(
                            likedDiscussion =>
                            likedDiscussion.Discussion.MovieId == Movie.Id &&
                            likedDiscussion.UserId == _authenticatedUser.Id
                            ).Result.ToList();
            }
            else
            {
                _likedDiscussion = new List<UserLikedDiscussion>(0);
                _dislikedDiscussions = new List<UserDislikedDiscussion>(0);
            }
        }


        private async Task<Movie?> GetMovieDetails()
        {
            Movie = await MovieManager.GetByTmdbId(MovieId);
            if (Movie != null)
            {
                return Movie;
            }

            HttpResponseMessage response = await _client.GetAsync($"movie?id={MovieId}");
            if (response.IsSuccessStatusCode)
            {
                return await MovieManager.GetByTmdbId(MovieId);
            }
            return null;
        }

        private void OnRatingSaved()
        {
            _hasRatedMovie = true;
            InvokeAsync(StateHasChanged);
        }

        private void OnTabChange(string tabName)
        {
            _activeTab = tabName;
            UpdateUrl(MovieId, _activeTab);
			InvokeAsync(StateHasChanged);
        }

        private void GetPageLayout( PageLayout instance)
        {
            if( _pageLayout == null )
                _pageLayout = instance;
        }

        private void UpdateUrl( int movieId, string tab ) 
        {
            JSRuntime.InvokeVoidAsync("updateUrl", movieId, tab);
        }

    }
}
