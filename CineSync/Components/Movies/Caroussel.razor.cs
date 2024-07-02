using CineSync.Controllers.MovieEndpoint;
using CineSync.Core.Adapters.ApiAdapters;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Timers;
using Newtonsoft.Json;

namespace CineSync.Components.Movies;

public partial class Caroussel : ComponentBase
{
    [Inject]
    private HttpClient _client { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; }

    private System.Timers.Timer _timer;

    private List<MovieSearchAdapter> CurrentMovies { get; set; } = new List<MovieSearchAdapter>(0);

    private Queue<MovieSearchAdapter> MovieQueue { get; } = new Queue<MovieSearchAdapter>();

    private static ICollection<MovieSearchAdapter> AllRatedMovies { get; set; }

    private static List<MovieSearchAdapter> TopRatedMovies { get; set; }

    private bool isCarouselActive = true;
    private bool hasLoaded = false;

    private DotNetObjectReference<Caroussel> objRef;

    protected override async Task OnInitializedAsync()
    {
        hasLoaded = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await FetchTopRatedMovies();
            InitializeQueue();
            objRef = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("addResizeListener", objRef);
            await UpdateDisplayMode();
            hasLoaded = true;
            StateHasChanged();
        }
    }

    private void InitializeQueue()
    {
        if (TopRatedMovies == null || !TopRatedMovies.Any())
            TopRatedMovies = AllRatedMovies.Take(10).ToList();

        foreach (var movie in TopRatedMovies.Take(5))
        {
            MovieQueue.Enqueue(movie);
        }
        UpdateCurrentMovies();

    }

    private void StartTimer()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Dispose();
        }
        _timer = new System.Timers.Timer(5000);
        _timer.Elapsed += UpdateCarousel;
        _timer.AutoReset = true;
        _timer.Enabled = true;
    }

    private void StopTimer()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }
    }


    private void UpdateCarousel(Object source, ElapsedEventArgs e)
    {
        if (AllRatedMovies.Count > 5)
        {
            MovieQueue.Dequeue();
            int nextMovieIndex = (TopRatedMovies.IndexOf(MovieQueue.Last()) + 1) % TopRatedMovies.Count;
            MovieQueue.Enqueue(TopRatedMovies[nextMovieIndex]);
            InvokeAsync(() =>
            {
                UpdateCurrentMovies();
                StateHasChanged();
            });
        }
    }

    private void UpdateCurrentMovies()
    {
        CurrentMovies = MovieQueue.ToList();
    }

    private async Task FetchTopRatedMovies()
    {
        if (AllRatedMovies == null || !AllRatedMovies.Any())
        {
            var response = await _client.GetAsync("/movie/top-rated"); // Adjust the URL path as needed.
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                AllRatedMovies = JsonConvert.DeserializeObject<ApiSearchResponse>(jsonResponse)?.Results;
            }
        }
    }

    private void MovieClickHandler(MovieSearchAdapter movie)
    {
        NavigationManager.NavigateTo($"/MovieDetails/{movie.MovieId}");
    }

    [JSInvokable]
    public async Task UpdateDisplayMode()
    {
        var screenWidth = await JSRuntime.InvokeAsync<int>("eval", "window.innerWidth");
        if (screenWidth >= 1900)
        {
            isCarouselActive = true;
            StartTimer();
        }
        else
        {
            isCarouselActive = false;
            StopTimer();
        }
        StateHasChanged();
    }

    public void Dispose()
    {
        _timer?.Stop();
        _timer?.Dispose();
        objRef?.Dispose();
    }
}
