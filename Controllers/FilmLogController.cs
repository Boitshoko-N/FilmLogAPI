using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FilmLogAPI.Repositories;
using FilmLogAPI.DTOs;
using FilmLogAPI.Services;
using FilmLogAPI.Data;
using Microsoft.EntityFrameworkCore;
using FilmLogAPI.Models;

namespace FilmLogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FilmLogController : ControllerBase
    {
        private readonly IWatchlistRepository _watchlistRepository;
        private readonly IWatchedlistRepository _watchedlistRepository;
        private readonly MovieService _movieService;
        private readonly ApplicationDbContext _context;

        public FilmLogController(
            IWatchlistRepository watchlistRepository,
            IWatchedlistRepository watchedlistRepository,
            MovieService movieService, ApplicationDbContext context)
        {
            _watchlistRepository = watchlistRepository;
            _watchedlistRepository = watchedlistRepository;
            _movieService = movieService;
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier) ??
                User.FindFirst("sub");

            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID not found in token");

            return int.Parse(userIdClaim.Value);
        }

        [HttpGet("watchlist")]
        public async Task<IActionResult> GetWatchlist()
        {
            var userId = GetUserId();
            var movies = await _watchlistRepository.GetUserWatchlist(userId);
            return Ok(movies);
        }

        [HttpPost("watchlist")]
        public async Task<IActionResult> AddToWatchlist([FromBody] MovieDetailsDto dto)
        {
            var userId = GetUserId();
            

            await _watchlistRepository.AddToWatchlist(userId, dto);
            return Ok(new { message = "Movie added to watchlist successfully." });
        }

        [HttpDelete("watchlist/{imdbId}")]
        public async Task<IActionResult> RemoveFromWatchlist(string imdbId)
        {
            var userId = GetUserId();

            var removed = await _watchlistRepository.RemoveFromWatchlist(userId, imdbId);

            if (!removed)
                return NotFound("Movie not found in watchlist");

            return Ok(new { message = "Removed successfully" });
        }

        [HttpGet("watched")]
        public async Task<IActionResult> GetWatchedlist()
        {
            var userId = GetUserId();
            var movies = await _watchedlistRepository.GetUserWatchedlist(userId);
            return Ok(movies);
        }

        [HttpPost("watched")]
        public async Task<IActionResult> AddToWatchedlist([FromBody] MovieDetailsDto dto)
        {
            var userId = GetUserId();

            await _watchlistRepository.RemoveIfExists(userId, dto.ImdbId);

            await _watchedlistRepository.AddToWatchedlist(userId, dto);

            var watchedlist = await _context.WatchedLists
                .Where(w => w.UserId == userId)
                .Select(w => new WatchedListResponseDto
                {
                    WatchedListId = w.WatchedlistId,
                    Movies = w.WatchedListMovies
                        .Select(m => new WatchedMovieDto
                        {
                            MovieId = m.MovieId,
                            Title = m.Movie.Title,
                            TimesWatched = m.TimesWatched
                        }).ToList()
                })
                .FirstAsync();

            return Ok(watchedlist);
        }

        [HttpDelete("watched/{imdbId}")]
        public async Task<IActionResult> RemoveFromWatchedlist(string imdbId)
        {
            var userId = GetUserId();

            var removed = await _watchedlistRepository.RemoveFromWatchedlist(userId, imdbId);

            if (!removed)
                return NotFound("Movie not found in watched list");

            return Ok(new { message = "Removed successfully" });
        }

        [HttpPost("watched/reset/{imdbId}")]
        public async Task<IActionResult> ResetTimesWatched(string imdbId)
        {
            var userId = GetUserId();

            var result = await _watchedlistRepository.ResetTimesWatched(userId, imdbId);

            if (!result)
                return NotFound("Movie not found");

            return Ok(new { message = "Times watched reset" });
        }

        [HttpGet("watchlist/exists/{imdbId}")]
        public async Task<IActionResult> WatchlistMovieExists(string imdbId)
        {
            var userId = GetUserId();
            var exists = await _watchlistRepository.MovieExists(userId, imdbId);
            return Ok(exists);
        }

        [HttpGet("watched/exists/{imdbId}")]
        public async Task<IActionResult> WatchedMovieExists(string imdbId)
        {
            var userId = GetUserId();
            var exists = await _watchedlistRepository.MovieExists(userId, imdbId);
            return Ok(exists);
        }

        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<IActionResult> SearchMovies([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query is required");

            var results = await _movieService.SearchMovies(query);
            return Ok(results);
        }

        [AllowAnonymous]
        [HttpGet("details")]
        public async Task<IActionResult> GetMovieDetails([FromQuery] string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("Title is required");

            var movie = await _movieService.GetMovieDetails(title);

            if (movie == null)
                return NotFound("Movie not found");

            return Ok(movie);
        }

        [HttpPost("watched/move-back/{imdbId}")]
        public async Task<IActionResult> MoveBackToWatchlist(string imdbId)
        {
            var userId = GetUserId();

            var entry = await _context.WatchedListMovies
                .Include(wm => wm.WatchedList)
                .Include(wm => wm.Movie)
                .FirstOrDefaultAsync(wm =>
                    wm.WatchedList.UserId == userId &&
                    wm.MovieId == imdbId);

            if (entry == null)
                return NotFound();

            _context.WatchedListMovies.Remove(entry);

            var watchlist = await _context.Watchlists
                .Include(w => w.WatchlistMovies)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (watchlist == null)
            {
                watchlist = new Watchlist { UserId = userId };
                _context.Watchlists.Add(watchlist);
            }

            _context.WatchlistMovies.Add(new WatchListMovie
            {
                Watchlist = watchlist,
                MovieId = imdbId,
                DateAdded = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Moved back to watchlist" });
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var userId = GetUserId();

            var stats = await _watchedlistRepository.GetStatistics(userId);

            return Ok(stats);
        }
    }
}