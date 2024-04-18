namespace CineSync.Controllers
{
    public class ApiService
    {
        private readonly HttpClient _client;
        private readonly string BEARER_TOKEN = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJkNTkyMTlhZTQ3MjcxZmE5NDg3YzI3MTJjMzRhMTZkMiIsInN1YiI6IjY2MGFjNzMzMTVkZWEwMDE2MjMyZTQxZiIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.qMp8g8AF-OnuKWknE-1-eN5BqqwHlrvyHXgTqvT_wG4";

        public ApiService(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", BEARER_TOKEN);
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<string> FetchDataAsync(string endpoint)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }
    }
}
