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

    private List<MovieSearchAdapter> CurrentMovies { get; set; } = new List<MovieSearchAdapter>();

    private Queue<MovieSearchAdapter> movieQueue = new Queue<MovieSearchAdapter>();

    private static List<MovieSearchAdapter> TopRatedMovies { get; set; }

    private bool isCarouselActive = true;

    private DotNetObjectReference<Caroussel> objRef;

    protected override async Task OnInitializedAsync()
    {
        await FetchTopRatedMovies();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            InitializeQueue();
            objRef = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("addResizeListener", objRef);
            await UpdateDisplayMode();
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

    private void MovieClickHandler(MovieSearchAdapter movie)
    {
        NavigationManager.NavigateTo($"/MovieDetails/{movie.MovieId}");
    }

    [JSInvokable]
    public async Task UpdateDisplayMode()
    {
        var screenWidth = await JSRuntime.InvokeAsync<int>("eval", "window.innerWidth");
        if (screenWidth >= 1600)
        {
            isCarouselActive = true;
        }
        else
        {
            isCarouselActive = false;
        }
        StateHasChanged();
    }

    public void Dispose()
    {
        _timer?.Stop();
        _timer?.Dispose();
    }
}
