using CineSync.Data;
using Microsoft.AspNetCore.Components;
using CineSync.Data.Models;
using CineSync.Services;
using CineSync.DbManagers;
using CineSync.Components.Layout;

namespace CineSync.Components.Pages
{
    public partial class MoviePage : ComponentBase
    {

        [Parameter]
        public int MovieId { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        public CollectionsManager CollectionsManager { get; set; }

        [Inject]
        public DbManager<UserLikedComment> DbUserLikedComment { get; set; }

        [Inject]
        public DbManager<UserDislikedComment> DbUserDislikedComment { get; set; }

        [Inject]
        public DbManager<UserLikedDiscussion> DbUserLikedDiscussion { get; set; }

        [Inject]
        public DbManager<UserDislikedDiscussion> DbUserDislikedDiscussion { get; set; }

        [Inject]
        private UserManager UserManager { get; set; }

        [Inject]
        private MovieManager MovieManager { get; set; }

        private PageLayout _pageLayout;
        private bool _initialized = false;


		private readonly string _youtubeLink = "https://www.youtube.com/embed/";

        private string MoviePosterBase64;

        private Movie Movie { get; set; }

        private string _activeTab = "Comments";

        private string[] _tabNames = { "Comments", "Discussions" };

        private ICollection<string> _userRoles;

        private ICollection<UserLikedComment> _likedComents = new List<UserLikedComment>();
        private ICollection<UserDislikedComment> _dislikedComents = new List<UserDislikedComment>();

        private ICollection<UserLikedDiscussion> _likedDiscussion = new List<UserLikedDiscussion>();
        private ICollection<UserDislikedDiscussion> _dislikedDiscussions= new List<UserDislikedDiscussion>();

        private ApplicationUser _authenticatedUser;

        private bool _hasRatedMovie = false;

        protected async void Initialize()
        {
            _authenticatedUser = _pageLayout!.AuthenticatedUser!;
            _userRoles = _pageLayout!.UserRoles;
            Movie = (await GetMovieDetails())!;
            GetUserStatusComments();
            GetUserStatusDiscussions();

            if (_authenticatedUser != null)
            {
                _hasRatedMovie = await HasUserRatedMovieAsync();
            }
            
            _initialized = true;
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
            InvokeAsync(StateHasChanged);
        }

        private void GetPageLayout( PageLayout instance) 
        {
            if( _pageLayout == null)
                _pageLayout = instance;
        }

    }
}
