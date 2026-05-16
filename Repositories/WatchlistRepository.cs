using FilmLogAPI.Models;
using Microsoft.EntityFrameworkCore;
using FilmLogAPI.Data;
using FilmLogAPI.DTOs;

namespace FilmLogAPI.Repositories
{
    public class WatchlistRepository : IWatchlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WatchlistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Watchlist> AddToWatchlist(int userId, MovieDetailsDto movieDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                throw new InvalidOperationException($"User with ID {userId} does not exist.");

            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.MovieId == movieDto.ImdbId);

            if (movie == null)
            {
                movie = new Movie
                {
                    MovieId = movieDto.ImdbId,
                    Title = movieDto.Title,
                    Year = movieDto.Year,
                    Genre = movieDto.Genre,
                    Actors = movieDto.Actors,
                    PosterUrl = movieDto.PosterUrl
                };

                _context.Movies.Add(movie);
            }

            var watchlist = await _context.Watchlists
                .Include(w => w.WatchlistMovies)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (watchlist == null)
            {
                watchlist = new Watchlist
                {
                    UserId = userId,
                    WatchlistMovies = new List<WatchListMovie>()
                };

                _context.Watchlists.Add(watchlist);
            }

            var alreadyLinked = watchlist.WatchlistMovies.Any(wm => wm.MovieId == movieDto.ImdbId);

            if (!alreadyLinked)
            {
                var watchlistMovie = new WatchListMovie
                {
                    Watchlist = watchlist,
                    Movie = movie,  
                    DateAdded = DateTime.Now
                };

                _context.WatchlistMovies.Add(watchlistMovie);
            }

            await _context.SaveChangesAsync();

            return await _context.Watchlists
                .Include(w => w.WatchlistMovies)
                .ThenInclude(wm => wm.Movie)
                .FirstAsync(w => w.WatchlistId == watchlist.WatchlistId);
        }

        public async Task<bool> RemoveFromWatchlist(int userId, string imdbId)
        {
            var watchlistMovie = await _context.WatchlistMovies
                .Include(wm => wm.Watchlist)
                .FirstOrDefaultAsync(wm =>
                    wm.Watchlist.UserId == userId &&
                    wm.MovieId == imdbId);

            if (watchlistMovie == null) return false;

            _context.WatchlistMovies.Remove(watchlistMovie);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Movie>> GetUserWatchlist(int userId)
        {
            return await _context.WatchlistMovies
                .Include(wm => wm.Movie)
                .Where(wm => wm.Watchlist.UserId == userId)
                .Select(wm => wm.Movie)
                .ToListAsync();
        }

        public async Task<bool> MovieExists(int userId, string movieId)
        {
            return await _context.WatchlistMovies
                .AnyAsync(wm =>wm.Watchlist.UserId == userId && wm.MovieId == movieId);
        }

        public async Task RemoveIfExists(int userId, string movieId)
        {
            var entry = await _context.WatchlistMovies
                .Include(wm => wm.Watchlist)
                .FirstOrDefaultAsync(wm => wm.Watchlist.UserId == userId && wm.MovieId == movieId);

            if (entry != null)
            {
                _context.WatchlistMovies.Remove(entry);
                await _context.SaveChangesAsync();
            }
        }
    }
}