using FilmLogAPI.Data;
using FilmLogAPI.DTOs;
using FilmLogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FilmLogAPI.Repositories
{
    public class WatchedlistRepository : IWatchedlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WatchedlistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WatchedMovieDto>> GetUserWatchedlist(int userId)
        {
            return await _context.WatchedListMovies
                .Include(wm => wm.Movie)
                .Include(wm => wm.WatchedList)
                .Where(wm => wm.WatchedList.UserId == userId)
                .Select(wm => new WatchedMovieDto
                {
                    MovieId = wm.MovieId,
                    Title = wm.Movie.Title,
                    Year = wm.Movie.Year,
                    PosterUrl = wm.Movie.PosterUrl,
                    TimesWatched = wm.TimesWatched
                })
                .ToListAsync();
        }

        public async Task<WatchedList> AddToWatchedlist(int userId, MovieDetailsDto movieDto)
        {
    
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

            if (!userExists)
                throw new InvalidOperationException($"User with ID {userId} does not exist.");

            var watchedlist = await _context.WatchedLists
                .Include(w => w.WatchedListMovies)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (watchedlist == null)
            {
                watchedlist = new WatchedList
                {
                    UserId = userId,
                    WatchedListMovies = new List<WatchedListMovie>()
                };

                _context.WatchedLists.Add(watchedlist);
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.MovieId == movieDto.ImdbId);

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

             var existingEntry = await _context.WatchedListMovies
               .Include(wm => wm.WatchedList)
               .FirstOrDefaultAsync(wm =>
                  wm.WatchedList.UserId == userId &&
                  wm.MovieId == movieDto.ImdbId);

            if (existingEntry == null)
            {
                _context.WatchedListMovies.Add(new WatchedListMovie
                {
                    WatchedList = watchedlist, 
                    Movie = movie,               
                    TimesWatched = 1
                });
            }
            else
            {
                existingEntry.TimesWatched += 1;
            }

            await _context.SaveChangesAsync();

            return watchedlist;
        }

        public async Task<bool> RemoveFromWatchedlist(int userId, string imdbId)
        {
            var watchedlist = await _context.WatchedLists
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (watchedlist == null)
                return false;

            var userMovie = await _context.WatchedListMovies
                .FirstOrDefaultAsync(wm =>
                    wm.WatchedListId == watchedlist.WatchedlistId &&
                    wm.MovieId == imdbId);

            if (userMovie == null)
                return false;

            _context.WatchedListMovies.Remove(userMovie);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MovieExists(int userId, string imdbId)
        {
            return await _context.WatchedListMovies
                .AnyAsync(wm =>
                    wm.WatchedList.UserId == userId &&
                    wm.MovieId == imdbId);
        }

        public async Task<bool> ResetTimesWatched(int userId, string imdbId)
        {
            var entry = await _context.WatchedListMovies
                .Include(wm => wm.WatchedList)
                .FirstOrDefaultAsync(wm =>
                    wm.WatchedList.UserId == userId &&
                    wm.MovieId == imdbId);

            if (entry == null)
                return false;

            entry.TimesWatched = 0;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<StatisticsDashboardDto> GetStatistics(int userId)
        {
            var watched = await _context.WatchedListMovies
                .Include(wm => wm.Movie)
                .Include(wm => wm.WatchedList)
                .Where(wm => wm.WatchedList.UserId == userId)
                .ToListAsync();

            var topGenres = watched
                .SelectMany(wm => wm.Movie.Genre.Split(','))
                .GroupBy(g => g.Trim())
                .Select(g => new GenreStatsDto
                {
                    Genre = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .Take(6)
                .ToList();

            var moviesByYear = watched
                .GroupBy(wm => wm.Movie.Year)
                .Select(g => new YearStatsDto
                {
                    Year = g.Key,
                    Count = g.Count()
                })
                .OrderBy(g => g.Year)
                .ToList();

            var topMovies = watched
                .GroupBy(wm => wm.Movie.Title)
                .Select(g => new MovieStatsDto
                {
                    Title = g.Key,
                    TimesWatched = g.Sum(x => x.TimesWatched)
                })
                .OrderByDescending(g => g.TimesWatched)
                .Take(10)
                .ToList();

            return new StatisticsDashboardDto
            {
                TopGenres = topGenres,
                MoviesByYear = moviesByYear,
                TopMovies = topMovies
            };
        }
    }
}