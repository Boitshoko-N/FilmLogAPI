using FilmLogAPI.DTOs;
using FilmLogAPI.Models;

namespace FilmLogAPI.Repositories
{
    public interface IWatchlistRepository
    {
        Task<IEnumerable<Movie>> GetUserWatchlist(int userId);
        Task<Watchlist>AddToWatchlist(int userId, MovieDetailsDto movieDto);
        Task<bool> RemoveFromWatchlist(int userId, string imdbId);
        Task<bool>MovieExists(int userId, string movieId);
        Task RemoveIfExists(int userId, string movieId);


    }
}
