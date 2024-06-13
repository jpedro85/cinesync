using CineSync.Data;
using Microsoft.AspNetCore.Components;
using CineSync.Data.Models;
using CineSync.Services;
using CineSync.DbManagers;

namespace CineSync.Components.Pages
{
    public partial class MoviePage : ComponentBase
    {

        [Parameter]
        public int MovieId { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        [Inject]
        public ApplicationDbContext ApplicationDbContext { get; set; }

        [Inject]
        public CollectionsManager CollectionsManager { get; set; }

        [Inject]
        public DbManager<UserLikedComment> DbUserLikedComment { get; set; }

        [Inject]
        public DbManager<UserDislikedComment> DbUserDislikedComment { get; set; }

        [Inject]
        private UserManager UserManager { get; set; }

        [Inject]
        private MovieManager MovieManager { get; set; }

        private readonly string _youtubeLink = "https://www.youtube.com/embed/";

        private string MoviePosterBase64;

        private Movie Movie { get; set; }

        private string _activeTab = "Comments";

        private string[] _tabNames = { "Comments", "Discutions" };

        private ICollection<string> _userRoles;

        private ICollection<UserLikedComment> _likedComents = new List<UserLikedComment>();

        private ICollection<UserDislikedComment> _dislikedComents = new List<UserDislikedComment>();

        private ApplicationUser _authenticatedUser;

        private bool _hasRatedMovie = false;

        protected override async Task OnInitializedAsync()
        {
            _userRoles = LayoutService.MainLayout.UserRoles;
            _authenticatedUser = LayoutService.MainLayout.AuthenticatedUser;
            Movie = await GetMovieDetails();
            GetUserStatusComments();
            if (_authenticatedUser != null)
            {
                _hasRatedMovie = await HasUserRatedMovieAsync();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
        }

        private async Task<bool> HasUserRatedMovieAsync()
        {
            string ratedMoviesCollectionName = "Classified";
            var ratedMovies = await CollectionsManager.GetFirstByConditionAsync(
                mc => mc.ApplicationUser == _authenticatedUser && mc.Name == ratedMoviesCollectionName,
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

    }
}
