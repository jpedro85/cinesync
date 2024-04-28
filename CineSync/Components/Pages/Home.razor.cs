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
    public partial class Home : ComponentBase, IDisposable
    {

        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private MainLayout MainLayout { get; set; }

        private bool showNavMenu = false;
        private Queue<MovieSearchAdapter> movieQueue = new Queue<MovieSearchAdapter>();

        private static List<MovieSearchAdapter> TopRatedMovies { get; set; }

        private System.Timers.Timer _timer;

        public List<MovieSearchAdapter> CurrentMovies { get; set; } = new List<MovieSearchAdapter>();

        public ApplicationUser? AuthenticatedUser { get; set; }

        private SearchButton SearchButton { get; set; } = new SearchButton();

        protected override async Task OnInitializedAsync()
        {
            MainLayout = LayoutService.MainLayout;
            MainLayout.RemoveSearchButton();

            await FetchTopRatedMovies();

            AuthenticatedUser = MainLayout.AuthenticatedUser;
            AuthenticatedUser = new ApplicationUser { UserName = "testuser" };
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                SearchButton.OnSearch += OnSearch;
                InitializeQueue();
                StartTimer();
                StateHasChanged();
            }
        }

        private void InitializeQueue()
        {

            foreach (var movie in TopRatedMovies.Take(5))
            {
                movieQueue.Enqueue(movie);
            }
            UpdateCurrentMovies();
        }

        private void StartTimer()
        {
            _timer = new System.Timers.Timer(5000);
            _timer.Elapsed += UpdateCarousel;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void UpdateCarousel(Object source, ElapsedEventArgs e)
        {
            if (TopRatedMovies.Count > 5)
            {
                movieQueue.Dequeue();
                var nextMovieIndex = (TopRatedMovies.IndexOf(movieQueue.Last()) + 1) % TopRatedMovies.Count;
                movieQueue.Enqueue(TopRatedMovies[nextMovieIndex]);
                InvokeAsync(() =>
                {
                    UpdateCurrentMovies();
                    StateHasChanged();
                });
            }
        }

        private void UpdateCurrentMovies()
        {
            CurrentMovies = movieQueue.ToList();
        }


        private void ToggleNavMenu()
        {
            showNavMenu = !showNavMenu;
        }

        private async Task FetchTopRatedMovies()
        {
            if (TopRatedMovies == null || !TopRatedMovies.Any())
            {
                var response = await _client.GetAsync("/movie/top-rated"); // Adjust the URL path as needed.
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    TopRatedMovies = JsonConvert.DeserializeObject<ApiSearchResponse>(jsonResponse)?.Results;
                }
            }
        }

        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
        }

        public void OnSearch(string searchQuery)
        {
            if (searchQuery != string.Empty)
                NavigationManager.NavigateTo($"/Search/{searchQuery}");
        }
    }
}
