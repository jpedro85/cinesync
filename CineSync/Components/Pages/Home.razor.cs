using CineSync.Data;
using CineSync.Controllers.MovieEndpoint;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Components;
using System.Timers;
using CineSync.Core.Adapters.ApiAdapters;


namespace CineSync.Components.Pages
{
    public partial class Home : ComponentBase, IDisposable
    {
        [Inject]
        private HttpClient _client { get; set; }
        private bool showNavMenu = false;
        private Queue<MovieSearchAdapter> movieQueue = new Queue<MovieSearchAdapter>();
        private List<MovieSearchAdapter> TopRatedMovies { get; set; }
        private System.Timers.Timer _timer;
        public ApplicationUser User { get; set; }
        public List<MovieSearchAdapter> CurrentMovies { get; set; } = new List<MovieSearchAdapter>();

        protected override async Task OnInitializedAsync()
        {
            await FetchTopRatedMovies();
            InitializeQueue();
            StartTimer();
            User = new ApplicationUser { UserName = "testuser" };
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
            var response = await _client.GetAsync("/movie/top-rated"); // Adjust the URL path as needed.
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                TopRatedMovies = JsonConvert.DeserializeObject<ApiSearchResponse>(jsonResponse)?.Results;
            }
        }

        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
        }
    }
}
