using FilmLogAPI.DTOs;
using FilmLogAPI.Models;
using FilmLogAPI.Repositories;
using System.Text.Json;

namespace FilmLogAPI.Services
{
    public class MovieService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IWatchlistRepository _watchlistRepository;
        private readonly IWatchedlistRepository _watchedlistRepository;

        public MovieService(
            HttpClient httpClient,
            IConfiguration config,
            IWatchlistRepository watchlistRepository,
            IWatchedlistRepository watchedlistRepository)
        {
            _httpClient = httpClient;
            _config = config;
            _watchlistRepository = watchlistRepository;
            _watchedlistRepository = watchedlistRepository;
        }

        public async Task<List<MovieSearchResultDto>> SearchMovies(string searchTerm)
        {
            var apiKey = _config["OMDb:ApiKey"];
            var baseUrl = _config["OMDb:BaseUrl"];

            var url = $"{baseUrl}?apikey={apiKey}&s={searchTerm}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<MovieSearchResultDto>();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("Search", out var results))
                return new List<MovieSearchResultDto>();

            var movies = new List<MovieSearchResultDto>();

            foreach (var item in results.EnumerateArray())
            {
                movies.Add(new MovieSearchResultDto
                {
                    ImdbId = item.GetProperty("imdbID").GetString() ?? string.Empty,
                    Title = item.GetProperty("Title").GetString() ?? string.Empty,
                    Year = item.GetProperty("Year").GetString() ?? string.Empty,
                    PosterUrl = item.GetProperty("Poster").GetString() ?? string.Empty,

                });
            }

            return movies;
        }

        public async Task<MovieDetailsDto?> GetMovieDetails(string title)
        {
            var apiKey = _config["OMDb:ApiKey"];
            var baseUrl = _config["OMDb:BaseUrl"];

            var url = $"{baseUrl}?apikey={apiKey}&t={title}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;

            if (root.GetProperty("Response").GetString() == "False")
                return null;

            return new MovieDetailsDto
            {
                ImdbId = root.GetProperty("imdbID").GetString() ?? string.Empty,
                Title = root.GetProperty("Title").GetString() ?? string.Empty,
                Year = root.GetProperty("Year").GetString() ?? string.Empty,
                Genre = root.GetProperty("Genre").GetString() ?? string.Empty,
                Actors = root.GetProperty("Actors").GetString() ?? string.Empty,
                PosterUrl = root.GetProperty("Poster").GetString() ?? string.Empty
            };
        }

        public async Task AddToWatchlist(int userId, MovieDetailsDto dto)
        {
            await _watchlistRepository.AddToWatchlist(userId, dto);
        }

        public async Task<IEnumerable<Movie>> GetWatchlist(int userId)
        {
            return await _watchlistRepository.GetUserWatchlist(userId);
        }

        public async Task RemoveFromWatchlist(int userId, string movieId)
        {
            await _watchlistRepository.RemoveFromWatchlist(userId, movieId);
        }

        public async Task AddToWatchedList(int userId, MovieDetailsDto dto)
        {

            await _watchlistRepository.RemoveIfExists(userId, dto.ImdbId);
            await _watchedlistRepository.AddToWatchedlist(userId, dto);
        }

        public async Task<IEnumerable<WatchedMovieDto>> GetWatchedList(int userId)
        {
            return await _watchedlistRepository.GetUserWatchedlist(userId);
        }

        public async Task RemoveFromWatchedList(int userId, string movieId)
        {
            await _watchedlistRepository.RemoveFromWatchedlist(userId, movieId);
        }

        public async Task ResetTimesWatched(int userId, string movieId)
        {
            await _watchedlistRepository.ResetTimesWatched(userId, movieId);
        }

        private Movie MapToMovie(MovieDetailsDto dto)
        {
            return new Movie
            {
                Title = dto.Title,
                Year = dto.Year,
                Genre = dto.Genre,
                Actors = dto.Actors,
                PosterUrl = dto.PosterUrl
            };
        }
    }
}